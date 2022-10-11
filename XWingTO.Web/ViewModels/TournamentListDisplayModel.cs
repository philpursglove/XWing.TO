using XWingTO.Core;

namespace XWingTO.Web.ViewModels
{
	public class TournamentListDisplayModel
	{
		public Guid Id { get; set; }
		public string Name { get; set; }
		public string Date { get; set; }
		public string PlayerCount { get; set; }
		public string TOName { get; set; }
		public string Location { get; set; }

		public TournamentListDisplayModel(Guid id, string name, DateOnly date, IEnumerable<TournamentPlayer> players)
		{
			Id = id;
			Name = name;
			Date = date.ToString("yyyy-MM-dd");
			if (players == null)
			{
				PlayerCount = "0";
			}
			else
			{
				PlayerCount = players.Any() ? players.Count().ToString() : "0";
			}
		}
	}
}
