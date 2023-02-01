using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace XWingTO.Core;

public class Game
{
	public Guid Id { get; set; }

	public Guid TournamentRoundId { get; set; }

	[ValidateNever]
	public TournamentRound Round { get; set; }

	public Guid TournamentPlayer1Id { get; set; }

	public Guid TournamentPlayer2Id { get; set; }

	[ValidateNever]
	public TournamentPlayer Player1 { get; set; }

	[ValidateNever]
	public TournamentPlayer Player2 { get; set; }

	public int Player1MissionPoints { get; set; }

	public int Player2MissionPoints { get; set; }

	public int Turns { get; set; }

	public bool OutOfTime { get; set; }

	public int TableNumber { get; set; }
}