using System;

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
