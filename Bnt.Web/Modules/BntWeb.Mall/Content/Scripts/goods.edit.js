
//页面级别的单品数据存储
var singleGoodsStore = [];
var selectedAttrs = [];

jQuery(function ($) {

    if (isGroupon == "True") {

        time($("#GrouponButton"), $("#GrouponTime").data("togrouponseconds"), $("#GrouponTime").data("grouponingseconds"));
    }

    bntToolkit.initForm($("#GoodsForm"), {
        Name: {
            required: true,
            maxlength: 200
        },
        Commission: {
            maxlength: 6
        },
        CategoryName: {
            required: true
        },
        Sort: {
            required: true,
            digits:true,
            min: 0,
            max:99999
        }
    }, beforeSubmit, success);


    $("#GrouponButton").on("click", function () {
        if (isGroupon == "True") {
            //alert($("#GrouponTime").date("grouponingseconds"));
            if ($("#GrouponTime").data("togrouponseconds") == "0" && $("#GrouponTime").data("grouponingseconds") > 0) {
                bntToolkit.confirm("该商品正在进行团购活动，随意下架可能需要承受不可预估的风险，确定要结束团购吗？", function () {
                    grouponExit($(this));
                });
            } else {
                //grouponExit($(this));
                bntToolkit.confirm("该商品已经参加过团购活动，是否确定再次参加团购活动？", function () {
                    grouponJoin($(this));
                });
            }
        } else {
            grouponJoin($(this));
        }
    });

    $("#GoodsType").on("change", function () {
        loadAttributes();
    });

    $("#AttributeList").on("click", ".attr-item", function () {
        loadSingleGoods();
    });

    $("#AssemblyList").on("change", ".stock", function () {
        var singleProductId = $(this).data("singe-product-id");
        var index = _.findIndex(singleGoodsStore, function (o) { return o.Id === singleProductId; });
        singleGoodsStore[index].Stock = $(this).val();
        var stock = $(this).val();
        singleGoodsStore[index].Stock = _.toInteger(stock);

    });

    $("#AssemblyList").on("change", ".unit", function () {
        var singleProductId = $(this).data("singe-product-id");
        var index = _.findIndex(singleGoodsStore, function (o) { return o.Id === singleProductId; });
        singleGoodsStore[index].Unit = $(this).val();
    });

    $("#AssemblyList").on("change", ".price", function () {
        var singleProductId = $(this).data("singe-product-id");
        var index = _.findIndex(singleGoodsStore, function (o) { return o.Id === singleProductId; });
        var price = $(this).val();
        singleGoodsStore[index].Price = _.toNumber(price);

        //取最小值显示到主表
        var min = _.minBy(singleGoodsStore, function (o) { return o.Price; });
        $("#ShopPrice").val(min.Price);
    });

    $("#AssemblyList").on("change", ".weight", function () {
        var singleProductId = $(this).data("singe-product-id");
        var index = _.findIndex(singleGoodsStore, function (o) { return o.Id == singleProductId; });
        var weight = $(this).val();
        singleGoodsStore[index].Weight = _.toNumber(weight);

    });

    $("#AssemblyList").on("change", ".grouponPrice", function () {
        var singleProductId = $(this).data("singe-product-id");
        var index = _.findIndex(singleGoodsStore, function (o) { return o.Id === singleProductId; });
        var grouponPrice = $(this).val();
        singleGoodsStore[index].GrouponPrice = _.toNumber(grouponPrice);

        //取最小值显示到主表
        var min = _.minBy(singleGoodsStore, function (o) { return o.GrouponPrice; });
        $("#GrouponPrice").val(min.GrouponPrice);
    });

    $.fn.editable.defaults.mode = 'inline';
    $.fn.editableform.loading = "<div class='editableform-loading'><i class='light-blue icon-2x icon-spinner icon-spin'></i></div>";
    $.fn.editableform.buttons = '<button type="submit" class="btn btn-info editable-submit"><i class="icon-ok icon-white"></i></button>' +
                                '<button type="button" class="btn editable-cancel"><i class="icon-remove"></i></button>';

    //初始化编辑数据
    loadAttributes();
});

function beforeSubmit(formData, jqForm, options) {
    if (_.isUndefined(_.find(singleGoodsStore, function (g) { return g.Stock > 0; }))) {
        bntToolkit.error("还没有选择任何商品属性或者库存全都为零");
        return false;
    }

    var index = _.findIndex(formData, function (o) { return o.name === 'SingleGoodsJson'; });
    formData[index].value = JSON.stringify(singleGoodsStore);
    return true;
}

// post-submit callback
function success(result, statusText, xhr, $form) {
    if (!result.Success) {
        bntToolkit.error(result.ErrorMessage);
    } else {
        location.href = url_list;
    }
}

