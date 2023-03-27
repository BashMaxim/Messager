using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Server
{
    internal class Program
    {
        static void Main(string[] args)
        {

            string executable = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string path = (System.IO.Path.GetDirectoryName(executable));
            AppDomain.CurrentDomain.SetData("DataDirectory", path);

            using (Context db = new Context())
            {
                db.Database.Initialize(false);
            }

            Server server;

            // Создаем экземпляр класса сервер, в конструктор передается
            // IP, порт и количество подключений
            if (args.Length == 0)
            {
                server = new Server("127.0.0.1", 8006, 50);
                // Инициализируем сервер
                server.Init();
            }
            else
            {
                string Ip = args[0];
                Console.WriteLine(Ip);
                server = new Server(Ip, 8006, 50);
                server.Init();
            }
        }
    }


}
