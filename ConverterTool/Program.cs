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
                Converter.RunTool(args);
            }
            catch (Exception e)
            {
                Log.Error("Tool failed due to: " + e.Message);
            }
        }
    }
}
