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
        $("#ParentId").attr("value", cid);

        mainTreeFun.hideMenu();
    },
    onBodyDown: function (event) {
        if (!(event.target.id == "menuBtn" || event.target.id == "CategoryName" || event.target.id == "menuContent" || $(event.target).parents("#menuContent").length > 0)) {
            mainTreeFun.hideMenu();
            extendTreeFun.hideMenu();
        }
    },
    showMenu: function () {
        extendTreeFun.hideMenu();
        var inputTypeName = $("#CategoryName");
        var cityOffset = $("#CategoryName").position();
        $("#menuContent").css({ left: cityOffset.left + "px", top: cityOffset.top + inputTypeName.outerHeight() + "px", width: inputTypeName.width() }).slideDown("fast");

        $("body").bind("mousedown", mainTreeFun.onBodyDown);
    },
    hideMenu: function () {
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

var extendTreeFun = {
    beforeClick: function (treeId, treeNode) {
        var zTree = $.fn.zTree.getZTreeObj("ExtendCategoryTree");
        zTree.checkNode(treeNode, !treeNode.checked, null, true);
        return false;
    },
    onCheck: function (e, treeId, treeNode) {
        var zTree = $.fn.zTree.getZTreeObj("ExtendCategoryTree"),
        nodes = zTree.getCheckedNodes(true),
        v = "",
        cid = "";
        nodes.sort(function compare(a, b) { return a.id - b.id; });
        for (var i = 0, l = nodes.length; i < l; i++) {
            v += nodes[i].name + ",";
            cid += nodes[i].id + ",";
        }
        if (v.length > 0) v = v.substring(0, v.length - 1);
        if (cid.length > 0) cid = cid.substring(0, cid.length - 1);
        $("#ExtendCategoryNames").attr("value", v);
        $("#ExtendCategoryIds").attr("value", cid);
    },
    onBodyDown: function (event) {
        if (!(event.target.id == "menuBtn" || event.target.id == "ExtendCategoryNames" || event.target.id == "extendMenuContent" || $(event.target).parents("#extendMenuContent").length > 0)) {
            mainTreeFun.hideMenu();
            extendTreeFun.hideMenu();
        }
    },
    showMenu: function () {
        mainTreeFun.hideMenu();
        var inputTypeName = $("#ExtendCategoryNames");
        var cityOffset = $("#ExtendCategoryNames").position();
        $("#extendMenuContent").css({ left: cityOffset.left + "px", top: cityOffset.top + inputTypeName.outerHeight() + "px", width: inputTypeName.width() }).slideDown("fast");

        $("body").bind("mousedown", extendTreeFun.onBodyDown);
    },
    hideMenu: function () {
        $("#extendMenuContent").fadeOut("fast");
        $("body").unbind("mousedown", extendTreeFun.onBodyDown);
    }
};
var settingExtend = {
    check: {
        enable: true,
        chkboxType: { "Y": "", "N": "" }
    },
    view: {
        dblClickExpand: false
    },
    data: {
        simpleData: {
            enable: true
        }
    },
    callback: {
        beforeClick: extendTreeFun.beforeClick,
        onCheck: extendTreeFun.onCheck
    }
}

$(document).ready(function () {
    $.fn.zTree.init($("#mainCategoryTree"), settingMain, zNodes);

    //主分类设置选中当前节点
    var mainTree_Menu = $.fn.zTree.getZTreeObj("mainCategoryTree");
    var node = mainTree_Menu.getNodeByParam("id", categoryId);
    mainTree_Menu.selectNode(node);
    mainTree_Menu.expandAll(true);//展开所有节点


    $.fn.zTree.init($("#ExtendCategoryTree"), settingExtend, zNodes);
    var extendTree_Menu = $.fn.zTree.getZTreeObj("ExtendCategoryTree");
    extendTree_Menu.expandAll(true);//展开所有节点
});