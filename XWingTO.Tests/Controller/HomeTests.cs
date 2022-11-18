using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using Microsoft.Azure.Documents;
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
		ApplicationUser _user;
		IUserStore<ApplicationUser> _userStore;

		[SetUp]
		public void Setup()
		{

		}

		[Test]
		public async Task Unauthenticated_Index_Shows_Anonymous_View()
		{
			_user = new ApplicationUser() { Id = Guid.NewGuid(), UserName = "test user"};
			_userStore = Substitute.For<IUserStore<ApplicationUser>>();
			_userStore.FindByIdAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(_user);
			_userStore.FindByNameAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(_user);
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

			HomeController controller = new HomeController(new UserManager<ApplicationUser>(_userStore, null, null, null, null, null,null,null, null), 
				tournamentRepository, Substitute.For<IRepository<TournamentPlayer, Guid>>());

			controller.ControllerContext = new ControllerContext(new ActionContext(context, new RouteData(), new ControllerActionDescriptor(), new ModelStateDictionary()));

			var result = await controller.Index();

			var viewResult = (ViewResult) result;

			Assert.That(viewResult.ViewName, Is.EqualTo("Index"));
		}

		[Test]
		public async Task Authenticated_Index_Shows_MyHome_View()
		{
			_user = new ApplicationUser() { Id = Guid.NewGuid() };
			_userStore = Substitute.For<IUserStore<ApplicationUser>>();
			_userStore.FindByIdAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(_user);
			_userStore.FindByNameAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(_user);
			HttpContext context = Substitute.For<HttpContext>();
			List<Claim> claims = new List<Claim>
			{
				new Claim(new IdentityOptions().ClaimsIdentity.UserIdClaimType, _user.Id.ToString(), "TestValueType", "TestIssuer")
			};
			ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, "TestAuthType");
			
			ClaimsPrincipal principal = new ClaimsPrincipal(claimsIdentity);

			var serviceProvider = Substitute.For<IServiceProvider>();
			serviceProvider
				.GetService(Arg.Is(typeof(ITempDataDictionaryFactory)))
				.Returns(Substitute.For<ITempDataDictionaryFactory>());
			serviceProvider
				.GetService(Arg.Is(typeof(IUrlHelperFactory)))
				.Returns(Substitute.For<IUrlHelperFactory>());
			context.RequestServices.Returns(serviceProvider);

			context.User.Returns(principal);

			IRepository<Tournament, Guid> tournamentRepository = Substitute.For<IRepository<Tournament, Guid>>();

			HomeController controller = new HomeController(new UserManager<ApplicationUser>(_userStore, null, null, null, null, null, null, null, null),
				tournamentRepository, Substitute.For<IRepository<TournamentPlayer, Guid>>());

			controller.ControllerContext = new ControllerContext(new ActionContext(context, new RouteData(), new ControllerActionDescriptor(), new ModelStateDictionary()));

			var result = await controller.Index();

			var viewResult = (ViewResult)result;

			Assert.That(viewResult.ViewName, Is.EqualTo("MyHome"));
		}
	}
}
