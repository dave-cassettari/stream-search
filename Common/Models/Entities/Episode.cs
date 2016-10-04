using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StreamSearch.Common.Models.Entities
{
    public class Episode : Entity
    {
        public int Order { get; set; }
        public string Page { get; set; }
        public string Link { get; set; }
        public string Title { get; set; }
        public string Synopsis { get; set; }

        [Required]
        public virtual Season Season { get; set; }

        [NotMapped]
        public Show Show
        {
            get { return Season.Show; }
        }

        [NotMapped]
        public string Cover
        {
            get { return Show.Cover; }
        }

        [NotMapped]
        public ICollection<Actor> Actors
        {
            get { return Show.Actors; }
        }

        [NotMapped]
        public ICollection<Genre> Genres
        {
            get { return Show.Genres; }
        }

        [NotMapped]
        public ICollection<Director> Directors
        {
            get { return Show.Directors; }
        }

        public override string ToString()
        {
            return string.Format("Episode {0}: {1}", Order, Title);
        }
    }
}
