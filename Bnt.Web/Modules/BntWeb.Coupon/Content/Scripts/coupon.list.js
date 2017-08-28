jQuery(function ($) {
    var couponTable = $('#CouponTable').dataTable({
        "processing": true,
        "serverSide": true,
        "ajax": url_loadPage,
        "sorting": [[5, "desc"]],
        "aoColumns":
            [
                { "mData": "Title", 'sClass': 'left', "orderable": false },
                {
                    "mData": "CouponType", 'sClass': 'left', 
                    "mRender": function (data, type, full) {
                        if (data == 1) {
                            return "满减券";
                        } else {
                            return "折扣券";
                        }
                    }
                },
                {
                    "mData": "Money", 'sClass': 'left',
                    "mRender": function (data, type, full) {
                        if (full.CouponType == 1) {
                            return "满"+full.Minimum+"减"+full.Money;
                        } else {
                            return full.Discount+"折";
                        }
                    }
                },
                {
                    "mData": "ExpiryType", 'sClass': 'left', "orderable": false,
                    "mRender": function (data, type, full) {
                        if (data == 1) {
                            return full.ExpiryDay + " 天";
                        } else {
                            return eval('new ' + full.StartTime.replace(/\//g, '')).Format("yyyy-MM-dd") + "至<br/>" + eval('new ' + full.EndTime.replace(/\//g, '')).Format("yyyy-MM-dd");
                        }
                    }
                },
                {
                    "mData": "Quantity",'sClass': 'left', "orderable": false
                },
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
                    "mData": "Status",
                    'sClass': 'left', "orderable": false,
                    "mRender": function (data, type, full) {
                        if (data) {
                            return '<span class="label label-sm label-success">已启用</span>';
                        }
                        return '<span class="label label-sm label-danger">已禁用</span>';
                    }
                },
                {
                    "mData": "Enabled", 'sClass': 'center', "orderable": false,
                    "sWidth": "150px",
                    "mRender": function (data, type, full) {
                        var render = '<div class="visible-md visible-lg hidden-sm hidden-xs action-buttons">';
                        render += '<a class="blue" href="' + url_edit + '?id=' + full.Id + '" title="编辑"><i class="icon-pencil bigger-130"></i></a>';
                        render += '<a class="red delete" data-id="' + full.Id + '" href="#" title="删除"><i class="icon-trash bigger-130"></i></a>';
                        render += '<a class="blue" href="' + url_sendCoupon + '?id=' + full.Id + '" title="发放优惠券"><i class="icon-money bigger-130"></i></a>';
                        render += '</div>';
                        return render;
                    }
                }
            ]
    });


    $('#CouponTable').on("click", ".delete", function (e) {
        var id = $(this).data("id");
        bntToolkit.confirm("确定删除该优惠券吗？", function () {
            bntToolkit.post(url_delete, { id: id }, function (result) {
                if (result.Success) {
                    $("#CouponTable").dataTable().fnDraw();
                } else {
                    bntToolkit.error(result.ErrorMessage);
                }
            });
        });
    });
});

function formatDate(now) {
    var year = now.getYear();
    var month = now.getMonth() + 1;
    var date = now.getDate();
    var hour = now.getHours();
    var minute = now.getMinutes();
    var second = now.getSeconds();
    return year + "-" + month + "-" + date + "   " + hour + ":" + minute + ":" + second;
}
