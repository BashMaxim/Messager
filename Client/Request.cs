namespace Client
{
    public class Request
    {
        public enum requestType
        {
            AUTH,
            REG,
            GETCHATS,
            ADDCHAT,
            ADDMESSAGE,
            GETMESSAGE,
            GETUSERS,
            RECEIVEDMESSAGE
        }

        public requestType Type { get; set; }
        public object[] Content { get; set; }
        public int Status { get; set; }

        /*
         КОДЫ ОШИБОК
            -100 - ошибка JSON
            -200 - ошибка на стороне клиента
            -300 - ошибка на стороне сервера
            
         КОДЫ УСПЕХА
            200 - OK
         */
    }
}
