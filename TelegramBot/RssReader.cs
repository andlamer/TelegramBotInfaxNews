using System.Collections.Generic;
using System.ServiceModel.Syndication;
using System.Xml;
using System.Linq;

namespace TelegramBot
{
	public class RssReader
	{
		private const string UrlPath = "https://lb.ua/rss/ukr/rss.xml";
		
		public List<SyndicationItem> ReadRss()
		{
			var xmlReader = XmlReader.Create(UrlPath);
			var feed = SyndicationFeed.Load(xmlReader);
			xmlReader.Close();

			return feed.Items.ToList();
		}
	}
}
