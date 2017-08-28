jQuery(function ($) {

    var options = $.datepicker.regional["zh-CN"];
    options["dateFormat"] = "yy-mm-dd";
    $("#CreateTimeBegin").datepicker(options);
    $("#CreateTimeEnd").datepicker(options);

    var fileType = $("#FileType").val();
    var fileName = $("#FileName").val();
    var createTimeBegin = $("#CreateTimeBegin").val();
    var createTimeEnd = $("#CreateTimeEnd").val();


    var filesTable = $('#FilesTable').dataTable({
        "processing": true,
        "serverSide": true,
        "ajax": {
            "url": url_loadPage,
            "data": function (d) {
                //添加额外的参数传给服务器
                d.extra_search = { "FileType": fileType, "FileName": fileName, "CreateTimeBegin": createTimeBegin, "CreateTimeEnd": createTimeEnd };
            }
        },
        "sorting": [[4, "desc"]],
        "aoColumns":
		[
			{
			    "mData": "FileName", 'sClass': 'left',
			    "sWidth": "180px"
			},
			{
			    "mData": "FileType", 'sClass': 'left',
			    "sWidth": "120px",
			    "mRender": function (data, type, full) {
			        var render = "类型:";
			        if (data === 0)
			            render += '图片';
			        else if (data === 1)
			            render += '视频';
			        else if (data === 2)
			            render += 'Zip压缩文件';
			        else if (data === 3)
			            render += 'Excel文件';
			        else
			            render += '其他文件';
			        render += ("<br/>后缀:" + full.FileExtension);

			        return render;
			    }
			},
			{
			    "mData": "FileSize", 'sClass': 'left',
			    "sWidth": "200px",
			    "mRender": function (data, type, full) {
			        var render = "大小:" + data + "  <strong>Kb</strong>";
			        if (full.FileType === 0) {
			            render += ("<br/>尺寸:" + full.Width + " x " + full.Height);
			        }
			        return render;
			    }
			},
			{ "mData": "RelativePath", 'sClass': 'left' },
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
			    "mData": "Id",
			    'sClass': 'center',
			    "sWidth": "100px",
			    "orderable": false,
			    "mRender": function (data, type, full) {
			        var render = '<div class="visible-md visible-lg hidden-sm hidden-xs action-buttons">';

			        if (canDeleteFile)
			            render += '<a class="red delete" data-id="' + full.Id + '" href="#" title="删除"><i class="icon-trash bigger-130"></i></a>';

			        render += '</div>';
			        return render;
			    }
			}
		]
    });

    //查询
    $('#QueryButton').on("click", function () {
        fileType = $("#FileType").val();
        fileName = $("#FileName").val();
        createTimeBegin = $("#CreateTimeBegin").val();
        createTimeEnd = $("#CreateTimeEnd").val();
        filesTable.api().ajax.reload();
    });



    $('#FilesTable').on("click", ".delete", function (e) {
        var id = $(this).data("id");

        bntToolkit.confirm("删除后不可恢复，确定还要删除此文件吗？", function () {
            bntToolkit.post(url_deleteFile, { fileId: id, confirm: 0 }, function (result) {
                if (result.Success) {
                    $("#FilesTable").dataTable().fnDraw();
                } else {
                    if (result.ErrorCode == "0001") {
                        bntToolkit.confirm("此文件已经被使用，确定要删除此文件并删除使用关联吗？", function () {
                            bntToolkit.post(url_deleteFile, { fileId: id, confirm: 1 }, function (result) {
                                if (result.Success) {
                                    $("#FilesTable").dataTable().fnDraw();
                                } else {
                                    bntToolkit.error(result.ErrorMessage);
                                }
                            });
                        });
                    } else
                        bntToolkit.error(result.ErrorMessage);
                }
            });
        });
    });
});