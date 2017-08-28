
var renderItem = function (evaluate, render) {
    var time = eval('new ' + evaluate.CreateTime.replace(/\//g, '')).Format("yyyy-MM-dd hh:mm:ss");

    render += '<li class="dd-item"><div class="dd-handle">';
    render += '<label class="inline"><input type="checkbox" class="ace" name="single-goods" data-id="' + evaluate.Id + '"><span class="lbl"></span></label>';
    render += evaluate.MemberName + '：（&nbsp;' + time + '&nbsp;）&nbsp;&nbsp;评分：' + evaluate.Score + '&nbsp;&nbsp;' + evaluate.Content;
    if (evaluate.Status == "1") {
        render += '<i class="icon-ok bigger-130 green">审核通过</i>';
    }
    render += '<div class="pull-right action-buttons"><a class="red delete" href="#" data-id="' + evaluate.Id + '"><i class="icon-trash bigger-130"></i></a></div></div>';


    //if (comment.ChildComments != null && comment.ChildComments.length > 0) {
    //    render += '<ol class="dd-list">';
    //    for (var i = 0; i < comment.ChildComments.length; i++) {
    //        var commentInner = comment.ChildComments[i];
    //        render = renderItem(commentInner, render);
    //    }
    //    render += '</ol>';
    //}
    render += '</li>';
    return render;
}

var load = function (index) {

    var pageSize = 10;
    bntToolkit.post(url_loadPage, { pageIndex: index, pageSize: pageSize }, function (result) {
        if (result.Success) {

            var evaluates = result.Data.Evaluates;

            var render = '<div class="dd" id="list_' + index + '" style="max-width: 100%;"><ol class="dd-list">';
            for (var i = 0; i < evaluates.length; i++) {
                var evaluate = evaluates[i];
                render = renderItem(evaluate, render);
            }
            render += '<ol class="dd-list"></div>';

            $("#EvaluateList").html(render);
            $("#pageDiv").html(bntToolkit.buildPagination(index, pageSize, result.Data.TotalCount));

            $('#list_' + index).nestable({ handleClass: "empty", collapseAll: true });

            $('.dd-handle a').on('mousedown', function (e) {
                e.stopPropagation();
            });

            $('[data-rel="tooltip"]').tooltip();
            $('#list_' + index).nestable("collapseAll");

        } else {
            bntToolkit.error(result.ErrorMessage);
        }
    });
}

var currentPageIndex = 1;
jQuery(function ($) {

    load(1);

    $("#EvaluateList").on("click", ".delete", function () {
        var id = $(this).data("id");

        bntToolkit.confirm("删除后不可恢复，确定还要删除该评论吗？", function () {
            bntToolkit.post(url_deleteEvaluate, { evaluateId: id }, function (result) {
                if (result.Success) {

                    load(currentPageIndex);
                } else {
                    bntToolkit.error(result.ErrorMessage);
                }
            });
        });
    });


    $("#pageDiv").on("click", "li[class!=disabled]", function () {
        currentPageIndex = $(this).data("index");
        load(currentPageIndex);
    });

    $("input[name='toggle-all']").on("click", function () {
        $('input[name="single-goods"]').prop("checked", $('input[name="toggle-all"]').prop("checked"));
    });

    $("#batch-approve").on("click", function () {
        if ($('input[name="single-goods"]:checked').length == 0) {
            bntToolkit.error("请选择要审核的评论");
            return false;
        }

        var ids = [];
        $('input[name="single-goods"]:checked').each(function () {
            ids.push($(this).data("id"));
        });

        bntToolkit.confirm("审核通过的评论将会在前台显示，确定还要审核评论吗？", function () {
            bntToolkit.post(url_BatchApprEvaluate, { evaluatesIds: ids }, function (result) {
                if (result.Success) {
                    load(currentPageIndex);
                } else {
                    bntToolkit.error(result.ErrorMessage);
                }
            });
        });
    });

    $("#batch-delete").on("click", function () {
        if ($('input[name="single-goods"]:checked').length == 0) {
            bntToolkit.error("请选择要删除的评论");
            return false;
        }

        var ids = [];
        $('input[name="single-goods"]:checked').each(function () {
            ids.push($(this).data("id"));
        });

        bntToolkit.confirm("确定要删除评论吗？", function () {
            bntToolkit.post(url_BatchDelEvaluate, { evaluatesIds: ids }, function (result) {
                if (result.Success) {
                    load(currentPageIndex);
                } else {
                    bntToolkit.error(result.ErrorMessage);
                }
            });
        });
    });

});