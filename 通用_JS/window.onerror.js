// JavaScript source code

//reference:http://xbingoz.com/328.html

/**
 * 
 * 
 * @param {any} message 
 * @param {any} url 
 * @param {any} line 
 * @returns 
 */
window.onerror = function (message, url, line) {
    if (!url) return;
    var msg = {};

    //记录客户端环境
    msg.ua = window.navigator.userAgent;

    //只记录message里的message属性就好了，
    //错误信息可能会比较晦涩，有些信息完全无用，应酌情过滤
    msg.message = message.message;
    msg.url = url;
    msg.line = line;
    msg.page = window.location.href;

    var s = [];

    //将错误信息转换成字符串
    for (var key in msg) {
        s.push(key + '=' + msg[key]);
    }
    s = s.join('&');

    //这里是用增加标签的方法调用日志收集接口，优点是比较简洁。
    new Image().src = '/ajax-jserror.php?' + encodeURIComponent(s) + '&t=' + Math.random();
};