using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Types;
using Telegram.Bot;

namespace TgMyGameInfoBot
{
    public class Method
    {
        public static async void ShowKeyBoard(ITelegramBotClient botClient, Message message)
        {
            ReplyKeyboardMarkup replyKeyboardMarkup = new
                   (
                   new[]
                       {
                        new KeyboardButton[] { "Instruction" }
                       }
                   )
            {
                ResizeKeyboard = true
            };
            await botClient.SendTextMessageAsync(message.Chat.Id, "Click", replyMarkup: replyKeyboardMarkup);
        }
    }
}
