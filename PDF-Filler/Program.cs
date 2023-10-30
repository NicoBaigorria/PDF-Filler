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
using System.Text.Json.Nodes;
using System.Text.Json;
using System.Linq.Expressions;
using Path = System.IO.Path;
using PDF_Filler;

class Program
{

    static void Main()
    {
        FormFiller filler = new FormFiller();

        string folderPath = @"C:\Users\Usuario\source\repos\PDF-Filler\PDF-Filler\InputFiles\"; // Replace with the path to your folder

        if (Directory.Exists(folderPath))
        {
            string[] pdfFiles = Directory.GetFiles(folderPath, "*.pdf", SearchOption.TopDirectoryOnly);

            if (pdfFiles.Length > 0)
            {
                Console.WriteLine("PDF files found in the folder:");

                foreach (string pdfFile in pdfFiles)
                {
                    Console.WriteLine(pdfFile);
                    filler.Process(pdfFile);
                }
            }
            else
            {
                Console.WriteLine("No PDF files found in the folder.");
            }
        }
        else
        {
            Console.WriteLine("The specified folder does not exist.");
        }
    }
}
