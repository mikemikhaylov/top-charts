using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using TopCharts.Domain.Model;

namespace TopCharts.DataAccess.Api
{
    public class TelegramPoster
    {
        private readonly PostingOptions _postingOptions;

        public TelegramPoster(PostingOptions postingOptions)
        {
            _postingOptions = postingOptions;
        }

        public async Task Post(string content, CancellationToken cancellationToken)
        {
            //content = "Hello";
            var botClient = new TelegramBotClient(_postingOptions.BotToken);
            await botClient.SendTextMessageAsync(
                chatId: _postingOptions.ChannelId,
                text: content,
                parseMode: ParseMode.MarkdownV2,
                disableWebPagePreview: true,
                cancellationToken: cancellationToken);
        }
    }
}