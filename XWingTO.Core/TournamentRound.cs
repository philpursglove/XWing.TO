using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace XWingTO.Core
{
	public class TournamentRound
	{
		public Guid Id { get; set; }

		public Guid TournamentId { get; set; }

		[ValidateNever]
		public Tournament Tournament { get; set; }

		public int RoundNumber { get; set; }

		[ValidateNever]
		public virtual IEnumerable<Game> Games { get; set; }
	}
}
