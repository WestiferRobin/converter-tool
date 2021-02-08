using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using ConverterTool;
using Newtonsoft.Json.Linq;

namespace ConverterToolTests
{
    [TestClass]
    public class JsonTests : TestGeneral
    {
        public JsonTests() : base("JsonFiles")
        {
        }

        [TestMethod]
        public void SimpleObject()
        {
            Cleanup();

            string startFile = Path.Combine(IsolatedPath, "SimpleObject.json");
            string outputFile = Path.Combine(IsolatedPath, "SimpleObject.xml");

            File.Copy(Path.Combine(SolutionPath, "SimpleObject.json"), startFile);

            Converter.RunTool(new string[] { startFile, outputFile });

            this.SetBaselinePath("XmlFiles");
            this.BaselinePath = Path.Combine(this.BaselinePath, "SimpleObject.xml");

            var startObject = File.ReadAllText(outputFile);
            var outputObject = File.ReadAllText(this.BaselinePath);

            bool isEqual = startObject.Length == outputObject.Length;
            for (int index = 0; index < outputObject.Length; index++)
            {
                if (startObject[index] != outputObject[index])
                {
                    isEqual = false;
                    break;
                }
            }

            Assert.IsTrue(isEqual);

            Cleanup();
        }

        [TestMethod]
        public void ArrayObject()
        {
            Cleanup();

            string startFile = Path.Combine(IsolatedPath, "ArrayObject.json");
            string outputFile = Path.Combine(IsolatedPath, "ArrayObject.xml");

            File.Copy(Path.Combine(SolutionPath, "ArrayObject.json"), startFile);

            Converter.RunTool(new string[] { startFile, outputFile });

            this.SetBaselinePath("XmlFiles");
            this.BaselinePath = Path.Combine(this.BaselinePath, "ArrayObject.xml");

            var startObject = File.ReadAllBytes(outputFile);
            var outputObject = File.ReadAllBytes(this.BaselinePath);

            bool isEqual = startObject.Length == outputObject.Length;
            for (int index = 0; index < outputObject.Length; index++)
            {
                char startChar = (char)startObject[index];
                char outputChar = (char)outputObject[index];
                if (startChar != outputChar)
                {
                    isEqual = false;
                    break;
                }
            }

            Assert.IsTrue(isEqual);

            Cleanup();
        }

        [TestMethod]
        public void NestedObject()
        {
            Cleanup();

            string startFile = Path.Combine(IsolatedPath, "NestedObject.json");
            string outputFile = Path.Combine(IsolatedPath, "NestedObject.xml");

            File.Copy(Path.Combine(SolutionPath, "NestedObject.json"), startFile);

            Converter.RunTool(new string[] { startFile, outputFile });

            this.SetBaselinePath("XmlFiles");
            this.BaselinePath = Path.Combine(this.BaselinePath, "NestedObject.xml");

            var startObject = File.ReadAllBytes(outputFile);
            var outputObject = File.ReadAllBytes(this.BaselinePath);

            bool isEqual = startObject.Length == outputObject.Length;
            for (int index = 0; index < outputObject.Length; index++)
            {
                char startChar = (char)startObject[index];
                char outputChar = (char)outputObject[index];
                if (startChar != outputChar)
                {
                    isEqual = false;
                    break;
                }
            }

            Assert.IsTrue(isEqual);

            Cleanup();
        }

        [TestMethod]
        public void ReversalObjects()
        {
            Cleanup();

            string startFile = Path.Combine(IsolatedPath, "SimpleObject.json");
            string rawFile = Path.Combine(IsolatedPath, "RawOutput.xml");
            string outputFile = Path.Combine(IsolatedPath, "Results.json");

            File.Copy(Path.Combine(SolutionPath, "SimpleObject.json"), startFile);

            Converter.RunTool(new string[] { startFile, rawFile });
            Converter.RunTool(new string[] { rawFile, outputFile });

            JObject startObject = JObject.Parse(File.ReadAllText(startFile));
            JObject outputObject = JObject.Parse(File.ReadAllText(outputFile));

            Assert.IsTrue(JToken.DeepEquals(startObject, outputObject));

            Cleanup();
        }

        [TestMethod]
        public void MultiFiles()
        {
            Cleanup();

            string startDir = SolutionPath;
            string destDir = IsolatedPath;
            this.SetBaselinePath("XmlFiles");

            Converter.RunTool(new string[] { "-json", startDir, "-xml", destDir });

            CheckMultiFiles(destDir, this.BaselinePath);

            Cleanup();
        }

        private void CheckMultiFiles(string completePath, string baselinePath)
        {
            var completeFiles = Directory.GetFiles(completePath);
            var completeDir = Directory.GetDirectories(completePath);
            var baselineDir = baselinePath.EndsWith('/') || baselinePath.EndsWith('\\') ?
                Path.GetDirectoryName(baselinePath) : baselinePath;

            foreach (var dir in completeDir)
            {
                var dirName = Path.GetFileName(dir);
                var destSubDir = Path.Combine(baselineDir, dirName);

                CheckMultiFiles(dir, destSubDir);
            }

            foreach (var file in completeFiles)
            {
                if (Path.GetExtension(file) == ".txt") continue;

                var destFile = string.Concat(Path.GetFileNameWithoutExtension(file), ".xml");
                var destFull = Path.Combine(baselineDir, destFile);

                var startObject = File.ReadAllBytes(file);
                var outputObject = File.ReadAllBytes(destFull);

                bool isEqual = startObject.Length == outputObject.Length;
                for (int index = 0; index < outputObject.Length; index++)
                {
                    char startChar = (char)startObject[index];
                    char outputChar = (char)outputObject[index];
                    if (startChar != outputChar)
                    {
                        isEqual = false;
                        break;
                    }
                }

                Assert.IsTrue(isEqual);
            }
        }

        [TestMethod]
        public void LogMultiFiles()
        {
            Cleanup();

            string startDir = SolutionPath;
            string destDir = IsolatedPath;
            string fileDir = Path.Combine(IsolatedPath, "log.txt");
            this.SetBaselinePath("XmlFiles");

            Converter.RunTool(new string[] { "-json", startDir, "-xml", destDir, "-log", fileDir });

            CheckMultiFiles(destDir, this.BaselinePath);
            Assert.IsTrue(File.Exists(fileDir));

            Cleanup();
        }

        [TestMethod]
        public void LogFile()
        {
            Cleanup();

            string startFile = Path.Combine(IsolatedPath, "SimpleObject.json");
            string outputFile = Path.Combine(IsolatedPath, "SimpleObject.xml");
            string fileDir = Path.Combine(IsolatedPath, "log.txt");

            File.Copy(Path.Combine(SolutionPath, "SimpleObject.json"), startFile);

            Converter.RunTool(new string[] { startFile, outputFile, "-log", fileDir });

            this.SetBaselinePath("XmlFiles");
            this.BaselinePath = Path.Combine(this.BaselinePath, "SimpleObject.xml");

            var startObject = File.ReadAllText(outputFile);
            var outputObject = File.ReadAllText(this.BaselinePath);

            bool isEqual = startObject.Length == outputObject.Length;
            for (int index = 0; index < outputObject.Length; index++)
            {
                if (startObject[index] != outputObject[index])
                {
                    isEqual = false;
                    break;
                }
            }

            Assert.IsTrue(isEqual);
            Assert.IsTrue(File.Exists(fileDir));

            Cleanup();
        }
    }
}
