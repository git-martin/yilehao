jQuery(function ($) {
    var status = $("#Status").val();
    var name = $("#Name").val();
    var goodsNo = $("#GoodsNo").val();

    var goodsTable = $('#GoodsTable').dataTable({
        "processing": true,
        "serverSide": true,
        "ajax": {
            "url": url_loadPage,
            "data": function (d) {
                //添加额外的参数传给服务器
                d.extra_search = { "Name": name, "Status": status, "GoodsNo": goodsNo };
            }
        },
        "sorting": [[1, "desc"]],
        "aoColumns":
        [
        {
            "mData": "Id",
            'sClass': 'left',
            "orderable": false,
            "mRender": function (data, type, full) {
                var render = '<img style="width:50px;height:50px;" src="' + full.MainImage.SmallThumbnail + '"/>';
                return render;
            }
        },
        { "mData": "Name", 'sClass': 'left' },
        { "mData": "GoodsNo", 'sClass': 'left' },
        { "mData": "ShopPrice", 'sClass': 'left' },
        { "mData": "OriginalPrice", 'sClass': 'left' },
        {
            "mData": "Status",
            'sClass': 'center',
            "sWidth": "100px",
            "mRender": function (data, type, full) {
                if (data == 1) {
                    return '<span class="label label-sm label-success">在售</span>';
                }
                return '<span class="label label-sm label-danger">未上架</span>';
            }
        },
        { "mData": "Stock", 'sClass': 'left', "sWidth": "100px" },
        { "mData": "SalesVolume", 'sClass': 'left', "sWidth": "100px" },
        {
            "mData": "IsPreSell",
            'sClass': 'center',
            "orderable": false,
            "sWidth": "100px",
            "mRender": function (data, type, full) {
                if (data) {
                    return '<span class="label label-sm label-success">是</span>';
                }
                return '<span class="label label-sm label-danger">否</span>';
            }
        },
        {
            "mData": "IsHot",
            'sClass': 'center',
            "orderable": false,
            "sWidth": "100px",
            "mRender": function (data, type, full) {
                if (data) {
                    return '<span class="label label-sm label-success">是</span>';
                }
                return '<span class="label label-sm label-danger">否</span>';
            }
        },
        {
            "mData": "GrouponEndTime", "sClass": 'left', "mRender": function (data, type, full) {
                var status = "";
                var star = new Date(parseInt(full.GrouponStartTime.replace("/Date(", "").replace(")/", ""), 10));
                var end = new Date(parseInt(full.GrouponEndTime.replace("/Date(", "").replace(")/", ""), 10));
                var dateNow = new Date();
                if (star > dateNow) {
                    status = "未开始";
                }
                else if (end < dateNow) {
                    status = "已结束";
                } else {
                    status = "进行中";
                }
                return status;
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

		            if (canEditCarousel) {
		                var url = url_addCarousel.replace('%5BsourceId%5D', full.Id).replace('%5BsourceTitle%5D', full.Name.substring(0, 10)).replace('%5BviewUrl%5D', "");
		                render += '<a class="blue" data-id="' + full.Id + '" href="' + url + '" title="加入轮播"><i class="icon-magic bigger-130"></i></a>';
		            }

		            if (canEditAdvert) {
		                var url = url_sendAdvert.replace('%5BsourceId%5D', full.Id).replace('%5BsourceTitle%5D', full.Name).replace('%5BviewUrl%5D', "");
		                render += '<a class="blue" data-id="' + full.Id + '" href="' + url + '" title="设为广告"><i class="icon-barcode bigger-130"></i></a>';
		            }
		            render += '<a class="green view" data-id="' + full.Id + '" href="' + url_Evaluate + '?goodsId=' + full.Id + '" title="查看商品评论"><i class="icon-comments bigger-130"></i></a>';

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

        bntToolkit.confirm("删除商品会使产品强制下架，确定还要删除该商品吗？", function () {
            bntToolkit.post(url_batchDeleteGoods, { goodsIds: ids }, function (result) {
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
        status = $("#Status").val();
        name = $("#Name").val();
        goodsNo = $("#GoodsNo").val();
        location.href = url_expert + "/?status=" + status + "&name=" + name + "&goodsNo=" + goodsNo;
    });
});