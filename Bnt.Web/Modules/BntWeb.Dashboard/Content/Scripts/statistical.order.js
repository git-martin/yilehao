jQuery(function ($) {
    var year = $("#year").val();
    var statisticalway = $("#statisticalway").val();
    var startTime = $("#CreateTimeBegin").val();
    var endTime = $("#CreateTimeEnd").val();
    var province = $("#State_Province").val();
    var city = $("#State_City").val();

    var totalSales = 0;
    var totalRefund = 0;
    var totalOrderNumber = 0;

    var goodsTable = $('#MembersTable').dataTable({
        "processing": true,
        "sProcessing": "正在统计中...",
        "bPaginate": false,
        "bInfo": false,
        "serverSide": true,
        "ajax": {
            "url": url_loadPage,
            "data": function (d) {
                //添加额外的参数传给服务器
                d.extra_search = { "Year": year, "Statisticalway": statisticalway, "StartTime": startTime, "EndTime": endTime, "State_Province": province, "State_City": city };
            }
        },
        "sorting": [[1, "desc"]],
        "aoColumns":
		[
		               {
		                   "mData": "Times", 'sClass': 'center', "orderable": false,
		                   "mRender": function (data, type, full) {
		                       if (data.length == 4 && statisticalway != 4) {
		                           return data + '年';
		                       } else if (data.length < 4 && statisticalway != 4) {
		                           return data + '月份';
		                       } else {
		                           return data;
		                       }

		                   }
		               },
			{ "mData": "OrderNumber", 'sClass': 'left', "orderable": false },
			{ "mData": "Sales", 'sClass': 'left', "orderable": false },
			{ "mData": "Refund", 'sClass': 'left', "orderable": false },
		    {
		        "mData": "Sales", 'sClass': 'left', "orderable": false,
		        "mRender": function (data, type, full) {
		            return (full.Sales - full.Refund).toFixed(2);
		        }
		    }
		],
        "fnRowCallback": function (nRow, aData, iDisplayIndex, iDisplayIndexFull) {
            if (iDisplayIndex == 0) {
                totalOrderNumber = 0;
                totalSales = 0;
                totalRefund = 0;
            }
            totalOrderNumber += parseFloat(aData.OrderNumber);//第几列  
            totalSales += parseFloat(aData.Sales);
            totalRefund += parseFloat(aData.Refund);

            $("#TotalOrderNumber").html(totalOrderNumber);
            $("#TotalSales").html(totalSales);
            $("#TotalRefund").html(totalRefund);
            $("#Total").html((totalSales - totalRefund).toFixed(2));
            return nRow;
        }
    });


    //查询
    $('#QueryButton').on("click", function () {
        year = $("#year").val();
        statisticalway = $("#statisticalway").val();
        startTime = $("#CreateTimeBegin").val();
        endTime = $("#CreateTimeEnd").val();
        province = $("#State_Province").val();
        city = $("#State_City").val();

        if (statisticalway == "1")
            $("#stateTitile").html("年份");
        else if (statisticalway == "2")
            $("#stateTitile").html("月份");
        else if (statisticalway == "3")
            $("#stateTitile").html("时间段");
        else if (statisticalway == "4") {
            $("#stateTitile").html("省/市/取");
            if (province == "" || city=="") {
                bntToolkit.error("请选择省和市");
                return false;
            }
        }


        $("#TotalOrderNumber").html(0);
        $("#TotalSales").html(0);
        $("#TotalRefund").html(0);
        $("#Total").html(0);
        goodsTable.api().ajax.reload();
    });


});