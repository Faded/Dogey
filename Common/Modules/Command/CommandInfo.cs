using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dogey.Common.Modules
{
    public class CommandInfo
    {
        /// <summary>The command's execution text.</summary>
        public string Name { get; set; }

        /// <summary>Defines the commands bind status.</summary>
        public Bind Bind { get; set; }
        /// <summary>Id of the object this command is bound to, null if global.</summary>
        public ulong? BoundTo { get; set; }

        /// <summary>The user that created this command.</summary>
        public ulong CreatedBy { get; set; }
        /// <summary>The date and time this command was created.</summary>
        public DateTime CreatedOn { get; set; }
        /// <summary>The user that last edited this command.</summary>
        public ulong EditedBy { get; set; }
        /// <summary>The date and time this command was last edited.</summary>
        public DateTime EditedOn { get; set; }

        /// <summary>The command's message pool.</summary>
        public List<string> Messages { get; set; }

        public CommandInfo()
        {
            Messages = new List<string>();
        }
    }

    public enum Bind
    {
        Global,
        Server,
        Channel
    }
}
