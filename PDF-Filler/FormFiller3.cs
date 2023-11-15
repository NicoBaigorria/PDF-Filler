using iText.Forms;
using iText.Kernel.Pdf;
using iText.Forms.Fields;
using Syncfusion.Pdf.Graphics;
using Syncfusion.Pdf.Xfa;

namespace PDF_Filler
{
    internal class FormFiller3
    {

        public void Process(string inputFile)
        {
            try
            {
                using (PdfReader reader = new PdfReader(inputFile))
                {
                    PdfDocument pdfDoc = new PdfDocument(reader);
                    PdfAcroForm acroForm = PdfAcroForm.GetAcroForm( pdfDoc, true);

                    foreach (string fieldName in acroForm.GetFormFields().Keys)
                    {
                        Console.WriteLine("Field Name: " + fieldName);
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
