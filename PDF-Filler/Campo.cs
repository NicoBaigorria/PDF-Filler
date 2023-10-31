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
        public List<string> Options { get; set; }

        public Campo(string json)
        {
            Campo data = JsonConvert.DeserializeObject<Campo>(json);
            Type = data.Type;
            Name = data.Name;
            Options = data.Options;
        }

    }
}
