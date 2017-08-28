jQuery(function ($) {
    var status = $("#Status").val();
    var name = $("#Name").val();
    var goodsNo = $("#GoodsNo").val();
    var categoryId = $("#CategoryId").val();
    var goodsTable = $('#GoodsTable').dataTable({
        "processing": true,
        "serverSide": true,
        "ajax": {
            "url": url_loadPage,
            "data": function (d) {
                //添加额外的参数传给服务器
                d.extra_search = { "Name": name, "Status": status, "GoodsNo": goodsNo,"CategoryId":categoryId };
            }
        },
        "sorting": [[5, "asc"]],
        "aoColumns":
		[
			{ "mData": "Name", 'sClass': 'left', "orderable": false },
            { "mData": "CategoryName", 'sClass': 'left', "orderable": false },
			{ "mData": "GoodsNo", 'sClass': 'left', "orderable": false },
			{ "mData": "ShopPrice", 'sClass': 'left', "orderable": false },
			{
			    "mData": "Status", 'sClass': 'center', "orderable": false,
			    "mRender": function (data, type, full) {
			        if (data == 1) {
			            return '<span class="label label-sm label-success">在售</span>';
			        }
			        return '<span class="label label-sm label-danger">未上架</span>';
			    }
			},
			{ "mData": "Stock", 'sClass': 'left', "orderable": false },
			{
			    "mData": "IsPreSell", 'sClass': 'center', "orderable": false,
			    "mRender": function (data, type, full) {
			        if (data) {
			            return '<span class="label label-sm label-success">是</span>';
			        }
			        return '<span class="label label-sm label-danger">否</span>';
			    }
			},
			{
			    "mData": "IsHot", 'sClass': 'center', "orderable": false,
			    "mRender": function (data, type, full) {
			        if (data) {
			            return '<span class="label label-sm label-success">是</span>';
			        }
			        return '<span class="label label-sm label-danger">否</span>';
			    }
			},
		    {
		        "mData": "Id", 'sClass': ' center', "orderable": false,
		        "sWidth": "200px",
		        "mRender": function (data, type, full) {
		            var render = '<div class="visible-md visible-lg hidden-sm hidden-xs action-buttons">';
		            if (full.Status == 1) {
		                render += '<a class="red switch" data-id="' + full.Id + '" data-value="off" href="#" title="下架"><i class="icon-circle bigger-130"></i></a>';
		            }
		            else if (full.Status == 0) {
		                render += '<a class="green switch" data-id="' + full.Id + '" data-value="on" href="#"  title="上架"><i class="icon-circle-blank bigger-130"></i></a>';
		            }

		            render += '<a class="blue" href="' + url_editGoods + '?id=' + full.Id + '" title="编辑"><i class="icon-pencil bigger-130"></i></a>';
		            render += '<a class="red delete" data-id="' + full.Id + '" href="#" title="删除"><i class="icon-trash bigger-130"></i></a>';
		            render += '</div>';
		            return render;
		        }
		    }
		]
    });


    //查询
    $('#QueryButton').on("click", function () {
        name = $("#Name").val();
        goodsNo = $("#GoodsNo").val();
        status = $("#Status").val();
        categoryId = $("#CategoryId").val();
        goodsTable.api().ajax.reload();
    });

    $('#GoodsTable').on("click", ".delete", function (e) {
        var id = $(this).data("id");

        bntToolkit.confirm("删除商品会使产品强制下架，确定还要删除该商品吗？", function () {
            bntToolkit.post(url_deleteGoods, { goodsId: id }, function (result) {
                if (result.Success) {
                    $("#GoodsTable").dataTable().fnDraw();
                } else {
                    bntToolkit.error(result.ErrorMessage);
                }
            });
        });
    });

    $('#GoodsTable').on("click", ".switch", function (e) {
        var id = $(this).data("id");
        var value = $(this).data("value");
        var url = "";
        if (value == "off")
            url = url_NotInSaleGoods;
        else {
            url = url_InSaleGoods;
        }
        bntToolkit.post(url, { id: id }, function (result) {
            if (result.Success) {
                $("#GoodsTable").dataTable().fnDraw();
            } else {
                bntToolkit.error(result.ErrorMessage);
            }
        });
    });
    //导出Excel
    $('#ExpertButton').on("click", function () {
        status = $("#Status").val();
        name = $("#Name").val();
        goodsNo = $("#GoodsNo").val();
        categoryId = $("#CategoryId").val();
        location.href = url_expert + "/?status=" + status + "&name=" + name + "&goodsNo=" + goodsNo+"&categoryId="+categoryId;
    });
});