using Newtonsoft.Json;
using StreamSearch.Common.Models.Entities;

namespace StreamSearch.Web.Controllers.Api
{
    public class SeasonApiModel : AbstractApiModel<Season>
    {
        [JsonProperty("order")]
        public int Order { get; private set; }
        [JsonProperty("episodes")]
        public EpisodeApiModel[] Episodes { get; private set; }

        public SeasonApiModel(Season entity)
            : base(entity)
        {
            Order = entity.Order;
            Episodes = GetViewModels(entity.Episodes, x => new EpisodeApiModel(x));
        }
    }
}