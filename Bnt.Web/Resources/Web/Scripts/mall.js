//弹出错误信息提示
function errorMsgShow(obj,str){
    if (str && str !="") {
        obj.html(str);
        setTimeout(function(){
            obj.stop(true, true).fadeOut('300');
        },2000)
    }
    obj.stop(true, true).fadeIn('300');
}
function errorMsgHide(obj){
    obj.stop(true, true).fadeOut('300');
}

//产品详细数量加减
function amount(){
    $(".amount-increase").click(function(){
        var $this = $(this),
            $input = $this.parent().find('.amount-input'),
            num = parseInt($input.val());
        num+=1;
        var goods_number = parseInt($input.attr('data-stock'));
        if(num > goods_number){
            num = goods_number;
        }
        $input.val(num);
    });
    $(".amount-decrease").click(function(){
        var $this = $(this),
            $input = $this.parent().find('.amount-input'),
            num = parseInt($input.val());
        if (num==0 ) {return false;};
        if(num-1<1){num=1}else{num-=1}
        $input.val(num);
    });

    $('.amount-input').keyup(function(){
        $(this).val($(this).val().replace(/\D|^0/g,'')).css('ime-mode', 'disabled');
        var $this = $(this),
            num = $this.val(),
            goods_number = parseInt($this.attr('data-stock'));

        if(num > goods_number){
            num = goods_number;
           // Ctf_ctock_remind(goods_number,368,160);
        }
        
        $this.val(num);     

    });
    $('.amount-input').blur(function(){
        var $this = $(this),
            num = $this.val();
        if(num == ''){
            $this.val(1);
        }
     });
}

//购物车加减
function cartAmount(){
    $('.amount-increase').click(function(){
        var $this = $(this),
        $input = $this.parent().find('.amount-input'),
        id = $this.parents('.cart-tr').data('id'),
        num = parseInt($input.val());
        num+=1;
        $input.val(num);
        cartChange(id,num); //个数
        refreshMoney(); //价格变化
    })
    $('.amount-decrease').click(function(){
        var $this = $(this),
            $input = $this.parent().find('.amount-input'),
            id = $this.parents('.cart-tr').data('id'),
            num = parseInt($input.val());
        if (num==0 ) {return false;};
        if (num-1<1){num=1}else{num-=1;}
        $input.val(num);
        cartChange(id,num);
        refreshMoney();
    })

    $('.amount-input').keyup(function(){
        $(this).val($(this).val().replace(/\D|^0/g,'')).css('ime-mode', 'disabled');
        var $this = $(this),
            num = $this.val(),
            id = $this.parents('.cart-tr').data('id');
        $this.val(num);  
        cartChange(id,num);//个数
        refreshMoney();   

    });
    $('.amount-input').blur(function(){
        var $this = $(this),
            id = $this.parents('.cart-tr').data('id'),
            num = $this.val();
        if(num == ''){
            $this.val(1);
            cartChange(id,1);//个数
        }
    });
}


//加减购物产品
function cartChange(id,num){    
    console.log(num)
    // $.ajax({
    //     url: SITE_URL+"cart/addgs",
    //     type: 'POST',
    //     dataType: 'json',
    //     data: {tid:id,num:num,_scfs:$.cookie('_scfc')},
    //     success: function(result){
    //         if(result.status!=1){
    //             $("#amount"+parseInt(id)).val(result.gs);
    //             //alert(result.msg); 
    //             errorMsgShow($('.error-msg'),result.msg)               
    //             return false;
    //         }
    //     }
    // });   
}

//当触发价格发生变动的条件后, 调用此方法
function refreshMoney(){
    var _num = 0;
    var _price = 0;
    var  arr=0;
    var _ttnum = 0;
    $('.cart-tr').each(function(){
        var $this = $(this),
            number =  parseInt(Number($this.find('.amount-input').val()) ), //件数
            price = parseInt(Number($this.find('.z-pri').html()));  //单价
            _num += number;
            unitPrice = parseInt(number * price);
            $this.find('.sumprice').html(unitPrice);

        if ($this.find('input[type="checkbox"]')[0].checked) {
            if (price > 0) {
                _price += unitPrice;
                arr += ','+ $this.data('id') ; //获取产品id
            }
            var ttnum = 1;//商品个数
            _ttnum += ttnum;
        }
    });
    // $("#J_Count").html(_ttnum);
    $("#J_Total").html(_price);
    $("#J_idArr").val(arr);
}


