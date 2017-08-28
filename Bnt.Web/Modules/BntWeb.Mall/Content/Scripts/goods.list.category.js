var mainTreeFun = {
    beforeClick: function (treeId, treeNode) {
        return true;
    },
    onClick: function (e, treeId, treeNode) {
        var zTree = $.fn.zTree.getZTreeObj("mainCategoryTree"),
        nodes = zTree.getSelectedNodes(),
        v = "",
        cid = "";
        nodes.sort(function compare(a, b) { return a.id - b.id; });
        for (var i = 0, l = nodes.length; i < l; i++) {
            v += nodes[i].name + ",";
            cid += nodes[i].id + ",";
        }
        if (v.length > 0) v = v.substring(0, v.length - 1);
        if (cid.length > 0) cid = cid.substring(0, cid.length - 1);
        $("#CategoryName").attr("value", v);
        $("#CategoryId").attr("value", cid);

        mainTreeFun.hideMenu();
    },
    onBodyDown:function(event) {
        if (!(event.target.id == "menuBtn" || event.target.id == "CategoryName" || event.target.id == "menuContent" || $(event.target).parents("#menuContent").length > 0)) {
            mainTreeFun.hideMenu();
        }
    },
    showMenu: function () {
        var inputTypeName = $("#CategoryName");
        var cityOffset = $("#CategoryName").position();
        $("#menuContent").css({ left: cityOffset.left + "px", top: cityOffset.top + inputTypeName.outerHeight() + "px", width: inputTypeName.width() }).slideDown("fast");

        $("body").bind("mousedown", mainTreeFun.onBodyDown);
    },
    hideMenu:function() {
        $("#menuContent").fadeOut("fast");
        $("body").unbind("mousedown", mainTreeFun.onBodyDown);
    }
};
var settingMain = {
    view: {
                dblClickExpand: false
    },
        data: {
                simpleData: {
                    enable: true
                }
        },
        callback: {
                beforeClick: mainTreeFun.beforeClick,
                onClick: mainTreeFun.onClick
        }
    }



$(document).ready(function () {
    $.fn.zTree.init($("#mainCategoryTree"), settingMain, zNodes);

    //主分类设置选中当前节点
    var mainTree_Menu = $.fn.zTree.getZTreeObj("mainCategoryTree");
    var node = mainTree_Menu.getNodeByParam("id", $("#CategoryId").val());
    mainTree_Menu.selectNode(node);
    mainTree_Menu.expandAll(false);//展开所有节点


    $("body").on("click", ".allCategory", function () {
        $("#CategoryName").attr("value", "");
        $("#CategoryId").attr("value", "");
        mainTreeFun.hideMenu();
    });
});