using ConverterTool.WrapperTypes;
using System.Collections.Generic;
using System.IO;

namespace ConverterTool.LanguageRules
{
    internal abstract class LanguageRule
    {
        // Keywords
        protected IList<string> Keywords { get; set; }
        
        // LanguageType
        public LanguageType TypeOfLanguage { get; }

        // Tokens
        protected IList<string> TokenList { get; set; }

        // ProgramType
        public ProgramType ProgramTypeLanguage { get; set; }

        // Filename
        public string Filename { get; }

        public string FullFile { get; }

        // Results of ParseFile function
        protected string Results { get; set; }

        public List<WrapperType> Structure { get; set; }

        public LanguageRule(LanguageType newType, ProgramType programType, string fileName)
        {
            this.ProgramTypeLanguage = programType;
            this.TypeOfLanguage = newType;
            this.FullFile = fileName;
            this.Filename = Path.GetFileName(fileName);
            this.Results = string.Empty;
            this.Keywords = new List<string>();
            this.TokenList = new List<string>();
            this.Results = string.Empty;
            this.Structure = new List<WrapperType>();
        }

        protected abstract void CreateKeywords();       // Should be interface for ProgramingLanguages to place privatly
        public abstract void ParseFile();
        public abstract void ScanFile();
        public abstract void BuildFile();
        public string GetResults()
        {
            //return this.Results;
            return "<Hello> Hello World </Hello>";
        }
    }
}
