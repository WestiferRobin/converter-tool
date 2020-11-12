using System;
using System.Collections.Generic;
using System.Text;

namespace ConverterToolTests.SolutionFiles.CsharpFiles
{
    struct SimpleStruct
    {
        private readonly string _name;
        public SimpleStruct(string newName)
        {
            this._name = newName;
        }
    }
}
