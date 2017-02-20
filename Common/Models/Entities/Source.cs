using System.ComponentModel.DataAnnotations;

namespace StreamSearch.Common.Models.Entities
{
    public class Source : Entity
    {
        [Required]
        public int Quality { get; set; }
        [Required]
        public string Link { get; set; }

        [Required]
        public virtual Video Video { get; set; }
    }
}
