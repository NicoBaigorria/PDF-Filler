
using Syncfusion.Pdf.Parsing;

namespace PDF_Filler
{
    internal class FormFiller3
    {


        public void Process(string inputFile)
        {

            byte[] pdfBytes = File.ReadAllBytes(inputFile);


            using (PdfLoadedDocument loadedDocument = new PdfLoadedDocument(pdfBytes))
            {
                // Create an instance of PdfLoadedComboBoxField
                foreach (var formField in loadedDocument.Form.Fields)
                {
                    if (formField is PdfLoadedComboBoxField comboBoxField)
                    {
                        // Display the ComboBox field name
                        Console.WriteLine("ComboBox Field Name: " + comboBoxField.Name);

                        // Display all options of the ComboBox field
                        foreach (var item in comboBoxField.Items)
                        {
                            Console.WriteLine("  Option: " + item.ToString());
                        }
                    }
                }
            }

        }
    }
}
        
