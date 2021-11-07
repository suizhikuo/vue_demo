window.Enum = {
};

//if (typeof window.Enum.WeekDay == "undefined") {
//    window.Enum.WeekDay = {
//        Sunday: 0,
//        Monday: 1,
//        Tuesday: 2,
//        Wedesay: 3,
//        Thursday: 4,
//        Friday: 5,
//        Saturday: 6
//    };
//}

// || 运算符  window.Enum.WeekDay为空,则新建一个object给其赋值,否则直接使用
window.Enum.WeekDay = window.Enum.WeekDay || {
    Sunday: 0,
    Monday: 1,
    Tuesday: 2,
    Wedesay: 3,
    Thursday: 4,
    Friday: 5,
    Saturday: 6
}