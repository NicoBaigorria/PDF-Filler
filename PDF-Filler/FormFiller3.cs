
using Newtonsoft.Json;
using OfficeOpenXml;
using Syncfusion.Pdf.Parsing;
using Syncfusion.Pdf.Xfa;

namespace PDF_Filler
{
    internal class FormFiller3
    {


        public void Process(string inputFile)
        {

            try
            {
                using (FileStream docStream = new FileStream(inputFile, FileMode.Open, FileAccess.Read))
                {
                    using (PdfLoadedDocument loadedDocument = new PdfLoadedDocument(docStream))
                    {
                        // Check if the PDF has AcroForms
                        if (loadedDocument.Form != null)
                        {

                            // Check if the PDF has XFA form

                            // Read XFA form fields
                            PdfLoadedXfaDocument xfaDoc = new PdfLoadedXfaDocument(docStream);

                            PdfLoadedXfaForm loadedForm = xfaDoc.XfaForm;

                            string[] completeFieldNames = loadedForm.CompleteFieldNames;

                            var data = new Dictionary<string, Campo>();

                            foreach (string nameField in completeFieldNames)
                            {
                                PdfLoadedXfaField field = loadedForm.TryGetFieldByCompleteName(nameField);

                                if (field != null)
                                {
                                    Console.WriteLine($"XFA Form Field: {field.Name}");
                                }
                            }

                        }
                        else
                        {

                            Console.WriteLine(inputFile + " is acroform ");
                            // Read AcroForm fields
                            foreach (PdfLoadedField field in loadedDocument.Form.Fields)
                            {
                                Console.WriteLine($"AcroForm Field: {field.Name}");
                            }

                        }

                        }
                        
                   
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error: {e.Message}");
            }

        }
    }
}
        
