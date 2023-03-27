using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class User
    {
        // Поле с Id пользователя (по соглашению автоинкремент и первичный ключ)
        public int Id { get; set; }
        // Поле с логином пользователя
        public string Login { get; set; }
        // Поле с паролем пользователя
        public string Password { get; set; }
        // Поле, указывающее, является ли пользователь админом (по умолчанию false)
        public bool IsAdmin { get; set; } = false;

        [InverseProperty("FromUser")]
        public ICollection<Message> SentMessages { get; set; }

        [InverseProperty("ToUser")]
        public ICollection<Message> ReceivedMessages { get; set; }


    }
}
