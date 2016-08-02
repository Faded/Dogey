using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dogey.Common.Modules
{
    public class CustomCmd
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public AccessLevel UserAccess { get; set; }
        
        public CommandInfo Created { get; set; }
        public CommandInfo Edited { get; set; }
        public CommandInfo Deleted { get; set; }
        public CommandInfo Restored { get; set; }

        public List<string> Messages { get; set; }

        public CustomCmd()
        {
            UserAccess = AccessLevel.User;
            Messages = new List<string>();
            Created = new CommandInfo();
            Edited = new CommandInfo();
        }

        public CustomCmd(string file)
        {
            var obj = JsonConvert.DeserializeObject<CustomCmd>(File.ReadAllText(file));
            Name = obj.Name;
            Description = obj.Description;
            UserAccess = obj.UserAccess;
            Created = obj.Created;
            Edited = obj.Edited;
            Deleted = obj.Deleted;
            Restored = obj.Restored;
            Messages = obj.Messages;
            
        }
    }
}
