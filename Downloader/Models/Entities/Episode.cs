using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StreamSearch.Models.Entities
{
    public class Episode : Video
    {
        public int Order { get; set; }
        public string Page { get; set; }
        public string Link { get; set; }
        public string Title { get; set; }
        public string Synopsis { get; set; }

        [Required]
        public virtual Season Season { get; set; }

        public Show Show
        {
            get { return Season.Show; }
        }

        public ICollection<Actor> Actors
        {
            get { return Show.Actors; }
        }

        public ICollection<Genre> Genres
        {
            get { return Show.Genres; }
        }

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
