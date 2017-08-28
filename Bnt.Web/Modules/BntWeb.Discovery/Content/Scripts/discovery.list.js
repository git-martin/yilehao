jQuery(function ($) {
    var options = $.datepicker.regional["zh-CN"];
    options["dateFormat"] = "yy-mm-dd";
    $("#CreateTimeBegin").datepicker(options);
    $("#CreateTimeEnd").datepicker(options);
   

    var title = $("#Title").val();
    var createName = $("#CreateName").val();
    var createTimeBegin = $("#CreateTimeBegin").val();
    var createTimeEnd = $("#CreateTimeEnd").val();
   

    var loadTable = $('#DiscoveriesTable').dataTable({
        "processing": true,
        "serverSide": true,
        "sorting": [[3, "desc"]],
        "ajax": {
            "url": url_loadPage,
            "data": function (d) {
                //添加额外的参数传给服务器
                d.extra_search = { "Title": title, "CreateName": createName, "CreateTimeBegin": createTimeBegin, "CreateTimeEnd": createTimeEnd };
            }
        },
        "aoColumns":
        [
            {
                "mData": "Title",
                'sClass': 'left',
                "mRender": function (data, type, full) {
                    return "<a href=\"#\">" + data + "</a>";
                }
            },
            { "mData": "Blurb", 'sClass': 'left', "orderable": false },
            { "mData": "CreateName", 'sClass': 'left', "orderable": false },
            {
                "mData": "CreateTime", 'sClass': 'left',
                "mRender": function (data, type, full) {
                    if (data != null && data.length > 0) {
                        return eval('new ' + data.replace(/\//g, '')).Format("yyyy-MM-dd hh:mm:ss");
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

                    render += '<a class="blue" data-id="' + full.Id + '" href="' + url_editDiscovery + '?id=' + full.Id + '" title="编辑"><i class="icon-pencil bigger-130"></i></a>';

                    if (canDeleteDiscovery)
                        render += '<a class="red delete" data-id="' + full.Id + '" href="#" title="删除"><i class="icon-trash bigger-130"></i></a>';
                    render += '</div>';
                    return render;
                }
            }
        ]
    });

    //查询
    $('#QueryButton').on("click", function () {
        title = $("#Title").val();
        createName = $("#CreateName").val();
        createTimeBegin = $("#CreateTimeBegin").val();
        createTimeEnd = $("#CreateTimeEnd").val();
      
        loadTable.api().ajax.reload();
    });

    $('#DiscoveriesTable').on("click", ".delete", function (e) {
        var id = $(this).data("id");
        
        bntToolkit.confirm("删除后不可恢复，确定还要删除吗？", function () {
            bntToolkit.post(url_deleteDiscovery, { id: id }, function (result) {
                if (result.Success) {
                    $("#DiscoveriesTable").dataTable().fnDraw();
                } else {
                    bntToolkit.error(result.ErrorMessage);
                }
            });
        });
    });
});
