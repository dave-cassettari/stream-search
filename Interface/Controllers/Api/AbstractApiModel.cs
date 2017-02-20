using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using StreamSearch.Common.Models.Entities;

namespace StreamSearch.Web.Controllers.Api
{
    public abstract class AbstractApiModel<T>
        where T : Entity
    {
        [JsonProperty("id")]
        public int Id { get; private set; }

        protected readonly T Entity;

        protected AbstractApiModel(T entity)
        {
            Id = entity.Id;
            Entity = entity;
        }

        protected U[] GetViewModels<T, U>(IEnumerable<T> entities, Func<T, U> constructor)
            where T : Entity
            where U : AbstractApiModel<T>
        {
            return entities.Select(x => constructor(x)).ToArray();
        }
    }
}