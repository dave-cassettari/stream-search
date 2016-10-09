using StreamSearch.Downloader.Spiders;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace StreamSearch.Downloader
{
    public class Program
    {
        //private static readonly string[] Letters = new string[] { "0-9", "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z" };
        private static readonly string[] Letters = new string[] { "j" };
        private static readonly ISpider[] Spiders = new ISpider[]
        {
             new Putlocker9Spider(),   
             //new PutlockerISSpider(),   
        };

        public static void Main(string[] args)
        {
            var timer = Stopwatch.StartNew();

            Parallel.ForEach(Spiders, spider =>
            {
                Parallel.ForEach(Letters, letter =>
                {
                    var index = 1;

                    bool valid;

                    do
                    {
                        Logger.Log("Downloading {0}, page {1:00}, after {2}", letter, index, timer.Elapsed);

                        try
                        {
                            valid = spider.ParsePage(letter, index);
                        }
                        catch (Exception ex)
                        {
                            Logger.Log(ex.Message);

                            valid = false;
                        }
                    }
                    while (valid);
                });
            });
        }
    }
}
