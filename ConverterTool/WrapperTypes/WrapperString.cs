using System;
using System.Collections.Generic;
using System.Text;

namespace ConverterTool.WrapperTypes
{
    public class WrapperString : WrapperType
    {
        public string Value { get; set; }

        public WrapperString(string wrapperName, string value) : base(wrapperName)
        {
            this.Value = value;
        }
    }
}
