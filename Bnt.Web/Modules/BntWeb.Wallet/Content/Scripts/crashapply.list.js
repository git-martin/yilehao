var loadTable;

var loadDataById = function (id) {
    var data = loadTable.api().data();
    var item = _.find(data, function (o) { return o.Id == id; });
    return item;
}

jQuery(function ($) {
    var applyStateType = $("#ApplyStateType").val();
    var paymentType = $("#PaymentType").val();
    loadTable = $('#crashapplyTable').dataTable({
        "processing": true,
        "serverSide": true,
        "sorting": [[2, "desc"]],
        "ajax": {
            "url": url_loadPage,
            "data": function (d) {
                //添加额外的参数传给服务器
                d.extra_search = { "ApplyStateType": applyStateType, "PaymentType": paymentType };
            }
        },
        "aoColumns":
        [
            {
                "mData": "Id",
                'sClass': 'center',
                "sWidth": "50px",
                "orderable": false,
                "mRender": function (data, type, full) {
                    var render = '<div class="visible-md visible-lg hidden-sm hidden-xs action-buttons">';
                    if (full.ApplyState === 1) {
                        render += '<input type="checkbox" name="checkbox1" value="' + full.Id + '"> ';
                    }
                    render += '</div>';
                    return render;
                }
            },
            {
                "mData": "TransactionNo", 'sClass': 'left',
                "sWidth": "200px", "orderable": false
            },
            {
                "mData": "RealName", 'sClass': 'left',
                "sWidth": "150px", "orderable": false
            },
            {
                "mData": "Account", 'sClass': 'left',
                "sWidth": "200px", "orderable": false
            },
            {
                "mData": "PaymentType",
                'sClass': 'left',
                "sWidth": "100px",
                "orderable": false,
                "mRender": function (data, type, full) {
                    if (data == 1) {
                        return '<span class="label label-sm label-info">支付宝</span>';
                    }
                    else if (data == 2) {
                        return '<span class="label label-sm label-success">微信</span>';
                    }
                }
            },

            {
                "mData": "Money", 'sClass': 'left',
                "sWidth": "100px", "orderable": false
            },
            {
                "mData": "CreateTime", 'sClass': 'left', "sWidth": "150px",
                "mRender": function (data, type, full) {
                    if (data != null && data.length > 0) {
                        return eval('new ' + data.replace(/\//g, '')).Format("yyyy-MM-dd hh:mm:ss");
                    }
                    return "";
                }
            },
            {
                "mData": "ApplyState",
                'sClass': 'left',
                "sWidth": "150px",
                "orderable": false,
                "mRender": function (data, type, full) {
                    if (data == 0) {
                        return '<span class="label label-sm label-warning">申请中</span>';
                    }
                    else if (data == 1) {
                        return '<span class="label label-sm label-success">申请通过</span>';
                    }
                    else if (data == 2) {
                        return '<span class="label label-sm label-success">打款中</span>';
                    }
                    else if (data == 3) {
                        return '<span class="label label-sm label-success">已打款</span>';
                    }
                    else if (data == 4) {
                        return '<span class="label label-sm label-success">提现失败</span>';
                    }
                }
            },
            {
                "mData": "AuditTime", 'sClass': 'left', "sWidth": "150px",
                "mRender": function (data, type, full) {
                    if (data != null && data.length > 0) {
                        return eval('new ' + data.replace(/\//g, '')).Format("yyyy-MM-dd hh:mm:ss");
                    }
                    return "";
                }
            },
            {
                "mData": "TransferTime", 'sClass': 'left', "sWidth": "150px",
                "mRender": function (data, type, full) {
                    if (data != null && data.length > 0) {
                        return eval('new ' + data.replace(/\//g, '')).Format("yyyy-MM-dd hh:mm:ss");
                    }
                    return "";
                }
            },

              { "mData": "Description", 'sClass': 'left', "orderable": false },
            {
                "mData": "Id",
                'sClass': 'center',
                "sWidth": "100px",
                "orderable": false,
                "mRender": function (data, type, full) {
                    var render = '<div class="visible-md visible-lg hidden-sm hidden-xs action-buttons">';
                    if (canProcesseWallet && full.ApplyState == 0) {
                        render += '<a class="blue auditApply" data-id="' + full.Id + '"  title="审核通过"><i class="icon-ok-sign bigger-130"></i></a>';
                    }
                    render += '</div>';
                    return render;
                }
            }
        ]
    });

    //查询
    $('#QueryButton').on("click", function () {
        applyStateType = $("#ApplyStateType").val();
        paymentType = $("#PaymentType").val();
        loadTable.api().ajax.reload();
    });


    $('#AlipayTransferButton').on("click", function () {
        var chkvalue = [];
        $(".applyIds").remove();
        var hasWeiXinApply = false;
        $('input[name="checkbox1"]:checked').each(function () {
            var val = $(this).val();
            chkvalue.push(val);
            var apply = loadDataById(val);
            if (apply != null && apply.PaymentType !== 1) {
                hasWeiXinApply = true;
            }
            $("#TransferForm").append('<input class="applyIds" type="hidden" name="ApplyIds[]" value="' + $(this).val() + '" />');
        });
        if (hasWeiXinApply) {
            bntToolkit.error("选择的转账条目包含提现到非支付宝的申请");
            return;
        }
        $("#PaymentCode").val("alipay");
        if (chkvalue[0] == null) {
            bntToolkit.error("请选择转账条目");
        } else {
            $("#applyIds").val(JSON.stringify(chkvalue));
            return bntToolkit.confirm("确定要转账吗？", function () {
                $("#TransferForm").submit();
            });
        }
    });

    $('#WeiXinTransferButton').on("click", function () {
        var chkvalue = [];
        $(".applyIds").remove();
        var hasAlipayApply = false;
        $('input[name="checkbox1"]:checked').each(function () {
            var val = $(this).val();
            chkvalue.push(val);
            var apply = loadDataById(val);
            if (apply != null && apply.PaymentType !== 2) {
                hasAlipayApply = true;
            }
            //$("#TransferForm").append('<input class="applyIds" type="hidden" name="ApplyIds[]" value="' + $(this).val() + '" />');
        });
        if (hasAlipayApply) {
            bntToolkit.error("选择的转账条目包含提现到非微信的申请");
            return;
        }
        //$("#PaymentCode").val("weixin");
        if (chkvalue[0] == null) {
            bntToolkit.error("请选择转账条目");
        } else {
            $("#applyIds").val(JSON.stringify(chkvalue));
            return bntToolkit.confirm("确定要转账吗？", function () {
                bntToolkit.post(url_transfer, { ApplyIds: chkvalue, PaymentCode: "weixin" }, function (result) {
                    result = eval("(" + result + ")");
                    if (result.Success) {
                        $("#crashapplyTable").dataTable().fnDraw();
                        if (result.ErrorMessage != "")
                            bntToolkit.success(result.ErrorMessage);
                    } else {
                        bntToolkit.error(result.ErrorMessage);
                    }
                });
                //$("#TransferForm").submit();
            });
        }
    });

    $('#OffLineTransferButton').on("click", function () {
        var chkvalue = [];
        $(".applyIds").remove();
        $('input[name="checkbox1"]:checked').each(function () {
            var val = $(this).val();
            chkvalue.push(val);
        });

        if (chkvalue[0] == null) {
            bntToolkit.error("请选择转账条目");
        } else {
            $("#applyIds").val(JSON.stringify(chkvalue));
            return bntToolkit.confirm("确定要操作吗？", function () {
                bntToolkit.post(url_transfer, { ApplyIds: chkvalue, PaymentCode: "offline" }, function (result) {
                    result = eval("(" + result + ")");
                    if (result.Success) {
                        $("#crashapplyTable").dataTable().fnDraw();
                        if (result.ErrorMessage != "")
                            bntToolkit.success(result.ErrorMessage);
                    } else {
                        bntToolkit.error(result.ErrorMessage);
                    }
                });
                //$("#TransferForm").submit();
            });
        }
    });


    $('#crashapplyTable').on("click", ".auditApply", function (e) {
        var id = $(this).data("id");

        bntToolkit.confirm("确定要通过该申请吗？", function () {
            bntToolkit.post(url_auditApply, { id: id }, function (result) {
                if (result.Success) {
                    $("#crashapplyTable").dataTable().fnDraw();
                } else {
                    bntToolkit.error(result.ErrorMessage);
                }
            });
        });
    });
});