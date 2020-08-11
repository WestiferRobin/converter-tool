using ConverterTool.Logger;
using System;
using System.IO;

namespace ConverterTool
{
    internal class Program
    {
        // THIS IS FOR WINDOWS!
        private const string FILE_PATH = @"D:\ProgramProjects\Convertertool\SolutionFiles\";

        // THIS IS FOR MAC/LINUX!
        private const string FILE_PATH = @"/Users/wesitferrobin/Projects/workspace/Convertertool/SolutionFiles/";
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
                                sourceFile = string.Concat(FILE_PATH, "WesTest.json");
                                targetFile = string.Concat(FILE_PATH, @"OUTPUT\OutputXml.xml");
                                break;
                            case 2:
                                sourceFile = string.Concat(FILE_PATH, "WessTest.xml");
                                targetFile = string.Concat(FILE_PATH, @"OUTPUT\OutputJson.json");
                                break;
                            case 3:
                                sourceFile = string.Concat(FILE_PATH, "HelloWorldSharp.cs");
                                targetFile = string.Concat(FILE_PATH, @"OUTPUT\OutputJV.java");
                                break;
                            case 4:
                                sourceFile = string.Concat(FILE_PATH, "HelloWorldJ.java");
                                targetFile = string.Concat(FILE_PATH, @"OUTPUT\OutputCS.cs");
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
                    sourceFile = string.Concat(FILE_PATH, "Value.cs");//"HelloWorldSharp.cs");
                    targetFile = string.Concat(FILE_PATH, @"OUTPUT\OutputJV.java");
                    //sourceFile = string.Concat(FILE_PATH, "HelloWorldJ.java");
                    //targetFile = string.Concat(FILE_PATH, @"OUTPUT\OutputCS.cs");

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
