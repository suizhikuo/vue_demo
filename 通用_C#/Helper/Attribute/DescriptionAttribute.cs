using System;
using System.Collections.Generic;
using System.Text;

namespace Common
{

    public class DescriptionAttribute : Attribute
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="remark"></param>
        public DescriptionAttribute(string description)
        {
            _description = description;

        }

        private string _description;
        /// <summary>
        /// 描述信息
        /// </summary>
        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }

       
    }
}
