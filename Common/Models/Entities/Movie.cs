using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StreamSearch.Common.Models.Entities
{
    public class Movie : Video
    {
        public int? Year { get; set; }
        public string Rating { get; set; }
    }
}
