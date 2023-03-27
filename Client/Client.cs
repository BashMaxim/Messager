using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Tulpep.NotificationWindow;

namespace Client
{
    /// <summary>
    /// Класс хранит информацию о работающем сокете, это нужно для получения
    /// информации в callback функциях, так как в них передается объект
    /// </summary>
    internal class StateObject
    {
        public Socket worksocket = null;
        public const int bufferSize = 700000;
        public byte[] buffer = new byte[bufferSize];
        public StringBuilder sb = new StringBuilder();
    }


    /// <summary>
    /// Класс отвечает за отправку и принятие запросов с сервера
    /// </summary>
    public class Client
    {
        private Socket client_socket;
        private EndPoint ip;
        private Auth authForm;
        private Main mainForm;

        private readonly ManualResetEvent received = new ManualResetEvent(false);

        private List<byte> data = new List<byte>();

        public Client(string ip, int port, Auth authForm)
        {
            this.authForm = authForm;
            this.ip = new IPEndPoint(IPAddress.Parse(ip), port);
            this.client_socket = new Socket(this.ip.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

        }

        public void Connect()
        {
            this.client_socket.BeginConnect(this.ip, new AsyncCallback(ConnectCallBack), this.client_socket);
        }

        private void ConnectCallBack(IAsyncResult ar)
        {
            Socket handler = ar.AsyncState as Socket;
            this.client_socket.EndConnect(ar);
        }

        public void Disconnect()
        {
            this.client_socket.BeginDisconnect(false, new AsyncCallback(DisconnectCallBack), this.client_socket);

        }

        private void DisconnectCallBack(IAsyncResult ar)
        {
            Socket handler = (Socket)ar.AsyncState as Socket;
            handler.EndDisconnect(ar);
            Console.WriteLine("Отключаюсь...");

        }

        public void Send(string message)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(message);
            this.client_socket.BeginSend(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(SendCallBack), this.client_socket);
        }

        private void SendCallBack(IAsyncResult ar)
        {
            Socket handler = ar.AsyncState as Socket;
            handler.EndSend(ar);
            Receive();
        }

        public void Receive()
        {
            StateObject state = new StateObject();
            state.worksocket = this.client_socket;
            state.worksocket.BeginReceive(state.buffer, 0, StateObject.bufferSize, SocketFlags.None, new AsyncCallback(ReceiveCallBack), state);
        }

        private void ReceiveCallBack(IAsyncResult ar)
        {
            try
            {
                // Получаем информацию с сокета
                string cmd = String.Empty;
                StateObject state = (StateObject)ar.AsyncState;
                Socket socket = state.worksocket;

                if (socket.Connected)
                {

                    int bytes = socket.EndReceive(ar);

                    if (bytes > 0)
                    {
                        
                        cmd = Encoding.UTF8.GetString(state.buffer, 0, bytes);

                        // Если ответ не является пустой строкой
                        if (!string.IsNullOrEmpty(cmd.ToString()))
                        {;
                            // Преобразуем его в класс Request
                            Request request = JsonSerializer.Deserialize<Request>(cmd.ToString());

                            // В случае ответа на авторизацию
                            if (request.Type == Request.requestType.AUTH)
                            {
                                // В случае если авторизация прошла успешно
                                if (request.Status == 200) // OK
                                {
                                    // Скрываем форму авторизации
                                    authForm.Hide();
                                    Main main = new Main(this);
                                    main.Text = request.Content[2].ToString();
                                    // Инициализируем переменную mainForm для взаимодействия с главной формой

                                    // Открываем главную форму, передаем в нее текущий экземпляр класса Client
                                    Application.Run(main);
                                }
                                // В случае проблем с авторизацией
                                else
                                {
                                    MessageBox.Show(request.Content[0].ToString());

                                }
                            }
                            // В случае ответа на регистрацию
                            else if (request.Type == Request.requestType.REG)
                            {
                                MessageBox.Show(request.Content[0].ToString());
                                Receive();
                            }
                            // В случае ответа на получение пользователей
                            else if (request.Type == Request.requestType.GETUSERS)
                            {
                                // Преобразуем ответ сервера в список UserInfo
                                List<UserInfo> users = JsonSerializer.Deserialize<List<UserInfo>>(request.Content[0].ToString());

                                // В первую очередь сортируем по онлайну
                                users = users.OrderBy(u => u.Online == false).ToList();

                                // Отправляем пользователей на форму для их отображения
                                mainForm.UpdateSearchResult(users);
                                Receive();
                            }
                            // В случае ответа на добавление сообщения
                            else if (request.Type == Request.requestType.ADDMESSAGE)
                            {
                                Receive();
                            }
                            // В случае если пришло новое сообщение
                            else if (request.Type == Request.requestType.RECEIVEDMESSAGE)
                            {
                                int userId = int.Parse(request.Content[0].ToString());
                                string msg = request.Content[1].ToString();
                                string senderName = request.Content[2].ToString();

                                // Если сейчас открыта переписка с этим пользователем
                                if (mainForm.OpenedUserId == userId && mainForm.WindowState != FormWindowState.Minimized)
                                {
                                    // Получаем сообщения
                                    mainForm.GetMessagesForUser(userId);
                                    // Обновляем последнее сообщение
                                    mainForm.UpdateLastMessage(msg, mainForm.OpenedUserId);
                                    // Делаем статус человека "Online"
                                    mainForm.UpdateOnlineStatus(true, userId);
                                }
                                // Если переписка не открыта
                                else     
                                {
                                    // Получаем сообщения
                                    mainForm.GetMessagesForUser(userId);
                                    // Обновляем последнее сообщение
                                    mainForm.UpdateLastMessage(msg, userId);
                                    // Показываем значок нового сообщения
                                    mainForm.ShowNewMessageNotify(userId);
                                    // Показываем уведомление 
                                    mainForm.ShowNewMessagePopup(senderName, msg);
                                    // Делаем статус человека "Online"
                                    mainForm.UpdateOnlineStatus(true, userId);
                                }

                                Receive();
                            }
                            // В случае ответа на получение сообщений
                            else if (request.Type == Request.requestType.GETMESSAGE)
                            {
                                List<MessageInfo> receivedMessages = JsonSerializer.Deserialize<List<MessageInfo>>(request.Content[0].ToString());

                                mainForm.UpdateMessagesFlowPanel(receivedMessages);

                                Receive();
                            }
                        }
                        else
                        {
                            socket.BeginReceive(state.buffer, 0, StateObject.bufferSize, SocketFlags.None, new AsyncCallback(ReceiveCallBack), state);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void BindMainForm(Main mf)
        {
            this.mainForm = mf;
        }
    }
}
