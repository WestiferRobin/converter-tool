using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

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
            this.IsolatedPath = baseFolder + (isWindows ? @"\FilesInUse\" : "/FilesInUse/" );
            this.SolutionPath = baseFolder +  (isWindows ? @$"\SolutionFiles\{solutionsFolder}\" : @$"/SolutionFiles/{solutionsFolder}/");
        }

        protected void Cleanup()
        {
            var di = new DirectoryInfo(this.IsolatedPath);
            foreach (FileInfo file in di.EnumerateFiles())
            {
                file.Delete();
            }
            foreach (DirectoryInfo dir in di.EnumerateDirectories())
            {
                dir.Delete(true);
            }
        }

        protected void SetBaselinePath(string baselineFolder)
        {
            this.BaselinePath = baseFolder + (isWindows ? @$"\SolutionFiles\{baselineFolder}\" : @$"/SolutionFiles/{baselineFolder}/");
        }
    }
}