//购物车删除
function cartDel(){
    //单个删除
    $(".cart-tr .detete").click(function(){
        var $this = $(this);
        if(confirm("确认要删除")){
            var id = $this.parents('.cart-tr').data('id');
            $("#del"+id) .parents('.cart-tr').remove();
           //  $("#del"+id) .parents('.cart-tr').remove();
           //  //删除单个产品
           //  $.ajax({
           //      url: SITE_URL+"cart/delone",
           //      type: 'POST',
           //      dataType: 'json',
           //      data: {id:id,_scfs:$.cookie('_scfc')},
           //      success: function(result){
           //           $("#del"+id) .parents('.cart-tr').remove();
           //      }
           // }); 
            refreshMoney();
        }
    });
    //选中删除
    $('#J_allDel').click(function(){
        if (confirm('你确定删除所选商品吗')) {
            var arr = '';
            $('.CheckBoxItem').each(function(){
                var $this = $(this);
                var id = $this.parents('.cart-tr').data('id');
                if (id) {
                   arr += $this.parents('.cart-tr').data('id') + ','; 
                }
                
            })
            arr = arr.substring(0, arr.length - 1);
            DeleteSelected(arr);//删除多个

            $('.CheckBoxItem').each(function(){
                var $this = $(this);
                if(this.checked){
                    $this.parents('.cart-tr').remove();
                }
            });
            refreshMoney();
        }
    });

}


//删除勾选
function DeleteSelected(id){
   // $.ajax({
   //      url: SITE_URL+" ",
   //      type: 'POST',
   //      dataType: 'json',
   //      data: {id:id,_scfs:$.cookie('_scfc')},
   //      success: function(result){

   //      }
   // }); 
} 

//购物车checkbox 
function CheckBox(){

    //全选
    $('.J_allselect').click(function(){
        var isSelect = this.checked;
        if (isSelect) {
            // $('.J_allselect').checked = true;
            $('.business').each(function() {
                $(this).find('input')[0].checked = true;
            });
            $('.name').each(function() {
                $(this).find('input')[0].checked = true;
            });
        } else {
            // $('.J_allselect').checked = false;
            $('.business').each(function() {
                $(this).find('input')[0].checked = false;
            });
            $('.name').each(function() {
                $(this).find('input')[0].checked = false;
            });
        }
        refreshMoney();
    })
    //商家选择
    $('.J_businessselect').click(function(){
        var isSelect = this.checked;
        if (isSelect){
            $(this).parents('tbody').find('.cart-tr').each(function() {
                $(this).find('input')[0].checked = true;
            });
        }else{
            $(this).parents('tbody').find('.cart-tr').each(function() {
                $(this).find('input')[0].checked = false;
            });
        }
        var flag =true;
        $('.J_businessselect').each(function(){
            if(!this.checked){
                 flag = false;
            }
        });
        if( flag ){
             $('.J_allselect').prop('checked', true );
        }else{
             $('.J_allselect').prop('checked', false );
        };
        refreshMoney();
    })
    //单选
    $('.CheckBoxItem').click(function(){
        var flag = true;
        $('.CheckBoxItem').each(function(){
            if(!this.checked){
                 flag = false;
            }
        });
        if( flag ){
             $('.J_allselect').prop('checked', true );
        }else{
             $('.J_allselect').prop('checked', false );
        };

        var businessflag = Array();
        $(this).parents('tbody').find('.cart-tr').each(function(){
            var _index = $(this).index()-1;
            businessflag[_index] = true;
            if(!$(this).find('input')[0].checked){
                businessflag[_index] = false;
            }
        });
        // if( $.inArray(false, businessflag) < 0  ){
        //     $(this).parents('tbody').find('.business').find('input')[0].checked = true;
        // }else{
        //     $(this).parents('tbody').find('.business').find('input')[0].checked = false;
        // };
        refreshMoney();
    })
}



function shopAction(){
    //加入购物车
    $('#J_Linkcart').click(function(){
        var $this = $(this),
            pid = 0,
            sum = $('.amount-input').val(),
            sort = $('#J_SaleProp li.select').data('value'),
            _url=SITE_URL+' ';
         $.ajax({
            url: _url,
            type:'post',
            data:{id:pid,prosum:sum,sort:sort},
            success: function(result){
                var cartNum = parseInt($('#J_CartNum').html());  
                $('#J_CartNum').html(cartNum+1);
                errorMsgShow($('.error-msg'),'加入购物车成功！')  
            }
        });
    })

    // 商品收藏
    $('#J_AddFavorite').click(function(){
        var $this = $(this),
            pid= 0;
       var _url= SITE_URL+'procoll/info';
        $.ajax({
            url: _url,
            type:'post',
            data:{id:pid},
            success: function(result){
                $this.find('.name').stop().html('已收藏');
                $this.find('.num').stop().html(parseInt($this.find('.num').html()+1));
                if(result.status==1){
                    //$this.find('span').stop().html('已收藏');
                }else{
                    //alert(result.msg);
                    return false;
                }
            }
          });          
    })

}


