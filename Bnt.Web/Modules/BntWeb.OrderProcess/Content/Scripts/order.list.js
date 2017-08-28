jQuery(function ($) {

    var options = $.datepicker.regional["zh-CN"];
    options["dateFormat"] = "yy-mm-dd";
    $("#CreateTimeBegin").datepicker(options);
    $("#CreateTimeEnd").datepicker(options);

    var orderNo = $("#OrderNo").val();
    var consignee = $("#Consignee").val();
    var memberName = $("#MemberName").val();
    var orderStatus = $("#OrderStatus").val();
    var refundStatus = $("#RefundStatus").val();
    var payStatus = $("#PayStatus").val();
    var shippingStatus = $("#ShippingStatus").val();
    var createTimeBegin = $("#CreateTimeBegin").val();
    var createTimeEnd = $("#CreateTimeEnd").val();
    var paymentId = $("#PaymentId").val();
    var goodsTable = $('#OrdersTable').dataTable({
        "processing": true,
        "serverSide": true,
        "ajax": {
            "url": url_loadPage,
            "data": function (d) {
                //添加额外的参数传给服务器
                d.extra_search = { "OrderNo": orderNo, "Consignee": consignee, "OrderStatus": orderStatus, "RefundStatus": refundStatus, "PayStatus": payStatus, "ShippingStatus": shippingStatus, "CreateTimeBegin": createTimeBegin, "CreateTimeEnd": createTimeEnd, "PaymentId": paymentId,"MemberName":memberName };
            }
        },
        "sorting": [[2, "desc"]],
        "aoColumns":
		[
			{ "mData": "OrderNo", 'sClass': 'left' },
            		{
            		    "mData": "Id", 'sClass': 'left', "mRender": function (data, type, full) {
            		        var render = "<ul class='pro-list-show'>";
            		        for (var i = 0; i < full.OrderGoods.length; i++) {
            		            if (full.OrderGoods[i].GoodsImage != null) {
            		                render += "<li><div class='pro-s-box'><div class='pro-s-img'><img style='width:50px;height:50px;' src='" + full.OrderGoods[i].GoodsImage.SmallThumbnail + "'/></div><div class='pro-s-name'>";
            		            } else {
            		                render += "<li><div class='pro-s-box'><div class='pro-s-img'><img style='width:50px;height:50px;' src=''/></div><div class='pro-s-name'>";
            		            }
            		            render += full.OrderGoods[i].GoodsName + "</div><div class='pro-s-gg'>规格：" + full.OrderGoods[i].GoodsAttribute + "      x"+full.OrderGoods[i].Quantity+full.OrderGoods[i].Unit+"</div></div></li>";
            		        }
            		        render += "</ul>";
            		        return render;
            		    }
            		},
			{
			    "mData": "CreateTime", 'sClass': 'left',
			    "mRender": function (data, type, full) {
			        var render = '<strong>' + full.MemberName + "</strong><br/>";
			        if (data != null && data.length > 0) {
			            render += eval('new ' + data.replace(/\//g, '')).Format("yyyy-MM-dd hh:mm:ss");
			        }
			        return render;
			    }
			},
			{
			    "mData": "Consignee", 'sClass': 'left', "mRender": function (data, type, full) {
			        var render = '<strong>' + full.Consignee + '</strong> [Tel:' + full.Tel + ']<br/>' + (full.PCDS != null ? (full.PCDS + '<br/>') : '') + full.Address;
			        if (full.BestTime != null) {
			            render += '<br/><strong>最佳送货时间:</strong>' + full.BestTime;
			        }
			        return render;
			    }
			},
			{
			    "mData": "OrderAmount", 'sClass': 'center', "sWidth": "200px", "orderable": false, "mRender": function (data, type, full) {
			        return '商品总价：￥' + data.toFixed(2) + '<br/> + 物流费用：￥' + full.ShippingFee.toFixed(2) + '<br/> - 积分折抵：￥' + full.IntegralMoney.toFixed(2);
			    }
			},
			{ "mData": "PayFee", 'sClass': 'center', "sWidth": "110px", "mRender": function (data, type, full) { return '￥' + data.toFixed(2); } },
            { "mData": "PaymentName", 'sClass': 'left', "orderable": false },
			{
			    "mData": "OrderStatus", 'sClass': 'center', "sWidth": "230px", "orderable": false, "mRender": function (data, type, full) {
			        var render = '';
			        switch (full.OrderStatus) {
			            case 0:
			                render += '待付款';
			                break;
			            case 1:
			                render += '待发货';
			                break;
			            case 2:
			                render += '待收货';
			                break;
			            case 3:
			                render += '已完成';
			                break;
			            case 4:
			                render += '<span class="red">已关闭</span>';
			                break;
			            case 5:
			                render += '退款中';
			                break;

			            default:
			        }

			        switch (full.RefundStatus) {
			            case 1:
			                render += '<br/><span class="red">退款中</span>';
			                break;
			            case 2:
			                render += '<br/><span class="red">已退款</span>';
			                break;
			            default:
			        }

			        switch (full.EvaluateStatus) {
			            case 1:
			                render += '<br/><span class="red">已评价</span>';
			                break;
			            case 2:
			                render += '<br/><span class="red">已回复</span>';
			                break;
			            default:
			                break;
			        }


			        //switch (full.PayStatus) {
			        //    case 0:
			        //        render += '未付款,';
			        //        break;
			        //    case 1:
			        //        render += '付款中,';
			        //        break;
			        //    case 2:
			        //        render += '已付款,';
			        //        break;

			        //    default:
			        //}

			        //switch (full.ShippingStatus) {
			        //    case 0:
			        //        render += '未发货';
			        //        break;
			        //    case 1:
			        //        render += '已发货';
			        //        break;
			        //    case 2:
			        //        render += '已取消';
			        //        break;

			        //    default:
			        //}

			        return render;
			    }
			},
		    {
		        "mData": "Id", 'sClass': ' center', "orderable": false,
		        "sWidth": "200px",
		        "mRender": function (data, type, full) {
		            var render = '<div class="visible-md visible-lg hidden-sm hidden-xs action-buttons">';

		            render += '<a class="green" href="' + url_detail + '?orderId=' + full.Id + '" title="查看"><i class="icon-eye-open bigger-130"></i></a>';
		            render += '</div>';
		            return render;
		        }

		    }
		]
    });

    //查询
    $('#QueryButton').on("click", function () {
        orderNo = $("#OrderNo").val();
        consignee = $("#Consignee").val();
        orderStatus = $("#OrderStatus").val();
        refundStatus = $("#RefundStatus").val();
        payStatus = $("#PayStatus").val();
        shippingStatus = $("#ShippingStatus").val();
        createTimeBegin = $("#CreateTimeBegin").val();
        createTimeEnd = $("#CreateTimeEnd").val();
        paymentId = $("#PaymentId").val();
        memberName = $("#MemberName").val();
        goodsTable.api().ajax.reload();
        stateOrderList();
    });

    $('#OrdersTable').on("click", ".delete", function (e) {
        var id = $(this).data("id");

        bntToolkit.confirm("删除商品会使产品强制下架，确定还要删除该商品吗？", function () {
            bntToolkit.post(url_deleteGoods, { typeId: id }, function (result) {
                if (result.Success) {
                    $("#GoodsTable").dataTable().fnDraw();
                } else {
                    bntToolkit.error(result.ErrorMessage);
                }
            });
        });
    });
    //导出Excel
    $('#ExpertButton').on("click", function () {
        orderNo = $("#OrderNo").val();
        consignee = $("#Consignee").val();
        orderStatus = $("#OrderStatus").val();
        refundStatus = $("#RefundStatus").val();
        payStatus = $("#PayStatus").val();
        shippingStatus = $("#ShippingStatus").val();
        createTimeBegin = $("#CreateTimeBegin").val();
        createTimeEnd = $("#CreateTimeEnd").val();
        paymentId = $("#PaymentId").val();
        memberName = $("#MemberName").val();
        location.href = url_orderToExecl + "/?orderNo=" + orderNo + "&consignee=" + consignee + "&orderStatus=" + orderStatus + "&refundStatus=" + refundStatus + "&payStatus=" + payStatus + "&shippingStatus=" + shippingStatus + "&createTimeBegin=" + createTimeBegin + "&createTimeEnd=" + createTimeEnd + "&paymentId=" + paymentId + "&memberName=" + memberName;
    });

    function stateOrderList() {
        var data =
        {
            "OrderNo": orderNo,
            "MemberName": memberName,
            "Consignee": consignee,
            "OrderStatus": orderStatus,
            "RefundStatus": refundStatus,
            "PayStatus": payStatus,
            "ShippingStatus": shippingStatus,
            "CreateTimeBegin": createTimeBegin,
            "CreateTimeEnd": createTimeEnd,
            "PaymentId": paymentId
        };
        bntToolkit.post(url_stateOrderList, data, function(result) {
            if (result.Success) {
                $("#TotalGoodsAmount").html(result.Data.TotalGoodsAmount);
                $("#TotalOrderAmount").html(result.Data.TotalOrderAmount);
                $("#TotalPayFee").html(result.Data.TotalPayFee);
                $("#TotalRefundFee").html(result.Data.TotalRefundFee);
                $("#TotalCouponMoney").html(result.Data.TotalCouponMoney);
                $("#TotalIntegralMoney").html(result.Data.TotalIntegralMoney);
                $("#TotalShippingFee").html(result.Data.TotalShippingFee);
                $("#TotalDiscountAmount").html(result.Data.TotalDiscountAmount);
            } else {
                $("#TotalGoodsAmount").html(0);
                $("#TotalOrderAmount").html(0);
                $("#TotalPayFee").html(0);
                $("#TotalRefundFee").html(0);
                $("#TotalCouponMoney").html(0);
                $("#TotalIntegralMoney").html(0);
                $("#TotalShippingFee").html(0);
                $("#TotalDiscountAmount").html(0);
                bntToolkit.error(result.ErrorMessage);
            }
        });
    }

    stateOrderList();
});

