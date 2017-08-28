jQuery(function ($) {
    var options = $.datepicker.regional["zh-CN"];
    options["dateFormat"] = "yy-mm-dd";
    var shippingName = $("#ShippingName").val();

    var loadTable = $('#ShippingsTable').dataTable({
        "processing": true,
        "serverSide": true,
        "sorting": [[1, "desc"]],
        "ajax": {
            "url": url_loadPage,
            "data": function (d) {
                //添加额外的参数传给服务器
                d.extra_search = { "Name": shippingName };
            }
        },
        "aoColumns":
        [
             { "mData": "Name", 'sClass': 'left', "orderable": false },
              { "mData": "Description", 'sClass': 'left', "orderable": false },
            {
                "mData": "IsDefault", 'sClass': 'left', "orderable": false,
                "mRender": function (data, type, full) {
                if (data) {
                    return '是';
                }
                else {
                    return '否';
                }
            } },
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
                "mData": "Status",
                'sClass': 'left',
                "sWidth": "250px",
                "orderable": false,
                "mRender": function (data, type, full) {
                    if (data == 1) {
                        return '<span class="label label-sm label-success">已启用</span>';
                    }
                    else {
                        return '<span class="label label-sm label-danger">已禁用</span>';
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
                    if (full.Status == 1) {
                        render += '<a class="red switch" data-id="' + full.Id + '" data-value="off" href="#" title="禁用"><i class="icon-circle bigger-130"></i></a>';
                        if (!full.IsDefault)
                            render += '<a class="blue default" data-id="' + full.Id + '" href="#" title="设为默认"><i class="icon-asterisk bigger-130"></i></a>';
                    }
                    else if (full.Status == 0) {
                        render += '<a class="green switch" data-id="' + full.Id + '" data-value="on" href="#" title="启用"><i class="icon-circle-blank bigger-130"></i></a>';
                    }
                   
                    render += '</div>';
                    return render;
                }
            }
        ]
    });

    //查询
    $('#QueryButton').on("click", function () {
        shippingName = $("#ShippingName").val();
        loadTable.api().ajax.reload();
    });

    $('#ShippingsTable').on("click", ".switch", function (e) {
        var id = $(this).data("id");
        bntToolkit.post(url_switchShipping, { shippingId: id }, function (result) {
            if (result.Success) {
                $("#ShippingsTable").dataTable().fnDraw();
            } else {
                bntToolkit.error(result.ErrorMessage);
            }
        });
    });

    $('#ShippingsTable').on("click", ".default", function (e) {
        var id = $(this).data("id");
        bntToolkit.post(url_defaultShipping, { shippingId: id }, function (result) {
            if (result.Success) {
                $("#ShippingsTable").dataTable().fnDraw();
            } else {
                bntToolkit.error(result.ErrorMessage);
            }
        });
    });
});