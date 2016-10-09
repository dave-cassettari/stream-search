using HtmlAgilityPack;
using Newtonsoft.Json;
using StreamSearch.Common.Models.Contexts;
using StreamSearch.Common.Models.Entities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;

namespace StreamSearch.Downloader.Spiders
{
    public abstract class AbstractSpider : ISpider
    {
        public abstract bool ParsePage(string letter, int index);

        protected void AddMeta(DatabaseContext context, Video video)
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
                    Logger.Log(ex.Message);
                }
            }
        }

        protected void AddMetaEntities<T>(DatabaseContext context, Video video, string text, Func<Video, ICollection<T>> getter)
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

        protected string ParseSource(HtmlNode node)
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

        protected T AddOrUpdate<T>(DatabaseContext context, string title)
            where T : Video, new()
        {
            var entity = context.Set<T>().FirstOrDefault(x => x.Title == title);

            if (entity == null)
            {
                entity = new T()
                {
                    Title = title,
                };

                context.Set<T>().Add(entity);
            }

            return entity;
        }

        protected string GetNumeric(string input)
        {
            return new string(input.Where(c => char.IsDigit(c) || c == '-').ToArray());
        }

        protected class MetaResponse
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