function loadAttributes() {
    var goodsTypeId = $("#GoodsType").val();
    $("#AttributeList").html("");
    $("#AssemblyList").html("");

    if (!_.isEmpty(goodsTypeId))
        bntToolkit.post(load_goodsAttribute, { goodsTypeId }, function(result) {
            if (result.Success) {
                var render = "";
                for (var i = 0; i < result.Data.length; i++) {
                    var attr = result.Data[i];
                    render += '<div class="space-4"></div><div class="form-group"><label class="col-sm-1 control-label no-padding-right" for=""> ' + attr.Name + ' </label><div class="col-sm-9"><div class="clearfix">';
                    var vals = attr.Values.split(',');
                    for (var j = 0; j < vals.length; j++) {
                        var attrId = attr.Id;
                        var val = $.trim(vals[j]);
                        var currentAttr = _.find(currentAttrs, function (o) { return o.Id === attrId; });
                        var checked = false;
                        if (!_.isNull(currentAttr) && !_.isUndefined(currentAttr)) {
                            checked = !_.isUndefined(_.find(currentAttr.Vals, function (o) { return o === val; }));
                        }

                        render += '<div class="checkbox pull-left"><label><input type="checkbox" class="ace attr-item" data-attrId="' + attrId + '" ' + (checked ? 'checked="checked"' : '') + ' value="' + val + '" /><span class="lbl"> ' + val + '</span></label></div>';
                    }
                    render += '</div></div></div>';
                }
                $("#AttributeList").html(render);
                loadSingleGoods();
            } else {
                bntToolkit.error(result.ErrorMessage);
            }
        });
    else
        singleGoodsStore = [];
}

function loadSingleGoods() {
    selectedAttrs = [];
    $('#AttributeList input:checkbox:checked').each(function (i) {
        var attrId = $(this).data("attrid");
        var attrVal = $(this).val();
        var attrVals = { Id: attrId, Vals: [] };

        _.forEach(selectedAttrs, function (n, key) {
            if (attrId === n.Id) {
                attrVals = n;
            }
        });
        _.remove(selectedAttrs, function (n) {
            return attrId === n.Id;
        });
        attrVals.Vals.push(attrVal);
        selectedAttrs.push(attrVals);
    });

    //循环建立单品
    $("#AssemblyList").html('<div class="space-4"></div>');
    singleGoodsStore = [];
    eachAttrVals(selectedAttrs);

    $.each($("#AssemblyList img"), function (i, n) {
        editableImage($(n));
    });
}

function findCurrentSingleGoods(attrs) {
    return _.find(currentSingleGoods, function (o) {
        if (o.Attributes.length !== attrs.length) return false;
        var have = true;
        _.forEach(o.Attributes, function (n, key) {
            var notIn = _.isUndefined(_.find(attrs, function (a) {
                return a.Val === n.AttributeValue && a.Id === n.AttributeId;
            }));
            if (notIn) {
                have = false;
            }
            return !notIn;
        });

        return have;
    });
}

function eachAttrVals(surplusAttrs, joinVals) {
    if (_.isNull(joinVals) || _.isUndefined(joinVals))
        joinVals = [];
    if (surplusAttrs.length === 0) {
        if (joinVals.length === 0) return;
        var singleGoodsId = bntToolkit.guid();
        var currentGoods = findCurrentSingleGoods(joinVals);

        var stock = 0;
        var unit = '件';
        var price = 0.00;
        var image = null;
        var weight = 0.00;
        var grouponPrice = 0.00;
        if (!_.isUndefined(currentGoods)) {
            singleGoodsId = currentGoods.Id;
            stock = currentGoods.Stock;
            unit = currentGoods.Unit;
            price = currentGoods.Price;
            image = currentGoods.Image;
            weight = currentGoods.Weight;
            grouponPrice = currentGoods.GrouponPrice;
        }
        var singleGoods = { Id: singleGoodsId, Stock: stock, Unit: unit, Price: price, GrouponPrice: grouponPrice, Attrs: joinVals, Image: image, Weight: weight };
        singleGoodsStore.push(singleGoods);

        var render = '';
        var title = '';
        _.forEach(joinVals, function (n, key) {
            title += ' "' + n.Val + '" +';
        });
        title = _.trim(title, '+');
        var options = '';
        _.each(units, function (u) {
            options += '<option ' + (u == unit ? 'selected="selected"' : '') + '>' + u + '</option>';
        });

        var imageUrl = "/Resources/Common/Images/FileType/image.jpg";
        if (image != null)
            imageUrl = image.MediumThumbnail;
        render += '<div class="clearfix"><span class="profile-picture pull-left"><img data-singe-product-id="' + singleGoodsId + '" class="editable img-responsive" src="' + imageUrl + '" /></span><div class="pull-left singleGoodsInfo"><div>' + title + '</div><div>库存：<input class="stock" data-singe-product-id="' + singleGoodsId + '" style="width: 100px;" value="' + stock + '"/> 单位：<select class="unit" style="width: 50px;" data-singe-product-id="' + singleGoodsId + '" >' + options + '</select> 价格：<input class="price" style="width: 100px;" data-singe-product-id="' + singleGoodsId + '" value="' + price + '"/>团购价：<input class="grouponPrice" style="width: 100px;" data-singe-product-id="' + singleGoodsId + '" value="' + grouponPrice + '"/>重量：<input class="weight" style="width:100px;" data-singe-product-id="' + singleGoodsId + '" value="' + weight + '"/>KG</div></div></div><hr />';

        $("#AssemblyList").append(render);
        return;
    }

    var attr = _.first(surplusAttrs);
    var attrId = attr.Id;

    _.forEach(attr.Vals, function (n, key) {
        var temp = _.cloneDeep(joinVals);
        temp.push({ Id: attrId, Val: n });
        eachAttrVals(_.drop(surplusAttrs, 1), temp);
    });
}

