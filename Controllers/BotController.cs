using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChatTelegramBotService;
using ChatTelegramBotService.Model;
using Microsoft.AspNetCore.Mvc;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace ChatTelegramBot.Controllers
{
    [ApiController]
    [Route("api/bot")]
    public class BotController : ControllerBase
    {
        public HotmartService hotmartService = new HotmartService();
        public List<string> ChatIdList = new List<string>();
        public List<string> AllowedChatIdList = new List<string>();
        public MemoryCacher memCacher = new MemoryCacher();

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Update update)
        {
            TelegramBotClient client = new TelegramBotClient("1417186445:AAGFG-jByzgAEhaZRAKLnnJOigAXbzM8dhU");
            bool wasFoundInChatIdList = false;
            bool checkNewUserInGroup = true;

            var rkm = new ReplyKeyboardRemove();

            InlineKeyboardMarkup myInlineKeyboard = new InlineKeyboardMarkup(

                new InlineKeyboardButton[][]
                {
                    new InlineKeyboardButton[] // First row
                    {
                        InlineKeyboardButton.WithCallbackData( // First Column
                            "/começar", // Button Name
                            "/começar" // Answer you'll recieve
                        ),
                        InlineKeyboardButton.WithCallbackData( //Second column
                            "/suporte", // Button Name
                            "/suporte" // Answer you'll recieve
                        ),
                        InlineKeyboardButton.WithCallbackData( //Second column
                            "/comprarVIP", // Button Name
                            "/comprarVIP" // Answer you'll recieve
                        )
                    }
                }
            );

            ChatIdList = (List<string>)memCacher.GetValue("chatList");
            AllowedChatIdList = (List<string>)memCacher.GetValue("AllowedChatList");

            if (AllowedChatIdList == null)
            {
                AllowedChatIdList = new List<string>();
            }


            if (ChatIdList != null && ChatIdList.Count > 0)
            {
                wasFoundInChatIdList = ChatIdList.Exists(x => x.Equals(update.Message.Chat.Id.ToString()));
            }
            else
            {
                ChatIdList = new List<string>();
            }

            if (checkNewUserInGroup)
            {
                if (update.Message != null && update.Message.NewChatMembers != null)
                {
                    if (update.Message.NewChatMembers.Length > 0)
                    {

                    }
                }
            }

            //first message
            if (update.Message != null)
            {
                if (update.Message.Chat.Id != -1001233703026 && !wasFoundInChatIdList)
                {
                    var matchEmail = ExtractEmails(update.Message.Text);

                    if ((update.Message.Text.Equals("/começar") || update.Message.Text.Equals("/suporte") || update.Message.Text.Equals("/comprarVIP"))
                        && matchEmail.Length == 0)
                    {
                        if (update.Message.Text.Equals("/começar"))
                        {
                            await client.SendTextMessageAsync(update.Message.Chat.Id, "Você selecionou: '/começar'." +
                                "\n" +
                                "\n" +
                                "Preciso que me informe o seu email para que eu possa consultar o nosso sistema de pagamentos. 💰\n" +
                                "Digite seu email abaixo por favor. Estou aguardando......⏳"
                                , replyMarkup: null);
                        }
                        else if (update.Message.Text.Equals("/suporte"))
                        {
                            InlineKeyboardMarkup myInlineSupportKeyboard = new InlineKeyboardMarkup(

                                new InlineKeyboardButton[][]
                                {
                                    new InlineKeyboardButton[] // First row
                                    {
                                        InlineKeyboardButton.WithCallbackData( // First Column
                                            "Falar com o ADM", // Button Name
                                            "/adm" // Answer you'll recieve
                                        ),
                                        InlineKeyboardButton.WithCallbackData( //Second column
                                            "Reclamar", // Button Name
                                            "/reclamar" // Answer you'll recieve
                                        ),
                                        InlineKeyboardButton.WithCallbackData( //Second column
                                            "Verificar status do meu pagamento", // Button Name
                                            "/statuspag" // Answer you'll recieve
                                        )
                                    }
                                }
                            );

                            await client.SendTextMessageAsync(update.Message.Chat.Id, "Você selecionou: '/suporte'." +
                                "\n" +
                                "\n" +
                                "Selecione uma das opções abaixo: \n" +
                                "Estou aguardando......⏳"
                                , replyMarkup: myInlineSupportKeyboard);
                        }
                    }
                    //in case when a user type directly email without selecting options
                    else if(matchEmail.Length > 0)
                    {
                        //get credentials
                        var cred = hotmartService.GetCredentials(true);
                        var auth = hotmartService.Authentication(cred);
                        var signatures = hotmartService.GetSignaturesByEmail(cred, auth, matchEmail[0]);

                        if (signatures.items.Count > 0)
                        {
                            var chatId = update.Message.Chat.Id.ToString();
                            ChatIdList.Add(chatId);

                            await client.SendTextMessageAsync(update.Message.Chat.Id, "Perfeito. Encontramos um pagamento associado " +
                                "a este email ✉️.\n" +
                                "Preciso que você confirme o nome da pessoa que realizou a compra na hotmart.\n" +
                                "Estou aguardando.....⏳", replyMarkup: rkm);

                            memCacher.Add("chatList", ChatIdList, DateTimeOffset.UtcNow.AddHours(1));
                        }
                        else
                        {
                            await client.SendTextMessageAsync(update.Message.Chat.Id, "IIXIII, Não consegui encontrar " +
                                "nenhum pagamento associado a este email ✉️.\n" +
                                "Tem certeza que foi este email? Poderia tentar novamente, por favor?.\n" +
                                "Estou aguardando.....⏳", replyMarkup: rkm);
                        }
                    }
                    else
                    {
                        await client.SendTextMessageAsync(update.Message.Chat.Id, "Olá, somos da equipe ANGEL SIGNALS 🇧🇷🇨🇮. " +
                        "Para iniciar, selecione: '/começar'. Assim conseguiremos " +
                        "garantir o suporte a você!", replyMarkup: myInlineKeyboard);
                    }
                }
                //reply from reply markup keyboard
                else
                {
                    if (update.CallbackQuery != null)
                    {
                        var match = ExtractEmails(update.CallbackQuery.Data);
                        //case of replying with email
                        if (match.Length > 0)
                        {
                            //get credentials
                            var cred = hotmartService.GetCredentials(true);
                            var auth = hotmartService.Authentication(cred);
                            var signatures = hotmartService.GetSignatures(cred, auth);

                        }
                        else
                        {
                            await client.SendTextMessageAsync(update.CallbackQuery.Message.Chat.Id, "Este email não é válido.\n" +
                                "Preciso que me passe seu email ✉️ corretamente.\n" +
                                "Vamos tentar novamente! Estou aguardando.....⏳", replyMarkup: rkm);
                        }
                    }
                    /**********************************************
                    *                                             *
                    *                   SUCCESS                   *
                    *                                             *
                    **********************************************/
                    //everything is alright and the user needs to be added into the VIP group
                    else if (wasFoundInChatIdList)
                    {
                        //get credentials
                        var cred = hotmartService.GetCredentials(true);
                        var auth = hotmartService.Authentication(cred);
                        var signatures = hotmartService.GetSignatures(cred, auth);

                        var user = signatures.items.FirstOrDefault(x => x.subscriber.name.ToUpper().Equals(update.Message.Text.ToUpper()));

                        if (user != null)
                        {
                            if (user.subscriber.name.ToString().ToUpper().Contains(update.Message.From.FirstName.ToString())
                                && user.subscriber.name.ToString().ToUpper().Contains(update.Message.From.LastName.ToString()))
                            {
                                var chatId = update.Message.Chat.Id.ToString();
                                AllowedChatIdList.Add(chatId);

                                await client.SendTextMessageAsync(update.Message.Chat.Id, "OBAAAA 😄😄😎😎.\n" +
                                    "Percebemos que é você mesmo que realizou o pagamento.🤑 \n" +
                                    "Aqui está o link para fazer parte do nosso VIP: https://t.me/joinchat/T9Lo55NGZZ4k3exC"
                                    , replyMarkup: rkm);

                                memCacher.Add("AllowedChatList", AllowedChatIdList, DateTimeOffset.UtcNow.AddHours(1));
                            }
                            else
                            {
                                ChatIdList.Remove(update.Message.Chat.Id.ToString());

                                await client.SendTextMessageAsync(update.Message.Chat.Id, "O nome associado ao pagamento não é o mesmo associado ao Telegram.\n" +
                                    "Favor falar com o Adminitrador selecionando a opção SUPORTE .", replyMarkup: rkm);
                            }
                        }
                        else
                        {
                            await client.SendTextMessageAsync(update.Message.Chat.Id, "Não encontrei este nome.\n" +
                                "Preciso que me passe exatamente o nome da pessoa que realizou a compra na Hotmart .\n" +
                                "Vamos tentar novamente! Estou aguardando.....⏳", replyMarkup: rkm);
                        }
                    }
                    else
                    {
                        await client.SendTextMessageAsync(update.Message.Chat.Id, "Olá, somos da equipe ANGEL SIGNALS 🇧🇷🇨🇮. " +
                            "Para iniciar, selecione: '/começar'. Assim conseguiremos " +
                            "garantir o suporte a você!", replyMarkup: myInlineKeyboard);
                    }
                }
            }
            //reply from reply markup keyboard
            else
            {
                if (update.CallbackQuery.Data != null)
                {                    
                    if (update.CallbackQuery.Data.Equals("/começar") || update.CallbackQuery.Data.Equals("/suporte") || update.CallbackQuery.Data.Equals("/comprarVIP"))
                    {
                        if (update.CallbackQuery.Data.Equals("/começar"))
                        {
                            await client.SendTextMessageAsync(update.CallbackQuery.Message.Chat.Id, "Você selecionou: '/começar'." +
                                "\n" +
                                "\n" +
                                "Preciso que me informe o seu email para que eu possa consultar o nosso sistema de pagamentos. 💰\n" +
                                "Digite seu email abaixo por favor. Estou aguardando......⏳"
                                , replyMarkup: null);
                        }
                        else if(update.CallbackQuery.Data.Equals("/suporte"))
                        {
                            InlineKeyboardMarkup myInlineSupportKeyboard = new InlineKeyboardMarkup(

                                new InlineKeyboardButton[][]
                                {
                                    new InlineKeyboardButton[] // First row
                                    {
                                        InlineKeyboardButton.WithCallbackData( 
                                            "Falar com o ADM", // Button Name
                                            "/adm" // Answer you'll recieve
                                        )
                                    },
                                    new InlineKeyboardButton[] // Second row
                                    {
                                        InlineKeyboardButton.WithCallbackData( 
                                            "Reclamar", // Button Name
                                            "/reclamar" // Answer you'll recieve
                                        )                                        
                                    },
                                    new InlineKeyboardButton[] // Third row
                                    {
                                        InlineKeyboardButton.WithCallbackData( 
                                            "Verificar status do meu pagamento", // Button Name
                                            "/statuspag" // Answer you'll recieve
                                        )
                                    }
                                }
                            );

                            await client.SendTextMessageAsync(update.CallbackQuery.Message.Chat.Id, "Você selecionou: '/suporte'." +
                                "\n" +
                                "\n" +
                                "Selecione uma das opções abaixo: \n" +
                                "Estou aguardando......⏳"
                                , replyMarkup: myInlineSupportKeyboard);
                        }
                    }
                    else if (update.CallbackQuery.Data.Equals("/adm") || update.CallbackQuery.Data.Equals("/reclamar") ||
                        update.CallbackQuery.Data.Equals("/statuspag"))
                    {
                        switch (update.CallbackQuery.Data)
                        {
                            case "/adm":
                                var message = string.Format("⚠️⚠️🚨🚨 Existe uma pessoa querendo falar com os admins. \n" +
                                    "Nome: {0} \n" +
                                    "Username: @{1}\n" +
                                    "Username: {1}\n"+
                                    "ID: {2} 🚨🚨⚠️⚠️", update.CallbackQuery.From.FirstName + " " + update.CallbackQuery.From.LastName,
                                    update.CallbackQuery.From.Username, update.CallbackQuery.From.Id);
                                var result = await SendMessage(client, 1079068893, message);
                                if (result)
                                {
                                    await SendMessage(client, update.CallbackQuery.Message.Chat.Id, "Mensagem enviada com sucesso para o admin. Em breve entraremos em contato! Peço que aguarde, obrigado!");
                                }
                                break;
                            case "/reclamar": //TODO: disponibilizar email para a pessoa reclamar
                                break;
                            case "/statuspag": //TODO: ir na hotmart e verificar o status do pagamento
                            default:
                                break;
                        }
                    }
                    else
                    {
                        await client.SendTextMessageAsync(update.CallbackQuery.Message.Chat.Id, "Olá, somos da equipe ANGEL SIGNALS 🇧🇷🇨🇮. " +
                        "Para iniciar, selecione: '/começar'. Assim conseguiremos " +
                        "garantir o suporte a você!", replyMarkup: myInlineKeyboard);
                    }                    
                }
            }

            return Ok();
        }

        private string[] ExtractEmails(string str)
        {
            string RegexPattern = @"\b[A-Z0-9._-]+@[A-Z0-9][A-Z0-9.-]{0,61}[A-Z0-9]\.[A-Z.]{2,6}\b";

            // Find matches
            System.Text.RegularExpressions.MatchCollection matches
                = System.Text.RegularExpressions.Regex.Matches(str, RegexPattern, System.Text.RegularExpressions.RegexOptions.IgnoreCase);

            string[] MatchList = new string[matches.Count];

            // add each match
            int c = 0;
            foreach (System.Text.RegularExpressions.Match match in matches)
            {
                MatchList[c] = match.ToString();
                c++;
            }

            return MatchList;
        }

        private async Task<bool> SendMessage(TelegramBotClient client, long chatId, string text, IReplyMarkup keyboard = null)
        {
            var returnedValue = false;

            await client.SendTextMessageAsync(chatId, text, replyMarkup: keyboard);

            returnedValue = true;

            return returnedValue;
        }
    }
}
