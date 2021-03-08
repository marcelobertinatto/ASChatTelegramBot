using System;
namespace ChatTelegramBotService.Model
{
    public class UserLog
    {
        public string Nome { get; set; }
        public string Telefone { get; set; }
        public string Email { get; set; }
        public string ID { get; set; }
        public long Data { get; set; }
        public DateTime Data_LOG { get; set; }
    }
}
