using System.ComponentModel.DataAnnotations;

namespace BakingIt.Models
{
    public class Measure
    {
        public int MeasureId { get; set; }

        [Required(ErrorMessage = "Please enter the Measure Name")]
        [Display(Name = "Measure Name")]
        public required string MeasureName {get; set;}

        [Required(ErrorMessage = "Please enter a common abbreviation")]
        [Display(Name = "Abbreviation")]
        public required string MeasureAbbreviation {get; set;}
    }
}