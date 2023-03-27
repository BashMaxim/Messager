using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
namespace Server
{
    internal class Context:DbContext
    {
        public Context():base("Connection")
        {

        }
        
        // Таблица с пользователями
        public DbSet<User> Users { get; set; }
        // Таблица с сообщениями
        public DbSet<Message> Messages { get; set; }
    }
}
