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
        internal static void RunTool(string source, string target)
        {
            ConvertSourceFile(source);
            CreateTargetFile(target);
        }

        private static void ConvertSourceFile(string sourceFile)
        {
            // Define the type of Rules by language type
            sourceRules = RulesFactory.GenerateRule(sourceFile);

            // Divide and organize Tokens and such
            sourceRules.ScanFile();
            sourceRules.ParseFile();
        }

        private static void CreateTargetFile(string targetFile)
        {
            // Define rules of the language type 
            targetRules = RulesFactory.GenerateRule(targetFile);

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
