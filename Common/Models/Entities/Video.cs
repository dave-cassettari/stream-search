using System.Collections.Generic;

namespace StreamSearch.Common.Models.Entities
{
    public abstract class Video : Entity
    {
        public string Title { get; set; }
        public string Cover { get; set; }
        public string Synopsis { get; set; }

        public virtual ICollection<Actor> Actors { get; set; }
        public virtual ICollection<Genre> Genres { get; set; }
        public virtual ICollection<Source> Sources { get; set; }
        public virtual ICollection<Director> Directors { get; set; }

        public Video()
        {
            Actors = new List<Actor>();
            Genres = new List<Genre>();
            Sources = new List<Source>();
            Directors = new List<Director>();
        }

        public override string ToString()
        {
            return Title;
        }
    }
}
