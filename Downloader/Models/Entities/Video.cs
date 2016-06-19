﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StreamSearch.Models.Entities
{
    public abstract class Video : Entity
    {
        //[Index]
        public string Page { get; set; }
        //[Index]
        public string Link { get; set; }
        //[Index]
        public string Title { get; set; }
        public string Cover { get; set; }
        public string Synopsis { get; set; }

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
