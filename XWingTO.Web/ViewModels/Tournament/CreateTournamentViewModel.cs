﻿using System.ComponentModel.DataAnnotations;

namespace XWingTO.Web.ViewModels.Tournament
{
    public class CreateTournamentViewModel : IValidatableObject
    {
	    public CreateTournamentViewModel()
	    {
		    Format = TournamentFormat.Standard;
	    }

        [Required]
        public string Name { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public Date Date { get; set; }

        public string? Country { get; set; }
        public string? State { get; set; }
        public string? City { get; set; }
        public string? Venue { get; set; }

        public TournamentFormat Format { get; set; }


        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            List<ValidationResult> results = new List<ValidationResult>();

	        if (Date < Date.FromDateTime(DateTime.Today))
	        {
                results.Add(new ValidationResult("Date cannot be in the past", new []{"Date"}));
	        }

	        return results;
        }
    }
}
