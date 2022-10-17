using System.Security.Claims;
using System.Security.Principal;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Routing;
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
			HttpContext context = Substitute.For<HttpContext>();
			ClaimsPrincipal principal = Substitute.For<ClaimsPrincipal>();
			IIdentity identity = Substitute.For<IIdentity>();
			identity.IsAuthenticated.Returns(false);
			principal.Identity.Returns(identity);
			context.User.Returns(principal);
			HomeController controller = new HomeController(new UserManager<ApplicationUser>(Substitute.For<IUserStore<ApplicationUser>>(), null, null, null, null, null,null,null, null), 
				Substitute.For<IRepository<Tournament, Guid>>(), Substitute.For<IRepository<TournamentPlayer, Guid>>());

			controller.ControllerContext = new ControllerContext(new ActionContext(context, new RouteData(), new ControllerActionDescriptor(), new ModelStateDictionary()));

			var result = await controller.Index();

			var viewResult = (ViewResult) result;

			Assert.That(viewResult.ViewName, Is.EqualTo("Index"));
		}
	}
}
