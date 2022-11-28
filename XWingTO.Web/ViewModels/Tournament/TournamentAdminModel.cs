namespace XWingTO.Web.ViewModels.Tournament
{
	public class TournamentAdminModel
	{
		public string Name { get; set; }

		public DateOnly Date { get; set; }
		public string TOName { get; set; }
		public int PlayerCount { get; set; }
		public string Location { get; set; }

		public List<TournamentPlayerDisplayModel> Players { get; set; }

		public List<TournamentRoundDisplayModel> Rounds { get; set; }
	}
}
