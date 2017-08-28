jQuery(function ($) {

    $('.order-actions').on("click", ".b", function (e) {
        var memo = $("#Memo").val();
        if (_.isEmpty(memo)) {
            bntToolkit.error("请填写操作备注");
            $("#Memo").focus();
            return;
        }
        var action = $(this).data("action");

        bntToolkit.post(_.find(urls, function (o) { return o.name == action; }).url, { orderId, memo: memo }, function (result) {
            if (result.Success) {
                location.reload();
            } else {
                bntToolkit.error(result.ErrorMessage);
            }
        });
    });

    $('.order-actions').on("click", ".a", function (e) {
        var action = $(this).data("action");
        var url = _.find(urls, function (o) { return o.name == action; }).url;
        location.href = url + "?orderId=" + orderId;
    });

    $('.order-actions').on("click", ".d", function (e) {
        var action = $(this).data("action");
        var url = _.find(urls, function (o) { return o.name == action; }).url;
        var shippingOptions = "";
        for (var i = 0; i < shippings.length; i++) {
            shippingOptions += "<option value=" + shippings[i].Id + " data-code=" + shippings[i].Code + " " + (shippings[i].IsDefault ? "selected=selected" : "") + ">" + shippings[i].Name + "</option>";
        }
        bootbox.dialog({
            title: "物流设置",
            size: "small",
            message: '<div class="row select-files">  ' +
                        '<div class="col-md-12"> ' +
                        '<div class="row-fluid">' +
                        '<div class="space-4"></div><div class="form-group"><label class="control-label no-padding-right" for="ShippingId"> 快递公司 </label><div ><div class="clearfix"><select id="ShippingId" name="ShippingId" style="width: 150px;">' + shippingOptions +
                        '</select></div></div></div>' +
                        '<div class="space-4"></div><div class="form-group"><label class="control-label no-padding-right" for="ShippingNo"> 快递单号 </label><div ><div class="clearfix"><input type="text" id="ShippingNo" name="ShippingNo" placeholder="快递单号" style="width: 250px;" value="" /></div></div></div>' +
                        '</div>' +
                        '</div></div>',
            buttons: {
                success: {
                    label: "确定",
                    className: "btn-success",
                    callback: function () {
                        bntToolkit.post(url, { orderId: orderId, shippingId: $("#ShippingId").val(), shippingCode: $("#ShippingId").find("option:selected").data("code"), shippingName: $("#ShippingId").find("option:selected").text(), shippingNo: $("#ShippingNo").val() }, function (result) {
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

    //修改价格
    $('.change-price').on("click", function () {
        var goodsPrice = $(this).data("goodsprice");
        var orderGoodsId = $(this).data("ordergoodsid");
        bootbox.dialog({
            title: "修改价格",
            size: "small",
            message: '<div class="row select-files">  ' +
                        '<div class="col-md-12"> ' +
                        '<div class="row-fluid">' +
                        '<div class="space-4"></div><div class="form-group"><label class="control-label no-padding-right" for="ShippingNo"> 商品单价 </label><div ><div class="clearfix"><input type="text" id="ChangedGoodsPrice" name="GoodsPrice" placeholder="商品单价" style="width: 250px;" value="' + goodsPrice + '" /></div></div></div>' +
                        '</div>' +
                        '</div></div>',
            buttons: {
                success: {
                    label: "确定",
                    className: "btn-success",
                    callback: function () {
                        bntToolkit.post(url_changePrice, { orderId: orderId, orderGoodsId: orderGoodsId, goodsPrice: $("#ChangedGoodsPrice").val() }, function (result) {
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

    $(".change-shippingfee").on("click", function () {
        var shippingFee = $(this).data("shippingfee");
        bootbox.dialog({
            title: "修改运费",
            size: "small",
            message: '<div class="row select-files">  ' +
                        '<div class="col-md-12"> ' +
                        '<div class="row-fluid">' +
                        '<div class="space-4"></div><div class="form-group"><label class="control-label no-padding-right" for="ShippingNo"> 运费总额 </label><div ><div class="clearfix"><input type="text" id="ChangedShippingFee" name="ChangedShippingFee" placeholder="运费总额" style="width: 250px;" value="' + shippingFee + '" /></div></div></div>' +
                        '</div>' +
                        '</div></div>',
            buttons: {
                success: {
                    label: "确定",
                    className: "btn-success",
                    callback: function () {
                        bntToolkit.post(url_changeShippingFee, { orderId: orderId, shippingFee: $("#ChangedShippingFee").val() }, function (result) {
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

    $("#editAddressBtn").on("click", function () {
        $(".hideAddress").hide();
        $(".showAddress").show();
    });
    $("#saveAddressBtn").on("click", function () {
        var url_saveAddress = _.find(urls, function (o) { return o.name == "editAddress"; }).url;
        bntToolkit.post(url_saveAddress,
            {
                orderId: orderId,
                consignee: $("#Consignee").val(),
                province: $("#Order_Province").val(),
                tel:$("#Tel").val(),
                city: $("#Order_City").val(),
                district: $("#Order_District").val(),
                street: $("#Order_Street").val(),
                address: $("#Address").val(),
                pCDS: $("#Order_Province").find("option:selected").text() + "," + $("#Order_City").find("option:selected").text() + "," + $("#Order_District").find("option:selected").text() + "," + $("#Order_Street").find("option:selected").text()
            }, function (result) {
                if (result.Success) {
                    location.reload();
                } else {
                    bntToolkit.error(result.ErrorMessage);
                }
            });
    });
    $("#cancelAddressBtn").on("click", function () {
        $(".hideAddress").show();
        $(".showAddress").hide();
    });
});