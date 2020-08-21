using ConverterTool.LanguageRules;
using System;

namespace ConverterTool
{
    internal static class Converter
    {
        // LanguageRules for source and target files
        private static LanguageRule sourceRules;
        private static LanguageRule targetRules;

        // configuration for application to run

        // include configuration
        internal static void RunTool(string sourceFile, string targetFile)
        {
            // Define the type of Rules by language type
            sourceRules = RulesFactory.GenerateRule(sourceFile);
            targetRules = RulesFactory.GenerateRule(targetFile);

            ConvertSourceFile();
            CreateTargetFile();
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
