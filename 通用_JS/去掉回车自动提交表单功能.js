//回车换Tab 方便，实现tab功能，屏蔽回车提交
//<script type="text/javascript"> 
document.onkeydown = function enterToTab() {
    if (event.srcElement.type != 'submit' && event.srcElement.type != "image" && event.srcElement.type != 'textarea'
        && event.keyCode == 13)
        event.keyCode = 9;
}
//</script>




//<script type="text/javascript">     
//JS监听整个页面的回车事件--作用:阻止回车出发submit按钮
document.onkeydown = keyDownSearch;
function keyDownSearch(e) {
    // 兼容FF和IE和Opera  
    var theEvent = e || window.event;
    var code = theEvent.keyCode || theEvent.which || theEvent.charCode;
    if (code == 13) {
        //var eventSrc = theEvent.target||theEvent.srcElement;
        //eventSrc.blur();//失去焦点

        code = 9;//回车执行tab事件
        return false;// 取消回车提交表单的默认的提交行为  
    }
    return true;
}
//</script>


//jquery写法
//兼容所有浏览楼主可以试试
//<script type="text/javascript" src="Scripts/jquery-1.4.4.min.js"></script>
//<script type="text/javascript">
$(function () {
    $("#form1 input:text").keypress(function (e) {
        if (e.which == 13) {// 判断所按是否回车键  
            var inputs = $("#form1 input:text"); // 获取表单中的所有输入框  
            var idx = inputs.index(this); // 获取当前焦点输入框所处的位置  
            if (idx == inputs.length - 1) {// 判断是否是最后一个输入框  
                if (confirm("最后一个输入框已经输入,是否提交?")) // 用户确认  
                    $("form[name='contractForm']").submit(); // 提交表单  
            } else {
                inputs[idx + 1].focus(); // 设置焦点  
                inputs[idx + 1].select(); // 选中文字  
            }
            return false; // 取消默认的提交行为  
        }
    });
});
//</script>
