using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace XWingTO.Web.ViewModels.Tournament
{
	public class SearchViewModel
	{
		[ValidateNever]
		public string Name { get; set; }

		public List<Core.Tournament> Tournaments { get; set; }

		public DateOnly StartDate { get; set; }
		public DateOnly EndDate { get; set; }
	}
}
