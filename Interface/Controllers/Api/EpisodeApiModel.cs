using Newtonsoft.Json;
using StreamSearch.Common.Models.Entities;

namespace StreamSearch.Web.Controllers.Api
{
    public class EpisodeApiModel : AbstractApiModel<Episode>
    {
        [JsonProperty("order")]
        public int Order { get; private set; }
        [JsonProperty("title")]
        public string Title { get; private set; }

        public EpisodeApiModel(Episode entity)
            : base(entity)
        {
            Order = entity.Order;
            Title = entity.Title;
        }
    }
}