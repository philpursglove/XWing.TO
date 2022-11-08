using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using NSubstitute;
using XWingTO.Core;
using XWingTO.Data;
using XWingTO.Web.Controllers;

namespace XWingTO.Tests.Controller
{
	[TestFixture]
	public class TournamentTests
	{
		Tournament _tournament;
		TournamentRound _round;
		IRepository<Tournament, Guid> _tournamentRepository;
		IQuery<Tournament> _tournamentQuery;
		IRepository<TournamentPlayer, Guid> _tournamentPlayerRepository;
		IRepository<Game, Guid> _gameRepository;
		IRepository<TournamentRound, Guid> _tournamentRoundRepository;
		IConfiguration _configuration;
		ApplicationUser _user;
		IUserStore<ApplicationUser> _userStore;
		IQuery<TournamentRound> _tournamentRoundQuery;
		ControllerContext _controllerContext;

		[SetUp]
		public void Setup()
		{
			_tournament = new Tournament() { Id = Guid.NewGuid() };
			_round = new TournamentRound() { Id = Guid.NewGuid(), TournamentId = _tournament.Id };
			_tournament.Rounds = new List<TournamentRound>() { _round };
			_tournamentRepository = Substitute.For<IRepository<Tournament, Guid>>();
			_tournamentQuery = Substitute.For<IQuery<Tournament>>();
			_tournamentQuery.Include(t => t.Rounds).FirstOrDefault(t => t.Id == _tournament.Id).Returns(_tournament);
			_tournamentRepository.Query().Returns(_tournamentQuery);
			_tournamentPlayerRepository = Substitute.For<IRepository<TournamentPlayer, Guid>>();
			_gameRepository = Substitute.For<IRepository<Game, Guid>>();
			_tournamentRoundRepository = Substitute.For<IRepository<TournamentRound, Guid>>();
			_configuration = Substitute.For<IConfiguration>();
			_user = new ApplicationUser() {Id = Guid.NewGuid()};
			_userStore = Substitute.For<IUserStore<ApplicationUser>>();
			_userStore.FindByIdAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(_user);
			_userStore.FindByNameAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(_user);
			

			_tournamentRoundQuery = new FakeQuery<TournamentRound>((new List<TournamentRound> {_round}).AsQueryable());
			_tournamentRoundRepository.Query().Returns(_tournamentRoundQuery);

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
			_controllerContext = new ControllerContext(new ActionContext(context, new RouteData(), new ControllerActionDescriptor(), new ModelStateDictionary()));

		}

		[Test]
		public async Task Player_Should_Not_Be_Able_To_Registe_For_A_Tournament_That_Has_Started()
		{
			TournamentController controller = new TournamentController(_tournamentRepository,
				_tournamentPlayerRepository, new UserManager<ApplicationUser>(_userStore, null, null, null, null, null, null, null, null ), 
				_gameRepository, _configuration, _tournamentRoundRepository);

			controller.ControllerContext = _controllerContext;

			await controller.Register(_tournament.Id);

			_tournamentPlayerRepository.DidNotReceive().Add(Arg.Any<TournamentPlayer>());
		}

		[Test]
		public void Player_Should_Not_Be_Able_To_Register_For_A_Tournament_Multiple_Times()
		{
			Assert.That(true, Is.False);
		}

		[Test]
		public void Unregistered_Player_Should_Not_Be_Able_To_Unregister_From_A_Tournament()
		{
			Assert.That(true, Is.False);
		}
	}
}
