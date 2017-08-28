jQuery(function ($) {

    var options = $.datepicker.regional["zh-CN"];
    options["dateFormat"] = "yy-mm-dd";
    $("#CreateTimeBegin").datepicker(options);
    $("#CreateTimeEnd").datepicker(options);

    var userName = $("#UserName").val();
    var createTimeBegin = $("#CreateTimeBegin").val();
    var createTimeEnd = $("#CreateTimeEnd").val();


    var backupTable = $('#backupTable').dataTable({
        "processing": true,
        "serverSide": true,
        "ajax": {
            "url": url_loadPage,
            "data": function (d) {
                //添加额外的参数传给服务器
                d.extra_search = { "UserName": userName, "CreateTimeBegin": createTimeBegin, "CreateTimeEnd": createTimeEnd };
            }
        },
        "sorting": [[0, "desc"]],
        "aoColumns":
		[
			{
			    "mData": "CreateTime", 'sClass': 'left', "sWidth": "200px",
			    "mRender": function (data, type, full) {
			        if (data != null && data.length > 0) {
			            return eval('new ' + data.replace(/\//g, '')).Format("yyyy-MM-dd hh:mm:ss");
			        }
			        return "";
			    }
			},
			{ "mData": "CreateUserName", "sWidth": "200px", 'sClass': 'left' },
			{
			    "mData": "FilePath", 'sClass': 'left', "orderable": false, "sWidth": "400px",
			    "mRender": function (data, type, full) {
			        return '<a href="/' + data + '" target="_blank">' + data + '</a>';
			    }
			},
			{
			    "mData": "Status", 'sClass': 'left', "sWidth": "200px",
			    "mRender": function (data, type, full) {
			        if (data == 0)
			            return '<span class="label label-sm">正在备份</span>';
			        if (data == 1)
			            return '<span class="label label-sm label-success">成功</span>';
			        if (data == 2)
			            return '<span class="label label-sm label-danger">失败</span>';
			    }
			},
			{ "mData": "Message", 'sClass': 'left', "orderable": false }
		]
    });

    //查询
    $('#QueryButton').on("click", function () {
        userName = $("#UserName").val();
        createTimeBegin = $("#CreateTimeBegin").val();
        createTimeEnd = $("#CreateTimeEnd").val();
        backupTable.api().ajax.reload();
    });

    //备份
    $('#BtnBackup').on("click", function () {
        bntToolkit.post(url_backup, [], function (data) {
            if (!data.Success) {
                bntToolkit.error(data.ErrorMessage);
            } else {
                bntToolkit.success("备份成功");
            }

            backupTable.api().ajax.reload();
        });
    });
});