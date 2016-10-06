using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace StreamSearch.Common.Models.Entities
{
    public class Episode : Video
    {
        public int Order { get; set; }

        [Required]
        public virtual Season Season { get; set; }

        public Show Show
        {
            get { return Season.Show; }
        }

        public override string Cover
        {
            get { return Show.Cover; }
        }

        public override ICollection<Actor> Actors
        {
            get { return Show.Actors; }
        }

        public override ICollection<Genre> Genres
        {
            get { return Show.Genres; }
        }

        public override ICollection<Director> Directors
        {
            get { return Show.Directors; }
        }

        public override string ToString()
        {
            return string.Format("Episode {0}: {1}", Order, Title);
        }
    }
}
