using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace XWingTO.Web.ViewModels.Tournament
{
	public class SearchViewModel
	{
		public string Name { get; set; }

		public List<TournamentListDisplayModel> Tournaments { get; set; }

		public Date StartDate { get; set; }
		public Date EndDate { get; set; }
	}
}
