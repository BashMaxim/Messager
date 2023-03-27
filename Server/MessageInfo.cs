using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    internal class MessageInfo
    {
        public string SenderName { get; set; }
        public string ReceiverName { get; set; }
        public int? SenderId { get; set; }
        public int? ReceiverId { get; set; }
        public string Text { get; set; }
        public DateTime SendTime { get; set; }
    }
}
