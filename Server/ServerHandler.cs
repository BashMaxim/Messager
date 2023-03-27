using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Data.Entity;

namespace Server
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
    /// При присоединении очередного клиента создается экземпляр этого класса, выполняется асинхронно
    /// </summary>
    internal class ServerHandler
    {
        // Объект сокета поделюченного клиента
        public Socket client_socket;

        // Текущее состояние пользователя
        private State state;

        // Ссылка на экземпляр класса Server
        private List<ServerHandler> ConnectedClients;

        // Если пользователь авторизировался, в эту переменную записывается его Id
        // Изначально равна -1
        public int AuthorizedUserId { get; private set; } = -1;

        // Перечисление состояний пользователя
        private enum State
        {
            NO_AUTH,    // Не авторизован
            AUTH,       // Авторизован
        }
        public ServerHandler(Socket client_socket, ref List<ServerHandler> connectedClients)
        {
            this.ConnectedClients = connectedClients;
            this.client_socket = client_socket;
            this.state = State.NO_AUTH;
        }

        public void Start()
        {
            // Начинаем получать запросы от клиента
            Receive();
        }

        private void Receive()
        {
            StateObject state = new StateObject();
            state.worksocket = this.client_socket;
            state.worksocket.BeginReceive(state.buffer, 0, StateObject.bufferSize, SocketFlags.None, new AsyncCallback(ReceiveCallBack), state);
        }

        public void Send(string message)
        {
            //byte[] terminator = { 0xFD };
            Socket socket = this.client_socket;
            byte[] buffer = Encoding.UTF8.GetBytes(message);
            //byte[] buffer = new byte[msg.Length + 1];
            //msg.CopyTo(buffer, 0);
            //buffer[buffer.Length - 1] = terminator[0];
            this.client_socket.BeginSend(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(SendCallBack), socket);
        }

        private void SendCallBack(IAsyncResult ar)
        {
            Socket handler = ar.AsyncState as Socket;
            handler.EndSend(ar);
            Receive();
        }


        private void ReceiveCallBack(IAsyncResult ar)
        {
            try
            {
                string cmd = String.Empty;
                StateObject state = ar.AsyncState as StateObject;
                Socket handler = state.worksocket;
                if (handler.Connected)
                {
                    int bytes = handler.EndReceive(ar);
                    if (bytes > 0)
                    {
                        state.sb.Append(Encoding.UTF8.GetString(state.buffer, 0, bytes));
                        cmd = Encoding.UTF8.GetString(state.buffer, 0, bytes);
                        if(!String.IsNullOrEmpty(cmd))
                        {
                            // Преобразуем запрос клиента в класс Request
                            Request request = JsonSerializer.Deserialize<Request>(cmd);

                            // Если пользователь не авторизирован
                            if(this.state == State.NO_AUTH) NoAuthRequestHandler(request);
                            // Если пользователь авторизирован
                            if (this.state == State.AUTH) AuthRequestHandler(request);

                        }
                        else
                        {
                            handler.BeginReceive(state.buffer, 0, StateObject.bufferSize, SocketFlags.None, new AsyncCallback(ReceiveCallBack), state);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                try
                {
                    ConnectedClients.Remove(this);
                }
                catch
                {
                    Console.WriteLine($"{client_socket.RemoteEndPoint}:{ex.Message}");
                }
            }
        }

        /// <summary>
        /// Метод обработки запросов на авторизацию и регистрацию
        /// </summary>
        /// <param name="request"></param>
        private void NoAuthRequestHandler(Request request)
        {
            // Если запрос не является запросом на авторизацию отправляем ответ и продолжаем прием данных
            if (request.Type != Request.requestType.AUTH && request.Type != Request.requestType.REG)
            {
                // Создаем ответ клиенту
                Request response = new Request();
                response.Type = request.Type;
                response.Status = -200;
                response.Content = new object[] { "Пользователь не авторизирован" };

                // Отправляем ответ клиенту
                Send(JsonSerializer.Serialize(response));

                // Продолжаем прием данных
                Receive();
                return;
            }
            
            // Если пришел запрос на регистрацию
            if (request.Type == Request.requestType.REG)
            {
                // Проверяем запрос на соответствие передаваемых параметров
                // Если количество объектов в запросе не равно двум (логин и пароль,
                // которые требуются для регистрации), то отправляем клиенту ошибку
                if (request.Content.Length != 2)
                {
                    // Создаем ответ клиенту
                    Request response = new Request();
                    response.Type = request.Type;
                    response.Status = -200;
                    response.Content = new object[] { "Неверный формат передачи запроса на сервер!" };

                    // Отправляем ответ клиенту
                    Send(JsonSerializer.Serialize(response));
                    // Продолжаем прием данных
                    Receive();
                    return;
                }

                // Если все хорошо, то пробуем добавить нового пользователя
                User user = new User();
                user.Login = request.Content[0].ToString();
                user.Password = request.Content[1].ToString();

                // Если пользователь был добавлен в базу данных
                if (Database.AddUser(user))
                {
                    // Создаем ответ клиенту
                    Request responce = new Request();
                    responce.Type = request.Type;
                    responce.Status = 200; // OK
                    responce.Content = new object[] { "Регистрация прошла успешно!" };

                    // Отправляем ответ клиенту
                    Send(JsonSerializer.Serialize(responce));

                    // Продолжаем прием данных
                    Receive();
                    return;
                }
                // Если пользователь не был добавлен в базу данных
                else
                {
                    // Создаем ответ клиенту
                    Request responce = new Request();
                    responce.Type = request.Type;
                    responce.Status = -300;
                    responce.Content = new object[] { "Не удалось зарегистрировать нового пользователя" };

                    // Отправляем ответ клиенту
                    Send(JsonSerializer.Serialize(responce));

                    // Продолжаем прием данных
                    Receive();
                    return;
                }
            }

            // Если пришел запрос на авторизацию
            if (request.Type == Request.requestType.AUTH)
            {
                // Проверяем запрос на соответствие передаваемых параметров
                // Если количество объектов в запросе не равно двум (логин и пароль,
                // которые требуются для регистрации), то отправляем клиенту ошибку
                if (request.Content.Length != 2)
                {
                    // Создаем ответ клиенту
                    Request response = new Request();
                    response.Type = request.Type;
                    response.Status = -200; // Ошибка клиента
                    response.Content = new object[] { "Неверный формат передачи запроса на сервер!" };

                    // Отправляем ответ клиенту
                    Send(JsonSerializer.Serialize(response));
                    // Продолжаем прием данных
                    Receive();
                    return;
                }

                // Если все хорошо, то проверяем наличие пользователя в базе данных
                User user = new User();
                user.Login = request.Content[0].ToString();
                user.Password = request.Content[1].ToString();

                // Получаем Id пользователя из базы данных
                int userId = Database.CheckUser(user);

                // При проверке пользователя в базе данных если он не найден, то
                // возвращается -1, в ином случае возвращается его Id
                if (userId == -1)
                {
                    // Отправляем ответ клиенту о том, что такого пользователя не существует
                    Request responce = new Request();
                    responce.Status = -300; // Ошибка сервера
                    responce.Type = request.Type;
                    responce.Content = new object[] { "Неверное имя пользователя или пароль!"};

                    // Отправка ответа клиенту
                    Send(JsonSerializer.Serialize(responce));

                    // Продолжаем прием данных
                    Receive();
                    return;
                }
                // В остальных случаях заканчиваем авторизацию
                else
                {
                    // Если пользователь уже авторизован и онлайн, то отправляем ошибку
                    if (ConnectedClients.FirstOrDefault(c => c.AuthorizedUserId == userId) != null)
                    {
                        Request Responce = new Request();
                        Responce.Status = -300;
                        Responce.Type = request.Type;
                        Responce.Content = new object[] { "Этот пользователь уже находиться в системе!" };

                        Send(JsonSerializer.Serialize(Responce));
                        return;
                    }

                    // Присваиваем переменной AuthorizedUserId значение Id пользователя
                    AuthorizedUserId = userId;

                    // Переключаем состояние авторизации в "Авторизирован"
                    // В этом состоянии сервер будет обрабатывать другие запросы
                    // клиента, помимо авторизации и регистрации
                    state = State.AUTH;

                    // Отправляем ответ клиенту о том, что авторизация прошла успешно
                    Request responce = new Request();
                    responce.Status = 200; // OK
                    responce.Type = request.Type;
                    // Передаем сообщение сервера и Id пользователя
                    responce.Content = new object[] { "Авторизация прошла успешно!", AuthorizedUserId, user.Login };

                    // Отправляем ответ клиенту
                    Send(JsonSerializer.Serialize(responce));

                    // Продолжаем прием данных
                    Receive();
                    return;
                }
            }
        }

        private void AuthRequestHandler(Request request)
        {
            // Если пришел запрос на получение пользователей
            if (request.Type == Request.requestType.GETUSERS)
            {
                // Проверяем содержимое запроса на корректность,
                // запрос на получение пользователей должен содержать только
                // 1 объект в Content
                if (request.Content.Length != 1)
                {
                    // Создаем ответ клиенту о том, что он отправил
                    // не правильный запрос
                    Request c_responce = new Request();
                    c_responce.Status = -200; // Ошибка клиента
                    c_responce.Type = request.Type;
                    c_responce.Content = new object[] { "Неверный формат передачи запроса на сервер!" };

                    // Отправляем ответ клиенту
                    Send(JsonSerializer.Serialize(c_responce));

                    // Продолжаем прием данных
                    Receive();
                    return;
                }

                // Получаем часть имени пользователя, которого надо искать из запроса
                string username = request.Content[0].ToString();

                // Создаем список, который будет передан клиенту с данными о Id пользователя
                // и его имени
                List<UserInfo> info = new List<UserInfo>();

                using (Context db = new Context())
                {
                    // Получаем пользователей, которые в своем логине имеют подстроку username
                    var users = db.Users.Where(u => u.Login.Contains(username)).ToList();

                    // Записываем их Id и имена в список info
                    foreach (User user in users)
                    {
                        var messages = db.Messages
                        .Include(u => u.FromUser)
                        .Include(u => u.ToUser)
                        .Where(u => (u.ReceiverId == user.Id && u.SenderId == AuthorizedUserId) || (u.ReceiverId == AuthorizedUserId && u.SenderId == user.Id))
                        .ToList();

                        string lastMsg = string.Empty;

                        foreach (Message message in messages)
                            lastMsg = message.Text;

                        bool isOnline = ConnectedClients.FirstOrDefault(c => c.AuthorizedUserId == user.Id) == null ? false : true;

                        info.Add(new UserInfo() { Id = user.Id, Username = user.Login, LastMessage = lastMsg, Online = isOnline});
                    }
                }

                // Создаем ответ клиенту
                Request responce = new Request();
                responce.Type = request.Type;
                responce.Status = 200; // OK
                responce.Content = new object[] { info };

                // Отправляем ответ клиенту
                Send(JsonSerializer.Serialize(responce));

                // Продолжаем прием данных
                Receive();
                return;
                
            }

            // Если пришел запрос на получение сообщений
            if (request.Type == Request.requestType.GETMESSAGE)
            {
                // Проверяем содержимое запроса на корректность,
                // запрос на получение пользователей должен содержать только
                // 1 объект в Content
                if (request.Content.Length != 1)
                {
                    // Создаем ответ клиенту о том, что он отправил
                    // не правильный запрос
                    Request c_responce = new Request();
                    c_responce.Status = -200; // Ошибка клиента
                    c_responce.Type = request.Type;
                    c_responce.Content = new object[] { "Неверный формат передачи запроса на сервер!" };

                    // Отправляем ответ клиенту
                    Send(JsonSerializer.Serialize(c_responce));

                    // Продолжаем прием данных
                    Receive();
                    return;
                }

                // Получаем id пользователя, для которого нужно получить сообщения
                int userId = int.Parse(request.Content[0].ToString());

                using (Context db = new Context())
                {
                    var messages = db.Messages
                        .Include(u => u.FromUser)
                        .Include(u => u.ToUser)
                        .Where(u => (u.ReceiverId == userId && u.SenderId == AuthorizedUserId) || (u.ReceiverId == AuthorizedUserId && u.SenderId == userId))
                        .ToList();

                    messages.Reverse();
                    messages = messages.Take(15).ToList();
                    messages.Reverse();

                    List<MessageInfo> messagesList = new List<MessageInfo>();

                    foreach (var message in messages)
                    {
                        MessageInfo messageInfo = new MessageInfo();
                        messageInfo.SenderId = message.SenderId;
                        messageInfo.ReceiverId = message.ReceiverId;
                        messageInfo.ReceiverName = message.ToUser.Login;
                        messageInfo.SenderName = message.FromUser.Login;
                        messageInfo.Text = message.Text;
                        messageInfo.SendTime = message.SendTime;                     

                        messagesList.Add(messageInfo);

                    }
                    // Создаем новый ответ клиенту
                    Request responce = new Request();
                    responce.Status = 200; // OK
                    responce.Type = request.Type;
                    responce.Content = new object[] { messagesList };

                    // Отправляем ответ клиенту
                    Send(JsonSerializer.Serialize(responce));

                    // Продолжаем прием данных
                    Receive();
                    return;
                }
            }

            // Если пришел запрос на добавление сообщения
            if (request.Type == Request.requestType.ADDMESSAGE)
            {
                // Проверяем содержимое запроса на корректность,
                // запрос на получение пользователей должен содержать только
                // 2 объекта в Content
                if (request.Content.Length != 2)
                {
                    // Создаем ответ клиенту о том, что он отправил
                    // не правильный запрос
                    Request c_responce = new Request();
                    c_responce.Status = -200; // Ошибка клиента
                    c_responce.Type = request.Type;
                    c_responce.Content = new object[] { "Неверный формат передачи запроса на сервер!" };

                    // Отправляем ответ клиенту
                    Send(JsonSerializer.Serialize(c_responce));

                    // Продолжаем прием данных
                    Receive();
                    return;
                }

                // Id получателя
                int ReceiverId = int.Parse(request.Content[0].ToString());
                string Message = request.Content[1].ToString();

                using (Context db = new Context())
                {
                    try
                    {
                        // Создаем экземпляр класса Message и заполняем его
                        Message msg = new Message();
                        msg.Text = Message;
                        msg.SenderId = AuthorizedUserId;
                        msg.ReceiverId = ReceiverId;
                        msg.SendTime = DateTime.Now;

                        // Сохраняем в базу данных
                        db.Messages.Add(msg);
                        db.SaveChanges();

                        // Отправляем ответ клиенту о успешном добавлении сообщения
                        Request responce = new Request();
                        responce.Status = 200; // OK
                        responce.Type = request.Type;
                        responce.Content = new object[] { "Сообщение успешно добавлено!" };

                        Send(JsonSerializer.Serialize(responce));

                        // Отправляем уведомление получателю о том, что для него добавлено сообщение

                        User sender = db.Users.FirstOrDefault(u => u.Id == AuthorizedUserId);

                        Request receiverResponce = new Request();
                        receiverResponce.Status= 200; // OK
                        receiverResponce.Type = Request.requestType.RECEIVEDMESSAGE;                        
                        receiverResponce.Content = new object[] { AuthorizedUserId, Message, sender.Login}; // Передаем Id пользователя, от которого получено сообщение

                        // Если пользователь онлайн, то ему отправиться сообщение, иначе не отправиться
                        try
                        {
                            var handler = ConnectedClients
                                .First(c => c.AuthorizedUserId == ReceiverId);

                            handler.Send(JsonSerializer.Serialize(receiverResponce));
                        }
                        catch { }

                        
                    }
                    catch (Exception ex)
                    {
                        // При неудаче отправляем ответ клиенту об ошибке
                        Request responce = new Request();
                        responce.Status = -300; // Ошибка сервера
                        responce.Type = request.Type;
                        responce.Content = new object[] {"Не удалось добавить сообщение"};

                        Send(JsonSerializer.Serialize(responce));

                        Console.WriteLine(ex.Message);
                        Console.WriteLine(ex.StackTrace);
                    }
                }

                // Продолжаем прием данных
                Receive();
                return;
            }
        }
    }
}
