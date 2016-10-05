using Newtonsoft.Json;
using StreamSearch.Common.Models.Entities;

namespace StreamSearch.Interface.Controllers.Api
{
    public class VideoApiModel : AbstractApiModel<Video>
    {
        [JsonProperty("title")]
        public string Title { get; private set; }
        [JsonProperty("cover")]
        public string Cover { get; private set; }
        [JsonProperty("seasons")]
        public SeasonApiModel[] Seasons { get; private set; }

        public VideoApiModel(Video entity)
            : base(entity)
        {
            Title = entity.Title;
            Cover = entity.Cover;

            if (string.IsNullOrWhiteSpace(Cover) || Cover == "N/A")
            {
                Cover = null;
            }

            var show = entity as Show;

            if (show != null)
            {
                Seasons = GetViewModels(show.Seasons, x => new SeasonApiModel(x));
            }
        }
    }
}