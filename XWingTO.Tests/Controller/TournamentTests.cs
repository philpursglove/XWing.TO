using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
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
		[Test]
		public Task Player_Should_Not_Be_Able_To_Register_For_A_Tournament_That_Has_Started()
		{
			Tournament _tournament = new Tournament() {Id = Guid.NewGuid()};
			TournamentRound _round = new TournamentRound() {Id = Guid.NewGuid(), TournamentId = _tournament.Id};
			_tournament.Rounds = new List<TournamentRound>() {_round};
			IRepository<Tournament, Guid> _tournamentRepository = Substitute.For<IRepository<Tournament, Guid>>();
			Query<Tournament> query = Substitute.For<Query<Tournament>>();
			query.FirstOrDefault(t => t.Id == _tournament.Id).Returns(_tournament);
			_tournamentRepository.Query().Returns(query);
			IRepository<TournamentPlayer, Guid> _tournamentPlayerRepository =
				Substitute.For<IRepository<TournamentPlayer, Guid>>();
			IRepository<Game, Guid> _gameRepository = Substitute.For<IRepository<Game, Guid>>();
			IRepository<TournamentRound, Guid> _tournamentRoundRepository =
				Substitute.For<IRepository<TournamentRound, Guid>>();
			IConfiguration _configuration = Substitute.For<IConfiguration>();

			TournamentController controller = new TournamentController(_tournamentRepository,
				_tournamentPlayerRepository, new UserManager<ApplicationUser>(), _gameRepository, _configuration, _tournamentRoundRepository);


			_tournamentPlayerRepository.DidNotReceive().Add(Arg.Any<TournamentPlayer>());
		}

		[Test]
		public Task Player_Should_Not_Be_Able_To_Register_For_A_Tournament_Multiple_Times()
		{

		}

		[Test]
		public Task Unregistered_Player_Should_Not_Be_Able_To_Unregister_From_A_Tournament()
		{

		}
	}
}
