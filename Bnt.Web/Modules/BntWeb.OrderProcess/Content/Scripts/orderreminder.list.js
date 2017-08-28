jQuery(function ($) {

    var options = $.datepicker.regional["zh-CN"];
    options["dateFormat"] = "yy-mm-dd";
    $("#CreateTimeBegin").datepicker(options);
    $("#CreateTimeEnd").datepicker(options);

    var createTimeBegin = $("#CreateTimeBegin").val();
    var createTimeEnd = $("#CreateTimeEnd").val();

    var goodsTable = $('#OrderRemindersTable').dataTable({
        "processing": true,
        "serverSide": true,
        "ajax": {
            "url": url_loadPage,
            "data": function (d) {
                //添加额外的参数传给服务器
                d.extra_search = { "CreateTimeBegin": createTimeBegin, "CreateTimeEnd": createTimeEnd };
            }
        },
        "sorting": [[1, "desc"]],
        "aoColumns":
		[
            { "mData": "MemberName", 'sClass': 'left' },
			{ "mData": "OrderNo", 'sClass': 'left' },
			{
			    "mData": "CreateTime", 'sClass': 'left',
			    "mRender": function (data, type, full) {
			        var render = "";
			        if (data != null && data.length > 0) {
			            render += eval('new ' + data.replace(/\//g, '')).Format("yyyy-MM-dd hh:mm:ss");
			        }
			        return render;
			    }
			},
		    {
		        "mData": "Id", 'sClass': ' center', "orderable": false,
		        "sWidth": "200px",
		        "mRender": function (data, type, full) {
		            var render = '<div class="visible-md visible-lg hidden-sm hidden-xs action-buttons">';

		            render += '<a class="green" href="' + url_detail + '?orderId=' + full.OrderId + '" title="查看订单"><i class="icon-eye-open bigger-130"></i></a>';
		            render += '</div>';
		            return render;
		        }

		    }
		]
    });

    //查询
    $('#QueryButton').on("click", function () {
        createTimeBegin = $("#CreateTimeBegin").val();
        createTimeEnd = $("#CreateTimeEnd").val();
        goodsTable.api().ajax.reload();
    });

});