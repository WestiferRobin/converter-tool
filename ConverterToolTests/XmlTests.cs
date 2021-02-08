using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using ConverterTool;
using Newtonsoft.Json.Linq;

namespace ConverterToolTests
{
    [TestClass]
    public class XmlTests : TestGeneral
    {
        public XmlTests() : base("XmlFiles")
        {
        }

        [TestMethod]
        public void SimpleObject()
        {
            Cleanup();

            string startFile = Path.Combine(IsolatedPath, "SimpleObject.xml");
            string outputFile = Path.Combine(IsolatedPath, "SimpleObject.json");

            File.Copy(Path.Combine(SolutionPath, "SimpleObject.xml"), startFile);

            Converter.RunTool(new string[] { startFile, outputFile });

            this.SetBaselinePath("JsonFiles");
            this.BaselinePath = Path.Combine(this.BaselinePath, "SimpleObject.json");

            JObject outputObject = JObject.Parse(File.ReadAllText(outputFile));
            JObject baselineObject = JObject.Parse(File.ReadAllText(this.BaselinePath));

            Assert.IsTrue(JToken.DeepEquals(outputObject, baselineObject));

            Cleanup();
        }

        [TestMethod]
        public void ArrayObject()
        {
            Cleanup();

            string startFile = Path.Combine(IsolatedPath, "ArrayObject.xml");
            string outputFile = Path.Combine(IsolatedPath, "ArrayObject.json");

            File.Copy(Path.Combine(SolutionPath, "ArrayObject.xml"), startFile);

            Converter.RunTool(new string[] { startFile, outputFile });

            this.SetBaselinePath("JsonFiles");
            this.BaselinePath = Path.Combine(this.BaselinePath, "ArrayObject.json");

            var outputText = File.ReadAllText(outputFile);
            var baselineText = File.ReadAllText(this.BaselinePath);

            var outputObject = JArray.Parse(outputText);
            var baselineObject = JArray.Parse(baselineText);

            Assert.IsTrue(JToken.DeepEquals(outputObject, baselineObject));

            Cleanup();
        }

        [TestMethod]
        public void NestedObject()
        {
            Cleanup();

            string startFile = Path.Combine(IsolatedPath, "NestedObject.xml");
            string outputFile = Path.Combine(IsolatedPath, "NestedObject.json");

            File.Copy(Path.Combine(SolutionPath, "NestedObject.xml"), startFile);

            Converter.RunTool(new string[] { startFile, outputFile });

            this.SetBaselinePath("JsonFiles");
            this.BaselinePath = Path.Combine(this.BaselinePath, "NestedObject.json");

            JObject outputObject = JObject.Parse(File.ReadAllText(outputFile));
            JObject baselineObject = JObject.Parse(File.ReadAllText(this.BaselinePath));

            Assert.IsTrue(JToken.DeepEquals(outputObject, baselineObject));

            Cleanup();
        }

        [TestMethod]
        public void ReversalObjects()
        {
            Cleanup();

            string startFile = Path.Combine(IsolatedPath, "SimpleObject.xml");
            string rawFile = Path.Combine(IsolatedPath, "SimpleObject.json");
            string outputFile = Path.Combine(IsolatedPath, "Results.xml");

            File.Copy(Path.Combine(SolutionPath, "SimpleObject.xml"), startFile);

            Converter.RunTool(new string[] { startFile, rawFile });
            Converter.RunTool(new string[] { rawFile, outputFile });

            var startObject = File.ReadAllBytes(startFile);
            var outputObject = File.ReadAllBytes(outputFile);

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
        public void MultiFiles()
        {
            Cleanup();

            string startDir = SolutionPath;
            string destDir = IsolatedPath;
            this.SetBaselinePath("JsonFiles");

            Converter.RunTool(new string[] { "-xml", startDir, "-json", destDir });

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

                var destFile = string.Concat(Path.GetFileNameWithoutExtension(file), ".json");
                var destFull = Path.Combine(baselineDir, destFile);

                try
                {
                    JObject outputObject = JObject.Parse(File.ReadAllText(file));
                    JObject baselineObject = JObject.Parse(File.ReadAllText(destFull));

                    Assert.IsTrue(JToken.DeepEquals(outputObject, baselineObject));
                }
                catch
                {

                    Assert.IsTrue(JToken.DeepEquals(JArray.Parse(File.ReadAllText(file)),
                        JArray.Parse(File.ReadAllText(destFull))));
                }
            }
        }

        [TestMethod]
        public void LogMultiFiles()
        {
            Cleanup();

            string startDir = SolutionPath;
            string destDir = IsolatedPath;
            string fileDir = Path.Combine(IsolatedPath, "log.txt");
            this.SetBaselinePath("JsonFiles");

            Converter.RunTool(new string[] { "-xml", startDir, "-json", destDir, "-log", fileDir});

            CheckMultiFiles(destDir, this.BaselinePath);
            Assert.IsTrue(File.Exists(fileDir));

            Cleanup();
        }

        [TestMethod]
        public void LogFile()
        {
            Cleanup();

            string startFile = Path.Combine(IsolatedPath, "SimpleObject.xml");
            string outputFile = Path.Combine(IsolatedPath, "SimpleObject.json");
            string fileDir = Path.Combine(IsolatedPath, "log.txt");

            File.Copy(Path.Combine(SolutionPath, "SimpleObject.xml"), startFile);

            Converter.RunTool(new string[] { startFile, outputFile, "-log", fileDir});

            this.SetBaselinePath("JsonFiles");
            this.BaselinePath = Path.Combine(this.BaselinePath, "SimpleObject.json");

            JObject outputObject = JObject.Parse(File.ReadAllText(outputFile));
            JObject baselineObject = JObject.Parse(File.ReadAllText(this.BaselinePath));

            Assert.IsTrue(JToken.DeepEquals(outputObject, baselineObject));

            Assert.IsTrue(File.Exists(fileDir));

            Cleanup();
        }
    }
}
