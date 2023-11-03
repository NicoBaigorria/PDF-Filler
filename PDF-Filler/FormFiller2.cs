using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aspose.Pdf;

namespace PDF_Filler
{
    internal class FormFiller2
    {

        public void Process(string file) {

            Document doc = new Document(file);

            // Get names of XFA form fields
            string[] names = doc.Form.XFA.FieldNames;

            Console.WriteLine(names.Length);

            foreach (string name in names)
            {
                //Console.WriteLine(name);
                //Console.WriteLine(doc.Form.XFA[name]);
               //Console.WriteLine(doc.Form.XFA.GetFieldTemplate(name).Name);
            }

            for (int count = 0; count < names.Length; count++)
            {
                Console.WriteLine(count);
                Console.WriteLine(names[count]);
                //Console.WriteLine(doc.Form.XFA[names[count]]);
                //Console.WriteLine(doc.Form.XFA.GetFieldTemplate(names[count]));
                /*
                if (doc.Form.XFA.GetFieldTemplate(names[count]).Value != null) {

                    // Get field details
                    var field = doc.Form.XFA.GetFieldTemplate(names[count]);
                    string fieldName = doc.Form.XFA.GetFieldTemplate(names[count]).InnerText;
                    string fieldValue = doc.Form.XFA[names[count]];

                    if (!string.IsNullOrEmpty(fieldValue) && fieldValue != "0")
                    {
                        Console.WriteLine("Field Name : " + field.Attributes["name"].Value);
                        Console.WriteLine("Field Description : " + fieldName);
                        Console.WriteLine("Field Value : " + fieldValue);
                        Console.WriteLine();
                    }
                }
                */
            }

            }

    }
}
