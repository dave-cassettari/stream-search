using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StreamSearch.Common.Models.Entities
{
    public class Season : Entity
    {
        [Required]
        public int Order { get; set; }

        [Required]
        public virtual Show Show { get; set; }

        public virtual ICollection<Episode> Episodes { get; set; }

        public Season()
        {
            Episodes = new List<Episode>();
        }

        public override string ToString()
        {
            return string.Format("Season {0}", Order);
        }
    }
}
