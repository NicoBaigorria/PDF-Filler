
using iText.Forms;
using iText.Forms.Xfa;
using iText.Kernel.Pdf;
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

                if (IsXfaForm(inputFile))
                {
                    Console.WriteLine("The PDF contains XFA form.");
                }
                else if (IsAcroForm(inputFile))
                {
                    Console.WriteLine("The PDF contains AcroForm.");
                }
                else
                {
                    Console.WriteLine("The PDF does not contain XFA or AcroForm.");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error: {e.Message}");
            }

        }

        static bool IsXfaForm(string filePath)
        {
            using (PdfReader reader = new PdfReader(filePath))
            {
                using (PdfDocument pdfDoc = new PdfDocument(reader))
                {
                    PdfAcroForm acroForm = PdfAcroForm.GetAcroForm(pdfDoc, true);
                    XfaForm xfaForm = acroForm.GetXfaForm();
                    return xfaForm != null;
                }
            }
        }

        static bool IsAcroForm(string filePath)
        {
            using (PdfReader reader = new PdfReader(filePath))
            {
                using (PdfDocument pdfDoc = new PdfDocument(reader))
                {
                    PdfAcroForm acroForm = PdfAcroForm.GetAcroForm(pdfDoc, true);
                    return acroForm.GetFormFields().Count > 0;
                }
            }
        }
    }
}
        
