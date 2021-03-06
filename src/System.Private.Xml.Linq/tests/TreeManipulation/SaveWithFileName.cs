// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using CoreXml.Test.XLinq;
using Microsoft.Test.ModuleCore;
using XmlCoreTest.Common;
using Xunit;

namespace XLinqTests
{
    public class SaveWithFileName
    {
        private string _fileName = "SaveBaseline";

        [Fact]
        public void XDocumentSaveToFile()
        {
            SerializeWithSaveOptions(SerializeXDocumentSaveToFile, testXElement: false, testXDocument: true);
        }

        [Fact]
        public void XDocumentSave()
        {
            string markup = "<e> <e2 /> </e>";
            try
            {
                XDocument d = XDocument.Parse(markup, LoadOptions.PreserveWhitespace);
                d.Save(_fileName);
            }
            finally
            {
                Assert.True(File.Exists(_fileName));
                Assert.Equal("<?xml version=\"1.0\" encoding=\"utf-8\"?>\r\n" + markup, File.ReadAllText(_fileName));
                File.Delete(_fileName);
            }
        }

        [Fact]
        public void XDocumentSave_SaveOptions()
        {
            string markup = "<e> <e2 /> </e>";
            try
            {
                XDocument d = XDocument.Parse(markup, LoadOptions.PreserveWhitespace);
                d.Save(_fileName, SaveOptions.DisableFormatting);
            }
            finally
            {
                Assert.True(File.Exists(_fileName));
                Assert.Equal("<?xml version=\"1.0\" encoding=\"utf-8\"?>" + markup, File.ReadAllText(_fileName));
                File.Delete(_fileName);
            }
        }

        [Fact]
        public void XDocumentSave_NullParameter()
        {
            Assert.Throws<ArgumentNullException>(() => new XDocument().Save((string)null));
            Assert.Throws<ArgumentNullException>(() => new XDocument().Save((string)null, SaveOptions.DisableFormatting));
            Assert.Throws<ArgumentNullException>(() => new XDocument().Save((string)null, SaveOptions.None));
        }

        [Fact]
        public void XElementSaveToFile()
        {
            SerializeWithSaveOptions(SerializeXElementSaveToFile, testXElement: true, testXDocument: false);
        }

        [Fact]
        public void XElementSave()
        {
            string markup = "<e a=\"value\"> <e2 /> </e>";
            try
            {
                XElement e = XElement.Parse(markup, LoadOptions.PreserveWhitespace);
                e.Save(_fileName);
            }
            finally
            {
                Assert.True(File.Exists(_fileName));
                Assert.Equal("<?xml version=\"1.0\" encoding=\"utf-8\"?>\r\n" + markup, File.ReadAllText(_fileName));
                File.Delete(_fileName);
            }
        }

        [Fact]
        public void XElementSave_SaveOptions()
        {
            string markup = "<e a=\"value\"> <e2 /> </e>";
            try
            {
                XElement e = XElement.Parse(markup, LoadOptions.PreserveWhitespace);
                e.Save(_fileName, SaveOptions.DisableFormatting);
            }
            finally
            {
                Assert.True(File.Exists(_fileName));
                Assert.Equal("<?xml version=\"1.0\" encoding=\"utf-8\"?>" + markup, File.ReadAllText(_fileName));
                File.Delete(_fileName);
            }
        }

        [Fact]
        public void XElementSave_NullParameter()
        {
            Assert.Throws<ArgumentNullException>(() => new XElement("e").Save((string)null));
            Assert.Throws<ArgumentNullException>(() => new XElement("e").Save((string)null, SaveOptions.DisableFormatting));
            Assert.Throws<ArgumentNullException>(() => new XElement("e").Save((string)null, SaveOptions.None));
        }

        [Fact]
        public void XStreamingElementSave_NullParameter()
        {
            Assert.Throws<ArgumentNullException>(() => new XStreamingElement("e").Save((string)null));
            Assert.Throws<ArgumentNullException>(() => new XStreamingElement("e").Save((string)null, SaveOptions.DisableFormatting));
            Assert.Throws<ArgumentNullException>(() => new XStreamingElement("e").Save((string)null, SaveOptions.None));
        }

        [Fact]
        public void XStreamingElementSave()
        {
            string markup = "<e a=\"value\"> <!--comment--> <e2> <![CDATA[cdata]]> </e2> <?pi target?> </e>";
            try
            {
                XElement e = XElement.Parse(markup, LoadOptions.PreserveWhitespace);
                XStreamingElement e2 = new XStreamingElement(e.Name, e.Attributes(), e.Nodes());
                e2.Save(_fileName);
            }
            finally
            {
                Assert.True(File.Exists(_fileName));
                Assert.Equal("<?xml version=\"1.0\" encoding=\"utf-8\"?>\r\n" + markup, File.ReadAllText(_fileName));
                File.Delete(_fileName);
            }
        }

