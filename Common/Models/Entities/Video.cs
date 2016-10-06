using System.Collections.Generic;

namespace StreamSearch.Common.Models.Entities
{
    public abstract class Video : Entity
    {
        public virtual string Page { get; set; }
        public virtual string Link { get; set; }
        public virtual string Title { get; set; }
        public virtual string Cover { get; set; }
        public virtual string Synopsis { get; set; }

        public virtual ICollection<Actor> Actors { get; set; }
        public virtual ICollection<Genre> Genres { get; set; }
        public virtual ICollection<Director> Directors { get; set; }

        public Video()
        {
            Actors = new List<Actor>();
            Genres = new List<Genre>();
            Directors = new List<Director>();
        }

        public override string ToString()
        {
            return Title;
        }
    }
}
