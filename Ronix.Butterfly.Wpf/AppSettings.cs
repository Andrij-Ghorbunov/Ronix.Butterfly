using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Ronix.Butterfly.Wpf
{
    public class AppSettings
    {
        public string RootFolder { get; set; }

        public static AppSettings Load()
        {
            using var file = new FileStream("AppSettings.json", FileMode.Open);
            using var reader = new StreamReader(file);
            var text = reader.ReadToEnd();

            return JsonConvert.DeserializeObject<AppSettings>(text);
        }

        public void Save()
        {
            var text = JsonConvert.SerializeObject(this);
            using var file = new FileStream("AppSettings.json", FileMode.Create);
            using var writer = new StreamWriter(file);
            writer.Write(text);
            writer.Flush();
        }
    }
}
