using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class Message
    {
        // Id сообщения (по соглашению автоинкремент и первичный ключ)
        public int Id { get; set; }
        // Сообщение
        public string Text { get; set; }
        // Id отправителя
        [ForeignKey("FromUser")]
        public int? SenderId { get; set; }
        public virtual User FromUser { get; set; }

        // Id получателя
        [ForeignKey("ToUser")]
        public int? ReceiverId { get; set; }
        public User ToUser { get; set; }

        public DateTime SendTime { get; set; }

    }
}
