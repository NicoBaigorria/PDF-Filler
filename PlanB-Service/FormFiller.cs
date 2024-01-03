using iText.Forms.Xfa;
using iText.Forms;
using iText.Kernel.Pdf;
using PlanB_Service.Models;
using Syncfusion.Pdf.Xfa;
using Newtonsoft.Json.Linq;
using Syncfusion.Office;

namespace PlanB_Service
{
    public class FormFiller
    {

        public async Task ProcessAsync(string inputFile, string nameFile, string properties)
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



                        JObject DataProperties = new JObject();

                        DataProperties = JObject.Parse(properties);

                        Console.WriteLine("Data para rellenar");

                        foreach (var property in DataProperties.Properties())
                        {

                            Console.WriteLine(property.Name + ": " + DataProperties[property.Name]);

                        }


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

                                        Console.WriteLine(field.Name);

                                        if (DataProperties[field.Name] != null)
                                        {
                                            try
                                            {
                                                (field as PdfLoadedXfaTextBoxField).Text = DataProperties[field.Name].ToString();
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

                        string jsonFilePath = @"PropsFiles/" + fileName + ".json";

                        // Save the JSON to the file
                        File.WriteAllText(jsonFilePath, json);

                        string outputFile = @"OutputFiles/Pdf/" + fileName + ".pdf"; // Replace with the path for the filled PDF.

                        //Create memory stream.
                        FileStream docStream2 = new FileStream(outputFile, FileMode.Create, FileAccess.Write);
                        //Save the document to memory stream.
                        loadedDocument.Save(docStream2);

                        //Close the document.
                        loadedDocument.Close();
                        docStream2.Close();

                        //Create Folder
                        Hubspot proceso = new Hubspot();
                        string IdFolder = await proceso.CreateFolder("145506339115", nameFile);

                        //Upload File to Folder in Hubspot
                        await proceso.UploadFile(IdFolder, outputFile);
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

        public String GetLinksPdfs(string folder)
        {

            string url = "https://app.hubspot.com/files/21669225/?folderId=" + folder;

            string bodyCard = $@"{{
  ""results"": [
    {{""objectId"": 245,
      ""title"": ""Link a PDFS"",
      ""link"": ""{url}"",
      ""created"": ""2016 - 09 - 15"",
      ""priority"": ""HIGH"",
      ""project"": ""API"",
      ""reported_by"": ""msmith @hubspot.com"",
      ""description"": ""Customer reported that the APIs are just running too fast. This is causing a problem in that they're so happy."",
      ""reporter_type"": ""Account Manager"",
      ""status"": ""In Progress"",
      ""ticket_type"": ""Bug"",
      ""updated"": ""2016 - 09 - 28""
    }}
  ]
}}";
            return bodyCard;
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
