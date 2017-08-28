jQuery(function ($) {
    var options = $.datepicker.regional["zh-CN"];
    options["dateFormat"] = "yy-mm-dd";
    $("#LastUpDateTime").datepicker(options);

    var title = $("#Title").val();
    var lastUpdateTime = $("#LastUpDateTime").val();


    var loadTable = $('#SinglePageInfoTable').dataTable({
        "processing": true,
        "serverSide": true,
        "sorting": [[3, "desc"]],
        "ajax": {
            "url": url_loadPage,
            "data": function (d) {
                //添加额外的参数传给服务器
                d.extra_search = { "Title": title, "LastUpDateTime": lastUpdateTime};
            }
        },
        "aoColumns":
        [
            {
                "mData": "Title", 'sClass': 'left', "orderable": false,
                "mRender": function (data, type, full) {
                    return "<a href=\"#\">" + data + "</a>";
                }
            },
               {
                   "mData": "SubTitle", 'sClass': 'left', "orderable": false,
                   "mRender": function (data, type, full) {
                       return "<a href=\"#\">" + data + "</a>";
                   }
               },
            { "mData": "Key", 'sClass': 'left', "orderable": false },
            {
                "mData": "LastUpdateTime", 'sClass': 'left',
                "sWidth": "200px",
                "mRender": function (data, type, full) {
                    if (data != null && data.length > 0) {
                        return eval('new ' + data.replace(/\//g, '')).Format("yyyy-MM-dd hh:mm");
                    }
                    return "";
                }
            },
            {
                "mData": "Id",
                'sClass': 'center',
                "sWidth": "150px",
                "orderable": false,
                "mRender": function (data, type, full) {
                    var render = '<div class="visible-md visible-lg hidden-sm hidden-xs action-buttons">';
                    if (canEditSinglePage)
                        render += '<a class="blue" data-id="' + full.Id + '" href="' + url_editsinglePage + '?id=' + full.Id + '&isView=false" title="处理"><i class="icon-pencil bigger-130"></i></a>';
                    render += '</div>';
                    return render;
                }
            }
        ]
    });

    //查询
    $('#QueryButton').on("click", function () {
        title = $("#Title").val();
        lastUpdateTime = $("#LastUpDateTime").val();
        loadTable.api().ajax.reload();
    });

    $('#SinglePageInfoTable').on("click", ".view", function (e) {
        var id = $(this).data("id");
    });

    $('#SinglePageInfoTable').on("click", ".delete", function (e) {
        var id = $(this).data("id");

        bntToolkit.confirm("删除后不可恢复，确定还要删除吗？", function () {
            bntToolkit.post(url_deletesinglePage, { singlePageId: id }, function (result) {
                if (result.Success) {
                    $("#SinglePageInfoTable").dataTable().fnDraw();
                } else {
                    bntToolkit.error(result.ErrorMessage);
                }
            });
        });
    });
});