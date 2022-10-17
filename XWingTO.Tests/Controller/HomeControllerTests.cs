using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using NSubstitute;
using XWingTO.Core;
using XWingTO.Data;
using XWingTO.Web.Controllers;

namespace XWingTO.Tests.Controller
{
	[TestFixture]
	public class HomeControllerTests
	{
		[Test]
		public async Task Unauthenticated_Index_Shows_Anonymous_View()
		{
			HomeController controller = new HomeController(null, Substitute.For<IRepository<Tournament, Guid>>(), Substitute.For<IRepository<TournamentPlayer, Guid>>());

			var result = await controller.Index();

			var viewResult = (ViewResult) result;

			Assert.That(viewResult.ViewName, Is.EqualTo("Index"));
		}
	}
}
