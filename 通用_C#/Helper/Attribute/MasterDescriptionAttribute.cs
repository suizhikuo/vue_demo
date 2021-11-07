using System;
using System.Collections.Generic;
using System.Text;

namespace Common
{
    public class MasterDescriptionAttribute:Attribute
    {
        private string m_masterDescription;

        public MasterDescriptionAttribute(string masterDescription)
        {
            this.m_masterDescription = masterDescription;
        }

        public string MasterDesciption
        {
            get { return this.m_masterDescription; }
            set { this.m_masterDescription = value; }
        }
    }
}
