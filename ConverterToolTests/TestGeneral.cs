using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace ConverterToolTests
{
    public class TestGeneral
    {
        protected string IsolatedPath { get; }
        protected string SolutionPath { get; }
        protected string BaselinePath { get; set; }

        private string baseFolder = Path.GetDirectoryName(Path.GetDirectoryName(Path.GetDirectoryName(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location))));
        private bool isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

        public TestGeneral(string solutionsFolder)
        {
            this.IsolatedPath = baseFolder + (isWindows ? @"\FilesInUse\" : "/FilesInUser/" );
            this.SolutionPath = baseFolder +  (isWindows ? @$"\SolutionFiles\{solutionsFolder}\" : @$"/SolutionFiles/{solutionsFolder}/");
        }

        protected void Cleanup()
        {
            var files = Directory.GetFiles(this.IsolatedPath, "*.*");
            foreach (var file in files)
            {
                File.Delete(file);
            }
        }

        protected void SetBaselinePath(string baselineFolder)
        {
            this.BaselinePath = baseFolder + (isWindows ? @$"\SolutionFiles\{baselineFolder}\" : @$"/SolutionFiles/{baselineFolder}/");
        }
    }
}
