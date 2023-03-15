namespace XWingTO.Core;

public class RandomPairingStrategy : IPairingStrategy
{
    public IEnumerable<Game> Pair(List<TournamentPlayer> players)
    {
        List<Game> games = new List<Game>();

        while (players.Any())
        {
	        Game newGame = new Game
	        {
		        Id = Guid.NewGuid()
	        };

            newGame.TableNumber = games.Count + 1;

            newGame.Player1 = players.Random()!;
            newGame.TournamentPlayer1Id = newGame.Player1.Id;

            players.Remove(newGame.Player1);

            if (!players.Any())
            {
                newGame.Player2 = new TournamentPlayer { Player = new ApplicationUser{UserName = "BYE" }};
            }
            else
            {
                newGame.Player2 = players.Random()!;
                players.Remove(newGame.Player2);
            }

            newGame.TournamentPlayer2Id = newGame.Player2.Id;

            games.Add(newGame);
        }

        return games;
    }
}