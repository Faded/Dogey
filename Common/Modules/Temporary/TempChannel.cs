using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dogey.Common.Modules
{
    public class TempChannel
    {
        public ulong Id { get; set; }
        public string Name { get; set; }

        public ulong CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}
