
var renderItem = function (comment, render) {
    var time = eval('new ' + comment.CreateTime.replace(/\//g, '')).Format("yyyy-MM-dd hh:mm:ss");

    render += '<li class="dd-item"><div class="dd-handle">' + comment.MemberName + '：（&nbsp;' + time + '&nbsp;）&nbsp;&nbsp;&nbsp;&nbsp;' + comment.Content + '<div class="pull-right action-buttons"><a class="red delete" href="#" data-id="' + comment.Id + '"><i class="icon-trash bigger-130"></i></a></div></div>';
    if (comment.ChildComments != null && comment.ChildComments.length > 0) {
        render += '<ol class="dd-list">';
        for (var i = 0; i < comment.ChildComments.length; i++) {
            var commentInner = comment.ChildComments[i];
            render = renderItem(commentInner, render);
        }
        render += '</ol>';
    }
    render += '</li>';
    return render;
}

var load = function (index) {
    var pageSize = 10;
    bntToolkit.post(url_loadPage, { pageIndex: index, pageSize: pageSize }, function (result) {
        if (result.Success) {
            var comments = result.Data.Comments;
            var render = '<div class="dd" id="list_' + index + '" style="max-width: 100%;"><ol class="dd-list">';
            for (var i = 0; i < comments.length; i++) {
                var comment = comments[i];
                render = renderItem(comment, render);
            }
            render += '<ol class="dd-list"></div>';

            $("#CommentList").html(render);
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

    $("#CommentList").on("click", ".delete", function () {
        var id = $(this).data("id");

        bntToolkit.confirm("删除后不可恢复，确定还要删除该评论吗？", function () {
            bntToolkit.post(url_deleteComment, { commentId: id }, function (result) {
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
});