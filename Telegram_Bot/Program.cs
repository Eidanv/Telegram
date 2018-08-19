using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot; //библиотека телеграм бота
using Telegram.Bot.Args;
using Telegram.Bot.Types.Enums; // библиотека для определения типа сообщения в чате (текст, картинка, видео и т.д)
using Telegram.Bot.Types.ReplyMarkups; //библиотека для создания клавиатуры и кнопок
using ApiAiSDK; // библиотека для dialogflow
using ApiAiSDK.Model; // библиотека для dialogflow



namespace Telegram_Bot_10
{
    class Program
    {

        static TelegramBotClient bot;
        static ApiAi apiAi;


        static void Main(string[] args)
        {

            bot = new TelegramBotClient(" telegram API Key"); //подключается к телеграму через api
            AIConfiguration config = new AIConfiguration("Dialog flow API key", SupportedLanguage.English); // Api для dialogflow
            apiAi = new ApiAi(config);

            bot.OnMessage += MessageReceiver; //вызов метода "MessageReceiver"
            bot.OnCallbackQuery += CallbackQuery; //вызов метода "CallbackQuery"


            var Me = bot.GetMeAsync().Result; //выводит имя бота
            Console.WriteLine(Me.FirstName);

            bot.StartReceiving(); //команда запуска приема сообщений от телеграма
            Console.Read();
            bot.StopReceiving();

        }

        private static async void CallbackQuery(object sender, Telegram.Bot.Args.CallbackQueryEventArgs e)
        {
            string buttonText = e.CallbackQuery.Data;
            string name = $"{e.CallbackQuery.From.FirstName} {e.CallbackQuery.From.LastName}";
            Console.WriteLine($"{name} Push on the button {buttonText}");

            await bot.AnswerCallbackQueryAsync(e.CallbackQuery.Id, $"you push the button {buttonText}");

        }

        private static async void MessageReceiver(object sender, Telegram.Bot.Args.MessageEventArgs e) // метод приема сообщений от телеграма
        {
            var message = e.Message;
            if (message == null || message.Type != MessageType.Text)
                return;



            string Name = $"{message.From.FirstName} {message.From.LastName}"; // выводит имя пользователя который написал боту.
            Console.WriteLine($"{Name} Sent a message: '{message.Text}'");

            switch (message.Text) //здесь начинается блок кода с выводом команд, кнопок и клавиатуры.
            {
                case "/start":
                    string text =
 @"список команд:
/start - запуск бота
/inline - вывод меню
/keyboard - вывод клавиатуры";
                    await bot.SendTextMessageAsync(message.From.Id, text);

                    break;
                case "/inline":
                    var inlinekeyboard = new InlineKeyboardMarkup(new[]
                    {
                        new[]
                        {
                            InlineKeyboardButton.WithUrl("VK", "https://vk.com/dima_dmitriev666"),
                            InlineKeyboardButton.WithUrl("youtube", "https://www.youtube.com/watch?v=3ah6FQP5ozk")
                        },
                        new[]
                        {
                            InlineKeyboardButton.WithCallbackData("Пункт 1"),
                            InlineKeyboardButton.WithCallbackData("Пункт 2"),
                        }
                    });

                    await bot.SendTextMessageAsync(message.From.Id, "выберите пункт меню", replyMarkup: inlinekeyboard);
                    break;
                case "/keyboard":
                    var replyKeyboard = new ReplyKeyboardMarkup(new[]
                    {
                        new[]
                        {
                            new KeyboardButton("Hello"),
                            new KeyboardButton("how are you")
                        },
                        new[]
                        {
                            new KeyboardButton("contacts"),
                            new KeyboardButton("GeoLocation")
                        }
                    });
                    await bot.SendTextMessageAsync(message.Chat.Id, "message",
                        replyMarkup: replyKeyboard);
                    break;
                default:


                    var response = apiAi.TextRequest(message.Text); // блок кода записывает текст пользователя и отправляет в dialogflow, принимает результат и отвечает пользователю
                    string answer = response.Result.Fulfillment.Speech;
                    if (answer == "")
                        answer = "I don't understand you";
                    await bot.SendTextMessageAsync(message.From.Id, answer);
                    break;
            }





        }
    }
}
