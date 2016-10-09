using HtmlAgilityPack;
using StreamSearch.Common.Models.Contexts;
using StreamSearch.Common.Models.Entities;
using System;
using System.Linq;
using System.Net;

namespace StreamSearch.Downloader.Spiders
{
    public class Putlocker9Spider : AbstractSpider
    {
        public override bool ParsePage(string letter, int index)
        {
            using (var client = new WebClient())
            {
                var url = string.Format("http://putlocker9.com/azlist/page/{1}?latter={0}", letter, index);
                var page = client.DownloadString(url);
                var document = new HtmlDocument();

                document.LoadHtml(page);

                var items = document.DocumentNode.SelectNodes("//div[contains(@class, 'aaa_item')]");

                if (items == null || items.Count == 0)
                {
                    return false;
                }

                using (var context = new DatabaseContext())
                {
                    foreach (var item in items)
                    {
                        var link = item.Descendants("a").FirstOrDefault();

                        if (link == null)
                        {
                            continue;
                        }

                        var image = link.Descendants("img").FirstOrDefault();

                        if (image == null)
                        {
                            continue;
                        }

                        var href = link.Attributes["href"].Value;
                        var title = image.Attributes["alt"].Value;

                        FindVideos(context, href, title);
                    }

                    context.SaveChanges();
                }

                return true;
            }
        }

        private bool FindVideos(DatabaseContext context, string url, string title)
        {
            try
            {
                var document = new HtmlDocument();

                using (var client = new WebClient())
                {
                    var page = client.DownloadString(url);

                    document.LoadHtml(page);
                }

                var source = FindSource(document.DocumentNode);

                if (source != null)
                {
                    var movie = AddOrUpdate<Movie>(context, url);

                    movie.Link = source;
                    movie.Title = title;

                    AddMeta(context, movie);

                    return true;
                }

                var headings = document.DocumentNode.SelectNodes("//h2");

                if (headings != null)
                {
                    var seasons = headings.Where(x => x.InnerText != null && x.InnerText.Contains("Season"));

                    if (seasons.Any())
                    {
                        var order = 1;
                        var show = AddOrUpdate<Show>(context, url);

                        show.Title = title;

                        AddMeta(context, show);

                        foreach (var heading in seasons)
                        {
                            var index = 1;
                            var table = heading.NextSibling;
                            var cells = table.Descendants("td");
                            var season = show.Seasons.FirstOrDefault(x => x.Order == order);

                            if (season == null)
                            {
                                season = new Season()
                                {
                                    Show = show,
                                    Order = order,
                                };

                                show.Seasons.Add(season);
                            }

                            foreach (var cell in cells)
                            {
                                var link = cell.Descendants("a").FirstOrDefault();

                                if (link == null)
                                {
                                    continue;
                                }

                                var episodeUrl = link.Attributes["href"].Value;
                                var episodeVideo = FindSource(episodeUrl);

                                if (episodeVideo == null)
                                {
                                    continue;
                                }

                                var name = link.NextSibling;
                                var text = Uri.UnescapeDataString(name.InnerText);
                                var clean = text.Replace("&nbsp;-", "").Replace("&nbsp;", "").Trim();
                                var episode = season.Episodes.FirstOrDefault(x => x.Title == clean);

                                if (episode == null)
                                {
                                    episode = new Episode()
                                    {
                                        Title = clean,
                                        Season = season,
                                    };

                                    season.Episodes.Add(episode);
                                }

                                episode.Link = episodeVideo;
                                episode.Order = index;
                                episode.Title = clean;

                                ++index;
                            }

                            ++order;
                        }

                        return true;
                    }
                }

                return false;
            }
            catch (Exception ex)
            {
                Logger.Log(ex.Message);

                return false;
            }
        }

        private string FindSource(string url)
        {
            var document = new HtmlDocument();

            using (var client = new WebClient())
            {
                var page = client.DownloadString(url);

                document.LoadHtml(page);
            }

            return FindSource(document.DocumentNode);
        }

        private string FindSource(HtmlNode documentNode)
        {
            var videos = documentNode.SelectNodes("//div[@class='video']");

            if (videos != null)
            {
                foreach (var video in videos)
                {
                    var source = ParseSource(video);

                    if (source != null)
                    {
                        return source;
                    }
                }
            }

            return null;
        }
    }
}
