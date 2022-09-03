using System;
using AngleSharp;
using AngleSharp.Html.Parser;
using AngleSharp.Html.Dom;
using System.Text;
using System.Linq;
using System.Threading;


class Program
{
    public static IConfiguration config = Configuration.Default.WithDefaultLoader();
    public static IBrowsingContext context = BrowsingContext.New(config);

    static async Task Main(string[] args)

    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        var testAddress = "https://www.nccal.gov.kw/%D8%B9%D8%A7%D9%84%D9%85-%D8%A7%D9%84%D9%85%D8%B9%D8%B1%D9%81%D8%A9?pagesize=30";
        var testdocument = await context.OpenAsync(testAddress);
        var lastPageSelector = "ul > li.last-page > a";
        string lastPageAnchor = testdocument.QuerySelector(lastPageSelector).ToHtml();
        int index = lastPageAnchor.IndexOf("pagenumber") + 11;
        int lastPageNumber = int.Parse(lastPageAnchor.Substring(index, 2));

        for (int i = 1; i <= lastPageNumber; i++)
        {
            Thread.Sleep(1000); 
            var address = $"https://www.nccal.gov.kw/%D8%B9%D8%A7%D9%84%D9%85-%D8%A7%D9%84%D9%85%D8%B9%D8%B1%D9%81%D8%A9?pagesize=30&pagenumber={i}";
            var document = await context.OpenAsync(address);
            var nameSelector = ":nth-child(n) > div > div.details > h2 > a";
            var numberSelector = ":nth-child(n) > div > div.details > h4 > a";


            var nameCells = document.QuerySelectorAll(nameSelector);
            var numberCells = document.QuerySelectorAll(numberSelector);

            var titles = nameCells.Select(name => name.TextContent);
            var numbers = numberCells.Select(number => number.TextContent);

            var books = numbers.Zip(titles, (n, t) => t + ":" + n);

            using (StreamWriter writer = new StreamWriter("books.txt", true))
            {
                foreach (var item in books)
                {
                    writer.WriteLine(item);
                }
            }

        }
    }
}