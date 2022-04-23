using System;

namespace TelegramBot
{
    public static class Constants
    {
        public const string GreetingsMessage = "Ласкаво просимо до InterfaxNews телеграм-бота";
        public const string SeeHelp = "Щоб дізнатися можливості бота введіть команду /help";

        public const string Help =
            "Даний бот призначений для зручного отримання новин з сайту lb.ua. \n " +
            "Для отримання повного списку можливостей введіть команду /commands";

        public const string Commands = "Нижче наведено список команд " +
                                       "\n /random - показує випадкову новину " +
                                       "\n /get_categories - показує категорії новин" +
                                       "\n /most_recent - показує найактуальнішу на поточний момент новину" +
                                       "\n /ten_latest - показує 10 останніх новин" +
                                       "\n /random - показує випадкову новину" +
                                       "\n /subscribe - підписка на розсилку" +
                                       "\n /unsubscribe - відписка від розсилки";

        public const string Subscribed = "Підписку підтверджено";
        public const string Unsubscribed = "Підписку скасовано";
    }
}