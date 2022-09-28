using System.ComponentModel.DataAnnotations;

namespace XWingTO.Web.ViewModels.Tournament
{
	public class EditTournamentViewModel
	{
		public Guid Id { get; set; }

		[Required]
		public string Name { get; set; }

		[Required]
		[DataType(DataType.Date)]
		public DateOnly Date { get; set; }

		public string Country { get; set; }
		public string State { get; set; }
		public string City { get; set; }
		public string Venue { get; set; }
	}
}
