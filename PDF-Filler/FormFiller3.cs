using iText.Forms;
using iText.Forms.Xfa;
using iText.Kernel.Pdf;
using OfficeOpenXml.Core.ExcelPackage;
using System.Xml;

namespace PDF_Filler
{
    internal class FormFiller3
    {

        static System.Collections.Generic.List<string> ExtractXfaFieldNames(string xfaXml)
        {
            System.Collections.Generic.List<string> fieldNames = new System.Collections.Generic.List<string>();

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xfaXml);

            // XPath to select all elements with a name attribute
            string xpathExpression = "//*[not(@name='')]/@name";

            // Select nodes based on XPath
            XmlNodeList fieldNodes = xmlDoc.SelectNodes(xpathExpression);

            // Extract field names
            foreach (XmlNode node in fieldNodes)
            {
                fieldNames.Add(node.Value);
            }

            return fieldNames;
        }


        static System.Collections.Generic.List<string> ExtractXfaComboBoxOptions(string xfaXml, string comboBoxFieldName)
        {
            System.Collections.Generic.List<string> options = new System.Collections.Generic.List<string>();

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xfaXml);

            // XPath to select the ComboBox field by name
            string xpathExpression = $"//*[local-name()='field' and @name='{comboBoxFieldName}']//*[local-name()='choice']/@value";

            // Select nodes based on XPath
            XmlNodeList optionNodes = xmlDoc.SelectNodes(xpathExpression);

            // Extract options
            foreach (XmlNode node in optionNodes)
            {
                options.Add(node.Value);
            }

            return options;
        }

        public void Process(string inputFile)
        {
            try
            {
                using (PdfReader reader = new PdfReader(inputFile))
                {
                    PdfDocument pdfDoc = new PdfDocument(reader);
                    PdfAcroForm acroForm = PdfAcroForm.GetAcroForm(pdfDoc, true);

                    foreach (string fieldName in acroForm.GetFormFields().Keys)
                    {
                        Console.WriteLine("Field Name: " + fieldName);
                    }

                    XfaForm xfaForm = acroForm.GetXfaForm();

                    string xfaXml = xfaForm.GetDomDocument().OuterXml();

                   
                    // Parse the XML to extract field names
                    var fieldNames = ExtractXfaFieldNames(xfaXml);

                    // Print the field names
                    foreach (var fieldName in fieldNames)
                    {
                        Console.WriteLine($"Xfa Field Name: {fieldName}");

                     var options = ExtractXfaComboBoxOptions(xfaXml, fieldName);

                    Console.WriteLine($"Options for ComboBox '{fieldName}':");
                    foreach (var option in options)
                    {
                        Console.WriteLine(option);
                    }
                    }


                }
            }
            catch (IOException e)
            {
                Console.WriteLine("Error: " + e.Message);
            }
        }


    }
}
