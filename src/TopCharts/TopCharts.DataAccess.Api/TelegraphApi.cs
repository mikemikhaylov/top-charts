using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Kvyk.Telegraph;
using Kvyk.Telegraph.Models;
using TopCharts.Domain.Model;

namespace TopCharts.DataAccess.Api
{
    public class TelegraphApi
    {
        private readonly TelegraphClient _telegraphClient = new TelegraphClient();
        private static readonly string AuthorName = "vctopcharts";
        private static readonly string AuthorUrl = "https://t.me/vctopcharts";

        public TelegraphApi(PostingOptions postingOptions)
        {
            if (!string.IsNullOrWhiteSpace(postingOptions.TelegraphToken))
            {
                _telegraphClient.AccessToken = postingOptions.TelegraphToken;
            }

            if (postingOptions.Site != Site.Vc)
            {
                throw new NotImplementedException();
            }
        }

        public async Task<string> CreateAccount(CancellationToken cancellationToken)
        {
            Account account = await _telegraphClient.CreateAccount(
                AuthorName,
                AuthorName,
                AuthorUrl
            );
            return account.AccessToken;
        }

        public async Task<string> CreatePageAsync(string title, CancellationToken cancellationToken)
        {
            Console.WriteLine("Creating page: " + title);
            //return "https://telegra.ph/For-future-use-01-30-2";
            await Task.Delay(GetTimeout(60, 90), cancellationToken);
            var nodes = new List<Node>
            {
                Node.P("Hello, World!"),
            };
            var page = await _telegraphClient.CreatePage(
                title,
                nodes,
                AuthorName,
                AuthorUrl
            );
            return page.Url;
        }

        public async Task EditPageAsync(string url, List<Node> nodes, CancellationToken cancellationToken)
        {
            Console.WriteLine("Editing page: " + url);
            await Task.Delay(GetTimeout(5, 10), cancellationToken);
            var page = await _telegraphClient.GetPage(url);
            page = await _telegraphClient.EditPage(
                url,
                page.Title, 
                nodes, 
                page.AuthorName,
                page.AuthorUrl
            );
        }

        private static readonly Random Rnd = new Random();
        private TimeSpan GetTimeout(int from, int to)
        {
            var t = TimeSpan.FromSeconds(Rnd.Next(from, to));
            Console.WriteLine($"Telegraph timeout {t}");
            return t;
        }
    }
}