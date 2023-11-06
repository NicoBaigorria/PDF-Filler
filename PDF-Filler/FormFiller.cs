using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
using System.util;
using System.Xml.Linq;
using static OfficeOpenXml.ExcelErrorValue;

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
            using (FileStream docStream = new FileStream(inputFile, FileMode.Open, FileAccess.ReadWrite))
            {
                //Load the existing XFA document.
                PdfLoadedXfaDocument loadedDocument = new PdfLoadedXfaDocument(docStream);
                //Load the existing XFA form.
                PdfLoadedXfaForm loadedForm = loadedDocument.XfaForm;
                //Get the complete field names
                string[] completeFieldNames = loadedForm.CompleteFieldNames;

                Console.WriteLine(completeFieldNames.Length);

                var data = new Dictionary<string, Campo>();

                foreach (string nameField in completeFieldNames)
                {

                    PdfLoadedXfaField field = loadedForm.TryGetFieldByCompleteName(nameField);

                    if (field != null)
                    {


                        string tipo = GetFieldType(field.ToString());

                        listFieldsType.Add(tipo);

                        Campo campo = new Campo(tipo, field.Name, "", new List<string>());

                        if (field.Name == "Sex") {

                           // (field as PdfLoadedXfaComboBoxField).Items.Add("Female");

                            (field as PdfLoadedXfaComboBoxField).Items.ForEach(item =>
                            {

                                Console.WriteLine(item);
                            });

                            (field as PdfLoadedXfaComboBoxField).SelectedIndex = 1;

                           // Console.WriteLine((field as PdfLoadedXfaComboBoxField).Items.ToString);
                        }

                        if (field.Name == "CRCNum") {
                            (field as PdfLoadedXfaTextBoxField).Text = "asdasd";
                        }

                        switch (tipo)
                        {
                            case "PdfLoadedXfaTextBoxField":
                                campo.Value = (field as PdfLoadedXfaTextBoxField).Text;
                                campo.Type = "texto" ;
                                break;
                            case "PdfLoadedXfaComboBoxField":
                                List<string> fields = new List<string>((field as PdfLoadedXfaComboBoxField).Items);

                                List<string> fieldHidden = new List<string>((field as PdfLoadedXfaComboBoxField).HiddenItems);

                                fields.Concat(fieldHidden);

                                 campo.Options = fields;
                                 campo.Type = "selector multiple" ;
                                break;
                            case "PdfLoadedXfaRadioButtonGroup":
                                List<PdfLoadedXfaRadioButtonField> fields2 = new List<PdfLoadedXfaRadioButtonField>((field as PdfLoadedXfaRadioButtonGroup).Fields);

                                List<string> options = new List<string>();
                                foreach (PdfLoadedXfaRadioButtonField name in fields2)
                                {
                                    options.Add(name.Name.ToString());
                                }

                                campo.Type = "selector";
                                campo.Options = options ;

                                break;
                            case "PdfLoadedXfaDateTimeField":
                                campo.Value = (field as PdfLoadedXfaDateTimeField).Value.ToString();
                                campo.Type = "fecha" ;
                                break;
                            case "PdfLoadedXfaCheckBoxField":
                                campo.Value = (field as PdfLoadedXfaCheckBoxField).IsChecked.ToString();
                                campo.Type = "booleano" ;
                                break;
                        }

                        data[field.Name] = campo;

                    }
                    else
                    {
                        Console.WriteLine("Field not found.");
                    }
                }

                string json = System.Text.Json.JsonSerializer.Serialize(data);


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
               // var newFile = new FileInfo("C:\\Users\\nicob\\OneDrive\\Documentos\\GitHub\\PDF-Filler\\PDF-Filler\\OutputExcelFiles\\" + fileName + ".xlsx");


                string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "OutputExcelFiles", fileName + ".xlsx");

                // Check if the file exists or create it if it doesn't.
                FileInfo newFile = new FileInfo(filePath);
                using (var package = new ExcelPackage(newFile))
                {
                    // Access the workbook
                    var workbook = package.Workbook;

                    // Add a worksheet (create a new one if it doesn't exist)
                    var worksheet = workbook.Worksheets.FirstOrDefault(ws => ws.Name == "Sheet1") ?? workbook.Worksheets.Add("Sheet1");

                    int row = 1;

                    // Convert data to JSON
                    string jsonData = JsonConvert.SerializeObject(data, Formatting.Indented);

                    // Deserialize JSON and populate the worksheet
                    var campos = JsonConvert.DeserializeObject<Dictionary<string, Campo>>(jsonData);
                    foreach (var kvp in campos)
                    {
                        Campo campo = kvp.Value;

                        worksheet.Cells[row, 1].Value = campo.Name;
                        worksheet.Cells[row, 2].Value = campo.Value;
                        worksheet.Cells[row, 3].Value = string.Join(", ", campo.Options);
                        worksheet.Cells[row, 4].Value = campo.Type;

                        row++;
                    }

                    // Save the package
                    package.Save();
                }

                Console.WriteLine($"Excel file '{filePath}' created or updated successfully.");
            }
        }

}
}