function editableImage(ele) {

    // *** editable avatar *** //
    try {//ie8 throws some harmless exception, so let's catch it

        //it seems that editable plugin calls appendChild, and as Image doesn't have it, it causes errors on IE at unpredicted points
        //so let's have a fake appendChild for it!
        if (/msie\s*(8|7|6)/.test(navigator.userAgent.toLowerCase())) Image.prototype.appendChild = function (el) { }

        var last_gritter;
        ele.editable({
            type: 'image',
            name: 'singleGoodsImage',
            value: null,
            image: {
                //specify ace file input plugin's options here
                btn_choose: '更换图片',
                droppable: true,
                /**
                //this will override the default before_change that only accepts image files
                before_change: function(files, dropped) {
                    return true;
                },
                */

                //and a few extra ones here
                name: 'singleGoodsImage', //put the field name here as well, will be used inside the custom plugin
                max_size: 307200, //~100Kb
                on_error: function (code) { //on_error function will be called when the selected file has a problem
                    if (last_gritter) $.gritter.remove(last_gritter);
                    if (code == 1) { //file format error
                        last_gritter = $.gritter.add({
                            title: '请选择图片!',
                            text: '请选择图片格式的文件!',
                            class_name: 'gritter-error gritter-center'
                        });
                    } else if (code == 2) { //file size rror
                        last_gritter = $.gritter.add({
                            title: '文件太大了!',
                            text: '图片尺寸请控制在 300Kb 以内!',
                            class_name: 'gritter-error gritter-center'
                        });
                    } else {
                        //other error
                    }
                },
                on_success: function () {
                    $.gritter.removeAll();
                }
            },
            url: function (params) {
                // ***UPDATE AVATAR HERE*** //
                //You can replace the contents of this function with examples/profile-avatar-update.js for actual upload

                var deferred = new $.Deferred;

                //if value is empty, means no valid files were selected
                //but it may still be submitted by the plugin, because "" (empty string) is different from previous non-empty value whatever it was
                //so we return just here to prevent problems
                var value = ele.next().find('input[type=hidden]:eq(0)').val();
                if (!value || value.length == 0) {
                    deferred.resolve();
                    return deferred.promise();
                }

                if ("FileReader" in window) {
                    //var fileName = ele.next().find('.file-name').data('title');
                    //for browsers that have a thumbnail of selected image
                    //var thumb = ele.next().find('img').data('thumb');

                    var r = new FileReader();  //本地预览
                    r.onload = function () {
                        var fileName = ele.next().find('.file-name').data('title');

                        //base64字符串保存到后台
                        $.post("/BntWeb/Files/UploadedBase64Image", { fileName: fileName, data: r.result, isPublic: true, mediumThumbnailWidth: 468, mediumThumbnailHeight: 468, smallThumbnailWidth: 234, smallThumbnailHeight: 234, thumbnailType: 1 }).done(function (result) {
                            if (result.Success) {
                                ele.attr("src", result.Data.MediumThumbnail);
                                ele.data("fileId", result.Data.Id);

                                var singleProductId = ele.data("singe-product-id");
                                var index = _.findIndex(singleGoodsStore, function (o) { return o.Id === singleProductId; });
                                singleGoodsStore[index].Image = result.Data;

                                deferred.resolve({ 'status': 'OK' });
                            } else {
                                bntToolkit.error(result.ErrorMessage);
                            }
                        });

                    }
                    var file = ele.next().find('input[type="file"]')[0].files[0];
                    r.readAsDataURL(file);    //Base64
                }

                return deferred.promise();
            },

            success: function (response, newValue) {
            }
        });
    } catch (e) { }
}

