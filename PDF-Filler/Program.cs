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

class Program
{

    static void Main()
    {

        string GetFieldType(string text)
        {

            string[] parts = text.Split('.');

            string className = parts[parts.Length - 1];

            return className;
        }

        string inputFile = "C:\\Users\\Usuario\\source\\repos\\PDF-Filler\\PDF-Filler\\InputFiles\\imm1294e.pdf"; // Replace with the path to your input XFA PDF form.
        string outputFile = "C:\\Users\\Usuario\\source\\repos\\PDF-Filler\\PDF-Filler\\OutputFiles\\imm1294e.pdf"; // Replace with the path for the filled PDF.

        HashSet<string> listFieldsType = new HashSet<string>();

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

            var data = new Dictionary<string, object>();

            foreach (string nameField in completeFieldNames)
            {
                //Console.WriteLine(nameField);

                PdfLoadedXfaField field = loadedForm.TryGetFieldByCompleteName(nameField);

                if (field != null)
                {
                    /*
                    try
                    {
                       // (field as PdfLoadedXfaTextBoxField).Text = "dfgfdgfdgdf";
                        // Display the field value.
                        Console.WriteLine($"{field.Name}: {(field as PdfLoadedXfaTextBoxField).Text}");
                    }
                    catch (Exception e) {

                        Console.WriteLine($"{field.Name}: {tipo}");
                    }
                    */


                    string tipo = GetFieldType(field.ToString());

                    listFieldsType.Add(tipo);

                    //Console.WriteLine($"{field.Name}: {tipo}");

                    data[field.Name] = new { tipo= tipo };


                    switch (tipo)
                    {
                        case "PdfLoadedXfaTextBoxField":
                            Console.WriteLine($"{field.Name}: {(field as PdfLoadedXfaTextBoxField).Text}");
                            data[field.Name] = new { value = (field as PdfLoadedXfaTextBoxField).Text };
                            break;
                        case "PdfLoadedXfaComboBoxField":
                            List<string> fields = new List<string>((field as PdfLoadedXfaComboBoxField).Items);
                            Console.WriteLine($"{field.Name}:");
                            data[field.Name] = new {values = fields };
                            foreach (string name in fields)
                            {
                                Console.WriteLine($"* {name}");
                            }
                            break;
                        case "PdfLoadedXfaRadioButtonGroup":
                            List<PdfLoadedXfaRadioButtonField> fields2 = new List<PdfLoadedXfaRadioButtonField>((field as PdfLoadedXfaRadioButtonGroup).Fields);
                            Console.WriteLine($"{field.Name}:");
                            data[field.Name] = new { values = fields2 };
                            foreach (PdfLoadedXfaRadioButtonField name in fields2)
                            {
                                Console.WriteLine($"* {name.Name}");
                            }
                            break;
                        case "PdfLoadedXfaDateTimeField":
                            data[field.Name] = new { value = (field as PdfLoadedXfaDateTimeField).Value };
                            Console.WriteLine($"{field.Name}: {(field as PdfLoadedXfaDateTimeField).Value}");
                            break;
                        case "PdfLoadedXfaCheckBoxField":
                            data[field.Name] = new { value = (field as PdfLoadedXfaCheckBoxField).IsChecked };
                            Console.WriteLine($"{field.Name}: {(field as PdfLoadedXfaCheckBoxField).IsChecked}");
                            break;
                    }

                }
                else
                {
                    Console.WriteLine("Field not found.");
                }
            }

            string json = JsonSerializer.Serialize(data);


            foreach (string key in listFieldsType)
            {
                Console.WriteLine($"{key}");
            }

            string jsonFilePath = "C:\\Users\\Usuario\\source\\repos\\PDF-Filler\\PDF-Filler\\OutputFiles\\listaCampos.json";

            // Save the JSON to the file
            File.WriteAllText(jsonFilePath, json);


            //Create memory stream.
            FileStream docStream2 = new FileStream(outputFile, FileMode.Create, FileAccess.Write);
            //Save the document to memory stream.
            loadedDocument.Save(docStream2);
            //Close the document.
            loadedDocument.Close();
        }

    }
}