        [Fact]
        public void XStreamingElementSave_SaveOptions()
        {
            string markup = "<e a=\"value\"> <!--comment--> <e2> <![CDATA[cdata]]> </e2> <?pi target?> </e>";
            try
            {
                XElement e = XElement.Parse(markup, LoadOptions.PreserveWhitespace);
                XStreamingElement e2 = new XStreamingElement(e.Name, e.Attributes(), e.Nodes());
                e2.Save(_fileName, SaveOptions.DisableFormatting);
            }
            finally
            {
                Assert.True(File.Exists(_fileName));
                Assert.Equal("<?xml version=\"1.0\" encoding=\"utf-8\"?>" + markup, File.ReadAllText(_fileName));
                File.Delete(_fileName);
            }
        }

        //
        // helpers
        //
        private static string SerializeXDocumentSaveToFile(XNode node)
        {
            string filename = "DontExist";
            string result;
            try
            {
                filename = Path.GetTempFileName();
                ((XDocument)node).Save(filename);
                using (TextReader tr = new StreamReader(filename))
                {
                    result = StripOffXmlDeclaration(tr.ReadToEnd());
                }
            }
            finally
            {
                Assert.True(File.Exists(filename));
                File.Delete(filename);
            }
            return result;
        }

        private static string SerializeXElementSaveToFile(XNode node)
        {
            string filename = "DontExist";
            string result;
            try
            {
                filename = Path.GetTempFileName();
                ((XElement)node).Save(filename);
                using (TextReader tr = new StreamReader(filename))
                {
                    result = StripOffXmlDeclaration(tr.ReadToEnd());
                }
            }
            finally
            {
                Assert.True(File.Exists(filename));
                File.Delete(filename);
            }
            return result;
        }

        private delegate string SerializeNode(XNode node);

        private static void SerializeWithSaveOptions(SerializeNode serialize, bool testXElement, bool testXDocument)
        {
            // Test both options at once as they don't really collide
            SaveOptions so = SaveOptions.DisableFormatting | SaveOptions.OmitDuplicateNamespaces;

            XElement root = XElement.Parse("<root xmlns:a='uri'><child xmlns:a='uri'><baby xmlns:a='uri'>text</baby></child></root>");
            XElement child = root.Element("child");
            XElement baby = child.Element("baby");
            XNode text = baby.FirstNode;

            // Verify that without annotation the output gets indented and the duplicate ns decls are not removed
            if (testXElement)
            {
                Assert.Equal(serialize(child), "<child xmlns:a=\"uri\">\r\n  <baby xmlns:a=\"uri\">text</baby>\r\n</child>");
            }

            // Now add annotation to the leaf element node
            // Even though it's in effect the output should stay the same (as there is only one namespace decl and mixed content).
            baby.AddAnnotation(so);

            if (testXElement)
            {
                Assert.Equal(serialize(baby), "<baby xmlns:a=\"uri\">text</baby>");
            }

            // Now add annotation to the middle node
            child.AddAnnotation(so);

            if (testXElement)
            {
                // Verify that the options are applied correctly
                Assert.Equal(serialize(child), "<child xmlns:a=\"uri\"><baby>text</baby></child>");
                // Verify that the root node is not affected as we don't look for the annotation among descendants
                Assert.Equal(serialize(root), "<root xmlns:a=\"uri\">\r\n  <child xmlns:a=\"uri\">\r\n    <baby xmlns:a=\"uri\">text</baby>\r\n  </child>\r\n</root>");
            }

            // And now add the annotation to the root and remove it from the child to test that we can correctly skip over a node
            root.AddAnnotation(so);
            child.RemoveAnnotations(typeof(SaveOptions));

            if (testXElement)
            {
                // Verify that the options are still applied to child
                Assert.Equal(serialize(child), "<child xmlns:a=\"uri\"><baby>text</baby></child>");
                // And they should be also applied to the root now
                Assert.Equal(serialize(root), "<root xmlns:a=\"uri\"><child><baby>text</baby></child></root>");
            }

            // Add a document node above it all to test that it works on non-XElement as well
            XDocument doc = new XDocument(root);
            // Add the annotation to the doc and remove it from the root
            doc.AddAnnotation(so);
            root.RemoveAnnotations(typeof(SaveOptions));

            // Options should still apply to root as well as the doc
            if (testXElement)
            {
                Assert.Equal(serialize(root), "<root xmlns:a=\"uri\"><child><baby>text</baby></child></root>");
            }

            if (testXDocument)
            {
                Assert.Equal(serialize(doc), "<root xmlns:a=\"uri\"><child><baby>text</baby></child></root>");
            }
        }

        private static string StripOffXmlDeclaration(string s)
        {
            if (s.StartsWith("<?xml "))
            {
                s = s.Substring(s.IndexOf('>') + 1);

                if (s.StartsWith("\r\n"))
                {
                    s = s.Substring(2);
                }
            }
            return s;
        }
    }
}
