using ConverterTool.Logger;
using System;
using System.IO;
using System.Runtime.InteropServices;

namespace ConverterTool
{
    internal class Program
    {

        public static void Main(string[] args)
        {
            try
            {
                string sourceFile = args[0];
                string targetFile = args[1];

                Converter.RunTool(args[0], args[1]);
            }
            catch (Exception e)
            {
                Log.Error("Tool failed due to: " + e.Message);
            }
        }
    }
}
