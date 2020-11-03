using ConverterTool.Logger;
using ConverterTool.WrapperTypes;
using System.Collections.Generic;
using System.IO;

namespace ConverterTool.LanguageRules
{
    public abstract class LanguageRule
    {
        // Keywords
        protected IList<string> ValidKeywords { get; set; }
        protected IList<string> WarningKeywords { get; set; }
        protected IList<string> ErrorKeywords { get; set; }
        
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
            this.ValidKeywords = new List<string>();
            this.WarningKeywords = new List<string>();
            this.ErrorKeywords = new List<string>();
            this.TokenList = new List<string>();
            this.Results = string.Empty;
            this.Structure = new List<WrapperType>();
        }

        protected abstract void InitKeywords();
        public abstract void ParseFile();
        public abstract void ScanFile();
        public abstract void BuildFile();
        public string GetResults()
        {
            //return this.Results;
            return "<Hello> Hello World </Hello>";
        }
        protected bool IsValidKeyword(string keyword)
        {
            return this.ValidKeywords.Contains(keyword) || this.WarningKeywords.Contains(keyword);
        }

        protected void LogValidKeyword(string keyword)
        {
            if (this.WarningKeywords.Contains(keyword))
            {
                Log.Warn($"The {keyword} keyword may cause issues for build. Please consider to refactor.");
            }
            else if (this.ErrorKeywords.Contains(keyword))
            {
                Log.Error($"The {keyword} keyword cannot be compiled for the tool. Please refactor.");
            }
        }
    }
}
