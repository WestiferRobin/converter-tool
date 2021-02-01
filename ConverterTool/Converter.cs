using ConverterTool.LanguageRules;
using ConverterTool.Logger;
using System;
using System.IO;

namespace ConverterTool
{
    public static class Converter
    {
        // LanguageRules for source and target files
        private static LanguageRule sourceRules;
        private static LanguageRule targetRules;

        // configuration for application to run
        public static void RunTool(string[] args)
        {
            if (args.Length == 2)
            {
                var sourceTarget = args[0];
                var destTarget = args[1];

                RunTool(sourceTarget, destTarget);
            }
            else if (args.Length == 4)
            {
                var sourceFiles = Directory.GetFiles(args[1]);
                var sourceDir = Directory.GetDirectories(args[1]);
                var destExtension = args[2].Replace("-", ".");
                var sourceExtension = args[0].Replace("-", ".");
                var destDir = args[3].EndsWith('/') || args[3].EndsWith('\\') ?
                    Path.GetDirectoryName(args[3]) : args[3];

                foreach (var dir in sourceDir)
                {
                    var dirName = Path.GetFileName(dir);
                    var destSubDir = Path.Combine(destDir, dirName);
                    Directory.CreateDirectory(destSubDir);
                    RunTool(new string[] { args[0], dir, args[2], destSubDir });
                }
                foreach (var file in sourceFiles)
                {
                    if (!file.Contains(sourceExtension)) continue;

                    var destFile = string.Concat(Path.GetFileNameWithoutExtension(file), destExtension);
                    var destFull = Path.Combine(destDir, destFile);

                    RunTool(new string[] { file, destFull });
                }
            }
            else
            {
                Log.Error("Invalid arguments. Please Check again.");
            }

        }

        // include configuration
        private static void RunTool(string sourceFile, string targetFile)
        {
            Log.Info("Conversion Tool is now running.");

            // Define the type of Rules by language type
            sourceRules = RulesFactory.GenerateRule(sourceFile);
            targetRules = RulesFactory.GenerateRule(targetFile);

            ConvertSourceFile();
            CreateTargetFile();

            // Convert target file to Desired file.
            Log.Success($"File is converted. Please check {Path.GetFileName(targetFile)}.");
        }

        private static void ConvertSourceFile()
        {
            // Divide and organize Tokens and such
            sourceRules.ScanFile();
            sourceRules.ParseFile();
        }

        private static void CreateTargetFile()
        {
            // compare source to target
            if (targetRules.TypeOfLanguage == sourceRules.TypeOfLanguage && targetRules.ProgramTypeLanguage != sourceRules.ProgramTypeLanguage)
            {
                targetRules.Structure = sourceRules.Structure;  // TODO: THIS IS GOING TO BE WHERE THE ADAPTER CLASS IS GOING TO BE!!!!!
                targetRules.BuildFile();
            }
            else
            {
                throw new Exception($"Error: {targetRules.Filename} and {sourceRules.Filename} are note the proper types to ");
            }
        }
    }
}
