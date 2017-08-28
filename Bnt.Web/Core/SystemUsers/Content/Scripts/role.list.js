jQuery(function ($) {
    $('#RolesTable').dataTable({
        "processing": true,
        "ajax": url_loadPage,
        "aoColumns":
		[
			{
			    "mData": "DisplayName",
			    'sClass': 'left',
			    "mRender": function (data, type, full) {
			        return "<a href=\"#\">" + data + "</a>";
			    }
			},
			{ "mData": "Description", 'sClass': 'left' },
			{
			    "mData": "Id",
			    'sClass': 'center',
			    "sWidth": "150px",
			    "orderable": false,
			    "mRender": function (data, type, full) {
			        var render = '<div class="visible-md visible-lg hidden-sm hidden-xs action-buttons">';
			        
			        if (!full.BuiltIn && canEditRole)
			        	render += '<a class="blue" href="' + url_editRole + '?id=' + full.Id + '" title="编辑"><i class="icon-pencil bigger-130"></i></a>';

			        if (!full.BuiltIn && canDeleteRole)
			        	render += '<a class="red delete" data-id="' + full.Id + '" href="#" title="删除"><i class="icon-trash bigger-130"></i></a>';
			        render += '</div>';
			        return render;
			    }
			}
		]
    });


    $('#RolesTable').on("click", ".delete", function (e) {
        var id = $(this).data("id");

        bntToolkit.confirm("删除后不可恢复，确定还要删除该角色吗？", function () {
            bntToolkit.post(url_deleteRole, { roleId: id }, function (result) {
                if (result.Success) {
                    $("#RolesTable").dataTable().api().ajax.reload();
                } else {
                    bntToolkit.error(result.ErrorMessage);
                }
            });
        });
    });
});