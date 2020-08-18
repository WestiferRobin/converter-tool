using ConverterTool.Logger;
using System;
using System.IO;
using System.Runtime.InteropServices;

namespace ConverterTool
{
    internal class Program
    {
        // Debug file path
        private static readonly string filePath = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? 
            @"D:\ProgramProjects\Convertertool\SolutionFiles\"
            : @"/Users/wesitferrobin/Projects/workspace/Convertertool/SolutionFiles/";

        public static void Main(string[] args)
        {
            try
            {
                string sourceFile = string.Empty;
                string targetFile = string.Empty;

                // this temp location is for automation testing.
                if (false)
                {
                    for (int flag = 1; flag <= 4; flag++)
                    {
                        switch (flag)
                        {
                            case 1:
                                sourceFile = string.Concat(filePath, "WesTest.json");
                                targetFile = string.Concat(filePath, @"OUTPUT\OutputXml.xml");
                                break;
                            case 2:
                                sourceFile = string.Concat(filePath, "WessTest.xml");
                                targetFile = string.Concat(filePath, @"OUTPUT\OutputJson.json");
                                break;
                            case 3:
                                sourceFile = string.Concat(filePath, "HelloWorldSharp.cs");
                                targetFile = string.Concat(filePath, @"OUTPUT\OutputJV.java");
                                break;
                            case 4:
                                sourceFile = string.Concat(filePath, "HelloWorldJ.java");
                                targetFile = string.Concat(filePath, @"OUTPUT\OutputCS.cs");
                                break;
                        }
                        Log.Info("Conversion Tool is now running.");

                        // Scan and Parse through target file
                        Converter.RunTool(sourceFile, targetFile);

                        // Convert target file to Desired file.
                        Log.Success($"File is converted. Please check {Path.GetFileName(targetFile)}.");
                        Console.WriteLine("\n\n\n");
                    }
                }
                else
                {
                    bool shouldUseMain = true;
                    //sourceFile = string.Concat(filePath, shouldUseMain ? "HelloWorldSharp.cs" : "Value.cs");
                    //targetFile = string.Concat(filePath, @"OUTPUT\OutputJV.java");
                    sourceFile = string.Concat(filePath, shouldUseMain ? "HelloWorldJ.java" : "Value.java");
                    targetFile = string.Concat(filePath, @"OUTPUT\OutputCS.cs");

                    Log.Info("Conversion Tool is now running.");

                    // Scan and Parse through target file
                    Converter.RunTool(sourceFile, targetFile);

                    // Convert target file to Desired file.
                    Log.Success($"File is converted. Please check {Path.GetFileName(targetFile)}.");
                }
            }
            catch (Exception e)
            {
                Log.Error("Tool failed due to: " + e.Message);
            }
        }
    }
}
