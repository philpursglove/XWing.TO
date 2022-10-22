namespace XWingTO.Core;

public class Game
{
	public Guid Id { get; set; }

	public Guid TournamentRoundId { get; set; }

	public TournamentRound Round { get; set; }

	public TournamentPlayer Player1 { get; set; }

	public TournamentPlayer Player2 { get; set; }

	public int Player1MissionPoints { get; set; }

	public int Player2MissionPoints { get; set; }

	public int Turns { get; set; }

	public bool OutOfTime { get; set; }

	public int TableNumber { get; set; }
}