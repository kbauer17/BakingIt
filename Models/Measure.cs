using System.ComponentModel.DataAnnotations;

namespace BakingIt.Models
{
    public class Measure
    {
        public int MeasureId { get; set; }

        [Required]
        [Display(Name = "Measure Name")]
        public required string MeasureName {get; set;}

        [Required]
        [Display(Name = "Abbreviation")]
        public required string MeasureAbbreviation {get; set;}
    }
}