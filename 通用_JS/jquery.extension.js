/* jquery.expend.js
* 功 能： jquery框架拓展
* aking 2013-07-02
*/

//jQuery expand 生成GUID
(function (window, undefined) {
    jQuery.newGuid = function () {
        var guid = "";
        for (var i = 1; i <= 32; i++) {
            var n = Math.floor(Math.random() * 16.0).toString(16);
            guid += n;
            if ((i == 8) || (i == 12) || (i == 16) || (i == 20))
                guid += "-";
        }
        return guid;
    };
    //判断字符串是否为空或null
    jQuery.isNullOrEmpty = function (str) {
        if (str != null && typeof ("undefined") != str && new String($.trim(str)).length > 0) {
            return false;
        } else { return true; }
    };
    //将时间戳转成时间格式
    jQuery.changeDateFormat = function (time) {
        if (time != null && typeof time == "string") {
            if (time.indexOf("/Date(") > -1) {
                var date = new Date(parseInt(time.replace("/Date(", "").replace(")/", ""), 10));
                var month = date.getMonth() + 1 < 10 ? "0" + (date.getMonth() + 1) : date.getMonth() + 1;
                var currentDate = date.getDate() < 10 ? "0" + date.getDate() : date.getDate();
                var hours = date.getHours();
                var minutes = date.getMinutes();
                var seconds = date.getSeconds();
                return date.getFullYear() + "-" + month + "-" + currentDate + " " + hours + ":" + minutes + ":" + seconds;
            }
            return time;
        }
        return "";
    };

    //格式化金额
    jQuery.Fmoney = function (s, n) {
        n = n > 0 && n <= 20 ? n : 2;
        s = parseFloat((s + "").replace(/[^\d\.-]/g, "")).toFixed(n) + "";
        var l = s.split(".")[0].split("").reverse(), r = s.split(".")[1];
        t = "";
        for (i = 0; i < l.length; i++)
            t += l[i] + ((i + 1) % 3 == 0 && (i + 1) != l.length ? "," : "");
        return t.split("").reverse().join("") + "." + r;
    };
    //还原金额
    jQuery.Rmoney = function (s) {
        return parseFloat(s.replace(/[^\d\.-]/g, ""));
    };
    //将josn 字符串转换成Josn 对象
    jQuery.O2Josn = function (data) {
        try {
            return eval("(" + data + ")");
        } catch (e) {
            //alert("数据转换失败\r\n错误:" + e);
            return null;
        }
    };
    //将josn对象转成字符串
    jQuery.O2String = function (O) {
        try {
            switch (typeof (O)) {
                case "object":
                    var ret = [];
                    if (O instanceof Array) {
                        for (var i = 0, len = O.length; i < len; i++) {
                            ret.push($.O2String(O[i]));
                        }
                        return "[" + ret.join(',') + "]";
                    }
                    else if (O instanceof RegExp) {
                        return O.toString();
                    }
                    else {
                        for (var a in O) {
                            ret.push(a + ":" + $.O2String(O[a]));
                        }
                        return "{" + ret.join(',') + "}";
                    }
                case "function":
                    return "function() {}";
                case "number":
                    return O.toString();
                case "string":
                    {
                        if (O.indexOf("/Date(") > -1) {
                            return "\"" + $.changeDateFormat(O) + "\"";
                        } else {
                            return "\"" + O.replace(/(\\|\")/g, "\\$1").replace(/\n|\r|\t/g, function (a) {
                                return ("\n" == a) ? "\\n" : ("\r" == a) ? "\\r" : ("\t" == a) ? "\\t" : "";
                            }) + "\"";
                        }
                    }
                case "boolean":
                    return O.toString();
                default: {
                    if ($.isNullOrEmpty(O)) return "";
                    else
                        return O.toString();
                }
            }
        } catch (e) { alert(e.message); }
        return null;
    };
    //将josn对象转成字符串
    jQuery.O2String1 = function (obj) {
        if (obj == null) return null;
        switch (obj.constructor) {
            case Object:
                var str = "{";
                for (var o in obj) {
                    str += o + ":" + $.O2String(obj[o]) + ",";
                }
                if (str.substr(str.length - 1) == ",")
                    str = str.substr(0, str.length - 1);
                return str + "}";
                break;
            case Array:
                var str = "[";
                for (var o in obj) {
                    str += $.O2String(obj[o]) + ",";
                }
                if (str.substr(str.length - 1) == ",")
                    str = str.substr(0, str.length - 1);
                return str + "]";
                break;
            case Boolean:
                return obj.toString();
                break;
            case Date:
                return "\"" + obj.toString() + "\"";
                break;
            case Function:
                break;
            case Number:
                return obj.toString();
                break;
            case String:
                if (obj.indexOf("/Date(") > -1) {
                    return "\"" + $.changeDateFormat(obj) + "\"";
                } else {
                    return "\"" + obj.replace(/(\\|\")/g, "\\$1").replace(/\n|\r|\t/g, function (a) {
                        return ("\n" == a) ? "\\n" : ("\r" == a) ? "\\r" : ("\t" == a) ? "\\t" : "";
                    }) + "\"";
                }
                break;
            default:
                return obj;
                break;
        }
    };
    //集合排序（参数：list为集合、propertyName为排序依据的名称）
    jQuery.listSort = function (list, propertyName) {
        try {
            var i = 0, len = list.length, j, d;
            for (; i < len; i++) {
                for (j = 0; j < len; j++) {
                    if (list[i][propertyName] < list[j][propertyName]) {
                        d = list[j];
                        list[j] = list[i];
                        list[i] = d;
                    }
                }
            }
            return list;
        } catch (e) { }
        return null;
    };
    //验证文本框是否为empty
    $.fn.CheckRequired = function () {
        if ($.trim(this.val()).length <= 0) { return false; }
        else { return true };
    };
    //手机号码检测
    $.fn.CheckMobile = function () {
        var Pho = /(^0{0,1}1[3|4|5|6|7|8|9][0-9]{9}$)/;
        return Pho.test(this.val());
    };
    //邮箱检测
    $.fn.CheckEmail = function () {
        var reEmail = /^\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$/;
        return reEmail.test(this.val());
    };
    //验证只能是数字
    $.fn.CheckNum = function () {
        var reg = /^\d+$/;
        return reg.test(this.val());
    };
    //验证金额
    $.fn.CheckMoney = function () {
        var reg = /^[0-9]*(\.[0-9]{1,2})?$/;
        return reg.test(this.val());
    };
    //验证金额并且为必填
    $.fn.CheckMoney_Required = function () {
        if ($.trim(this.val()).length <= 0)
            return false;
        var reg = /^[0-9]*(\.[0-9]{1,2})?$/;
        return reg.test(this.val());
    };
    //验证电话
    $.fn.CheckPhone = function () {
        var reg = /^\d{11}|\d{7,8}|(\d{4}|\d{3})-(\d{7,8})|(\d{4}|\d{3})-(\d{7,8})-(\d{4}|\d{3})|(\d{7,8})-(\d{4}|\d{3})|(\d{3})-(\d{3})-(\d{4})/;
        if (this.val().match(reg) == null) { return false; } else { return true; }
    };
    //网址验证
    $.fn.CheckUrl = function () {
        var strRegex = "^((https|http|ftp|rtsp|mms)?://)"
                         + "?(([0-9a-zA-Z_!~*'().&=+$%-]+: )?[0-9a-zA-Z_!~*'().&=+$%-]+@)?" //ftp的user@    
                         + "(([0-9]{1,3}\.){3}[0-9]{1,3}" // IP形式的URL- 199.194.52.184    
                         + "|" // 允许IP和DOMAIN（域名）    
                         + "([0-9a-zA-Z_!~*'()-]+\.)*" // 域名- www.    
                         + "([0-9a-zA-Z][0-9a-zA-Z-]{0,61})?[0-9a-zA-Z]\." // 二级域名    
                         + "[a-zA-Z]{2,6})" // first level domain- .com or .museum    
                         + "(:[0-9]{1,4})?" // 端口- :80    
                         + "((/?)|"
                         + "(/[0-9a-zA-Z_!~*'().;?:@&=+$,%#-]+)+/?)$";

        return this.val().match(strRegex);
    };
    //验证邮编
    $.fn.CheckPostCode = function () {
        var reg = /[0-9]\d{5}(?!\d)/;
        if (this.val().match(reg) == null) { return false; } else { return true; }
    };
    //判断字符长度
    $.fn.CheckLength = function (len) {
        var lens = 0;
        var zf = this.val().split("");
        for (var i = 0; i < zf.length; i++) {
            if (zf[i].charCodeAt(0) < 299) {
                lens++;
            } else {
                lens += 2;
            }
        }
        if (lens > len) return false;
        return true;
    };

    $.fn.autosize = function () {
        $(this).height('0px');
        var setheight = $(this).get(0).scrollHeight;
        if ($(this).attr("_height") != setheight)
            $(this).height(setheight + "px").attr("_height", setheight);
        else
            $(this).height($(this).attr("_height") + "px");
    };

})(window);


//防止IE浏览器中的内存泄漏
jQuery(window).bind("unload", function () {
    try {
        //jQuery("*").add(document).unbind();
        //.removeClass().removeAttr("style");
        if (typeof (art) != "undefined" && art != null) {
            delete art.dialog;
            art = null;
        }

        CollectGarbage();

    } catch (e) { }
});
