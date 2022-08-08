using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using WebScape04.Models;

using HtmlAgilityPack;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Net;
using System.Text;
using System.IO;

namespace WebScape04.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            string url;

            for (int pageCount = 1; pageCount < 5; pageCount++)
            {
                if (pageCount == 1)
                {
                    url = "https://www.quotetab.com/quotes/by-ashleigh-brilliant";
                }
                else
                {
                    url = "https://www.quotetab.com/quotes/by-ashleigh-brilliant/" + pageCount;
                }

                var response = CallUrl(url).Result;

                var quoteList = ParseHtml(response);

                WriteToCsv(quoteList);
            }

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        private static async Task<string> CallUrl(string fullUrl)
        {
            HttpClient client = new HttpClient();
            var response = await client.GetStringAsync(fullUrl);
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls13;
            client.DefaultRequestHeaders.Accept.Clear();
            return response;
        }

        private List<string> ParseHtml(string html)
        {
            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);

            var brilliantQuotes = htmlDoc.DocumentNode.SelectNodes("//blockquote")
            .ToList();

            List<string> quoteList = new List<string>();

            foreach (var quote in brilliantQuotes)
            {
                //if (link.FirstChild.Attributes.Count > 0) wikiLink.Add("https://en.wikipedia.org/" + link.FirstChild.Attributes[0].Value);
                //if (quote.LastChild.Attributes.Count > 0) bQuote.Add(quote.LastChild.Attributes[2].Value);
                //quote.ToString();
                quoteList.Add(quote.InnerText);
            }

            return quoteList;
        }
        private void WriteToCsv(List<string> quotes)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var quote in quotes)
            {
                sb.AppendLine(quote);
            }

            //System.IO.File.WriteAllText("quotes.txt", sb.ToString());
            System.IO.File.AppendAllText("quotes.txt", sb.ToString());
        }
    }
}
