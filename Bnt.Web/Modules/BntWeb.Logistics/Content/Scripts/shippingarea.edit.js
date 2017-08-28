

jQuery(function ($) {
    bntToolkit.initForm($("#ShippingAreaForm"), {
    }, null, success);

    $("#dllDrovince").change(function () {
        var val = $(this).val();
        if (val != "0") {
            $.ajax({
                type: "Get",
                url: url_loadDistrict + "?parentId=" + val,
                dataType: "json",
                success: function (data) {
                    $("#dllCity").html("<option value=\"0\">请选择市</option>" + data);
                }
            });

        } else {
            $("#dllCity").html("<option value=\"0\">请选择市</option>");
            //$("#dllArea").html("<option value=\"0\">请选择区</option>");
        }
    });

    //$("#dllCity").change(function () {
    //    var val = $(this).val();
    //    if (val != "0") {
    //        $.ajax({
    //            type: "Get",
    //            url: url_loadDistrict + "?parentId=" + val,
    //            dataType: "json",
    //            success: function (data) {
    //                $("#dllArea").html("<option value=\"0\">请选择区</option>" + data);
    //            }
    //        });
    //    } else {
    //        $("#dllArea").html("<option value=\"0\">请选择区</option>");
    //    }
    //});

    $('#ShippingAreaForm').on("click", "#addArea", function (e) {
        var areaId;
        var areaName;
        //if ($("#dllArea").val() != "0") {
        //    areaId = $("#dllArea").val();
        //    areaName = $("#dllArea").find("option:selected").text();
        //} else
        if ($("#dllCity").val() != "0") {
            areaId = $("#dllCity").val();
            areaName = $("#dllCity").find("option:selected").text();
        }
        else if ($("#dllDrovince").val() != "0") {
            areaId = $("#dllDrovince").val();
            areaName = $("#dllDrovince").find("option:selected").text();
        } else {
            bntToolkit.error("请选择区域");
            return false;
        }

        $("#AreaBox").append("<label><input name=\"AreaId\" type=\"checkbox\" class=\"ace\" checked value=\"" + areaId + "\"><span class=\"lbl\"> " + areaName + "</span></label>");
    });

    //点击选择市区
    $('.addrbox li span').on('click', function () {
        $('.addrbox .box').removeClass('db');
        $(this).parents('li').find('.box').addClass('db');
    })
    //市区全选
    $('.addrbox .listcheck').on('change', function () {
        if ($(this).is(':checked')) {
            $(this).next('.box').find('.childcheck').prop('checked', true);
        } else {
            $(this).next('.box').find('.childcheck').prop('checked', false);
        }
    });
    //关闭市区
    $('.closebox').on('click', function () {
        $(this).parents('.box').removeClass('db')
    })
    //全选
    $('.addrbox .allcheck').on('change', function () {
        if ($(this).is(':checked')) {
            $('.listcheck').prop('checked', true);
            $('.childcheck').prop('checked', true);
        } else {
            $('.listcheck').prop('checked', false);
            $('.childcheck').prop('checked', false);
        }
    })
    //关闭
    $('.addrbox .close').on('click', function () {
        $('.addrbox').hide();
    })

});

function success(result, statusText, xhr, $form) {
    if (!result.Success) {
        bntToolkit.error(result.ErrorMessage);
    } else {
        location.href = url_loadPage;
    }
}
