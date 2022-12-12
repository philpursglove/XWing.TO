namespace XWingTO.Web.ViewModels.Tournament
{
	public class TournamentAdminModel
	{
		public string Name { get; set; }

		public Date Date { get; set; }
		public string TOName { get; set; }
		public int PlayerCount { get; set; }
		public string Location { get; set; }

		public List<TournamentPlayerDisplayModel> Players { get; set; }

		public List<TournamentRoundDisplayModel> Rounds { get; set; }
		public Guid Id { get; set; }
		public string? Country { get; set; }
		public string? State { get; set; }
		public string? City { get; set; }
		public string? Venue { get; set; }
	}

	public class TournamentAdminUpdateModel
	{
		public string Name { get; set; }
		public Date Date { get; set; }
		public Guid Id { get; set; }
		public string? Country { get; set; }
		public string? State { get; set; }
		public string? City { get; set; }
		public string? Venue { get; set; }

	}
}
