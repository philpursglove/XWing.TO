using System.ComponentModel.DataAnnotations;

namespace XWingTO.Web.ViewModels.Tournament
{
	public class ResultSubmissionViewModel : IValidatableObject
	{
		public string Player1Name { get; set; }
		public string Player2Name { get; set; }
		public int Player1MissionPoints { get; set; }
		public int Player2MissionPoints { get; set; }
		public int Turns { get; set; }
		public bool OutOfTime { get; set; }
		public Guid GameId { get; set; }
		public Guid TournamentId { get; set; }
		public bool Player1Concede { get; set; }
		public bool Player2Concede { get; set; }

		public bool Player1Drop { get; set; }
		public bool Player2Drop { get; set; }
		public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
		{
			List<ValidationResult> results =
				new List<ValidationResult>();

			if (Turns < 1)
			{
				results.Add(new ValidationResult("Must have at least one turn", new string[] {"Turns"}));
			}

			return results;
		}
	}
}
