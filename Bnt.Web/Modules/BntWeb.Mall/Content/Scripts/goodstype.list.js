jQuery(function ($) {

    var typesTable = $('#TypesTable').dataTable({
        "processing": true,
        "serverSide": true,
        "ajax": url_loadPage,
        "sorting": [[0, "desc"]],
        "aoColumns":
		[
			{ "mData": "Name", 'sClass': 'left' },
			{
			    "mData": "Enabled", 'sClass': 'center',
			    "sWidth": "200px",
			    "mRender": function (data, type, full) {
			        if (data) {
			            return '<span class="label label-sm label-success">已启用</span>';
			        }
			        return '<span class="label label-sm label-danger">已禁用</span>';
			    }
			},
		    {
		        "mData": "Enabled", 'sClass': ' center', "orderable": false,
		        "sWidth": "200px",
		        "mRender": function (data, type, full) {
		            var render = '<div class="visible-md visible-lg hidden-sm hidden-xs action-buttons">';
		            if (data) {
		                render += '<a class="red switch" data-id="' + full.Id + '" data-value="off" href="#" title="禁用"><i class="icon-circle bigger-130"></i></a>';
		            } else {
		                render += '<a class="green switch" data-id="' + full.Id + '" data-value="on" href="#" title="启用"><i class="icon-circle-blank bigger-130"></i></a>';
		            }

		            render += '<a class="blue" href="' + url_editGoodsType + '?id=' + full.Id + '" title="编辑"><i class="icon-pencil bigger-130"></i></a>';
		            render += '<a class="red delete" data-id="' + full.Id + '" href="#" title="删除"><i class="icon-trash bigger-130"></i></a>';
		            render += '<a class="green" href="' + url_attributeList + '?typeId=' + full.Id + '" title="属性列表"><i class="icon-tasks bigger-130"></i></a>';
		            render += '</div>';
		            return render;
		        }
		    }
		]
    });


    $('#TypesTable').on("click", ".switch", function (e) {
        var id = $(this).data("id");
        var val = $(this).data("value");
        bntToolkit.post(url_switchGoodsType, { TypeId: id, Enabled: val == "on" }, function (result) {
            if (result.Success) {
                $("#TypesTable").dataTable().fnDraw();
            } else {
                bntToolkit.error(result.ErrorMessage);
            }
        });
    });

    $('#TypesTable').on("click", ".delete", function (e) {
        var id = $(this).data("id");

        bntToolkit.confirm("删除商品类型会同时删除类型下的所有属性，确定还要删除该类型吗？", function () {
            bntToolkit.post(url_deleteGoodsType, { typeId: id }, function (result) {
                if (result.Success) {
                    $("#TypesTable").dataTable().fnDraw();
                } else {
                    bntToolkit.error(result.ErrorMessage);
                }
            });
        });
    });
});