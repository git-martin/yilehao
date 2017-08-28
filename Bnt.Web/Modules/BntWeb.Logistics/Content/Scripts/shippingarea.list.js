jQuery(function ($) {
    var options = $.datepicker.regional["zh-CN"];
    options["dateFormat"] = "yy-mm-dd";
    var shippingAreaName = $("#ShippingAreaName").val();

    var loadTable = $('#ShippingAreasTable').dataTable({
        "processing": true,
        "serverSide": true,
        "sorting": [[1, "desc"]],
        "ajax": {
            "url": url_loadPage,
            "data": function (d) {
                //添加额外的参数传给服务器
                d.extra_search = { "Name": shippingAreaName };
            }
        },
        "aoColumns":
        [
            { "mData": "Name", 'sClass': 'left', "orderable": false },
            { "mData": "AreaNames", 'sClass': 'left', "orderable": false, "sWidth": "400px" },
            { "mData": "Weight", 'sClass': 'center', "orderable": false },
            { "mData": "Freight", 'sClass': 'left', "orderable": false },
            { "mData": "SFreight", 'sClass': 'left', "orderable": false },
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
                "mData": "Id",
                'sClass': 'center',
                "sWidth": "150px",
                "orderable": false,
                "mRender": function (data, type, full) {
                    var render = '<div class="visible-md visible-lg hidden-sm hidden-xs action-buttons">';
                    render += '<a class="blue" href="' + url_editShippingArea + '?id=' + full.Id + '" title="编辑"><i class="icon-pencil bigger-130"></i></a>';
                    if (full.IsDefualt == "0") {
                        render += '<a class="red delete" data-id="' + full.Id + '" href="#" title="删除"><i class="icon-trash bigger-130"></i></a>';
                    }
                    render += '</div>';
                    return render;
                }
            }
        ]
    });

    //查询
    $('#QueryButton').on("click", function () {
        shippingAreaName = $("#ShippingAreaName").val();
        loadTable.api().ajax.reload();
    });


    $('#ShippingAreasTable').on("click", ".delete", function (e) {
        var id = $(this).data("id");

        bntToolkit.confirm("删除配送区域后不再显示，确定还要删除该配送区域吗？", function () {
            bntToolkit.post(url_deleteShippingArea, { id: id }, function (result) {
                if (result.Success) {
                    $("#ShippingAreasTable").dataTable().fnDraw();
                } else {
                    bntToolkit.error(result.ErrorMessage);
                }
            });
        });

    });
});