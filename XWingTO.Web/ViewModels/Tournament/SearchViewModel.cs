using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace XWingTO.Web.ViewModels.Tournament
{
	public class SearchViewModel
	{
		public string Name { get; set; }

		public List<TournamentListDisplayModel> Tournaments { get; set; }

		public DateOnly StartDate { get; set; }
		public DateOnly EndDate { get; set; }
	}
}
