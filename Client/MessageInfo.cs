using System;

namespace Client
{
    public class MessageInfo
    {
        public string SenderName { get; set; }
        public string ReceiverName { get; set; }
        public int? SenderId { get; set; }
        public int? ReceiverId { get; set; }
        public string Text { get; set; }
        public DateTime SendTime { get; set; }
    }
}
