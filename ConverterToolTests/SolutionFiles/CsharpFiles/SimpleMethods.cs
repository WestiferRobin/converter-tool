using System;
using System.Collections.Generic;
using System.Text;

namespace ConverterToolTests.SolutionFiles.CsharpFiles
{
    public class SimpleMethods
    {
        public SimpleMethods()
        {

        }

        public string GetHello()
        {
            return "Hello";
        }

        private void PrintMessage(string message)
        {
            Console.WriteLine(message);
        }
    }
}
