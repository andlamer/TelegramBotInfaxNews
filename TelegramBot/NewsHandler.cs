﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Threading.Tasks;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace TelegramBot
{
    public class NewsHandler
    {
        private const int SecondsToUpdate = 60;

        public event Action<string> LatestNewsWhereUpdated;

        private DateTime _lastNewsTime;
        private string _lastId = System.IO.File.ReadAllText("lastId.txt");

        public NewsHandler()
        {
            var jsonString = System.IO.File.ReadAllText("lastTime.json");
            _lastNewsTime = JsonSerializer.Deserialize<DateTime>(jsonString);
            TrackLatestNews();
        }

        private async void TrackLatestNews()
        {
            while (true)
            {
                await Task.Delay(TimeSpan.FromSeconds(SecondsToUpdate));
                var newsList = _rssReader.ReadRss().OrderBy(item => item.PublishDate.DateTime);
                var recent = newsList.Last();
                if (_lastNewsTime == recent.PublishDate.DateTime && _lastId.Equals(recent.Id)) continue;
                
                foreach (var item in newsList.Where(item =>
                             (item.PublishDate.DateTime - _lastNewsTime).TotalSeconds > 0
                             || (item.PublishDate.DateTime - _lastNewsTime).TotalSeconds == 0 &&
                             !item.Id.Equals(_lastId)))
                {
                    Console.WriteLine(item.PublishDate.DateTime);
                    LatestNewsWhereUpdated?.Invoke(ConvertNewsToString(item));
                    _lastId = item.Id;
                    System.IO.File.WriteAllText("lastId.txt", _lastId);
                }

                _lastNewsTime = recent.PublishDate.DateTime;
                var jsonString = JsonSerializer.Serialize(_lastNewsTime);
                System.IO.File.WriteAllText("lastTime.json", jsonString);
            }
        }

        private readonly RssReader _rssReader = new RssReader();
        private static readonly Random Random = new Random();

        public string PickRandom()
        {
            var newsList = _rssReader.ReadRss();
            return ConvertNewsToString(newsList.ElementAt(Random.Next(newsList.Count)));
        }

        public string LoadMostRecent()
        {
            var newsList = _rssReader.ReadRss();
            return ConvertNewsToString(newsList.OrderBy(news => news.PublishDate.DateTime).Last());
        }

        public string GetCategories()
        {
            var newsList = _rssReader.ReadRss();
            var i = 1;
            var categoriesString = "";
            foreach (var category in newsList.SelectMany(item => item.Categories))
            {
                if (!categoriesString.Contains(category.Name))
                {
                    categoriesString += i + ". " + category.Name + '\n';
                    i++;
                }
            }

            return categoriesString;
        }

        public List<string> LoadTenLatest()
        {
            var newsList = _rssReader.ReadRss();
            var newsStringList = new List<string>();
            newsList.OrderBy(news => news.PublishDate.DateTime).Reverse().Take(10).ToList()
                .ForEach(item => newsStringList.Add(ConvertNewsToString(item)));
            return newsStringList;
        }

        private string ConvertNewsToString(SyndicationItem syndicationItem)
        {
            var newsString = syndicationItem.Title.Text + "\n";
            if (syndicationItem.Authors != null && syndicationItem.Authors.Count > 0)
            {
                newsString += "\nАвтор(и) статті: ";
                foreach (var author in syndicationItem.Authors)
                {
                    newsString += author.Email + " ";
                }
            }

            newsString += "\n" + syndicationItem.PublishDate + "\n\n" + syndicationItem.Summary.Text;

            if (syndicationItem.Links != null && syndicationItem.Links.Count > 0)
            {
                newsString += "\n\nПосилання: \n";
                foreach (var link in syndicationItem.Links)
                {
                    if (!link.Uri.ToString().Contains(".jpg") && !link.Uri.ToString().Contains(".jpeg") &&
                        !link.Uri.ToString().Contains(".png") && !link.Uri.ToString().Contains(".webp"))
                        newsString += link.Uri + "\n";
                }
            }

            newsString = newsString.Replace("<p>", "").Replace("</p>", "");
            return newsString;
        }
    }
}