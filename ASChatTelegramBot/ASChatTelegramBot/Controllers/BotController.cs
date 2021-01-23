using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace ASChatTelegramBot.Controllers
{
    [ApiController]
    [Route("api/bot")]
    public class BotController : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Update update)
        {
            TelegramBotClient client = new TelegramBotClient("1417186445:AAGFG-jByzgAEhaZRAKLnnJOigAXbzM8dhU");

            if (update.Type == Telegram.Bot.Types.Enums.UpdateType.Message)
            {
                await client.SendTextMessageAsync(update.Message.From.Id, "answer");
            }

            return Ok();
        }
    }
}
