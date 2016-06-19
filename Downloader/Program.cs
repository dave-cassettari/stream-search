using HtmlAgilityPack;
using Newtonsoft.Json;
using StreamSearch.Models.Contexts;
using StreamSearch.Models.Entities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace StreamSearch
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var timer = Stopwatch.StartNew();
            var letters = new string[] { "0-9", "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z" };

            Parallel.ForEach(letters, letter =>
            {
                var index = 1;
                var valid = true;

                using (var client = new WebClient())
                {
                    while (valid)
                    {
                        Debug.WriteLine("Downloading {0}, page {1:00}, after {2}", letter, index, timer.Elapsed);

                        var url = string.Format("http://putlocker.is/a-z/{1}/{0}.html", letter, index);

                        try
                        {
                            valid = false;

                            var page = client.DownloadString(url);
                            var document = new HtmlDocument();

                            document.LoadHtml(page);

                            var heading = document.DocumentNode.SelectSingleNode("//h2[contains(text(), 'Movies Beginning With')]");
                            var table = heading.ParentNode.ChildNodes.FirstOrDefault(x => x.Name == "table");
                            var cells = table.Descendants("td");

                            using (var context = new DatabaseContext())
                            {
                                foreach (var cell in cells)
                                {
                                    var link = cell.Descendants("a").FirstOrDefault();

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

                                    valid = valid || FindVideos(context, href, title);
                                }

                                context.SaveChanges();
                            }

                            ++index;
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine(ex.Message);

                            valid = false;
                        }
                    }
                }
            });
        }

        private static bool FindVideos(DatabaseContext context, string url, string title)
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
                                var episode = season.Episodes.FirstOrDefault(x => x.Page == episodeUrl);

                                if (episode == null)
                                {
                                    episode = new Episode()
                                    {
                                        Page = episodeUrl,
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
                Debug.WriteLine(ex.Message);

                return false;
            }
        }

        private static string FindSource(string url)
        {
            var document = new HtmlDocument();

            using (var client = new WebClient())
            {
                var page = client.DownloadString(url);

                document.LoadHtml(page);
            }

            return FindSource(document.DocumentNode);
        }

        private static string FindSource(HtmlNode documentNode)
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

        private static void AddMeta(DatabaseContext context, Video video)
        {
            using (var client = new WebClient())
            {
                try
                {
                    var url = string.Format("http://www.omdbapi.com/?t={0}", video.Title);
                    var json = client.DownloadString(url);
                    var data = JsonConvert.DeserializeObject<MetaResponse>(json);

                    if (data == null || data.Response != "True")
                    {
                        return;
                    }

                    video.Cover = data.Cover;
                    video.Synopsis = data.Synposis;

                    AddMetaEntities(context, video, data.Genre, x => x.Genres);
                    AddMetaEntities(context, video, data.Actors, x => x.Actors);
                    AddMetaEntities(context, video, data.Director, x => x.Directors);

                    var movie = video as Movie;

                    if (movie != null)
                    {
                        movie.Rating = data.Rating;

                        int year;

                        if (int.TryParse(GetNumeric(data.Years), out year))
                        {
                            movie.Year = year;
                        }
                    }

                    var show = video as Show;

                    if (show != null)
                    {
                        var years = GetNumeric(data.Years).Split('-');

                        if (years.Length > 0)
                        {
                            int year;

                            if (int.TryParse(years[0], out year))
                            {
                                show.YearStart = year;
                            }
                        }

                        if (years.Length > 1)
                        {
                            int year;

                            if (int.TryParse(years[1], out year))
                            {
                                show.YearFinish = year;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
            }
        }

        private static void AddMetaEntities<T>(DatabaseContext context, Video video, string text, Func<Video, ICollection<T>> getter)
            where T : class, IMeta, new()
        {
            var values = text.Split(',');

            foreach (var value in values)
            {
                var name = value.Trim();

                if (name == "N/A")
                {
                    continue;
                }

                var set = context.Set<T>();
                var entity = set.FirstOrDefault(x => x.Name == name);
                var collection = getter(video);

                if (entity == null)
                {
                    entity = new T()
                    {
                        Name = name,
                    };

                    set.Add(entity);
                }

                collection.Add(entity);
            }
        }

        private static string ParseSource(HtmlNode node)
        {
            var decoder = new Decoder();
            var element = new HtmlDocument();
            var script = node.SelectSingleNode("script");

            if (script == null)
            {
                return null;
            }

            var html = decoder.Decode(script.InnerText);

            element.LoadHtml(html);

            var frame = element.DocumentNode.ChildNodes.FirstOrDefault();

            if (frame == null)
            {
                return null;
            }

            return frame.Attributes["src"].Value;
        }

        private static T AddOrUpdate<T>(DatabaseContext context, string url)
            where T : Video, new()
        {
            var entity = context.Set<T>().FirstOrDefault(x => x.Page == url);

            if (entity == null)
            {
                entity = new T()
                {
                    Page = url,
                };

                context.Set<T>().Add(entity);
            }

            return entity;
        }

        private static string GetNumeric(string input)
        {
            return new string(input.Where(c => char.IsDigit(c) || c == '-').ToArray());
        }

        private class MetaResponse
        {
            [JsonProperty("Year")]
            public string Years { get; set; }
            [JsonProperty("Title")]
            public string Title { get; set; }
            [JsonProperty("Genre")]
            public string Genre { get; set; }
            [JsonProperty("Poster")]
            public string Cover { get; set; }
            [JsonProperty("imdbRating")]
            public string Rating { get; set; }
            [JsonProperty("Actors")]
            public string Actors { get; set; }
            [JsonProperty("Director")]
            public string Director { get; set; }
            [JsonProperty("Plot")]
            public string Synposis { get; set; }
            [JsonProperty("Response")]
            public string Response { get; set; }
        }
    }
}
