using XWingTO.Core;

namespace XWingTO.Web.ViewModels.Tournament
{
	public class TournamentDisplayModel
	{
		public Core.Tournament Tournament { get; set; }

		public List<TournamentPlayerDisplayModel> Players { get; set; }

		public List<TournamentRoundDisplayModel> Rounds { get; set; }

		public string FormatName { get; set; }

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
						Player2Score = roundGame.Player2MissionPoints,
						GameId = roundGame.Id
					};
					roundModel.Games.Add(gameModel);
				}
				Rounds.Add(roundModel);
			}

			PlayerCount = Players.Any() ? Players.Count.ToString() : "0";
		}

		public string TOName { get; set; }
		public string PlayerCount { get; set; }
		public bool UserIsRegistered { get; set; }
	}
}
