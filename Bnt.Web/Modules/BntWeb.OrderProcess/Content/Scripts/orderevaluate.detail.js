jQuery(function ($) {

    $('.evaluate-actions').on("click", ".b", function (e) {
        bntToolkit.confirm("提交后回复内容不得修改，确定还要提交吗？", function () {

            var isPass = true;
            var replayArr = [];
            $(".replayContent").each(function () {
                var id = $(this).data("id");
                var replayContent = $(this).val();
                if (replayContent == "") {
                    bntToolkit.error("回复内容不能为空");
                    isPass = false;
                    return ;
                }
                var replay = { Id: id, ReplayContent: replayContent };
                replayArr.push(replay);
            });
            if (isPass) {
                bntToolkit.post(url_replayEvaluate, { orderId: orderId, replayList: replayArr }, function (result) {
                    if (result.Success) {
                        location.reload();
                    } else {
                        bntToolkit.error(result.ErrorMessage);
                    }
                });
            }
        });
    });
});