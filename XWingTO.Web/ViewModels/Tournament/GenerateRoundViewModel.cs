using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using XWingTO.Core;

namespace XWingTO.Web.ViewModels.Tournament
{
	public class GenerateRoundViewModel : IValidatableObject
	{
		private IEnumerable<TournamentPlayerViewModel> PlayerList;

		public GenerateRoundViewModel()
		{
			
		}

		public GenerateRoundViewModel(List<TournamentPlayer> Players, TournamentRound round)
		{
			TournamentPlayers = Players;

			PlayerList = TournamentPlayers.Select(p => new TournamentPlayerViewModel(p.Id, p.Player.DisplayName));

			Round = round;
		}
		public TournamentRound Round { get; set; }

		public List<TournamentPlayer> TournamentPlayers { get; set; }

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

		public SelectList MakePlayerSelectList(Guid currentSelectedPlayerId)
		{
			return new SelectList(PlayerList, "Id", "Name", currentSelectedPlayerId);
		}

		class TournamentPlayerViewModel
		{
			public Guid Id { get; set; }
			public string Name { get; set; }

			public TournamentPlayerViewModel(Guid id, string name)
			{
				Id = id;
				Name = name;
			}
		}
	}
}
