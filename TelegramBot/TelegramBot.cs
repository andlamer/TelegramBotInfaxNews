using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;

namespace TelegramBot
{
    public class TelegramBot
    {
        private const string HttpToken = "5345618106:AAG3MlAm_f4YP8gBM30ueyxBJW7HBoiR8sU";

        private static readonly ITelegramBotClient TelegramBotClient = new TelegramBotClient(HttpToken);
        private static readonly MessageHandler MessageHandler = new MessageHandler(TelegramBotClient);

        public static async Task HandleUpdateAsync(ITelegramBotClient client, Update update,
            CancellationToken cancellationToken)
        {
            if (update.Type == Telegram.Bot.Types.Enums.UpdateType.Message)
            {
                try
                {
                    await MessageHandler.HandleMessage(update.Message);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }

        public static async Task HandleErrorAsync(ITelegramBotClient client, Exception exception,
            CancellationToken cancellationToken)
        {
            await MessageHandler.HandleError(exception);
        }

        static void Main(string[] args)
        {
            var cts = new CancellationTokenSource();
            var receiverOptions = new ReceiverOptions()
            {
                AllowedUpdates = { }
            };
            TelegramBotClient.StartReceiving(HandleUpdateAsync, HandleErrorAsync, receiverOptions, cts.Token);


            Console.ReadLine();
        }
    }
}