using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using NSubstitute;
using System.Security.Claims;
using System.Security.Principal;
using XWingTO.Core;
using XWingTO.Data;
using XWingTO.Web.Controllers;

namespace XWingTO.Tests.Controller
{
	[TestFixture]
	public class HomeTests
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
			var serviceProvider = Substitute.For<IServiceProvider>();
			serviceProvider
				.GetService(Arg.Is(typeof(ITempDataDictionaryFactory)))
				.Returns(Substitute.For<ITempDataDictionaryFactory>());
			serviceProvider
				.GetService(Arg.Is(typeof(IUrlHelperFactory)))
				.Returns(Substitute.For<IUrlHelperFactory>());
			context.RequestServices.Returns(serviceProvider);

			IRepository<Tournament, Guid> tournamentRepository = Substitute.For<IRepository<Tournament, Guid>>();
			
			tournamentRepository.Query().Returns(new FakeQuery<Tournament>(new List<Tournament>{new Tournament{Name = "test tournament"}}.AsQueryable()));

			HomeController controller = new HomeController(new UserManager<ApplicationUser>(Substitute.For<IUserStore<ApplicationUser>>(), null, null, null, null, null,null,null, null), 
				tournamentRepository, Substitute.For<IRepository<TournamentPlayer, Guid>>());

			controller.ControllerContext = new ControllerContext(new ActionContext(context, new RouteData(), new ControllerActionDescriptor(), new ModelStateDictionary()));

			var result = await controller.Index();

			var viewResult = (ViewResult) result;

			Assert.That(viewResult.ViewName, Is.EqualTo("Index"));
		}
	}
}
