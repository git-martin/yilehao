jQuery(function ($) {

    $('.refund-actions').on("click", ".auditBtn", function (e) {
        var action = $(this).data("action");
        var url = _.find(urls, function (o) { return o.name == action; }).url;
        var applyId = $(this).parent().attr("refundid");
        bootbox.dialog({
            title: "退款审核",
            size: "small",
            message: '<div class="row select-files">  ' +
                        '<div class="col-md-12"> ' +
                        '<div class="row-fluid">' +
                        '<div class="space-4"></div><div class="form-group"><label class="control-label no-padding-right" for="AuditResult"> 审核结果 </label><div ><div class="clearfix">' +
                        '<div class="radio"><label><input name="auditResult" type="radio" class="ace" value="1"><span class="lbl"> 通过</span></label>' +
                        '<label><input name="auditResult" type="radio" class="ace" value="0"><span class="lbl"> 不通过</span></label></div>' +
                        '</div></div></div>' +
                        '<div class="space-4"></div><div class="form-group"><label class="control-label no-padding-right" for="ReviemMemo"> 审核说明 </label><div ><div class="clearfix"><textarea class="col-sm-12" rows="5" id="ReviemMemo"></textarea></div></div></div>' +
                        '</div>' +
                        '</div></div>',
            buttons: {
                success: {
                    label: "确定",
                    className: "btn-success",
                    callback: function () {
                        var auditResult = $("input[name='auditResult']:checked").val();
                        if (!auditResult) {
                            bntToolkit.error("请填写审核结果");
                            return;
                        }
                        var reviemMemo = $("#ReviemMemo").val();
                        if (_.isEmpty(reviemMemo)) {
                            bntToolkit.error("请填写审核说明");
                            $("#ReviemMemo").focus();
                            return;
                        }
                        bntToolkit.post(url, { applyId: applyId, auditResult: $("input[name='auditResult']:checked").val(), remark: reviemMemo }, function (result) {
                            if (result.Success) {
                                location.reload();
                            } else {
                                bntToolkit.error(result.ErrorMessage);
                            }
                        });
                    }
                }
            }
        });
    });

    $('.refund-actions').on("click", ".payBtn", function (e) {
        var action = $(this).data("action");
        var url = _.find(urls, function (o) { return o.name == action; }).url;
        var applyId = $(this).parent().attr("refundid");

        bntToolkit.confirm("打款后退款金额会打入买家账号，确定还要打款吗？", function () {
            bntToolkit.post(url, { applyId: applyId }, function (result) {
                if (result.Success) {
                    location.reload();
                } else {
                    bntToolkit.error(result.ErrorMessage);
                }
            });
        });
    });
});