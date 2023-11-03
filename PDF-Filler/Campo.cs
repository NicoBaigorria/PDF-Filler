using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDF_Filler
{
    internal class Campo
    {

        public string Type { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        public List<string> Options { get; set; }

        public Campo(string Type, string Name, string Value, List<string> Options)
        {
            this.Type = Type;
            this.Name = Name;
            this.Value = Value;
            this.Options = Options;

            //Console.WriteLine(Name);
        }

    }
}
