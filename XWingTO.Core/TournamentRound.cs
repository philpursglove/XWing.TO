namespace XWingTO.Core
{
	public class TournamentRound
	{
		public Guid Id { get; set; }

		public Guid TournamentId { get; set; }

		public Tournament Tournament { get; set; }

		public int RoundNumber { get; set; }

		public virtual IEnumerable<Game> Games { get; set; }

		public int ScenarioId { get; set; }
	}
}
