using System;
using System.Collections.Generic;
using System.Text;

namespace ConverterTool.WrapperTypes
{
    public class WrapperType
    {
        public string VariableName { get; set; }

        public WrapperType(string name)
        {
            this.VariableName = name;
        }

    }
}
