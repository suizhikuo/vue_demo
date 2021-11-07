// 表单验证

//构造函数
var PageValidate = function (RegPhone, RegNumber, RegNumberSign, RegDecimal, RegDecimalSign, RegEmail, RegCHZN) {
    if (RegPhone)
        this.RegPhone = RegPhone;
    if (RegNumber)
        this.RegNumber = RegNumber;
    if (RegNumberSign)
        this.RegNumberSign = RegNumberSign;
    if (RegDecimal)
        this.RegDecimal = RegDecimal;
    if (RegDecimalSign)
        this.RegDecimalSign = RegDecimalSign;
    if (RegEmail)
        this.RegEmail = RegEmail;
    if (RegCHZN)
        this.RegCHZN = RegCHZN;
};

//prototype
PageValidate.prototype = {
    RegPhone: new RegExp("^[0-9]+[-]?[0-9]+[-]?[0-9]$"),
    RegNumber: new RegExp("^[0-9]+$"),
    RegNumberSign: new RegExp("^[+-]?[0-9]+$"),
    RegDecimal: new RegExp("^[0-9]+[.]?[0-9]+$"),
    RegDecimalSign: new RegExp("^[+-]?[0-9]+[.]?[0-9]+$"), //等价于^[+-]?\d+[.]?\d+$
    RegEmail: new RegExp("^[\\w-]+@[\\w-]+\\.(com|net|org|edu|mil|tv|biz|info)$"), //w 英文字母或数字的字符串，和 [a-zA-Z0-9] 语法一样 
    RegCHZN: new RegExp("[\u4e00-\u9fa5]"),
    Check: function (cheType, inputData) {
        if (cheType === "Phone") {
            return this.RegPhone.test(inputData);
        } else if (cheType === "Number") {
            return this.RegNumber.test(inputData);
        } else if (cheType === "NumberSign") {
            return this.RegNumberSign.test(inputData);
        } else if (cheType === "Decimal") {
            return this.RegDecimal.test(inputData);
        } else if (cheType === "DecimalSign") {
            return this.RegDecimalSign.test(inputData);
        } else if (cheType === "Email") {
            return this.RegEmail.test(inputData);
        } else if (cheType === "CHZN") {
            return this.RegCHZN.test(inputData);
        } else {
            return true;
        }
    }
};

var PageValidateHelper = function () {
};

PageValidateHelper.Check = function (cheType, inputData) {
    var pv = new PageValidate();
    return pv.Check(cheType, inputData);
};

//使用示例：
var boolResult = PageValidateHelper.Check("Phone", "13111111111");