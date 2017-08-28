jQuery(function ($) {
    var year = $("#year").val();
    var statisticalway = $("#statisticalway").val();
    var startTime = $("#CreateTimeBegin").val();
    var endTime = $("#CreateTimeEnd").val();

    var totalMemberNumber = 0;
    var totalNewMemberNumber = 0;
    var goodsTable = $('#MembersTable').dataTable({
        "processing": true,
        "sProcessing": "正在统计中...",
        "bPaginate": false,
        "bInfo": false,
        "serverSide": true,
        "ajax": {
            "url": url_loadPage,
            "data": function (d) {
                //添加额外的参数传给服务器
                d.extra_search = { "Year": year, "Statisticalway": statisticalway, "StartTime": startTime, "EndTime": endTime };
            }
        },
        "sorting": [[1, "desc"]],
        "aoColumns":
		[
            {
                "mData": "Times", 'sClass': 'center', "orderable": false,
                "mRender": function (data, type, full) {
                    if (data.length == 4) {
                        return data + '年';
                    } else if (data.length < 4 ) {
                        return data + '月份';
                    }else {
                        return data;
                    }

                }
            },
			{ "mData": "MemberNumber", 'sClass': 'left', "orderable": false },
			{ "mData": "NewMemberNumber", 'sClass': 'left', "orderable": false }

		],
        "fnRowCallback": function (nRow, aData, iDisplayIndex, iDisplayIndexFull) {
            if (iDisplayIndex == 0) {
                totalMemberNumber = 0;
                totalNewMemberNumber = 0;
            }
            totalMemberNumber += parseFloat(aData.MemberNumber);
            totalNewMemberNumber += parseFloat(aData.NewMemberNumber);

            $("#TotalMemberNumber").html(totalMemberNumber);
            $("#TotalNewMemberNumber").html(totalNewMemberNumber);
            return nRow;
        }
    });


    //查询
    $('#QueryButton').on("click", function () {
        year = $("#year").val();
        statisticalway = $("#statisticalway").val();
        startTime = $("#CreateTimeBegin").val();
        endTime = $("#CreateTimeEnd").val();

        $("#TotalMemberNumber").html(0);
        $("#TotalNewMemberNumber").html(0);
        goodsTable.api().ajax.reload();

    });
});