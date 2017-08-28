jQuery(function ($) {
    var name = $("#Name").val();
    var goodsNo = $("#GoodsNo").val();

    var goodsTable = $('#GoodsTable').dataTable({
        "processing": true,
        "serverSide": true,
        "ajax": {
            "url": url_loadPage,
            "data": function (d) {
                //添加额外的参数传给服务器
                d.extra_search = { "Name": name, "GoodsNo": goodsNo };
            }
        },
        "sorting": [[1, "desc"]],
        "aoColumns":
		[
		    {
		        "mData": "Id", 'sClass': ' center', "orderable": false,
		        "mRender": function (data, type, full) {
		            var render = '<label class="inline"><input type="checkbox" class="ace" name="single-goods" data-id="' + full.Id + '"><span class="lbl"></span></label>';
		            return render;
		        }
		    },
			{ "mData": "Name", 'sClass': 'left' },
			{ "mData": "GoodsNo", 'sClass': 'left' },
			{ "mData": "ShopPrice", 'sClass': 'left' },
			{
			    "mData": "Status", 'sClass': 'center',
			    "mRender": function (data, type, full) {
			        if (data == 1) {
			            return '<span class="label label-sm label-success">在售</span>';
			        }
			        return '<span class="label label-sm label-danger">未上架</span>';
			    }
			},
			{ "mData": "Stock", 'sClass': 'left' },
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
		            render += '<a class="blue restore" data-id="' + full.Id + '" href="#" title="还原"><i class="icon-reply bigger-130"></i></a>';
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
        goodsTable.api().ajax.reload();
    });

    $('#GoodsTable').on("click", ".delete", function (e) {
        var id = $(this).data("id");

        bntToolkit.confirm("删除商品后将无法找回，确定还要删除该商品吗？", function () {
            bntToolkit.post(url_deleteGoods, { goodsId: id }, function (result) {
                if (result.Success) {
                    $("#GoodsTable").dataTable().fnDraw();
                } else {
                    bntToolkit.error(result.ErrorMessage);
                }
            });
        });
    });

    $('#GoodsTable').on("click", ".restore", function (e) {
        var id = $(this).data("id");

        bntToolkit.confirm("还原商品后可在商品列表查看对商品进行重新编辑上架，确定还要还原商品吗？", function () {
            bntToolkit.post(url_restoreGoods, { goodsId: id }, function (result) {
                if (result.Success) {
                    $("#GoodsTable").dataTable().fnDraw();
                } else {
                    bntToolkit.error(result.ErrorMessage);
                }
            });
        });
    });

    $("input[name='toggle-all']").on("click", function () {
        $('input[name="single-goods"]').prop("checked", $('input[name="toggle-all"]').prop("checked"));
    });

    $('#GoodsTable').on("click", 'input[name="single-goods"]', function (e) {
        if ($('input[name="single-goods"]:checked').length == $('input[name="single-goods"]').length) {
            $("input[name='toggle-all']").prop("checked", true);
        } else {
            $("input[name='toggle-all']").prop("checked", false);
        }
    });

    $("#batch-delete").on("click", function () {
        if ($('input[name="single-goods"]:checked').length == 0) {
            bntToolkit.error("请选择要删除的商品");
            return false;
        }

        var ids = [];
        $('input[name="single-goods"]:checked').each(function () {
            ids.push($(this).data("id"));
        });

        bntToolkit.confirm("删除商品后将无法找回，确定还要删除该商品吗？", function () {
            bntToolkit.post(url_batchDeleteGoods, { goodsIds: ids }, function (result) {
                if (result.Success) {
                    $("#GoodsTable").dataTable().fnDraw();
                } else {
                    bntToolkit.error(result.ErrorMessage);
                }
            });
        });
    });
});