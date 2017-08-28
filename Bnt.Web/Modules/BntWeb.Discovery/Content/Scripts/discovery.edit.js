jQuery(function ($) {
    
    $("#selGoods").click(function () {
        bootbox.dialog({
            message: '<div class="row select-files">  ' +
                '<div class="col-md-12"> ' +
                '<div class="row-fluid">' +
                '<div class="space-4"></div>' +
                '<div class="form-group">' +
                '<div class="col-sm-9">' +
                '<span class="input-icon"><input type="text" placeholder="请输入 ..." class="nav-search-input" id="SearchInput" autocomplete="off"><i class="icon-search nav-search-icon"></i></span><button class="btn btn-xs btn-primary" onclick="search();" >查询</button>' +
                '</div>' +
                '<div class="col-sm-3"><div class="clearfix"><label><input name="selAll" type="checkbox" class="ace" onclick="changeSelectAll();"><span class="lbl"> 全选</span></label></div></div>' +
                '</div>' +
                '<div class="space-4"></div>' +
                '<div class="form-group"><div id="SearchResult"></div></div>' +
                '</div>' +
                '</div></div>',
            title: "选择商品",
            buttons: {
                Cancel: {
                    label: "取消",
                    className: "btn-default",
                    callback: function () {

                    }
                },
                OK: {
                    label: "添加",
                    className: "btn-primary",
                    callback: function () {
                        $('input[name="goodscheck"]:checked').each(function () {
                            var goodsId = $(this).val();
                            var mem = $('#GoodsBox').find("input:checkbox[value='" + goodsId + "']");
                            if (mem.length == 0) {
                                var render = '<label><input name="Goods" type="checkbox" class="ace " value="' + goodsId + '" checked="checked" onclick="$(this).parent().remove();"><span class="lbl"> ' + $(this).parent().find(".lbl").text() + '</span></label>';
                                $("#GoodsBox").append(render);
                            }
                        });
                    }
                }
            }
        });
    });
});

function search() {
    var keyword = $("#SearchInput").val();
    if (!(keyword == "")) {
        bntToolkit.post(url_search, { keyword: keyword }, function (result) {
            if (result.Success) {
                if (result.Data != null) {
                    var render = '';
                    var goodsArray = result.Data;
                    for (var i = 0; i < goodsArray.length; i++) {
                        render += '<label><input name="goodscheck" type="checkbox" class="ace" value="' + goodsArray[i].Id + '"><span class="lbl"> ' + goodsArray[i].Name + '</span></label>';
                    }
                    $("#SearchResult").html(render);
                }
            }
        });
    }
}

function changeSelectAll() {
    $("input[name='goodscheck']").prop("checked", !!$("input[name='selAll']").prop("checked"));
}





