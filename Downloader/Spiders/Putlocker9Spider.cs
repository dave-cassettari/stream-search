using HtmlAgilityPack;
using StreamSearch.Common.Models.Contexts;
using StreamSearch.Common.Models.Entities;
using System;
using System.Collections.Generic;
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
                //var url = string.Format("http://putlocker9.com/azlist/page/{1}?latter={0}", letter, index);
                var url = "http://putlocker9.com/?s=friends&submit=Search+Now%21";
                var page = client.DownloadString(url);
                var document = new HtmlDocument();

                document.LoadHtml(page);

                var items = document.DocumentNode.SelectNodes("//div[contains(@class, 'aaa_item')]");

                if (items == null || items.Count == 0)
                {
                    return false;
                }

                var isValid = false;

                using (var context = new DatabaseContext())
                {
                    foreach (var item in items)
                    {
                        try
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

                            isValid = FindVideos(context, href, title) || isValid;
                        }
                        catch (Exception ex)
                        {

                        }
                    }

                    context.SaveChanges();
                }

                return isValid;
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

                var sources = FindSources(document.DocumentNode);

                if (sources.Any())
                {
                    var movie = AddOrUpdate<Movie>(context, title);

                    AddMeta(context, movie);

                    foreach (var source in sources)
                    {
                        AddOrUpdate(context, movie, source);
                    }

                    return true;
                }

                var headings = document.DocumentNode.SelectNodes("//h2");

                if (headings != null)
                {
                    var seasons = headings.Where(x => x.InnerText != null && x.InnerText.Contains("Season"));

                    if (seasons.Any())
                    {
                        var order = 1;
                        var show = AddOrUpdate<Show>(context, title);

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

                                var clean = link.GetAttributeValue("title", string.Empty).Trim();
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

                                episode.Order = index;

                                var episodeUrl = link.Attributes["href"].Value;
                                var episodeSources = FindSources(episodeUrl);

                                foreach (var episodeSource in episodeSources)
                                {
                                    AddOrUpdate(context, episode, episodeSource);
                                }

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

        private IEnumerable<string> FindSources(string url)
        {
            var document = new HtmlDocument();

            using (var client = new WebClient())
            {
                var page = client.DownloadString(url);

                document.LoadHtml(page);
            }

            return FindSources(document.DocumentNode);
        }

        private IEnumerable<string> FindSources(HtmlNode documentNode)
        {
            var videoNodes = documentNode.SelectNodes("//iframe");

            if (videoNodes != null)
            {
                foreach (var videoNode in videoNodes)
                {
                    var source = videoNode.GetAttributeValue("SRC", null);

                    if (source != null)
                    {
                        yield return source;
                    }
                }
            }
        }
    }
}
