﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using DocumentFormat.OpenXml.Packaging;
using OpenXmlPowerTools;

class FieldRetriever01
{
    static void Main(string[] args)
    {
        var docWithFooter = new FileInfo("../../DocWithFooter1.docx");
        var scrubbedDocument = new FileInfo("DocWithFooterScrubbed1.docx");
        if (scrubbedDocument.Exists)
            scrubbedDocument.Delete();
        File.Copy(docWithFooter.FullName, scrubbedDocument.FullName);
        using (WordprocessingDocument wDoc = WordprocessingDocument.Open(scrubbedDocument.FullName, true))
        {
            ScrubFooter(wDoc, new [] { "PAGE" });
        }

        docWithFooter = new FileInfo("../../DocWithFooter2.docx");
        scrubbedDocument = new FileInfo("DocWithFooterScrubbed2.docx");
        if (scrubbedDocument.Exists)
            scrubbedDocument.Delete();
        File.Copy(docWithFooter.FullName, scrubbedDocument.FullName);
        using (WordprocessingDocument wDoc = WordprocessingDocument.Open(scrubbedDocument.FullName, true))
        {
            ScrubFooter(wDoc, new[] { "PAGE", "DATE" });
        }
    }

    private static void ScrubFooter(WordprocessingDocument wDoc, string[] fieldTypesToKeep)
    {
        foreach (var footer in wDoc.MainDocumentPart.FooterParts)
        {
            FieldRetriever.AnnotateWithFieldInfo(footer);
            XElement root = footer.GetXDocument().Root;
            RemoveAllButSpecificFields(root, fieldTypesToKeep);
            footer.PutXDocument();
        }
    }

    private static void RemoveAllButSpecificFields(XElement root, string[] fieldTypesToRetain)
    {
        var cachedAnnotationInformation = root.Annotation<Dictionary<int, List<XElement>>>();
        List<XElement> runsToKeep = new List<XElement>();
        foreach (var item in cachedAnnotationInformation)
        {
            var runsForField = root
                .Descendants()
                .Where(d =>
                {
                    Stack<FieldRetriever.FieldElementTypeInfo> stack = d.Annotation<Stack<FieldRetriever.FieldElementTypeInfo>>();
                    if (stack == null)
                        return false;
                    if (stack.Any(stackItem => stackItem.Id == item.Key))
                        return true;
                    return false;
                })
                .Select(d => d.AncestorsAndSelf(W.r).FirstOrDefault())
                .GroupAdjacent(o => o)
                .Select(g => g.First())
                .ToList();
            foreach (var r in runsForField)
                runsToKeep.Add(r);
        }
        foreach (var paragraph in root.Descendants(W.p).ToList())
        {
            if (paragraph.Elements(W.r).Any(r => runsToKeep.Contains(r)))
            {
                paragraph.Elements(W.r)
                    .Where(r => !runsToKeep.Contains(r) &&
                        !r.Elements(W.tab).Any())
                    .Remove();
                paragraph.Elements(W.r)
                    .Where(r => !runsToKeep.Contains(r))
                    .Elements()
                    .Where(rc => rc.Name != W.rPr &&
                        rc.Name != W.tab)
                    .Remove();
            }
            else
            {
                paragraph.Remove();
            }
        }
        root.Descendants(W.tbl).Remove();
    }
}
