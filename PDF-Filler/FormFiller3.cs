
using iText.Forms;
using iText.Forms.Xfa;
using iText.Kernel.Pdf;
using Newtonsoft.Json;
using OfficeOpenXml;
using PDF_Filler.Services;
using Syncfusion.Pdf.Parsing;
using Syncfusion.Pdf.Xfa;

namespace PDF_Filler
{
    internal class FormFiller3
    {


        public async Task ProcessAsync(string inputFile)
        {

            try
            {

                if (IsXfaForm(inputFile))
                {
                    Console.WriteLine("The PDF contains XFA form.");

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

                        var data = new Dictionary<string, Campo>();

                        foreach (string nameField in completeFieldNames)
                        {

                            PdfLoadedXfaField field = loadedForm.TryGetFieldByCompleteName(nameField);

                            if (field != null)
                            {


                                string tipo = GetFieldType(field.ToString());

                                listFieldsType.Add(tipo);

                                Campo campo = new Campo(tipo, field.Name, "", new List<string>());


                                switch (tipo)
                                {
                                    case "PdfLoadedXfaTextBoxField":
                                        campo.Value = (field as PdfLoadedXfaTextBoxField).Text;
                                        campo.Type = "texto";

                                        if (field.Name == "FamilyName")
                                        {
                                            try
                                            {
                                                (field as PdfLoadedXfaTextBoxField).Text = "sadsa";
                                            }
                                            catch (Exception e)
                                            {
                                                Console.WriteLine("error al editar campo de texto" + field.Name);
                                            }
                                        }
                                        break;
                                    case "PdfLoadedXfaComboBoxField":
                                        List<string> fields = new List<string>((field as PdfLoadedXfaComboBoxField).Items);

                                        if (field != null)
                                        {

                                            foreach (var option in fields)
                                            {
                                                string optionText = option;
                                                Console.WriteLine("opcion: " + optionText);
                                            }

                                            if (field.Name == "Sex")
                                            {

                                                //(field as PdfLoadedXfaComboBoxField).Items.Add("asdsad");

                                                // (field as PdfLoadedXfaComboBoxField).SelectedIndex = 0;

                                                Console.WriteLine((field as PdfLoadedXfaComboBoxField).SelectedValue);

                                                // (field as PdfLoadedXfaComboBoxField).SelectedValue = "Female";

                                                Console.WriteLine((field as PdfLoadedXfaComboBoxField).Items.Capacity);
                                            }
                                        }


                                        campo.Options = fields;
                                        campo.Type = "selector multiple";
                                        break;
                                    case "PdfLoadedXfaRadioButtonGroup":
                                        List<PdfLoadedXfaRadioButtonField> fields2 = new List<PdfLoadedXfaRadioButtonField>((field as PdfLoadedXfaRadioButtonGroup).Fields);

                                        List<string> options = new List<string>();
                                        foreach (PdfLoadedXfaRadioButtonField name in fields2)
                                        {
                                            options.Add(name.Name.ToString());
                                        }

                                        campo.Type = "selector";
                                        campo.Options = options;

                                        break;
                                    case "PdfLoadedXfaDateTimeField":
                                        campo.Value = (field as PdfLoadedXfaDateTimeField).Value.ToString();
                                        campo.Type = "fecha";
                                        break;
                                    case "PdfLoadedXfaCheckBoxField":
                                        campo.Value = (field as PdfLoadedXfaCheckBoxField).IsChecked.ToString();
                                        campo.Type = "booleano";
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

                        string outputFile = @"C:\Users\Usuario\source\repos\PDF-Filler\PDF-Filler\OutputFiles\Pdf\" + fileName + ".pdf"; // Replace with the path for the filled PDF.

                        //Create memory stream.
                        FileStream docStream2 = new FileStream(outputFile, FileMode.Create, FileAccess.Write);
                        //Save the document to memory stream.
                        loadedDocument.Save(docStream2);

                        //Close the document.
                        loadedDocument.Close();
                        docStream2.Close();

                        //Ceate Folder
                        Hubspot proceso = new Hubspot();
                        string IdFolder = await proceso.CreateFolder("145506339115", "fede123456789" );

                        //Upload File to Folder in Hubspot
                        await proceso.UploadFile(IdFolder, outputFile);


                        //Create XML
                        string outputFileXml = @"C:\Users\Usuario\source\repos\PDF-Filler\PDF-Filler\OutputFiles\Xml\" + fileName + ".xml";

                        FileStream docStream3 = new FileStream(outputFileXml, FileMode.Create, FileAccess.Write);

                        loadedForm.ExportXfaData(docStream3);


                        // Create a new Excel package

                        // var newFile = new FileInfo("C:\\Users\\nicob\\OneDrive\\Documentos\\GitHub\\PDF-Filler\\PDF-Filler\\OutputExcelFiles\\" + fileName + ".xlsx");


                        string filePath = "C:\\Users\\Usuario\\source\\repos\\PDF-Filler\\PDF-Filler\\OutputFiles\\Xlsx\\" + fileName + ".xlsx";

                        Console.WriteLine(filePath);

                        // Check if the file exists or create it if it doesn't.
                        FileInfo newFile = new FileInfo(filePath);
                        using (var package = new ExcelPackage(newFile))
                        {
                            // Access the workbook
                            var workbook = package.Workbook;

                            // Add a worksheet (create a new one if it doesn't exist)
                            var worksheet = workbook.Worksheets.FirstOrDefault(ws => ws.Name == "Sheet1") ?? workbook.Worksheets.Add("Sheet1");

                            int i = 1;

                            // Convert data to JSON
                            string jsonData = JsonConvert.SerializeObject(data, Formatting.Indented);


                            foreach (var prop in data)
                            {

                                Campo campo = prop.Value as Campo;

                                worksheet.Cells[i, 1].Value = campo.Name;

                                worksheet.Cells[i, 2].Value = campo.Value;

                                campo.Options.ForEach(option => Console.WriteLine(option));

                                worksheet.Cells[i, 3].Value = String.Join(", ", campo.Options.ToArray());

                                worksheet.Cells[i, 4].Value = campo.Type;

                                i++;

                            }

                            // Save the package
                            package.Save();
                        }


                        Console.WriteLine($"Excel file '{filePath}' created or updated successfully.");

                    }
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

