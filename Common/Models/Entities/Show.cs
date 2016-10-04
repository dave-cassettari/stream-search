using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StreamSearch.Common.Models.Entities
{
    public class Show : Video
    {
        public int? YearStart { get; set; }
        public int? YearFinish { get; set; }

        public virtual ICollection<Season> Seasons { get; set; }

        public Show()
        {
            Seasons = new List<Season>();
        }
    }
}