function grouponJoin(cur) {
    if (cur.val() == 0) {
        var goodsId = $("#GoodsId").val();
        bntToolkit.post(url_grouponValidate, { goodsId: goodsId }, function (result) {
            if (result.Success) {
                bootbox.dialog({
                    title: "加入团购",
                    size: "small",
                    message: '<div class="row select-files">  ' +
                                '<div class="col-md-12"> ' +
                                '<div class="row-fluid">' +
                                '<div class="space-4"></div><div class="form-group"><label class="control-label no-padding-right" for="GrouponStartTime"> 开始时间 </label><div ><div class="clearfix"><input type="text" id="GrouponStartTime" name="GrouponStartTime" placeholder="开始时间" style="width: 250px;" value="" /></div></div></div>' +
                                '<div class="space-4"></div><div class="form-group"><label class="control-label no-padding-right" for="GrouponEndTime"> 结束时间 </label><div ><div class="clearfix"><input type="text" id="GrouponEndTime" name="GrouponEndTime" placeholder="结束时间" style="width: 250px;" value="" /></div></div></div>' +
                                '</div>' +
                                '</div></div>',
                    buttons: {
                        success: {
                            label: "确定",
                            className: "btn-success",
                            callback: function () {
                                bntToolkit.post(url_grouponJoin, { goodsId: goodsId, startTime: $("#GrouponStartTime").val(), endTime: $("#GrouponEndTime").val() }, function (result) {
                                    if (result.Success) {
                                        location.reload();
                                    } else {
                                        bntToolkit.error(result.ErrorMessage);
                                    }
                                });
                            }
                        }
                    }
                });
                var options = $.datepicker.regional["zh-CN"];
                $('#GrouponStartTime').datetimepicker({
                    todayBtn: "linked",
                    autoclose: true,
                    todayHighlight: true,
                    endDate: new Date()
                }).on('changeDate', function (e) {
                    var startTime = e.date;
                    $('#GrouponEndTime').datepicker('setStartDate', startTime);
                });
                $("#GrouponEndTime").datetimepicker(options);
            } else {
                bntToolkit.error(result.ErrorMessage);
            }
        });
    } else if (cur.val() == 1) {

    }
}

//退出团购
function grouponExit(cur) {
    bntToolkit.post(url_grouponExit, { goodsId: $("#GoodsId").val() }, function (result) {
        if (result.Success) {
            location.reload();
        } else {
            bntToolkit.error(result.ErrorMessage);
        }
    });
}

//团购计时
function time(cur, toGrouponSeconds, grouponingSeconds) {
    var seconds = toGrouponSeconds > 0 ? toGrouponSeconds : grouponingSeconds;
    var minutes = Math.floor(seconds / 60);
    var hours = Math.floor(minutes / 60);
    var days = Math.floor(hours / 24);
    var cDay = days;
    var cHour = hours % 24;
    var cMinute = minutes % 60;
    var cSecond = Math.floor(seconds % 60);//"%"是取余运算，可以理解为60进一后取余数，然后只要余数。
    if (cSecond < 10) {
        cSecond = "0" + cSecond;
    };
    if (cDay < 10) {
        cDay = "0" + cDay;
    };
    if (cHour < 10) {
        cHour = "0" + cHour;
    };
    if (cMinute < 10) {
        cMinute = "0" + cMinute;
    };
    if (toGrouponSeconds > 0) {
        //如果toGrouponSeconds>0则团购尚未开始，反之已经开始
        $(".clock").remove();
        cur.removeClass("btn-default").addClass("btn-warning").html('<i class="icon-group"></i>取消团购').after('<span class="clock">距离团购开始还剩<i><b>' + cDay + '</b></i> 天<i><b>' + cHour + '</b></i> 小时<i><b>' + cMinute + '</b></i> 分<i><b>' + cSecond + '</b></i> 秒</span>');

    } else if (grouponingSeconds < 0) {
        $(".clock").remove();
        cur.removeClass("btn-warning").removeClass("btn-default").addClass("btn-danger").html('<i class="icon-group"></i>团购结束');
    } else {
        $(".clock").remove();
        cur.removeClass("btn-warning").addClass("btn-danger").html('<i class="icon-group"></i>终止团购').after('<span class="clock">距离团购结束还剩<i><b>' + cDay + '</b></i> 天<i><b>' + cHour + '</b></i> 小时<i><b>' + cMinute + '</b></i> 分<i><b>' + cSecond + '</b></i> 秒</span></span>');
    }


    setTimeout(function () {
        if (toGrouponSeconds > 0) {
            $("#GrouponTime").data("toGrouponSeconds", --seconds);//实时更新开团倒计时给取消参团判断
            time(cur, seconds, grouponingSeconds);
        } else {
            time(cur, toGrouponSeconds, --grouponingSeconds);
        }

    }, 1000);
}