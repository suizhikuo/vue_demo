//在所有js文件之前放置，用于设置

"use strict"; //启用严格模式

//Use the function form of "use strict". (W097)
//遇到这问题是因为jslint建议将use strict包装在函数中,防止js在压缩合并过程中造成歧义。当然你也可以忽略掉这个警告。
//标准的写法
// (function (){
// 　　"use strict";
// 　　// some code here
// })();