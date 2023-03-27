using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    internal class Database
    {
        /// <summary>
        /// Метод добавляет нового пользователя в базу данных,
        /// добавление происходит только для уникальных пользователей,
        /// возвращает true если пользователь добавлен, и false, если пользователь
        /// не был добавлен
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public static bool AddUser(User user)
        {
            try
            {
                using (Context db = new Context())
                {
                    if (db.Users
                        .Include(u => u.SentMessages)
                        .Include(u => u.ReceivedMessages)
                        .FirstOrDefault(u => u.Login == user.Login) == null)
                    {

                        db.Users.Add(user);
                        db.SaveChanges();
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
                return false;
            }
        }

        /// <summary>
        /// Метод проверяет наличие пользователя в базе данных по логину и паролю,
        /// если пользователь с заданными логином и паролем имеются, возвращается его Id,
        /// в противном случае возвращается -1
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public static int CheckUser(User user)
        {
            try
            {
                using (Context db = new Context())
                {
                    User checkUser = db.Users
                        .Include(u => u.SentMessages)
                        .Include(u => u.ReceivedMessages)
                        .FirstOrDefault(u => u.Login == user.Login && u.Password == user.Password);
                    
                    if (checkUser != null)
                    {
                        return checkUser.Id;
                    }
                    else
                    {
                        return -1;
                    }
                }
        }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
                return -1;
            }
}

    }
}