// 订单确认页 地址 
function addrCheck(){

    // 地址选择
    $('#J_addrlist label').click(function(){
        $('#J_addrlist td').stop().removeClass("on");
        $this = $(this);
        var isSelect = $this.find("input")[0].checked;
        if (isSelect) {
            $this.parent().stop().addClass("on");
        } else {
            $this.parent().stop().removeClass("on");
        }
    });

    // 设为默认地址  
    $(".default").click(function(){
        _this = $(this);
        var id = _this.parent().parent().data('id');

        if (_this.hasClass("isdefault")) {}else{
             var _url= SITE_URL+'member/setdef';
             var id=_this.attr("data-id");
            $.ajax({
                // url: _url,
                // type: 'post',
                // dataType: 'json',
                //data:{id:id,_scfc:$.cookie("_scfc")}
            })
            .done(function(result) {
                if(result.status==1){
                    $('#J_addrlist td').stop().removeClass("isdefault");
                    $('#J_addrlist td .default').stop().html("设为默认地址");
                    $("#default"+parseInt(id)).stop().html("默认地址");
                    $("#default"+parseInt(id)).parent().stop().addClass("isdefault");
                }else{
                    alert(result.msg);
                    return false;
                }
            });
        }
    })

    //添加新地址
    $(".newaddress").click(function(){
         $(".editbox").show();
     })
}

//订单 积分计算
function score(){
    var n = parseFloat($('.scoreinput').val());
    var score = parseFloat($('.order-points').data('score'));//总积分
    var scorenumber;
    var m = parseFloat(n);
    if(n <= 0){
        m = 0;
        $('.scoreinput').stop().val(m);
    }else if(n>score){
        m = score;
        $('.scoreinput').stop().val(m);
    }else{
        if (m>0) {
           if ($('#scorecheck').is(':checked') == true) {
               var scorenum = m/100;
               $('.score').stop().html(scorenum);
               var paynum = parseFloat($('.order-points').data('payable'))
               $('.payable').stop().html( (paynum - scorenum) )
           }else{
                var paynum = parseFloat($('.order-points').data('payable'))
                $(".payable").stop().html( paynum );
           }
           
        }
    }; 
}


//运费计算
function yfjs(){
     var sf ="";
     sf = $('.minfo-th input:checked').data('sf'); 
      $('#shpaddr').val($('.minfo-th input:checked').val());  
     var _url = SITE_URL+"proorder/yfjs" ;
     var zj=$('#yf').attr("data-zj");     
     if(parseFloat(sf)>0){ 
        $.ajax({
        url: _url,
        type: 'post',
        dataType: 'json',
        data:{sfid:sf}
        })
        .done(function(result) {
            if(result.status==1){
                var price=parseFloat(result.price);
                $('#yf').html("￥"+price);  
                $(".order-points").attr('data-payable',parseFloat(zj)+parseFloat(price))      
                 $('#payable').html(parseFloat(zj)+parseFloat(price)); 
                 
                return false;
            }else{
              $('#yf').html(result.msg); 
               $('#payable').html(parseFloat(zj));  
               $(".order-points").attr('data-payable',parseFloat(zj))           
             return false;
            }
        });
     }
} 
//使用优惠券计算
function yhjs(){
    $("#J_Promotion").change(function(){
        var $this = $(this),
            id = $this.val(),
            zj= $('#yf').attr("data-zj");
            yf = $('#yf').attr("data-yf");
            yh = $this.find("option:selected").data('price'),
            price = Number(zj)+Number(yf);
            $('#yh').html(parseFloat(yh).toFixed(2));
            $('#payable').html(parseFloat(price)-parseFloat(yh)); 
    });
} 

//换一批效果
function refresh(Listobj,btn){  
    var objUl = Listobj.find("ul");
    var objBtn = btn;
    var liN = objUl.length;
    objUl.eq(0).show();
    //执行效果
    var rf = 1;
    function ulRefresh(i){
        objUl.eq(i).show().siblings("ul").hide();
    }
    objBtn.click(function(){
            ulRefresh(rf);
            rf++;
            if(rf==liN){rf=0;}
    })

}


//团购时间
function saletime(){
  $('.saletime').each(function(){
        var lxfday=$(this).attr('lxfday');//用来判断是否显示天数的变量
        var endtime = new Date($(this).attr('endtime')).getTime();//取结束日期(毫秒值)
        var nowtime = new Date().getTime();        //今天的日期(毫秒值)
        var youtime = endtime-nowtime;//还有多久(毫秒值)
        var seconds = youtime/1000;
        var minutes = Math.floor(seconds/60);
        var hours = Math.floor(minutes/60);
        var days = Math.floor(hours/24);
        var CDay= days ;
        var CHour= hours % 24;
        var CMinute= minutes % 60;
        var CSecond= Math.floor(seconds%60);//'%'是取余运算，可以理解为60进一后取余数，然后只要余数。
        if(endtime<=nowtime){
                    $(this).html('已过期')//如果结束日期小于当前日期就提示过期啦
                }else{
                    $(this).html('<span>'+days+'</span>天<span>'+CHour+'</span>小时<span>'+CMinute+'</span>分<span>'+CSecond+'</span>秒');  //输出数据
                }
  });
    setTimeout('saletime()',1000);
};




// 店铺收藏
$('#JScollect').click(function(){
    console.log("已收藏")
    var $this = $(this),
        pid= 0;
    var _url= SITE_URL+'procoll/info';
    $.ajax({
        url: _url,
        type:'post',
        data:{id:pid},
        success: function(result){
            $this.stop().html('已收藏');
            if(result.status==1){
                //$this.find('span').stop().html('已收藏');
            }else{
                //alert(result.msg);
                return false;
            }
        }
      });          
})