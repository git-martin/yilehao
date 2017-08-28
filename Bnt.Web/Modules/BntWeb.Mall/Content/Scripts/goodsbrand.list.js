jQuery(function ($) {

    var brandsTable = $('#BrandsTable').dataTable({
        "processing": true,
        "serverSide": true,
        "ajax": url_loadPage,
        "sorting": [[0, "desc"]],
        "aoColumns":
		[
			{
			    "mData": "Name", 'sClass': 'left',
			    "sWidth": "250px"
			},
			{
			    "mData": "SiteUrl", 'sClass': 'left',
			    "sWidth": "250px", "mRender": function (data, type, full) {
			        if (data != null && data.length > 0)
			            return "<a href='" + data + "' target='_blank'>" + data + "</a>";
			        return "";
			    }
			},
			{ "mData": "Description", 'sClass': 'left' },
			{
		        "mData": "Id", 'sClass': ' center', "orderable": false,
		        "sWidth": "200px",
		        "mRender": function (data, type, full) {
		            var render = '<div class="visible-md visible-lg hidden-sm hidden-xs action-buttons">';

		            render += '<a class="blue" href="' + url_edit + '?id=' + full.Id + '" title="编辑"><i class="icon-pencil bigger-130"></i></a>';
		            render += '<a class="red delete" data-id="' + full.Id + '" href="#" title="删除"><i class="icon-trash bigger-130"></i></a>';
		            render += '</div>';
		            return render;
		        }
		    }
		]
    });


    $('#BrandsTable').on("click", ".delete", function (e) {
        var id = $(this).data("id");

        bntToolkit.confirm("删除商品品牌无法恢复，确定还要删除该品牌吗？", function () {
            bntToolkit.post(url_delete, { brandId: id }, function (result) {
                if (result.Success) {
                    $("#BrandsTable").dataTable().fnDraw();
                } else {
                    bntToolkit.error(result.ErrorMessage);
                }
            });
        });
    });
});