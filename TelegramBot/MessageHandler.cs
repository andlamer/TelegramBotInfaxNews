using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using File = System.IO.File;

namespace TelegramBot
{
    public class MessageHandler
    {
        private ITelegramBotClient _client;
        private NewsHandler _newsHandler;
        private List<long> _cachedChatIds = new List<long>();

        public MessageHandler(ITelegramBotClient client)
        {
            _client = client;
            _newsHandler = new NewsHandler();
            _newsHandler.LatestNewsWhereUpdated += SendLatestNews;
            var lines = File.ReadAllLines("cached_ids.txt");
            lines.ToList().ForEach(x => _cachedChatIds.Add(long.Parse(x)));
            ;
        }

        public async Task HandleMessage(Message message)
        {
            switch (message.Text)
            {
                case "/start":
                    await HandleStart(message);
                    break;
                case "/help":
                    await HandleHelp(message);
                    break;
                case "/commands":
                    await HandleCommands(message);
                    break;
                case "/random":
                    await HandleRandom(message);
                    break;
                case "/ten_latest":
                    await HandleTenLatest(message);
                    break;
                case "/most_recent":
                    await HandleMostRecent(message);
                    break;
                case "/get_categories":
                    await HandleCategories(message);
                    break;
                case "/subscribe":
                    await Subscribe(message);
                    break;
                case "/unsubscribe":
                    await Unsubscribe(message);
                    break;
            }

            Console.WriteLine(message.Text);
        }


        #region Basic

        private async Task HandleStart(Message message)
        {
            await SendMessage(message, Constants.GreetingsMessage);
            await SendMessage(message, Constants.SeeHelp);
        }

        private async Task HandleCommands(Message message)
        {
            await SendMessage(message, Constants.Commands);
        }

        private async Task HandleHelp(Message message)
        {
            await SendMessage(message, Constants.Help);
        }

        private async Task Subscribe(Message message)
        {
            if (!_cachedChatIds.Contains(message.Chat.Id))
            {
                _cachedChatIds.Add(message.Chat.Id);
                await File.WriteAllLinesAsync("cached_ids.txt", _cachedChatIds.ToList().Select(x => x.ToString()));
                await SendMessage(message, Constants.Subscribed);
            }
        }

        private async Task Unsubscribe(Message message)
        {
            if (_cachedChatIds.Contains(message.Chat.Id))
            {
                _cachedChatIds.Remove(message.Chat.Id);
                await File.WriteAllLinesAsync("cached_ids.txt", _cachedChatIds.ToList().Select(x => x.ToString()));
                await SendMessage(message, Constants.Unsubscribed);
            }
        }

        public async Task HandleError(Exception exception)
        {
        }

        #endregion

        #region Advanced

        private async Task HandleRandom(Message message)
        {
            var content = _newsHandler.PickRandom();
            if (content == null) return;
            await SendMessage(message, content);
        }

        private async Task HandleTenLatest(Message message)
        {
            var newsList = _newsHandler.LoadTenLatest();
            foreach (var news in newsList)
            {
                await SendMessage(message, news);
            }
        }

        private async Task HandleMostRecent(Message message)
        {
            await SendMessage(message, _newsHandler.LoadMostRecent());
        }

        private async Task HandleCategories(Message message)
        {
            await SendMessage(message, _newsHandler.GetCategories());
        }

        private async void SendLatestNews(string news)
        {
            foreach (var chatId in _cachedChatIds)
            {
                await SendMessage(chatId, news);
            }
        }

        #endregion

        #region Utility

        private async Task SendMessage(Message message, string content) =>
            await _client.SendTextMessageAsync(message.Chat, content);

        private async Task SendMessage(long id, string content) =>
            await _client.SendTextMessageAsync(id, content);

        #endregion
    }
}