var bntToolkit = {
    showLoading: function () {
        $.blockUI({ message: '<div class="spinner"><div class="dot1"></div><div class="dot2"></div></div>' });
    },
    hideLoading: function () {
        $.unblockUI();
    },

    alert: function (message, remaining) {
        if (remaining == null || typeof (remaining) === "undefined") {
            remaining = 3;
        }

        bootbox.alert({
            buttons: {
                ok: {
                    label: '知道了（<span id="SecondsRemaining">' + remaining + '</span>s 关闭）',
                    className: 'btn-primary'
                }
            },
            message: message,
            title: "提醒"
        });

        // Number of seconds
        var obj = document.getElementById("SecondsRemaining");
        var timeout = window.setInterval(function () {
            remaining--;
            if (remaining === 0) {
                // Time is up, stop the timer and hide the bootbox
                window.clearInterval(timeout);
                bootbox.hideAll();
                return;
            }
            obj.innerHTML = remaining; // Update the value displayed
        }, 1000);
    },
    info: function (message) {
        $.gritter.add({
            title: '信息',
            text: message,
            class_name: 'gritter-info'
        });

    },
    success: function (message) {
        $.gritter.add({
            title: '成功了',
            text: message,
            class_name: 'gritter-success'
        });

    },
    error: function (message) {
        $.gritter.add({
            title: 'Oops，出错了',
            text: message,
            class_name: 'gritter-error'
        });

    },
    confirm: function (message, callback) {
        bootbox.dialog({
            message: message,
            title: "确认",
            buttons: {
                Cancel: {
                    label: "取消",
                    className: "btn-default",
                    callback: function () {
                    }
                }
                , OK: {
                    label: "确定",
                    className: "btn-danger",
                    callback: callback
                }
            }
        });
    },

    initForm: function (form, rules, beforeSubmit, success) {

        var _beforeSubmit = function (formData, jqForm, options) {
            bntToolkit.showLoading();
            if (beforeSubmit != null) {
                if (!beforeSubmit(formData, jqForm, options)) {
                    bntToolkit.hideLoading();
                    return false;
                }
            }
        };

        var _success = function (responseText, statusText, xhr, $form) {
            bntToolkit.hideLoading();
            if (success != null)
                success(responseText, statusText, xhr, $form);
        };

        var _error = function (xhr, statusText, error, $form) {
            bntToolkit.hideLoading();

        };

        form.validate({
            errorElement: 'div',
            errorClass: 'help-block',
            focusInvalid: false,
            rules: rules,
            ignore: "",
            highlight: function (e) {
                $(e).closest('.form-group').removeClass('has-info').addClass('has-error');
            },

            success: function (e) {
                $(e).closest('.form-group').removeClass('has-error').addClass('has-info');
                $(e).remove();
            },

            errorPlacement: function (error, element) {
                error.insertAfter(element.parent());
            },

            submitHandler: function (form) {
                $(form).ajaxSubmit({
                    //target: url,
                    type: 'POST',
                    method: "POST",
                    beforeSubmit: _beforeSubmit,
                    success: _success,
                    error: _error
                });

            }
        });

    },

    post: function (url, data, success) {
        bntToolkit.showLoading();

        $.post(url, data, function (result) {
            bntToolkit.hideLoading();
            if (success != null)
                success(result);
        })
         .error(function () {
             bntToolkit.hideLoading();

         })
        .complete(function () {
            bntToolkit.hideLoading();
        });

    },

    buildPagination: function (pageIndex, pageSize, totalCount) {
        var allPage = parseInt(totalCount / pageSize);
        if (totalCount % pageSize > 0) allPage = allPage + 1;
        if (pageIndex < 1)
            pageIndex = 1;
        if (pageIndex > allPage)
            pageIndex = allPage;

        var min = 1;
        var max = 5;

        if (allPage < 5)
            max = allPage;
        else {
            min = pageIndex - 2;
            max = pageIndex + 2;
            if (min < 1) min = 1;
            if (min + 4 > max) max = min + 4;
            if (max > allPage) max = allPage;
            if (max - 4 < min && max - 4 > 0) min = max - 4;
        }

        var pageHtml = '<ul class="pagination pull-right">';
        pageHtml += ('<li' + (pageIndex <= 1 ? ' class="disabled"' : '') + ' data-index="' + (pageIndex <= 1 ? 1 : (pageIndex - 1)) + '"><a href="#"><i class="icon-double-angle-left"></i></a></li>');

        for (var i = min; i <= max; i++) {
            pageHtml += '<li' + (i === pageIndex ? ' class="active"' : '') + ' data-index="' + i + '"><a href="#">' + i + '</a></li>';
        }

        pageHtml += '<li' + (allPage === pageIndex ? ' class="disabled"' : '') + ' data-index="' + (allPage === pageIndex ? allPage : (pageIndex + 1)) + '"><a href="#"><i class="icon-double-angle-right"></i></a></li>';

        pageHtml += '</ul>';
        return pageHtml;
    },

    spinner: function (ele) {
        $(ele).spinner({
            create: function (event, ui) {
                //add custom classes and icons
                $(this)
                    .next().addClass('btn btn-success').html('<i class="icon-plus"></i>')
                    .next().addClass('btn btn-danger').html('<i class="icon-minus"></i>');

                //larger buttons on touch devices
                if (ace.click_event == "tap") $(this).closest('.ui-spinner').addClass('ui-spinner-touch');
            }
        });
    },

    guid: function () {
        var guid = "";
        for (var i = 1; i <= 32; i++) {
            var n = Math.floor(Math.random() * 16.0).toString(16);
            guid += n;
            if ((i === 8) || (i === 12) || (i === 16) || (i === 20))
                guid += "-";
        }
        return guid;
    },

    selectFiles: function (maxCount, fileType, selectedFiles, callback) {
        if (maxCount == null || typeof (maxCount) == "undefined") {
            maxCount = 1;
        }

        if (selectedFiles == null || typeof (selectedFiles) == "undefined") {
            selectedFiles = [];
        }
        if (maxCount != 1 && maxCount - selectedFiles.length <= 0) {
            bntToolkit.error("达到最大上传数量");
            return;
        }

        var currentPageFiles = [];
        var dialog = bootbox.dialog({
            title: "选择文件",
            size: "large",
            message: '<div class="row select-files">  ' +
                        '<div class="col-md-12"> ' +
                        '<div class="row-fluid"><ul class="ace-thumbnails files"></ul></div>' +
                        '<div class="pull-right pagination-container" style="min-width: 900px;"></div>' +
                        '</div></div>',
            buttons: {
                success: {
                    label: "确定",
                    className: "btn-success",
                    callback: function () {
                        if (callback != null) {
                            callback(selectedFiles);
                        }
                    }
                }
            }
        });

        var load = function (pageIndex) {
            var url = "/BntWeb/Files/SelectOnPage";
            bntToolkit.post(url, { pageIndex: pageIndex, fileType: fileType }, function (result) {
                var totalCount = result.recordsTotal;
                dialog.find(".pagination-container").html(bntToolkit.buildPagination(pageIndex, 20, totalCount));
                dialog.find(".files").html("");
                currentPageFiles = result.data;
                for (var i = 0; i < result.data.length; i++) {
                    var item = result.data[i];
                    var selected = _.findIndex(selectedFiles, function (n) { return n.Id === item.Id; }) !== -1;

                    var fileImage = "/" + item.SmallThumbnail;
                    var fileText = item.FileName;

                    switch (item.FileType) {
                        case 0:
                            {
                                var mSize = item.MediumThumbnail.substring(item.MediumThumbnail.lastIndexOf("_") + 1, item.MediumThumbnail.lastIndexOf("."));
                                var sSize = item.SmallThumbnail.substring(item.SmallThumbnail.lastIndexOf("_") + 1, item.SmallThumbnail.lastIndexOf("."));

                                fileText = "原图：" + item.Width + "x" + item.Height + "<br/>中图：" + mSize + "<br/>小图：" + sSize;
                            }
                            break;
                        case 1:
                            fileImage = "/Resources/Common/Images/FileType/video.jpg";
                            break;
                        case 2:
                            fileImage = "/Resources/Common/Images/FileType/zip.jpg";
                            break;
                        case 3:
                            fileImage = "/Resources/Common/Images/FileType/pdf.jpg";
                            break;
                        default:
                            fileImage = "/Resources/Common/Images/FileType/other.jpg";
                            break;
                    }
                    dialog.find(".files").append('<li data-id="' + item.Id + '" data-selected="' + (selected ? 1 : 0) + '"><a href="#" title="' + item.FileName + '"><img alt="150x150" style="width:150px;height:150px;" src="' + fileImage + '"/><div class="text"><div class="inner">' + fileText + '</div></div><div class="tags" style="display:' + (selected ? "block" : "none") + '"><span class="label-holder"><span class="label label-danger">已选择</span></span></div></a></li>');
                }
            });
        };

        dialog.on("click", ".files>li", function () {
            var id = $(this).data("id");
            var selected = $(this).data("selected");
            if (selected === 0) {
                if (maxCount === 1) {
                    selectedFiles = [_.find(currentPageFiles, function (n) { return n.Id === id; })];
                    dialog.find(".files li").data("selected", 0);
                    dialog.find(".tags").hide();
                    $(this).data("selected", 1);
                    $(this).find(".tags").show();
                } else if (selectedFiles.length >= maxCount) {
                    bntToolkit.error("最多只能选择" + maxCount + "个文件");
                } else {
                    selectedFiles.push(_.find(currentPageFiles, function (n) { return n.Id === id; }));
                    $(this).data("selected", 1);
                    $(this).find(".tags").show();
                }
            } else {
                _.remove(selectedFiles, function (n) { return n.Id === id });
                $(this).data("selected", 0);
                $(this).find(".tags").hide();
            }
        });

        dialog.on("click", ".pagination>li", function () {
            var current = $(this).data("index");
            load(current);
        });

        load(1);
    }
};

