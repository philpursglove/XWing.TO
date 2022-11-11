using XWingTO.Core;

namespace XWingTO.Web.ViewModels.Tournament
{
	public class TournamentDisplayModel
	{
		public Core.Tournament Tournament { get; set; }

		public List<TournamentPlayerDisplayModel> Players { get; set; }

		public List<TournamentRoundDisplayModel> Rounds { get; set; }

		public TournamentDisplayModel(Core.Tournament tournament)
		{
			Players = new List<TournamentPlayerDisplayModel>();
			Rounds = new List<TournamentRoundDisplayModel>();

			foreach (TournamentRound round in tournament.Rounds)
			{
				TournamentRoundDisplayModel roundModel = new TournamentRoundDisplayModel();
				roundModel.Round = round.RoundNumber;

				foreach (Game roundGame in round.Games)
				{
					TournamentGameDisplayModel gameModel = new TournamentGameDisplayModel
					{
						Player1Score = roundGame.Player1MissionPoints,
						Player2Score = roundGame.Player2MissionPoints
					};
					roundModel.Games.Add(gameModel);
				}
				Rounds.Add(roundModel);
			}
		}

		public string TOName { get; set; }
		public string PlayerCount { get; set; }
		public bool UserIsRegistered { get; set; }
	}

	public class TournamentPlayerDisplayModel
	{
		public string Name { get; set; }
		public int Points { get; set; }

	}

	public class TournamentRoundDisplayModel
	{
		public int Round { get; set; }

		public List<TournamentGameDisplayModel> Games { get; set; }

		public TournamentRoundDisplayModel()
		{
			Games = new List<TournamentGameDisplayModel>();
		}
	}

	public class TournamentGameDisplayModel
	{
		public string Player1 { get; set; }
		public string Player2 { get; set; }
		public int Player1Score { get; set; }
		public int Player2Score { get; set; }
	}
}
