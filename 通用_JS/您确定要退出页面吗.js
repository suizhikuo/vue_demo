﻿//jquery离开页面弹出提示代码：
//绑定beforeunload事件
$(window).bind('beforeunload', function () { return '您输入的内容尚未保存，确定离开此页面吗？'; });
//解除绑定，一般放在提交触发事件中$(window).unbind('beforeunload');


//js离开页面提示代码如下：
window.onbeforeunload = function (event) { return confirm("确定离开此页面吗？"); }