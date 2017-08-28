jQuery(function ($) {
    var options = $.datepicker.regional["zh-CN"];
    options["dateFormat"] = "yy-mm-dd";
    $("#CreateTimeBegin").datepicker(options);
    $("#CreateTimeEnd").datepicker(options);

    var feedbackType = $("#FeedbackType").val();
    var sourceType = $("#SourceType").val();
    var sourceId = $("#SourceId").val();
    var status = $("#ProcesseStatus").val();
    var createTimeBegin = $("#CreateTimeBegin").val();
    var createTimeEnd = $("#CreateTimeEnd").val();


    var loadTable = $('#FeedbacksTable').dataTable({
        "processing": true,
        "serverSide": true,
        "sorting": [[1, "desc"]],
        "ajax": {
            "url": url_loadPage,
            "data": function (d) {
                //添加额外的参数传给服务器
                d.extra_search = { "FeedbackType": feedbackType, "SourceType": sourceType, "SourceId": sourceId, "ProcesseStatus": status, "CreateTimeBegin": createTimeBegin, "CreateTimeEnd": createTimeEnd };
            }
        },
        "aoColumns":
        [
            { "mData": "Content", 'sClass': 'left', "orderable": false},
            {
                "mData": "CreateTime", 'sClass': 'left', "sWidth": "250px",
                "mRender": function (data, type, full) {
                    if (data != null && data.length > 0) {
                        return eval('new ' + data.replace(/\//g, '')).Format("yyyy-MM-dd hh:mm:ss");
                    }
                    return "";
                }
            },
            {
                "mData": "ProcesseStatus",
                'sClass': 'left',
                "sWidth": "250px",
                "orderable": false,
                "mRender": function (data, type, full) {
                    if (data == 1) {
                        return '<span class="label label-sm label-warning">已处理</span>';
                    }
                    else {
                        return '<span class="label label-sm label-warning">未处理</span>';
                    }
                }
            },
            {
                "mData": "Id",
                'sClass': 'center',
                "sWidth": "150px",
                "orderable": false,
                "mRender": function (data, type, full) {
                    var render = '<div class="visible-md visible-lg hidden-sm hidden-xs action-buttons">';
                    render += '<a class="green view" data-id="' + full.Id + '" href="' + url_processeFeedback + '?id=' + full.Id + '&isView=true" title="查看"><i class="icon-eye-open bigger-130"></i></a>';
                    if (canProcesseFeedback &&full.ProcesseStatus==0)
                        render += '<a class="blue" data-id="' + full.Id + '" href="' + url_processeFeedback + '?id=' + full.Id + '&isView=false" title="处理"><i class="icon-pencil bigger-130"></i></a>';
                    if (canDeleteFeedback)
                        render += '<a class="red delete" data-id="' + full.Id + '" href="#" title="删除"><i class="icon-trash bigger-130"></i></a>';
                    render += '</div>';
                    return render;
                }
            }
        ]
    });

    //查询
    $('#QueryButton').on("click", function () {
        feedbackType = $("#FeedbackType").val();
        sourceType = $("#SourceType").val();
        sourceId = $("#SourceId").val();
        status = $("#ProcesseStatus").val();
        createTimeBegin = $("#CreateTimeBegin").val();
        createTimeEnd = $("#CreateTimeEnd").val();
        loadTable.api().ajax.reload();
    });

    $('#FeedbacksTable').on("click", ".view", function (e) {
        var id = $(this).data("id");
    });

    $('#FeedbacksTable').on("click", ".delete", function (e) {
        var id = $(this).data("id");

        bntToolkit.confirm("删除后不可恢复，确定还要删除吗？", function () {
            bntToolkit.post(url_deleteFeedback, { feedbackId: id }, function (result) {
                if (result.Success) {
                    $("#FeedbacksTable").dataTable().fnDraw();
                } else {
                    bntToolkit.error(result.ErrorMessage);
                }
            });
        });
    });
});