// 对Date的扩展，将 Date 转化为指定格式的String   
// 月(M)、日(d)、小时(h)、分(m)、秒(s)、季度(q) 可以用 1-2 个占位符，   
// 年(y)可以用 1-4 个占位符，毫秒(S)只能用 1 个占位符(是 1-3 位的数字)   
// 例子：   
// (new Date()).Format("yyyy-MM-dd hh:mm:ss.S") ==> 2006-07-02 08:09:04.423   
// (new Date()).Format("yyyy-M-d h:m:s.S")      ==> 2006-7-2 8:9:4.18   
Date.prototype.Format = function (fmt) { //author: meizz   
    var o = {
        "M+": this.getMonth() + 1,                 //月份   
        "d+": this.getDate(),                    //日   
        "h+": this.getHours(),                   //小时   
        "m+": this.getMinutes(),                 //分   
        "s+": this.getSeconds(),                 //秒   
        "q+": Math.floor((this.getMonth() + 3) / 3), //季度   
        "S": this.getMilliseconds()             //毫秒   
    };
    if (/(y+)/.test(fmt))
        fmt = fmt.replace(RegExp.$1, (this.getFullYear() + "").substr(4 - RegExp.$1.length));
    for (var k in o)
        if (new RegExp("(" + k + ")").test(fmt))
            fmt = fmt.replace(RegExp.$1, (RegExp.$1.length == 1) ? (o[k]) : (("00" + o[k]).substr(("" + o[k]).length)));
    return fmt;
}