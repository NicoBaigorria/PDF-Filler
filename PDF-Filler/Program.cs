using System;
using System.IO;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using static iTextSharp.text.pdf.codec.TiffWriter;
using Syncfusion.Pdf.Xfa;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;

class Program
{
    static void Main()
    {
        string inputFile = "C:\\Users\\Usuario\\source\\repos\\PDF-Filler\\PDF-Filler\\InputFiles\\imm1294e.pdf"; // Replace with the path to your input XFA PDF form.
        string outputFile = "C:\\Users\\Usuario\\source\\repos\\PDF-Filler\\PDF-Filler\\OutputFiles\\imm1294e.pdf"; // Replace with the path for the filled PDF.

        //Load the PDF document.
        using (FileStream docStream = new FileStream(inputFile, FileMode.Open, FileAccess.Read))
        {
            //Load the existing XFA document.
            PdfLoadedXfaDocument loadedDocument = new PdfLoadedXfaDocument(docStream);
            //Load the existing XFA form.
            PdfLoadedXfaForm loadedForm = loadedDocument.XfaForm;
            //Get the complete field names
            string[] completeFieldNames = loadedForm.CompleteFieldNames;


            Console.WriteLine(completeFieldNames.Length);

            foreach (string nameField in completeFieldNames) {
                Console.WriteLine(nameField);

                PdfLoadedXfaField field = loadedForm.TryGetFieldByCompleteName(nameField);

                if (field != null)
                {
                    try
                    {
                       // (field as PdfLoadedXfaTextBoxField).Text = "dfgfdgfdgdf";
                        // Display the field value.
                        Console.WriteLine($"{field.Name}: {(field as PdfLoadedXfaTextBoxField).Text}");
                    }
                    catch (Exception e) { 
                        Console.WriteLine($"{field.Name} no es un texto");
                    }

                }
                else
                {
                    Console.WriteLine("Field not found.");
                }
            }



            //Create memory stream.
            FileStream docStream2 = new FileStream(outputFile, FileMode.Create, FileAccess.Write);
            //Save the document to memory stream.
            loadedDocument.Save(docStream2);
            //Close the document.
            loadedDocument.Close();
        }

    }
}
