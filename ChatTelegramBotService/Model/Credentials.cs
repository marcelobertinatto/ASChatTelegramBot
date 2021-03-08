using System;
namespace ChatTelegramBotService.Model
{
    public class Credentials
    {
        public string authURL { get; set; }
        public string client_secret { get; set; }
        public string basic { get; set; }
        public string client_id { get; set; }
        public string signaturesUrl { get; set; }
        public string newPurchasesUrl { get; set; }
    }
}
