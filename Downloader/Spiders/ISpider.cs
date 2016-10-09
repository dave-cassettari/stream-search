
namespace StreamSearch.Downloader.Spiders
{
    public interface ISpider
    {
        bool ParsePage(string letter, int index);
    }
}
