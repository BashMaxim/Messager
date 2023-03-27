using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net;
using System.Net.Sockets;


namespace Server
{
    internal class Server
    {
        private IPEndPoint ip;
        private Socket socket;
        private int max_conn;
        private ManualResetEvent acceptEvent = new ManualResetEvent(false);

        // Подключенные клиенты
        public List<ServerHandler> Clients = new List<ServerHandler>();

        public class Ref<T> where T : class
        {
            public T Value { get; set; }
        }

        public Server(string ip, int port, int max_conn)
        {
            this.ip = new IPEndPoint(IPAddress.Parse(ip), port);
            this.socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            this.max_conn = max_conn;
        }

        public void Init()
        {
            this.socket.Bind(this.ip);
            this.socket.Listen(this.max_conn);
            StartListerning();
            
        }

        private void StartListerning()
        {
            Console.WriteLine("Сервер запущен");
            while (true)
            {
                acceptEvent.Reset();
                this.socket.BeginAccept(new AsyncCallback(AcceptCallBack), this.socket);
                acceptEvent.WaitOne();
            }
        }

        private void AcceptCallBack(IAsyncResult ar)
        {
            Socket socket = ar.AsyncState as Socket;
            Socket accept_socket = socket.EndAccept(ar);
            ServerHandler handler = new ServerHandler(accept_socket, ref Clients);

            // Заносим клиента в список подключенных по обработчику
            try
            {
                Clients.Add(handler);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }

            // Запускаем обработчик подключения
            handler.Start();
            this.acceptEvent.Set();
            
            Console.WriteLine($"Появилось новое подключение; IP:port = {accept_socket.RemoteEndPoint.ToString()}");
        }
    }
}
