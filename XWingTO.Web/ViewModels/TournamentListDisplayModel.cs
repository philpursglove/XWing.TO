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

		public bool UserIsTO { get; set; }

		public string FormatName { get; set; }

		public TournamentListDisplayModel(Guid id, string name, Date date, IEnumerable<TournamentPlayer> players, string toName, string location, bool userIsTo, string formatName)
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

			TOName = toName;

			Location = location;

			UserIsTO = userIsTo;

			FormatName = formatName;
		}
	}
}
