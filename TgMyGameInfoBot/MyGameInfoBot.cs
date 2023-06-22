using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Extensions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Polling;
using Telegram.Bot.Exceptions;
using Newtonsoft.Json;
using Microsoft.VisualBasic;

namespace TgMyGameInfoBot
{
    public class MyGameInfoBot
    {
        TelegramBotClient botClient = new TelegramBotClient("6224877364:AAG1VrEWoq9lI-7FVFXY278_9_lAFm3yXU0");
        CancellationToken cancellationToken = new CancellationToken();
        ReceiverOptions receiverOptions = new ReceiverOptions { AllowedUpdates={ } };    
        public async Task Start()
        {
            botClient.StartReceiving(HandlerUpdateAsync, HandlerErrorAsync, receiverOptions, cancellationToken);
            var botMe = await botClient.GetMeAsync();
            Console.WriteLine($"Бот {botMe.Username} розпочав роботу");
            Console.ReadKey();
        }
        private async Task HandlerErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            var ErrorMessage = exception switch
            {
                ApiRequestException apiRequestException => $"Помилка в телеграм бот АПІ:\n {apiRequestException.ErrorCode}" 
                + $"\n{apiRequestException.Message}",
                _ => exception.ToString()
            };
            Console.WriteLine(ErrorMessage);
            if (!cancellationToken.IsCancellationRequested)
            {
                try
                {

                    await Task.Delay(1000, cancellationToken);

                    botClient.StartReceiving(HandlerUpdateAsync, HandlerErrorAsync, receiverOptions, cancellationToken);
                }
                catch (Exception restartException)
                {
                    Console.WriteLine($"An error occurred while restarting the bot: {restartException.Message}");
                }
            }
            return;
        }
        private async Task HandlerUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            if(update.Type == UpdateType.Message && update?.Message?.Text != null)
            {
               await HandlerMessageAsync(botClient,  update.Message);
            }
        }
        private async Task HandlerMessageAsync(ITelegramBotClient botClient, Message message)
        {
            if (message.Text == "/start")
            {
                Method.ShowKeyBoard(botClient, message);
                return;
            }

            if (message.Text == "Instruction")
            {
                await botClient.SendTextMessageAsync(message.Chat.Id, "If you need to enter the name of the game, enter it with a small letter, if there are several words, write them with (-)");
                return;
            }
            if (message.Text.Contains("/gamedes "))
            {

                var Game = message.Text.Replace("/gamedes ", "").ToLower();
                var responce = await new HttpClient().GetAsync($"https://mygameinfoapi.azurewebsites.net/GameInfo?GameName={Game}");
                var content = responce.Content.ReadAsStringAsync().Result;             
                await botClient.SendTextMessageAsync(message.Chat.Id, content);
                return;
            }
            if (message.Text.Contains("/redditposts "))
            {
                var Game = message.Text.Replace("/redditposts ", "").ToLower();
                var responce = await new HttpClient().GetAsync($"https://mygameinfoapi.azurewebsites.net/GameInfo/{Game}/reddit");
                var content = responce.Content.ReadAsStringAsync().Result;                               
                await botClient.SendTextMessageAsync(message.Chat.Id, content);
                return;                           
            }
            if (message.Text.Contains("/storelinks "))
            {
                var Game = message.Text.Replace("/storelinks ", "").ToLower();
                var responce = await new HttpClient().GetAsync($"https://mygameinfoapi.azurewebsites.net/GameInfo/{Game}/stores");
                var content = responce.Content.ReadAsStringAsync().Result;
                await botClient.SendTextMessageAsync(message.Chat.Id, content);
                return;
            }
            if (message.Text.Contains("/gif "))
            {
                var Game = message.Text.Replace("/gif ", "").ToLower();
                var responce = await new HttpClient().GetAsync($"https://mygameinfoapi.azurewebsites.net/GameInfo/gifs/search?GameName={Game}");
                var content = responce.Content.ReadAsStringAsync().Result;
                List<string> urlList = new List<string>(content.Split(" "));
                foreach (var url in urlList)
                {
                    try
                    {
                        
                        await botClient.SendDocumentAsync(message.Chat.Id, new InputFileUrl(url), caption: "");
                    }
                    catch (Exception e)
                    {
                        await botClient.SendTextMessageAsync(message.Chat.Id, "Not Found");
                    }
                }              
                return;
            }
            if(message.Text.Contains("/savegamelinks "))
            {
                var Game = message.Text.Replace("/savegamelinks ", "").ToLower();
                HttpResponseMessage response = await new HttpClient().PostAsync($"https://mygameinfoapi.azurewebsites.net/GameInfo?message={message.Chat.Id}&GameName={Game}", new StringContent(System.Text.Json.JsonSerializer.Serialize(new { }), Encoding.Unicode, "application/json"));
           
                if (response.IsSuccessStatusCode)
                {
                    await botClient.SendTextMessageAsync(message.Chat.Id, "Game has been added");
                }
                else
                {
                    await botClient.SendTextMessageAsync(message.Chat.Id, "Something went wrong:(");

                }
                
                return;
            }
            if (message.Text.Contains("/delete ()"))
            {
                HttpResponseMessage response = await new HttpClient().DeleteAsync($"https://mygameinfoapi.azurewebsites.net/GameInfo?ID={message.Chat.Id}");
                if (response.IsSuccessStatusCode)
                {
                    await botClient.SendTextMessageAsync(message.Chat.Id, "The list has been cleared");
                }
                else
                {
                    await botClient.SendTextMessageAsync(message.Chat.Id, "Something went wrong:(");

                }

                return;
            }
            if (message.Text.Contains("/checklist"))
            {
                var responce = await new HttpClient().GetAsync($"https://mygameinfoapi.azurewebsites.net/DataBase?ID={message.Chat.Id}");
                var content = responce.Content.ReadAsStringAsync().Result;
                var Data = JsonConvert.DeserializeObject<DataModel>(content);
                if (Data?.DataList != null)
                {
                    foreach (var result in Data.DataList)
                    {
                        await botClient.SendTextMessageAsync(message.Chat.Id, result.GameName + "\n" + result.StoreLink);
                    }
                }
                else
                {
                    await botClient.SendTextMessageAsync(message.Chat.Id, "Empty");
                }

            }


        }



    }
}
