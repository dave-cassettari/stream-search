using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StreamSearch.Common.Models.Entities
{
    public interface IMeta
    {
        string Name { get; set; }

        ICollection<Video> Videos { get; set; }
    }
}
