jQuery(function ($) {

    var options = $.datepicker.regional["zh-CN"];
    options["dateFormat"] = "yy-mm-dd";
    $("#CreateTimeBegin").datepicker(options);
    $("#CreateTimeEnd").datepicker(options);

    var moduleKey = $("#ModuleKey").val();
    var securityLevel = $("#SecurityLevel").val();
    var userName = $("#UserName").val();
    var createTimeBegin = $("#CreateTimeBegin").val();
    var createTimeEnd = $("#CreateTimeEnd").val();


    var logsTable = $('#LogsTable').dataTable({
        "processing": true,
        "serverSide": true,
        "ajax": {
            "url": url_loadPage,
            "data": function (d) {
                //添加额外的参数传给服务器
                d.extra_search = { "ModuleKey": moduleKey, "SecurityLevel": securityLevel, "UserName": userName, "CreateTimeBegin": createTimeBegin, "CreateTimeEnd": createTimeEnd };
            }
        },
        "sorting": [[4, "desc"]],
        "aoColumns":
		[
			{
			    "mData": "SecurityLevel", 'sClass': 'left',
			    "mRender": function (data, type, full) {
			        if (data === 2)
			            return '<span class="label label-sm label-danger">危险</span>';
			        if (data === 1)
			            return '<span class="label label-sm label-warning">警告</span>';

			        return '<span class="label label-sm">一般</span>';
			    }
			},
			{ "mData": "ModuleName", 'sClass': 'left' },
			{ "mData": "UserName", 'sClass': 'left' },
			{ "mData": "Message", 'sClass': 'left' },
			{
			    "mData": "CreateTime", 'sClass': 'left',
			    "mRender": function (data, type, full) {
			        if (data != null && data.length > 0) {
			            return eval('new ' + data.replace(/\//g, '')).Format("yyyy-MM-dd hh:mm:ss");
			        }
			        return "";
			    }
			}
		]
    });

    //查询
    $('#QueryButton').on("click", function () {
        moduleKey = $("#ModuleKey").val();
        securityLevel = $("#SecurityLevel").val();
        userName = $("#UserName").val();
        createTimeBegin = $("#CreateTimeBegin").val();
        createTimeEnd = $("#CreateTimeEnd").val();
        logsTable.api().ajax.reload();
    });
});