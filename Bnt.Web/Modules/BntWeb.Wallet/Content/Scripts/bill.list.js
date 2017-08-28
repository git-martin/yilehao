jQuery(function ($) {

    $('#billTable').dataTable({
        "processing": true,
        "serverSide": true,
        "sorting": [[2, "desc"]],
        "ajax": {
            "url": url_loadPage,
            "data": function (d) {
                //添加额外的参数传给服务器
                d.extra_search = {};
            }
        },
        "aoColumns":
        [
            {
                "mData": "CreateTime", 'sClass': 'left', "sWidth": "250px",
                "mRender": function (data, type, full) {
                    if (data != null && data.length > 0) {
                        return eval('new ' + data.replace(/\//g, '')).Format("yyyy-MM-dd hh:mm:ss");
                    }
                    return "";
                }
            },
            { "mData": "Money", 'sClass': 'left', "orderable": false },
            {
                "mData": "BillType",
                'sClass': 'left',
                "sWidth": "250px",
                "orderable": false,
                "mRender": function (data, type, full) {
                    if (data == 1) {
                        return '<span class="label label-sm label-success">收入</span>';
                    }
                    else if (data == 2) {
                        return '<span class="label label-sm label-danger">支出</span>';
                    }
                }

            },
            { "mData": "Remark", 'sClass': 'left', "orderable": false }
        ]
    });


    //积分添加
    $("#addIntegral").click(function () {
        var postModel = getPostData();
        if (postModel) {
            bntToolkit.confirm("确定要添加吗？", function () {
                bntToolkit.post(url_append, { postModel: postModel }, function (result) {
                    if (result.Success) {
                        location.reload();
                    } else {
                        bntToolkit.error(result.ErrorMessage);
                    }
                });
            });
        }
    });

    //积分扣除
    $("#deductIntegral").click(function () {
        var postModel = getPostData();
        if (postModel) {
            bntToolkit.confirm("确定要扣除吗？", function () {
                bntToolkit.post(url_deduct, { postModel: postModel }, function (result) {
                    if (result.Success) {
                        location.reload();
                    } else {
                        bntToolkit.error(result.ErrorMessage);
                    }
                });
            });
        }
    });

});

function getPostData() {
    var integral = $("#Integral").val();
    if (integral === "") {
        bntToolkit.error("积分不能为空");
        return null;
    }
    var remark = $("#Remark").val();
    if (remark === "") {
        bntToolkit.error("备注不能为空");
        return null;
    }

    var obj = { Integral: integral, Remark: remark };
    return obj;
}