
var setting = {
    async: {
        enable: true,
        url: url_load,
        autoParam: ["Id=pid"],
        dataFilter: filter
    },
    view: {
        expandSpeed: "",
        addHoverDom: addHoverDom,
        removeHoverDom: removeHoverDom,
        selectedMulti: false
    },
    callback: {
        onAsyncSuccess: onAsyncSuccess,
        beforeClick: beforeClick,
        onClick: onClick
    }
};

var newCount = 1;
function addHoverDom(treeId, treeNode) {
    if (treeNode.Level >= 4 || !canEditDistrict) return;

    var sObj = $("#" + treeNode.tId + "_span");
    if (treeNode.editNameFlag || $("#AddBtn_" + treeNode.tId).length > 0) return;
    var addStr = "<span class='button add' id='AddBtn_" + treeNode.tId
        + "' title='add node' onfocus='this.blur();'></span>";
    sObj.after(addStr);
    var btn = $("#AddBtn_" + treeNode.tId);
    if (btn) btn.bind("click", function () {
        zTree.selectNode(treeNode);

        //新建节点
        $("#ParentId").val(treeNode.Id);
        var parent = treeNode.MergerShortName;
        if (parent.lastIndexOf(',') === parent.length)
            parent = parent.substring(0, parent.lastIndexOf(','));
        if (parent === "") parent = "中国";
        $("#Parent").val(parent);

        $("#Id").removeAttr("readonly");
        $("#Id").val("");
        $("#FullName").val("");
        $("#ShortName").val("");
        $("#Lng").val("");
        $("#Lat").val("");
        $("#Sort").val(0);

        $("#EditMode").val(0);

        $("#DeleteButton").hide();

        return false;
    });
};
function removeHoverDom(treeId, treeNode) {
    $("#AddBtn_" + treeNode.tId).unbind().remove();
};

function onAsyncSuccess(event, treeId, msg) {
    var nodes = zTree.getNodes();
    zTree.expandNode(nodes[0], true);
}

function beforeClick(treeId, treeNode) {
    if (treeNode.Id === "0") return false;
    return true;
}

function onClick(event, treeId, treeNode, clickFlag) {
    if (treeNode.Id === "0") return;
    if (!canEditDistrict) {
        $("#DistrictForm input").attr("readonly", "readonly");
    }

    $("#ParentId").val(treeNode.ParentId);
    var parent = treeNode.MergerShortName.replace(treeNode.ShortName, "");
    if (parent.lastIndexOf(',') === parent.length - 1)
        parent = parent.substring(0, parent.lastIndexOf(','));
    if (parent === "") parent = "中国";
    $("#Parent").val(parent);

    $("#Id").attr("readonly", "readonly");
    $("#Id").val(treeNode.Id);
    $("#FullName").val(treeNode.FullName);
    $("#ShortName").val(treeNode.ShortName);
    $("#Lng").val(treeNode.Lng);
    $("#Lat").val(treeNode.Lat);
    $("#Sort").val(treeNode.Sort);

    $("#EditMode").val(1);

    $("#DeleteButton").show();

    $("#DistrictForm").valid();
}

function filter(treeId, parentNode, childNodes) {
    if (!childNodes) return null;
    for (var i = 0, l = childNodes.length; i < l; i++) {

        childNodes[i].pId = childNodes[i].ParentId;
        childNodes[i].name = childNodes[i].ShortName;
        childNodes[i].isParent = childNodes[i].Level !== 4;
        childNodes[i].icon = iconOpen;
        childNodes[i].iconOpen = iconOpen;
        childNodes[i].iconClose = iconClose;
    }
    return childNodes;
}

var zTree;
jQuery(function ($) {
    zTree = $.fn.zTree.init($("#tree"), setting);

    bntToolkit.initForm($("#DistrictForm"), {
        Id: {
            required: true,
            digits: true
        },
        FullName: {
            required: true
        },
        ShortName: {
            required: true
        },
        Lng: {
            required: true,
            number: true
        },
        Lat: {
            required: true,
            number: true
        },
        Sort: {
            required: true,
            digits: true
        }
    }, null, success);

    if (!canEditDistrict) {
        $("#DistrictForm input").attr("readonly", "readonly");
    }

    $("#DeleteButton").click(function () {
        bntToolkit.confirm("删除后不可恢复，确定还要删除该行政区吗？", function () {
            bntToolkit.post(url_deleteDistrict, { districtId: $("#Id").val() }, function (result) {
                if (result.Success) {
                    //删除节点
                    var nodes = zTree.getSelectedNodes();
                    if (nodes.length > 0)
                        zTree.removeNode(nodes[0]);
                } else {
                    bntToolkit.error(result.ErrorMessage);
                }
            });
        });
    });
});

// post-submit callback
function success(result, statusText, xhr, $form) {
    if (!result.Success) {
        bntToolkit.error(result.ErrorMessage);
    } else {
        //刷新节点
        var nodes = zTree.getSelectedNodes();
        if (nodes.length > 0) {
            var currentNode = nodes[0];
            if ($("#EditMode").val() === "1") {
                //更新本地字段
                currentNode.name = $("#ShortName").val();

                currentNode.FullName = $("#FullName").val();
                currentNode.ShortName = $("#ShortName").val();
                currentNode.Lng = $("#Lng").val();
                currentNode.Lat = $("#Lat").val();
                currentNode.Sort = $("#Sort").val();

                zTree.updateNode(currentNode);
            }
            zTree.reAsyncChildNodes(currentNode, "refresh");
        }
    }
}

$(window).resize(function () {
    $("#treeParent").attr("style", "max-height:" + ($(window).height() - 200) + "px;overflow-y:auto;");
});
$(window).trigger("resize");