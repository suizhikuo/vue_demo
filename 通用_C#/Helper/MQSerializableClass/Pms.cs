using System;
using System.IO;
using System.Net.Configuration;

namespace Common.Helper.MQSerializableClass
{
    [Serializable]
    public class Pms
    {
        public byte[] PmsByte { get; set; }

        public string FileName { get; set; }
    }
}