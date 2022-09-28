using System.ComponentModel.DataAnnotations;

namespace XWingTO.Web.ViewModels.Tournament
{
    public class CreateTournamentViewModel : IValidatableObject
    {
        [Required]
        public string Name { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateOnly Date { get; set; }

        public string Country { get; set; }
        public string State { get; set; }
        public string City { get; set; }
        public string Venue { get; set; }


        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            throw new NotImplementedException();
        }
    }
}
