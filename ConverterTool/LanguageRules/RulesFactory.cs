using ConverterTool.Logger;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ConverterTool.LanguageRules
{
    public static class RulesFactory
    {
        public static LanguageRule GenerateRule(string file)
        {
            string extension = Path.GetExtension(file).ToUpper();
            switch (extension)
            {
                case ".XML":
                case ".HTML":
                    Log.Info("Creating a XML Rule.");
                    return new XmlRule(file);
                case ".JSON":
                    Log.Info("Creating a JSON Rule.");
                    return new JsonRule(file);
                case ".CS":
                    Log.Info("Creating a Csharp Rule.");
                    return new CsharpRule(file);
                case ".JAVA":
                    Log.Info("Creating a Java Rule.");
                    return new JavaRule(file);
                default:
                    throw new Exception($"Error: {extension} is not a valid extension.");
            }
        }
    }
}
