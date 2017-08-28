jQuery(function ($) {
    $('#UsersTable').dataTable({
        "processing": true,
        "serverSide": true,
        "sorting": [[4, "desc"]],
        "ajax": url_loadPage,
        "aoColumns":
		[
			{
			    "mData": "UserName",
			    'sClass': 'left',
			    "mRender": function (data, type, full) {
			        return "<a href=\"#\">" + data + "</a>";
			    }
			},
			{ "mData": "Email", 'sClass': 'left' },
			{ "mData": "PhoneNumber", 'sClass': 'left' },
			{
			    "mData": "AllRoles",
			    'sClass': 'left',
			    "orderable": false,
			    "mRender": function (data, type, full) {
			        var render = '';
			        for (var i = 0; i < data.length; i++) {
			            var role = data[i];
			            if (!role.Hidden)
			                render += ((render.length > 0 ? "，" : "") + role.DisplayName);
			        }
			        return render;
			    }
			},
			{
			    "mData": "CreateTime", 'sClass': 'left',
			    "mRender": function (data, type, full) {
			        if (data != null && data.length > 0) {
			            return eval('new ' + data.replace(/\//g, '')).Format("yyyy-MM-dd hh:mm:ss");
			        }
			        return "";
			    }
			},
			{
			    "mData": "LockoutEnabled",
			    'sClass': 'left',
			    "mRender": function (data, type, full) {
			        if (data) {
			            return '<span class="label label-sm label-danger">已禁用</span>';
			        }
			        return '<span class="label label-sm label-success">已启用</span>';
			    }
			},
			{
			    "mData": "LockoutEnabled",
			    'sClass': 'center',
			    "sWidth": "150px",
			    "orderable": false,
			    "mRender": function (data, type, full) {
			        var render = '<div class="visible-md visible-lg hidden-sm hidden-xs action-buttons">';

			        if (full.UserName !== "bocadmin" && canSwitchUser) {
			            if (data) {
			                render += '<a class="green switch" data-id="' + full.Id + '" data-value="on" href="#" title="启用"><i class="icon-circle-blank bigger-130"></i></a>';
			            } else {
			                render += '<a class="red switch" data-id="' + full.Id + '" data-value="off" href="#" title="禁用"><i class="icon-circle bigger-130"></i></a>';
			            }
			        }

			        if (canEditUser)
			            render += '<a class="blue" href="' + url_editUser + '?id=' + full.Id + '" title="编辑"><i class="icon-pencil bigger-130"></i></a>';

			        if (full.UserName !== "bocadmin" && canDeleteUser)
			            render += '<a class="red delete" data-id="' + full.Id + '" href="#" title="删除"><i class="icon-trash bigger-130"></i></a>';
			        render += '</div>';
			        return render;
			    }
			}
		]
    });

    $('#UsersTable').on("click", ".switch", function (e) {
        var id = $(this).data("id");
        var val = $(this).data("value");
        bntToolkit.post(url_switchUser, { UserId: id, Enabled: val == "off" }, function (result) {
            if (result.Success) {
                $("#UsersTable").dataTable().fnDraw();
            } else {
                bntToolkit.error(result.ErrorMessage);
            }
        });
    });

    $('#UsersTable').on("click", ".delete", function (e) {
        var id = $(this).data("id");

        bntToolkit.confirm("删除后不可恢复，确定还要删除该用户吗？", function () {
            bntToolkit.post(url_deleteUser, { userId: id }, function (result) {
                if (result.Success) {
                    $("#UsersTable").dataTable().fnDraw();
                } else {
                    bntToolkit.error(result.ErrorMessage);
                }
            });
        });
    });
});