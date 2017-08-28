jQuery(function ($) {

    var options = $.datepicker.regional["zh-CN"];
    options["dateFormat"] = "yy-mm-dd";
    $("#CreateTimeBegin").datepicker(options);
    $("#CreateTimeEnd").datepicker(options);

    var softType = $("#SoftType").val();
    var softName = $("#SoftName").val();
    var createTimeBegin = $("#CreateTimeBegin").val();
    var createTimeEnd = $("#CreateTimeEnd").val();


    var softTable = $('#SoftTable').dataTable({
        "processing": true,
        "serverSide": true,
        "ajax": {
            "url": url_loadPage,
            "data": function (d) {
                //添加额外的参数传给服务器
                d.extra_search = { "SoftType": softType, "SoftName": softName, "CreateTimeBegin": createTimeBegin, "CreateTimeEnd": createTimeEnd };
            }
        },
        "sorting": [[4, "desc"]],
        "aoColumns":
		[
			{
			    "mData": "SoftType", 'sClass': 'left',
			    "sWidth": "200px",
			    "mRender": function (data, type, full) {
			        if (data === 2)
			            return '苹果';
			        if (data === 1)
			            return '安卓';

			        return '未知';
			    }
			},
			{ "mData": "SoftName", 'sClass': 'left' },
			{
			    "mData": "Version", "sWidth": "200px", 'sClass': 'left'
			},
			{
			    "mData": "ForceUpdating", 'sClass': 'left',
			    "sWidth": "200px",
			    "mRender": function (data, type, full) {
			        if (data) {
			            return '<span class="label label-sm label-danger">是</span>';
			        }
			        return '<span class="label label-sm label-success">否</span>';
			    }
			},
			{
			    "mData": "CreateTime", 'sClass': 'left',
			    "sWidth": "200px",
			    "mRender": function (data, type, full) {
			        if (data != null && data.length > 0) {
			            return eval('new ' + data.replace(/\//g, '')).Format("yyyy-MM-dd hh:mm:ss");
			        }
			        return "";
			    }
			},
            {
                "mData": "CreateTime", 'sClass': ' center', "orderable": false,
                "sWidth": "200px",
                "mRender": function (data, type, full) {
                    var render = '<div class="visible-md visible-lg hidden-sm hidden-xs action-buttons">';
                    render += '<a class="blue" data-id="' + full.Id + '" href="' + url_edit + '?id=' + full.Id + '" title="编辑"><i class="icon-pencil bigger-130"></i></a>';
                    render += '<a class="red delete" data-id="' + full.Id + '" href="#" title="删除"><i class="icon-trash bigger-130"></i></a>';
                    render += '</div>';
                    return render;
                }
            }
		]
    });

    //查询
    $('#QueryButton').on("click", function () {
        softType = $("#SoftType").val();
        softName = $("#SoftName").val();
        createTimeBegin = $("#CreateTimeBegin").val();
        createTimeEnd = $("#CreateTimeEnd").val();
        softTable.api().ajax.reload();
    });


    $('#SoftTable').on("click", ".delete", function (e) {
        var id = $(this).data("id");

        bntToolkit.confirm("删除后不可恢复，确定还要删除吗？", function () {
            bntToolkit.post(url_delete, { id: id }, function (result) {
                if (result.Success) {
                    $("#SoftTable").dataTable().fnDraw();
                } else {
                    bntToolkit.error(result.ErrorMessage);
                }
            });
        });
    });
});