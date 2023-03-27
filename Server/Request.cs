namespace Server
{
    internal class Request
    {
        public enum requestType
        {
            AUTH,
            REG,
            ADDMESSAGE,
            GETMESSAGE,
            GETUSERS,
            RECEIVEDMESSAGE
        }


        public int Status { get; set; }
        public requestType Type { get; set; }
        public object[] Content { get; set; }

        /*
         КОДЫ ОШИБОК
            -100 - ошибка JSON
            -200 - ошибка на стороне клиента
            -300 - ошибка на стороне сервера
         
         
         */

    }
}
