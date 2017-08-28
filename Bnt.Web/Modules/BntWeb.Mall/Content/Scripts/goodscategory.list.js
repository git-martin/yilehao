
var renderItem = function (category, render) {
    renderIndex++;
    var childs = category.ChildCategories;

    render += '<li class="dd-item"><div class="dd-handle">' +  category.Name + '<div class="pull-right action-buttons"><a class="blue" href="' + url_edit + '?categoryId=' + category.Id + '" title="编辑"><i class="icon-pencil bigger-130"></i></a>';

    if (category.Level < 3)
        render += '<a class="blue" href="' + url_create + '?parentId=' + category.Id + '" title="添加子分类"><i class="icon-plus bigger-130"></i></a>';


    if (canEditCarousel) {
        var url = url_addCarousel.replace('%5BsourceId%5D', category.Id).replace('%5BsourceTitle%5D', category.Name.substring(0, 10)).replace('%5BviewUrl%5D', "");
        render += '<a class="blue" data-id="' + category.Id + '" href="' + url + '" title="加入轮播"><i class="icon-magic bigger-130"></i></a>';
    }

    if (canEditAdvert) {
        var url = url_sendAdvert.replace('%5BsourceId%5D', category.Id).replace('%5BsourceTitle%5D', category.Name).replace('%5BviewUrl%5D', "");
        render += '<a class="blue" data-id="' + category.Id + '" href="' + url + '" title="设为广告"><i class="icon-barcode bigger-130"></i></a>';
    }


    render += '<a class="red delete" href="#" data-id="' + category.Id + '" title="删除"><i class="icon-trash bigger-130"></i></a></div></div>';


    if (childs != null && childs.length > 0) {
        childs = _.orderBy(childs, ['Sort'], ['desc']);
        render += '<ol class="dd-list">';
        for (var i = 0; i < childs.length; i++) {
            var categoryInner = childs[i];
            render = renderItem(categoryInner, render);
        }
        render += '</ol>';
    }
    render += '</li>';
    return render;
}
var renderIndex = 0;
var load = function (index) {
    var pageSize = 10;
    bntToolkit.post(url_loadPage, { pageIndex: index, pageSize: pageSize }, function (result) {
        if (result.Success) {
            var categories = result.Data.Categories;
            var render = '<div class="dd" id="list_' + index + '" style="max-width: 100%;"><ol class="dd-list">';
            for (var i = 0; i < categories.length; i++) {
                var category = categories[i];
                renderIndex = 0;
                render = renderItem(category, render);
            }
            render += '<ol class="dd-list"></div>';

            $("#CategoryList").html(render);
            $("#pageDiv").html(bntToolkit.buildPagination(index, pageSize, result.Data.TotalCount));

            $('#list_' + index).nestable({ handleClass: "empty", collapseAll: true });

            $('.dd-handle a').on('mousedown', function (e) {
                e.stopPropagation();
            });

            $('[data-rel="tooltip"]').tooltip();
            $('#list_' + index).nestable("collapseAll");

        } else {
            bntToolkit.error(result.ErrorMessage);
        }
    });
}

var currentPageIndex = 1;
jQuery(function ($) {

    load(1);

    $("#CategoryList").on("click", ".delete", function () {
        var id = $(this).data("id");

        bntToolkit.confirm("删除后不可恢复，确定还要删除该分类吗？", function () {
            bntToolkit.post(url_delete, { categoryId: id }, function (result) {
                if (result.Success) {

                    load(currentPageIndex);
                } else {
                    bntToolkit.error(result.ErrorMessage);
                }
            });
        });
    });

    $("#pageDiv").on("click", "li[class!=disabled]", function () {
        currentPageIndex = $(this).data("index");
        load(currentPageIndex);
    });
});