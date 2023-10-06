using System;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using static iTextSharp.text.pdf.codec.TiffWriter;

class Program
{
    static void Main()
    {
        string inputFile = "C:\\Users\\Usuario\\source\\repos\\PDF-Filler\\PDF-Filler\\InputFiles\\imm1294e.pdf"; // Replace with the path to your input XFA PDF form.
        string outputFile = "C:\\Users\\Usuario\\source\\repos\\PDF-Filler\\PDF-Filler\\OutputFiles\\imm1294e.pdf"; // Replace with the path for the filled PDF.

        using (var pdfReader = new PdfReader(inputFile))
        {
            PdfReader.unethicalreading = true;

            // Check if the PDF has a user password (open password).
            if (pdfReader.IsOpenedWithFullPermissions)
            {
                Console.WriteLine("PDF can be opened with full permissions.");
            }
            else
            {
                Console.WriteLine("PDF can be opened with restricted permissions.");
            }

            // Retrieve the permissions as a bitmask.
            long permissions = pdfReader.Permissions;

            // Check individual permission flags.
            if ((permissions & PdfWriter.ALLOW_PRINTING) == PdfWriter.ALLOW_PRINTING)
            {
                Console.WriteLine("Printing is allowed.");
            }
            if ((permissions & PdfWriter.ALLOW_COPY) == PdfWriter.ALLOW_COPY)
            {
                Console.WriteLine("Copy and extract text/images is allowed.");
            }
            if ((permissions & PdfWriter.ALLOW_MODIFY_CONTENTS) == PdfWriter.ALLOW_MODIFY_CONTENTS)
            {
                Console.WriteLine("Modifying content is allowed.");
            }
            if ((permissions & PdfWriter.ALLOW_FILL_IN) == PdfWriter.ALLOW_FILL_IN)
            {
                Console.WriteLine("Filling in form fields is allowed.");
            }
            if ((permissions & PdfWriter.ALLOW_SCREENREADERS) == PdfWriter.ALLOW_SCREENREADERS)
            {
                Console.WriteLine("Accessibility is allowed.");
            }

            // You can check other permissions as needed.

            Console.WriteLine("PDF permissions read successfully.");


            // Create a PdfStamper to modify the PDF.
            using (var pdfStamper = new PdfStamper(pdfReader, new FileStream(outputFile, FileMode.Create)))
            {
                // Get the form fields.
                var formFields = pdfStamper.AcroFields;

                // List the field names.
                var fieldNames = formFields.Fields.Keys;

                // Print the field names.
                foreach (var fieldName in fieldNames)
                {
                    Console.WriteLine("Field Name: " + fieldName);

                    // Fill the form fields. Replace field names and values as needed.
                    formFields.SetField(fieldName, "asdasdsadasdasfadasdfsad ");
                }

            }

            Console.WriteLine("PDF form filled successfully.");

        }

    }
}
