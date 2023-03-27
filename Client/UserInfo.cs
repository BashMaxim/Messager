using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    /// <summary>
    /// Вспомогательный класс, который хранит Id пользователя и его имя,
    /// используется для удобной передачи и приема информации
    /// </summary>
    public class UserInfo
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string LastMessage { get; set; }
        public bool Online { get; set; }
    }
}
