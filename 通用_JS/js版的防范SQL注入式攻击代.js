//http://www.cnblogs.com/xiepeixing/archive/2012/11/13/2767733.html

/*
<script language="javascript">
<!--
var url = location.search;
var re=/^\?(.*)(select%20|insert%20|delete%20from%20|count\(|drop%20table|update%20truncate%20|asc\(|mid\(|char\(|xp_cmdshell|exec%20master|net%20localgroup%20administrators|\"|:|net%20user|\|%20or%20)(.*)$/gi;
var e = re.test(url);
if(e) {
    alert("地址中含有非法字符～");
    location.href="error.asp";
}
//-->
<script>
*/


var url = location.search;
var re = /^\?(.*)(select%20|insert%20|delete%20from%20|count\(|drop%20table|update%20truncate%20|asc\(|mid\(|char\(|xp_cmdshell|exec%20master|net%20localgroup%20administrators|\"|:|net%20user|\|%20or%20)(.*)$/gi;
var e = re.test(url);
if (e) {
    alert("地址中含有非法字符～");
    location.href = "error.HTML";
}
