using System;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;

namespace TelegramBot
{
    public class TelegramBot
    {
        private const string HttpToken = "5345618106:AAH7L4zGuxygRYF9xIArERfrsnYH-41FDGY";

        private static ITelegramBotClient _telegramBotClient = new TelegramBotClient(HttpToken);

        public static async Task HandleUpdateAsync(ITelegramBotClient client, Update update,
            CancellationToken cancellationToken)
        {
            if (update.Type == Telegram.Bot.Types.Enums.UpdateType.Message)
            {
                await MessageHandler.HandleMessage(update.Message?.Text);
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
            _telegramBotClient.StartReceiving(HandleUpdateAsync, HandleErrorAsync, receiverOptions, cts.Token);
            Console.ReadLine();
        }
    }
}