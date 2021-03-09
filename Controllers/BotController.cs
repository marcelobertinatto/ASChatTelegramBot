using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChatTelegramBotService;
using ChatTelegramBotService.Data;
using ChatTelegramBotService.Model;
using Microsoft.AspNetCore.Http;
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
        public List<string> ChatIdToBeIgnored = new List<string>();
        public List<string> AdminsList = new List<string>();
        public List<ChatTelegramBotService.User> RegisteredUsers = new List<ChatTelegramBotService.User>();
        public Item user = null;
        private ChatBotContext _context;
        private string bootToken = "1417186445:AAGFG-jByzgAEhaZRAKLnnJOigAXbzM8dhU";

        public BotController(ChatBotContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Update update)
        {
            TelegramBotClient client = new TelegramBotClient("1417186445:AAGFG-jByzgAEhaZRAKLnnJOigAXbzM8dhU");
            bool wasFoundInUserRegisteredList = false;
            
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

            //List of Admins
            AdminsList.AddRange(new List<string> { "1079068893" });

            //Chat Id's to be ignored by the BOT
            ChatIdToBeIgnored.AddRange(new List<string> { "-1001233703026", "-1001150279812", "-1001150279812" });

            //RegisteredUsers = (List<ChatTelegramBotService.User>)memCacher.GetValue("registeredUsers");
            RegisteredUsers = _context.User.ToList();

            if (RegisteredUsers != null && RegisteredUsers.Count > 0)
            {
                wasFoundInUserRegisteredList = RegisteredUsers.Exists(x =>
                {
                    if (update.CallbackQuery != null)
                    {
                        if (x.ID.Equals(update.CallbackQuery.Message.Chat.Id.ToString()))
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        if (x.ID == update.Message.Chat.Id)
                        {
                            return true;
                        }
                       else
                        {
                            return false;
                        }
                    }
                });
            }
            else
            {
                RegisteredUsers = new List<ChatTelegramBotService.User>();
            }

            //first message
            if (update.Message != null)
            {
                //it's not in the chat to be ignored
                var chat = ChatIdToBeIgnored.FirstOrDefault(x => x == update.Message.Chat.Id.ToString());
                if (chat == null && !wasFoundInUserRegisteredList)
                {
                    //checking the message if it's an email
                    var matchEmail = ExtractEmails(update.Message.Text);
                    //checking the message if it's a phone number
                    var matchPhoneNumber = ExtractPhoneNumber(update.Message.Text);

                    //first message and the user dit not answer directly with email
                    if ((update.Message.Text.Equals("/começar") || update.Message.Text.Equals("/suporte")
                        || update.Message.Text.Equals("/comprarVIP"))
                        && matchEmail.Length == 0)
                    {
                        /**********************************************
                        *                                             *
                        *                   COMEÇAR                   *
                        *                                             *
                        **********************************************/
                        if (update.Message.Text.Equals("/começar"))
                        {
                            var chatId = update.Message.Chat.Id;
                            var u = RegisteredUsers.FirstOrDefault(x => x.ID == chatId);
                            if (u == null)
                            {
                                var user = new ChatTelegramBotService.User();
                                user.Data = DateTime.Now;
                                user.ID = chatId;
                                user.Nome = new StringBuilder(update.Message.From.FirstName)
                                                            .Append(" ")
                                                            .Append(update.Message.From.LastName).ToString();

                                _context.User.Add(user);
                                _context.SaveChanges();
                            }
                            else
                            {
                                u.Nome = new StringBuilder(update.Message.From.FirstName)
                                                            .Append(" ")
                                                            .Append(update.Message.From.LastName).ToString();
                                _context.User.Update(u);
                                _context.SaveChanges();
                            }

                            //_context.User.AddRange(u);

                            //memCacher.Add("registeredUsers", RegisteredUsers, DateTimeOffset.UtcNow.AddHours(1));

                            await client.SendTextMessageAsync(update.Message.Chat.Id, "Você selecionou: '/começar'." +
                                "\n" +
                                "\n" +
                                "Preciso que me informe o seu telefone (somente números e com DDD) para que eu possa consultar o nosso sistema de pagamentos. 💰\n" +
                                "Digite seu telefone abaixo por favor. Estou aguardando......⏳"
                                , replyMarkup: null);
                        }
                        /**********************************************
                        *                                             *
                        *                   SUPORTE                   *
                        *                                             *
                        **********************************************/
                        else if (update.Message.Text.Equals("/suporte"))
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

                            await client.SendTextMessageAsync(update.Message.Chat.Id, "Você selecionou: '/suporte'." +
                                "\n" +
                                "\n" +
                                "Selecione uma das opções abaixo: \n" +
                                "Estou aguardando......⏳"
                                , replyMarkup: myInlineSupportKeyboard);
                        }
                        //TODO: ELSE IF for COMPRARVIP
                    }
                    /**********************************************
                    *                                             *
                    *                   PHONE NUMBER              *
                    *                                             *
                    **********************************************/
                    //in case when a user type his/her phone number
                    else if (matchPhoneNumber.Length > 0 && wasFoundInUserRegisteredList)
                    {
                        var chatId = update.Message.Chat.Id.ToString();
                        var u = RegisteredUsers.FirstOrDefault(x => x.ID.Equals(chatId));
                        if (u != null)
                        {
                            u.Telefone = update.Message.Text;

                            _context.User.Update(u);
                            _context.SaveChanges();
                        }

                        await client.SendTextMessageAsync(update.Message.Chat.Id, "Perfeito. Poderia me informar o email associado " +
                                "a este pagamento?\n" +
                                "Estou aguardando.....⏳", replyMarkup: rkm);

                    }
                    /**********************************************
                    *                                             *
                    *          EMAIL (LAST STEP)                  *
                    *                                             *
                    **********************************************/
                    //in case when a user type his/her email
                    else if (matchEmail.Length > 0 && wasFoundInUserRegisteredList)
                    {
                        //get credentials
                        var cred = hotmartService.GetCredentials(false);
                        var auth = hotmartService.Authentication(cred);
                        var signatures = hotmartService.GetSignaturesByEmail(cred, auth, matchEmail[0]);

                        //if the user was found by email
                        if (signatures.items.Count > 0)
                        {
                            var chatId = update.Message.Chat.Id;
                            var u = RegisteredUsers.FirstOrDefault(x => x.ID == chatId);
                            if (u != null)
                            {
                                u.Email = update.Message.Text;
                            }

                            if (!CheckUserRegistered(u))
                            {

                                //memCacher.Delete("registeredUsers");
                                //RegisteredUsers.Remove(u);
                                //memCacher.Add("registeredUsers", RegisteredUsers, DateTimeOffset.UtcNow.AddHours(1));
                                _context.User.Update(u);
                                _context.SaveChanges();

                                await client.SendTextMessageAsync(update.Message.Chat.Id, "OBAAAA 😄😄😎😎.\n" +
                                       "Percebemos que é você mesmo que realizou o pagamento.🤑 \n" +
                                       "Aqui está o link para fazer parte do nosso VIP: https://t.me/joinchat/T9Lo55NGZZ4k3exC"
                                       , replyMarkup: rkm);
                            }
                            else
                            {
                                await client.SendTextMessageAsync(update.Message.Chat.Id, "Este dados já foram registrados antes para " +
                                    "outro usuário.\n" +
                                      "Favor verificar isto e tentar novamente."
                                      , replyMarkup: rkm);
                            }
                        }
                        else
                        {
                            await client.SendTextMessageAsync(update.Message.Chat.Id, "IIXIII, Não consegui encontrar " +
                                "nenhum pagamento associado a este email ✉️.\n" +
                                "Tem certeza que foi este email? Poderia tentar novamente, por favor?.\n" +
                                "Estou aguardando.....⏳", replyMarkup: rkm);
                        }
                    }
                    /**********************************************
                    *                                             *
                    *               INICIAR O BOT                 *
                    *                                             *
                    **********************************************/
                    else
                    {
                        await client.SendTextMessageAsync(update.Message.Chat.Id, "Olá, somos da equipe ANGEL SIGNALS 🇧🇷🇨🇮. " +
                        "Para iniciar, selecione: uma das opções abaixo. Assim conseguiremos " +
                        "garantir o suporte a você!", replyMarkup: myInlineKeyboard);
                    }
                }
                /**********************************************
                *                                             *
                *      REPLY TYPING FROM KEYBOARD             *
                *                                             *
                **********************************************/
                else
                {
                    //checking the message if it's an email
                    var matchEmail = ExtractEmails(update.Message.Text);
                    //checking the message if it's a phone number
                    var matchPhoneNumber = ExtractPhoneNumber(update.Message.Text);

                    /**********************************************
                    *                                             *
                    *                   PHONE NUMBER              *
                    *                                             *
                    **********************************************/
                        //in case when a user type his/her phone number
                    if (matchPhoneNumber.Length > 0 && wasFoundInUserRegisteredList)
                    {
                        var chatId = update.Message.Chat.Id;
                        var u = RegisteredUsers.FirstOrDefault(x => x.ID == chatId);
                        if (u != null)
                        {
                            u.Telefone = update.Message.Text;
                            _context.User.Update(u);
                            _context.SaveChanges();
                        }

                        await client.SendTextMessageAsync(update.Message.Chat.Id, "Perfeito. Poderia me informar o email associado " +
                                "a este pagamento?\n" +
                                "Estou aguardando.....⏳", replyMarkup: rkm);

                    }
                    /**********************************************
                    *                                             *
                    *          EMAIL (LAST STEP)                  *
                    *                                             *
                    **********************************************/
                    //in case when a user type his/her email
                    else if (matchEmail.Length > 0 && wasFoundInUserRegisteredList)
                    {
                        //get credentials
                        var cred = hotmartService.GetCredentials(false);
                        var auth = hotmartService.Authentication(cred);
                        var signatures = hotmartService.GetSignaturesByEmail(cred, auth, matchEmail[0]);

                        //if the user was found by email
                        if (signatures.items.Count > 0)
                        {
                            var chatId = update.Message.Chat.Id;
                            var u = RegisteredUsers.FirstOrDefault(x => x.ID.Equals(chatId));
                            if (u != null)
                            {
                                u.Email = update.Message.Text;
                            }

                            if (!CheckUserRegistered(u))
                            {
                                _context.User.Update(u);
                                _context.SaveChanges();
                                //memCacher.Delete("registeredUsers");
                                //RegisteredUsers.Remove(u);
                                //memCacher.Add("registeredUsers", RegisteredUsers, DateTimeOffset.UtcNow.AddHours(1));

                                await client.SendTextMessageAsync(update.Message.Chat.Id, "OBAAAA 😄😄😎😎.\n" +
                                       "Percebemos que é você mesmo que realizou o pagamento.🤑 \n" +
                                       "Aqui está o link para fazer parte do nosso VIP: https://t.me/joinchat/T9Lo55NGZZ4k3exC"
                                       , replyMarkup: rkm);
                            }
                            else
                            {
                                await client.SendTextMessageAsync(update.Message.Chat.Id, "Este dados já foram registrados antes para " +
                                    "outro usuário.\n" +
                                      "Favor verificar isto e tentar novamente."
                                      , replyMarkup: rkm);
                            }
                        }
                        else
                        {
                            await client.SendTextMessageAsync(update.Message.Chat.Id, "IIXIII, Não consegui encontrar " +
                                "nenhum pagamento associado a este email ✉️.\n" +
                                "Tem certeza que foi este email? Poderia tentar novamente, por favor?.\n" +
                                "Estou aguardando.....⏳", replyMarkup: rkm);
                        }
                    }
                    /**********************************************
                    *                                             *
                    *               INICIAR O BOT                 *
                    *                                             *
                    **********************************************/
                    else
                    {
                        await client.SendTextMessageAsync(update.Message.Chat.Id, "Olá, somos da equipe ANGEL SIGNALS 🇧🇷🇨🇮. " +
                        "Para iniciar, selecione: uma das opções abaixo. Assim conseguiremos " +
                        "garantir o suporte a você!", replyMarkup: myInlineKeyboard);
                    }
                }
            }
            /**********************************************
            *                                             *
            *      REPLY FROM AUTOMATED KEYBOARD          *
            *                                             *
            **********************************************/
            else
            {
                if (update.CallbackQuery.Data != null)
                {
                    if (update.CallbackQuery.Data.Equals("/começar") || update.CallbackQuery.Data.Equals("/suporte")
                        || update.CallbackQuery.Data.Equals("/comprarVIP"))
                    {
                        /**********************************************
                        *                                             *
                        *                   COMEÇAR                   *
                        *                                             *
                        **********************************************/
                        if (update.CallbackQuery.Data.Equals("/começar"))
                        {
                            var chatId = update.CallbackQuery.Message.Chat.Id;
                            var u = RegisteredUsers.FirstOrDefault(x => x.ID == chatId);
                            if (u == null)
                            {
                                var user = new ChatTelegramBotService.User();
                                user.Data = DateTime.Now;
                                user.ID = chatId;
                                user.Nome = new StringBuilder(update.CallbackQuery.Message.From.FirstName)
                                                            .Append(" ")
                                                            .Append(update.CallbackQuery.Message.From.LastName).ToString();

                                _context.User.Add(user);
                                _context.SaveChanges();
                            }
                            else
                            {
                                u.Nome = new StringBuilder(update.CallbackQuery.Message.From.FirstName)
                                                            .Append(" ")
                                                            .Append(update.CallbackQuery.Message.From.LastName).ToString();

                                _context.User.Update(u);
                                _context.SaveChanges();
                            }

                            //memCacher.Add("registeredUsers", RegisteredUsers, DateTimeOffset.UtcNow.AddHours(1));

                            //_context.User.AddRange(RegisteredUsers);

                            await client.SendTextMessageAsync(update.CallbackQuery.Message.Chat.Id, "Você selecionou: '/começar'." +
                                "\n" +
                                "\n" +
                                "Preciso que me informe o seu telefone (somente números e com DDD) para que eu possa consultar o nosso sistema de pagamentos. 💰\n" +
                                "Digite seu telefone abaixo por favor. Estou aguardando......⏳"
                                , replyMarkup: null);
                        }
                        /**********************************************
                        *                                             *
                        *                   SUPORTE                   *
                        *                                             *
                        **********************************************/
                        else if (update.CallbackQuery.Data.Equals("/suporte"))
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
                        //TODO: ELSE IF for COMPRARVIP
                    }
                    else if (update.CallbackQuery.Data.Equals("/adm") || update.CallbackQuery.Data.Equals("/reclamar") ||
                        update.CallbackQuery.Data.Equals("/statuspag"))
                    {
                        switch (update.CallbackQuery.Data)
                        {
                            /**********************************************
                            *                                             *
                            *               FALAR COM O ADM               *
                            *                                             *
                            **********************************************/
                            case "/adm":
                                var message = string.Format("⚠️⚠️🚨🚨 Existe uma pessoa querendo falar com os admins. \n" +
                                    "Nome: {0} \n" +
                                    "Username: @{1}\n" +
                                    "Username: {1}\n" +
                                    "ID: {2} 🚨🚨⚠️⚠️", update.CallbackQuery.From.FirstName + " " + update.CallbackQuery.From.LastName,
                                    update.CallbackQuery.From.Username, update.CallbackQuery.From.Id);
                                var result = await SendMessage(client, AdminsList, message);
                                if (result)
                                {
                                    await SendMessage(client, update.CallbackQuery.Message.Chat.Id, "Mensagem enviada com sucesso para o admin. Em breve entraremos em contato! Peço que aguarde, obrigado!");
                                }
                                break;
                            /**********************************************
                            *                                             *
                            *                   RECLAMAR                  *
                            *                                             *
                            **********************************************/
                            case "/reclamar": //TODO: disponibilizar email para a pessoa reclamar
                                break;
                            /**********************************************
                            *                                             *
                            *               STATUS DO PAG                 *
                            *                                             *
                            **********************************************/
                            case "/statuspag": //TODO: ir na hotmart e verificar o status do pagamento
                            default:
                                break;
                        }
                    }
                    /**********************************************
                    *                                             *
                    *               INICIAR O BOT                 *
                    *                                             *
                    **********************************************/
                    else
                    {
                        await client.SendTextMessageAsync(update.CallbackQuery.Message.Chat.Id, "Olá, somos da equipe ANGEL SIGNALS 🇧🇷🇨🇮. " +
                        "Para iniciar, selecione das opções abaixo. Assim conseguiremos " +
                        "garantir o suporte a você!", replyMarkup: myInlineKeyboard);
                    }
                }
            }


            return Ok();
        }

        [HttpPost("botsendmessage")]
        public async Task<IActionResult> BotSendMessage([FromBody] string destID)
        {
            try
            {
                var message = "🇧🇷 Olá, somos da ANGEL SIGNALS. O método Europeu agora no Brasil 🇧🇷\n" +
                              "Estamos mandando esta mensagem com o seu presente.\n" +
                              "Acesse o link abaixo e assista o nosso mini curso.\n" +
                              "🔥 Também não deixe de nos seguir no instagram e acompanhe as operações nos Stories.\n" +
                              "➡ http://bit.ly/Instagram-Angel" +
                              "🚨 MINI-CURSO 100% GRÁTIS \n" +
                              "➡ http://bit.ly/mini-curso-traderX";
                var bot = new Telegram.Bot.TelegramBotClient(bootToken);
                await bot.SendTextMessageAsync(destID, message);
                return new StatusCodeResult(StatusCodes.Status200OK);
            }
            catch (Exception e)
            {
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPost("chatbot")]
        public async Task<IActionResult> ChatBot([FromBody] Update update)
        {
            try
            {
                var ChatIdToBeIgnoredChatBot = new List<string>();

                //Chat Id's to be ignored by the BOT
                ChatIdToBeIgnoredChatBot.AddRange(new List<string> { "-1001233703026", "-1001150279812", "-1001150279812" });
                TelegramBotClient client = new TelegramBotClient("1417186445:AAGFG-jByzgAEhaZRAKLnnJOigAXbzM8dhU");

                InlineKeyboardMarkup myInlineKeyboard = new InlineKeyboardMarkup(
                    new InlineKeyboardButton[][]
                    {
                    new InlineKeyboardButton[] // First row
                    {
                        InlineKeyboardButton.WithCallbackData(
                            "Sou Iniciante 🙂", // Button Name
                            "iniciante" // Answer you'll recieve
                        )
                    },
                    new InlineKeyboardButton[] // second row
                    {
                        InlineKeyboardButton.WithCallbackData(
                            "Suporte 🤓", // Button Name
                            "suporte" // Answer you'll recieve
                        )
                    },
                    new InlineKeyboardButton[] // third row
                    {
                        InlineKeyboardButton.WithCallbackData(
                            "Planos e Formas de Pagamentos 🤑", // Button Name
                            "planos" // Answer you'll recieve
                        )
                    },
                    new InlineKeyboardButton[] // third row
                    {
                        InlineKeyboardButton.WithCallbackData(
                            "Site e Instagram ✍🏼", // Button Name
                            "site" // Answer you'll recieve
                        )
                    },
                    new InlineKeyboardButton[] // third row
                    {
                        InlineKeyboardButton.WithCallbackData(
                            "Confirmar Pagamento 💵", // Button Name
                            "pag" // Answer you'll recieve
                        )
                    }
                }
                );

                InlineKeyboardMarkup secondKeyboard = new InlineKeyboardMarkup(
                       new InlineKeyboardButton[][]
                       {
                            new InlineKeyboardButton[] // First row
                            {
                                InlineKeyboardButton.WithCallbackData(
                                    "Volta Menu Inicial ⬅️", // Button Name
                                    "voltar" // Answer you'll recieve
                                )
                            }
                       }
                    );


                //first message
                if (update.Message != null)
                {
                    var chat = ChatIdToBeIgnoredChatBot.FirstOrDefault(x => x == update.Message.Chat.Id.ToString());
                    if (chat == null)
                    {
                        switch (update.Message.Text)
                        {
                            case "/start":
                                await client.SendTextMessageAsync(update.Message.Chat.Id,
                                    "Olá, somos da equipe ANGEL SIGNALS 🇧🇷🇨🇮. \n" +
                                    "Para iniciar, selecione: uma das opções abaixo. Assim conseguiremos \n" +
                                    "garantir o suporte a você! 👇🏼👇🏼",
                                    replyMarkup: myInlineKeyboard);
                                break;
                            default: await client.SendTextMessageAsync(update.Message.Chat.Id,
                                    "Olá, somos da equipe ANGEL SIGNALS 🇧🇷🇨🇮. \n" +
                                    "Para iniciar, selecione: uma das opções abaixo. Assim conseguiremos \n" +
                                    "garantir o suporte a você! 👇🏼👇🏼",
                                    replyMarkup: myInlineKeyboard);
                                break;
                        }
                    }
                }
                else
                {
                    //reply from inline keyboard
                    if (update.CallbackQuery.Data != null)
                    {
                        var chat = ChatIdToBeIgnoredChatBot.FirstOrDefault(x => x == update.CallbackQuery.Message.Chat.Id.ToString());
                        if (chat == null)
                        {
                            switch (update.CallbackQuery.Data)
                            {
                                case "iniciante":
                                    await client.SendTextMessageAsync(update.CallbackQuery.Message.Chat.Id,
                                        GetInicianteMessage(),
                                        replyMarkup: secondKeyboard);
                                    break;
                                case "suporte":
                                    await client.SendTextMessageAsync(update.CallbackQuery.Message.Chat.Id,
                                        GetSuporteMessage(),
                                        replyMarkup: secondKeyboard);
                                    break;
                                case "planos":
                                    await client.SendTextMessageAsync(update.CallbackQuery.Message.Chat.Id,
                                        GetPlanosMessage(),
                                        replyMarkup: secondKeyboard);
                                    break;
                                case "site":
                                    await client.SendTextMessageAsync(update.CallbackQuery.Message.Chat.Id,
                                        GetSiteMessage(),
                                        replyMarkup: secondKeyboard);
                                    break;
                                case "pag":
                                    await client.SendTextMessageAsync(update.CallbackQuery.Message.Chat.Id,
                                        GetPagMessage(),
                                        replyMarkup: secondKeyboard);
                                    break;
                                case "voltar":
                                    await client.SendTextMessageAsync(update.CallbackQuery.Message.Chat.Id,
                                        "Olá, somos da equipe ANGEL SIGNALS 🇧🇷🇨🇮. \n" +
                                        "Para iniciar, selecione: uma das opções abaixo. Assim conseguiremos \n" +
                                        "garantir o suporte a você! 👇🏼👇🏼",
                                        replyMarkup: myInlineKeyboard);
                                    break;
                                default: await client.SendTextMessageAsync(update.CallbackQuery.Message.Chat.Id,
                                    "Olá, somos da equipe ANGEL SIGNALS 🇧🇷🇨🇮. \n" +
                                    "Para iniciar, selecione: uma das opções abaixo. Assim conseguiremos \n" +
                                    "garantir o suporte a você! 👇🏼👇🏼",
                                    replyMarkup: myInlineKeyboard);
                                    break;
                            }
                        }
                    }
                }
                

                return Ok();
            }
            catch (Exception ex)
            {
                return Ok();
            }
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

        private string[] ExtractPhoneNumber(string str)
        {
            string RegexPattern = @"\(?\b([0-9]{2,3}|0((x|[0-9]){2,3}[0-9]{2}))\)?\s*[0-9]{4,5}[- ]*[0-9]{4}\b";

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

        private async Task<bool> SendMessage(TelegramBotClient client, List<string> ListChatId, string text, IReplyMarkup keyboard = null)
        {
            var returnedValue = false;

            foreach (var admin in ListChatId)
            {
                await client.SendTextMessageAsync(admin, text, replyMarkup: keyboard);

                returnedValue = true;
            }

            return returnedValue;
        }

        private bool CheckUserRegistered(ChatTelegramBotService.User user)
        {
            bool returnedValue = false;

            var foundUser = _context.User.FirstOrDefault(x => x.Email.ToUpper().Equals(user.Email.ToUpper())
            && x.Telefone.Equals(user.Telefone));

            if (foundUser != null)
            {
                returnedValue = true;
            }

            return returnedValue;
        }

        private string GetInicianteMessage()
        {
            var returnedMessage = new StringBuilder();

            returnedMessage.Append("Olá , somos da equipe ANGEL SIGNALS 🇧🇷🇨🇮. \n");
            returnedMessage.Append("Nós operamos no mercado de operações binárias utilizando método Europeu utilizando a IQ Option. \n");
            returnedMessage.Append("Mas você pode escolher outras corretoras de seu gosto como, por exmeplo, a Binomo. \n");
            returnedMessage.Append("O que você precisa para começar a operar? Veja nossas dicas abaixo: \n\n");
            returnedMessage.Append("1 - Abrir uma conta na corretora. Segue o link para você começar agora mesmo https://bit.ly/CadastroIqOptionn \n");
            returnedMessage.Append("2 - Fazer um depósito mínimo e começar a operar. \n\n");
            returnedMessage.Append("Quer começar na frente de muitos utilizando o nosso método Europeu de graça? \n");
            returnedMessage.Append("Se sim, acesso o link abaixo e faça o seu cadastro para receber o nosso mini curso gratuito. \n");
            returnedMessage.Append("👉🏼 https://bit.ly/CadastroIqOptionn");

            return returnedMessage.ToString();
        }

        private string GetSuporteMessage()
        {
            var returnedMessage = new StringBuilder();

            returnedMessage.Append("Está com problemas/dúvidas e não sabe o que fazer? \n\n");
            returnedMessage.Append("Mande mensagem para o @joao_brito5");

            return returnedMessage.ToString();
        }

        private string GetPlanosMessage()
        {
            var returnedMessage = new StringBuilder();

            returnedMessage.Append("Planos e Formas de Pagamentos 💵💵\n\n");
            returnedMessage.Append("Formas de Pagamentos: \n");
            returnedMessage.Append("1 - Boleto \n");
            returnedMessage.Append("2 - Pix \n"); 
            returnedMessage.Append("3 - Cartão de Crédito \n");
            returnedMessage.Append("4 - Cartão de Débito \n\n");
            returnedMessage.Append("Planos: \n");
            returnedMessage.Append("1 - Sala de sinais 24 horas do nosso robô Mister X: R$80,00/mês\n");
            returnedMessage.Append("2 - Sala de sinais VIP (apenas 15 vagas): R$120,00/mês \n");
            returnedMessage.Append("3 - Sala de sinais VIP + Curso do básico ao avançado (apenas 10 vagas): R$297,00 com 1 mês de sala " +
                "VIP. Após isto, R$120,00/mês. 👈 Este é o mais recomendado para você aprender a operar e não somente ficar copiando" +
                " e colando sinais!!! \n\n\n");
            returnedMessage.Append("👉🏼 Quer conhecer mais sobre o nosso trabalho e não sabe como? Acess o nosso site: https://angelsignals.live/acesso \n");



            return returnedMessage.ToString();
        }

        private string GetSiteMessage()
        {
            var returnedMessage = new StringBuilder();

            returnedMessage.Append("Nosso site e Instagram 🤳\n\n");
            returnedMessage.Append("1 - Site: https://angelsignals.live/acesso \n");
            returnedMessage.Append("2 - Instagram:http://bit.ly/Instagram-Angel \n");
            returnedMessage.Append("3 - Facebook: https://www.facebook.com/angelsignalsbr \n");
            returnedMessage.Append("4 - Nossa sala gratuita: https://t.me/joinchat/NwYx80mI0HLcq3SX37zfjw ");

            return returnedMessage.ToString();
        }

        private string GetPagMessage()
        {
            var returnedMessage = new StringBuilder();

            returnedMessage.Append("Atenção⚠️🚨\n\n");
            returnedMessage.Append("👉🏼 Mande a foto de seu comprovante para: @joao_brito5\n");


            return returnedMessage.ToString();
        }
    }
}
