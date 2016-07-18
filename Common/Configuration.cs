using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dogey.Common
{
    public class Configuration
    {
        public char Prefix { get; set; }
        public List<ulong> Owner { get; set; }
        public Tokens Token { get; set; }

        public Configuration()
        {
            Token = new Tokens();
            Owner = new List<ulong>();
        }

        public Configuration FromFile(string file)
        {
            return JsonConvert.DeserializeObject<Configuration>(File.ReadAllText(file));
        }

        public void ToFile(string file)
        {
            File.WriteAllText(file, JsonConvert.SerializeObject(this, Formatting.Indented));
        }

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }
    }

    public class Tokens
    {
        public string Discord { get; set; }
        public string Google { get; set; }
    }
}