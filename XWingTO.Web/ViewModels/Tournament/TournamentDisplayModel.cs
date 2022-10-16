namespace XWingTO.Web.ViewModels.Tournament
{
	public class TournamentDisplayModel
	{
		public Core.Tournament Tournament { get; set; }

		public List<TournamentPlayerDisplayModel> Players { get; set; }

		public TournamentDisplayModel()
		{
			Players = new List<TournamentPlayerDisplayModel>();
		}

		public string TOName { get; set; }
		public string PlayerCount { get; set; }
		public bool UserIsRegistered { get; set; }
	}

	public class TournamentPlayerDisplayModel
	{
		public string Name { get; set; }
		public int Points { get; set; }

	}
}
