using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace Packages.Rider.Tests.Editor
{
    public static class XMLUtilities
    {
        public static void AssertReferencesContainAll(XmlDocument projectXml, IEnumerable<string> expectedReferences)
        {
            var references = projectXml.SelectAttributeValues("/msb:Project/msb:ItemGroup/msb:Reference/@Include", GetModifiedXmlNamespaceManager(projectXml)).ToArray();
            var splitReferences = references.Select(r => r.Split('\\').Last());
            CollectionAssert.IsSubsetOf(expectedReferences, splitReferences);
        }

        public static void AssertProjectReferencesMatchExactly(XmlDocument projectXml, IEnumerable<string> expectedReferences)
        {
            var references = projectXml.SelectInnerTextOfNodes("/msb:Project/msb:ItemGroup/msb:ProjectReference/msb:Name", GetModifiedXmlNamespaceManager(projectXml)).ToArray();
            CollectionAssert.AreEquivalent(expectedReferences, references);
        }

        public static void AssertCompileItemsMatchExactly(XmlDocument projectXml, IEnumerable<string> expectedCompileItems)
        {
            var compileItems = projectXml.SelectAttributeValues("/msb:Project/msb:ItemGroup/msb:Compile/@Include", GetModifiedXmlNamespaceManager(projectXml)).ToArray();
            CollectionAssert.AreEquivalent(RelativeAssetPathsFor(expectedCompileItems), compileItems);
        }

        public static void AssertNonCompileItemsMatchExactly(XmlDocument projectXml, IEnumerable<string> expectedNoncompileItems)
        {
            var nonCompileItems = projectXml.SelectAttributeValues("/msb:Project/msb:ItemGroup/msb:None/@Include", GetModifiedXmlNamespaceManager(projectXml)).ToArray();
            CollectionAssert.AreEquivalent(RelativeAssetPathsFor(expectedNoncompileItems), nonCompileItems);
        }

        public static void AssertDefinesContain(XmlDocument projectFile, string[] defines)
        {
            foreach (var define in defines)
            {
                AssertDefinesContain(projectFile, define);
            }
        }

        public static void AssertDefinesContain(XmlDocument projectFile, string define)
        {
            var xmlNamespaces = new XmlNamespaceManager(projectFile.NameTable);
            xmlNamespaces.AddNamespace("msb", "http://schemas.microsoft.com/developer/msbuild/2003");

            string[] defines = projectFile.SelectSingleNode("/msb:Project/msb:PropertyGroup/msb:DefineConstants", xmlNamespaces).InnerText.Split(';');
            Assert.AreEqual(true, defines.Contains("PLATFORM_STANDALONE"), "Missing Unity-related defines in {0} project: {1}", projectFile.Name, defines);
            Assert.IsTrue(defines.Contains(define), "Incorrect user-specified defines in {0} project: {1}", projectFile.Name, defines);
        }

        static XmlNamespaceManager GetModifiedXmlNamespaceManager(XmlDocument projectXml)
        {
            var xmlNamespaces = new XmlNamespaceManager(projectXml.NameTable);
            xmlNamespaces.AddNamespace("msb", "http://schemas.microsoft.com/developer/msbuild/2003");
            return xmlNamespaces;
        }

        static IEnumerable<string> RelativeAssetPathsFor(IEnumerable<string> fileNames)
        {
            return fileNames.Select(fileName => fileName.Replace('/', '\\')).ToArray();
        }

        static IEnumerable<string> SelectAttributeValues(this XmlDocument xmlDocument, string xpathQuery, XmlNamespaceManager xmlNamespaceManager)
        {
            var result = xmlDocument.SelectNodes(xpathQuery, xmlNamespaceManager);
            foreach (XmlAttribute attribute in result)
                yield return attribute.Value;
        }

        static IEnumerable<string> SelectInnerTextOfNodes(this XmlDocument xmlDocument, string xpathQuery, XmlNamespaceManager xmlNamespaceManager)
        {
            return xmlDocument.SelectNodes(xpathQuery, xmlNamespaceManager).Cast<XmlElement>().Select(e => e.InnerText);
        }

        public static XmlDocument FromFile(string fileName)
        {
            var csProj = new XmlDocument();
            csProj.Load(fileName);
            return csProj;
        }

        public static XmlDocument FromText(string textContent)
        {
            var xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(textContent);
            return xmlDocument;
        }
    }
}
