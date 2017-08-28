jQuery(function ($) {
    $('#SystemMessageTable').dataTable({
        "processing": true,
        "serverSide": true,
        "sorting": [[2, "desc"]],
        "ajax": url_loadPage,
        "aoColumns":
    	[
    		{
    		    "mData": "Title",
    		    'sClass': 'left',
    		    "mRender": function (data, type, full) {
    		        return "<a href=\"#\">" + data + "</a>";
    		    }
    		},
    		{
    		    "mData": "Content",
    		    'sClass': 'left'
    		},
    		{
    		    "mData": "CreateTime", 'sClass': 'left', "sWidth": "250px",
    		    "mRender": function (data, type, full) {
    		        if (data != null && data.length > 0) {
    		            return eval('new ' + data.replace(/\//g, '')).Format("yyyy-MM-dd");
    		        }
    		        return "";
    		    }
    		},
            {
                "mData": "ReadCount",
                'sClass': 'left',
                "sWidth": "150px",
                "mRender": function (data, type, full) {
                    return "<a href=\"#\">" + data + "</a>";
                }
            },
    		{
    		    "mData": "LockoutEnabled",
    		    'sClass': 'center',
    		    "sWidth": "150px",
    		    "orderable": false,
    		    "mRender": function (data, type, full) {
    		        var render = '<div class="visible-md visible-lg hidden-sm hidden-xs action-buttons">';

    		        if (canDeleteSystemMessage)
    		            render += '<a class="red delete" data-id="' + full.Id + '" href="#" title="删除"><i class="icon-trash bigger-130"></i></a>';
    		        render += '</div>';
    		        return render;
    		    }
    		}
    	]
    });

    $('#SystemMessageTable').on("click", ".delete", function (e) {
        var id = $(this).data("id");

        bntToolkit.confirm("删除后不可恢复，确定还要删除吗？", function () {
            bntToolkit.post(url_deleteSystemMessage, { systemMessageId: id }, function (result) {
                if (result.Success) {
                    $("#SystemMessageTable").dataTable().fnDraw();
                } else {
                    bntToolkit.error(result.ErrorMessage);
                }
            });
        });
    });
});