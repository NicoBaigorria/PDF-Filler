using Newtonsoft.Json;
using OfficeOpenXml;
using Syncfusion.Pdf.Xfa;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace PDF_Filler
{
    public class FormFiller
    {

        public void Process(string inputFile)
        {


            string GetFieldType(string text)
            {

                string[] parts = text.Split('.');

                string className = parts[parts.Length - 1];

                return className;
            }

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

                        data[field.Name] = new { Type = tipo };


                        switch (tipo)
                        {
                            case "PdfLoadedXfaTextBoxField":
                                //Console.WriteLine($"{field.Name}: {(field as PdfLoadedXfaTextBoxField).Text}");
                                data[field.Name] = new { Value = (field as PdfLoadedXfaTextBoxField).Text, Type = "texto" };
                                break;
                            case "PdfLoadedXfaComboBoxField":
                                Console.WriteLine($"{field.Name}:");
                                List<string> fields = new List<string>((field as PdfLoadedXfaComboBoxField).Items);

                                List<string> fieldHidden = new List<string>((field as PdfLoadedXfaComboBoxField).HiddenItems);

                                fields.Concat(fieldHidden);

                                //Console.WriteLine($"{fields}:");
                                data[field.Name] = new { Options = fields, Type = "selector multiple" };
                                foreach (string name in fields)
                                {
                                    Console.WriteLine($"* {name}");
                                }
                                break;
                            case "PdfLoadedXfaRadioButtonGroup":
                                List<PdfLoadedXfaRadioButtonField> fields2 = new List<PdfLoadedXfaRadioButtonField>((field as PdfLoadedXfaRadioButtonGroup).Fields);
                                //Console.WriteLine($"{field.Name}:");

                                List<string> options = new List<string>();
                                foreach (PdfLoadedXfaRadioButtonField name in fields2)
                                {
                                    options.Add(name.Name.ToString());
                                    //Console.WriteLine($"* {name.Name}");
                                }

                                data[field.Name] = new { Type = "selector", Options = options };

                                break;
                            case "PdfLoadedXfaDateTimeField":
                                data[field.Name] = new { Value = (field as PdfLoadedXfaDateTimeField).Value, Type = "fecha" };
                                //Console.WriteLine($"{field.Name}: {(field as PdfLoadedXfaDateTimeField).Value}");
                                break;
                            case "PdfLoadedXfaCheckBoxField":
                                data[field.Name] = new { Value = (field as PdfLoadedXfaCheckBoxField).IsChecked, Type = "booleano" };
                                //Console.WriteLine($"{field.Name}: {(field as PdfLoadedXfaCheckBoxField).IsChecked}");
                                break;
                        }

                    }
                    else
                    {
                        Console.WriteLine("Field not found.");
                    }
                }

                string json = System.Text.Json.JsonSerializer.Serialize(data);


                foreach (string key in listFieldsType)
                {
                    Console.WriteLine($"{key}");
                }


                string fileName = Path.GetFileNameWithoutExtension(docStream.Name);

                string jsonFilePath = @"C:\Users\Usuario\source\repos\PDF-Filler\PDF-Filler\PropsFiles\" + fileName + ".json";

                // Save the JSON to the file
                File.WriteAllText(jsonFilePath, json);

                string outputFile = @"C:\Users\Usuario\source\repos\PDF-Filler\PDF-Filler\OutputFiles\" + fileName + ".pdf"; // Replace with the path for the filled PDF.

                //Create memory stream.
                FileStream docStream2 = new FileStream(outputFile, FileMode.Create, FileAccess.Write);
                //Save the document to memory stream.
                loadedDocument.Save(docStream2);

                //Close the document.
                loadedDocument.Close();

                
                
                // Create a new Excel package
                var newFile = new FileInfo("C:\\Users\\Usuario\\source\\repos\\PDF-Filler\\PDF-Filler\\OutputExcelFiles\\" + fileName + ".xlsx");
                using (var package = new ExcelPackage(newFile))
                {
                    // Access the workbook
                    var workbook = package.Workbook;
                    // Add a worksheet
                    var worksheet = workbook.Worksheets["Sheet1"];
                    // Add data to cells

                    int i = 1;

                    List<Campo> info = JsonConvert.DeserializeObject<List<Campo>>(json);

                    /*

                    foreach (Campo propiedad in info)
                    {
                       // Console.WriteLine(propiedad.Name);

                       // worksheet.Cells[i, 1].Value = propiedad.Name;

                        i++;
                    }
                    */

                    // Save the package
                    package.Save();

                }
                

            }

        }

    }
}
