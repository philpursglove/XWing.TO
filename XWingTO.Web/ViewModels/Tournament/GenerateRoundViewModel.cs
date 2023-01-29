using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using XWingTO.Core;

namespace XWingTO.Web.ViewModels.Tournament
{
	public class GenerateRoundViewModel : IValidatableObject
	{
		public List<SelectListItem> PlayerSelectList { get; set; }

		public GenerateRoundViewModel()
		{
			
		}

		public GenerateRoundViewModel(List<TournamentPlayer> Players, TournamentRound round)
		{
			PlayerSelectList = Players.Select(i => new SelectListItem(i.Player.DisplayName, i.Id.ToString())).ToList();

			Round = round;

			Games = round.Games.ToList();
		}
		public TournamentRound Round { get; set; }

		public List<Game> Games { get; set; }

		public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
		{
			List<ValidationResult> results = new List<ValidationResult>();
			// CHeck that no player appears twice

			IEnumerable<TournamentPlayer> player1Set = Round.Games.Select(g => g.Player1);
			IEnumerable<TournamentPlayer> player2Set = Round.Games.Select(g => g.Player2);

			IEnumerable<TournamentPlayer> distinctPlayer1 = player1Set.Distinct();
			if (distinctPlayer1.Count() != Round.Games.Count())
			{
				results.Add(new ValidationResult("Player appears in Player 1 multiple times"));
			}

			IEnumerable<TournamentPlayer> distinctPlayer2 = player2Set.Distinct();
			if (distinctPlayer2.Count() != Round.Games.Count())
			{
				results.Add(new ValidationResult("Player appears in Player 2 multiple times"));
			}

			IEnumerable<TournamentPlayer> playersInBothSets = player1Set.Intersect(player2Set);
			if (playersInBothSets.Any())
			{
				results.Add(new ValidationResult("Player appears in Player 1 and Player 2"));
			}

			return results;
		}
	}
}
