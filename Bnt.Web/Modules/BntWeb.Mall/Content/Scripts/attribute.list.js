jQuery(function ($) {

    var attributesTable = $('#AttributesTable').dataTable({
        "processing": true,
        "serverSide": true,
        "ajax": url_loadPage,
        "sorting": [[0, "desc"]],
        "aoColumns":
		[
			{
			    "mData": "Name", 'sClass': 'left',
			    "sWidth": "200px"
			},
			{
			    "mData": "InputType", 'sClass': 'center',
			    "sWidth": "200px",
			    "mRender": function (data, type, full) {
			        if (data === 0)
			            return "手工输入";
			        return "选择输入";
			    }
			},
			{
			    "mData": "Values", 'sClass': 'center',
			    "mRender": function (data, type, full) {
			        return data;
			    }
			},
		    {
		        "mData": "Id", 'sClass': ' center', "orderable": false,
		        "sWidth": "200px",
		        "mRender": function (data, type, full) {
		            var render = '<div class="visible-md visible-lg hidden-sm hidden-xs action-buttons">';
		            render += '<a class="blue" href="' + url_editAttribute + '?typeId=' + full.GoodsTypeId + '&id=' + full.Id + '" title="编辑"><i class="icon-pencil bigger-130"></i></a>';
		            render += '<a class="red delete" data-id="' + full.Id + '" href="#" title="删除"><i class="icon-trash bigger-130"></i></a>';
		            render += '</div>';
		            return render;
		        }
		    }
		]
    });


    $('#AttributesTable').on("click", ".delete", function (e) {
        var id = $(this).data("id");

        bntToolkit.confirm("删除商品类型会同时删除类型下的所有属性，确定还要删除该类型吗？", function () {
            bntToolkit.post(url_deleteAttribute, { id: id }, function (result) {
                if (result.Success) {
                    $("#AttributesTable").dataTable().fnDraw();
                } else {
                    bntToolkit.error(result.ErrorMessage);
                }
            });
        });
    });

    $('#AddButton').on("click", function () {
        var count = $("#AttributesTable").DataTable().data().length;

        if (count >= 2) {
            bntToolkit.error("每个类型最多添加两个属性");
            return;
        }

        var url = $(this).data("href");
        location.href = url;
    });
});