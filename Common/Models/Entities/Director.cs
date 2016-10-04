using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StreamSearch.Common.Models.Entities
{
    public class Director : Entity, IMeta
    {
        public string Name { get; set; }

        public virtual ICollection<Video> Videos { get; set; }

        public Director()
        {
            Videos = new List<Video>();
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
