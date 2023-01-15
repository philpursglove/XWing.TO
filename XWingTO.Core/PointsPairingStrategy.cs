namespace XWingTO.Core;

public class PointsPairingStrategy : IPairingStrategy
{
    public IEnumerable<Game> Pair(List<TournamentPlayer> players)
    {
        List<Game> games = new List<Game>();
        List<Bracket> brackets = new List<Bracket>();

        IEnumerable<int> bracketPoints = players.Select(p => p.Points).Distinct().OrderByDescending(p => p);

        foreach (var bracketPoint in bracketPoints)
        {
            Bracket bracket = new Bracket()
            { Points = bracketPoint, Players = players.Where(p => p.Points == bracketPoint).ToList() };
            brackets.Add(bracket);
        }

        for (int i = 0; i < brackets.Count; i++)
        {
            List<TournamentPlayer> bracketPlayers = brackets[i].Players;
            if (bracketPlayers.Count % 2 == 1)
            {
                if (i < brackets.Count - 1)
                {
                    TournamentPlayer pairedUp = brackets[i + 1].Players.Random();
                    bracketPlayers.Add(pairedUp);
                    brackets[i + 1].Players.Remove(pairedUp);
                }
            }

            while (bracketPlayers.Any())
            {
                Game newGame = new Game();

				newGame.TableNumber = games.Count + 1;

                if (bracketPlayers.Any(p => p.Bye))
                {
                    newGame.Player1 = bracketPlayers.Where(p => p.Bye).Random();
                }
                else
                {
                    newGame.Player1 = bracketPlayers.Random();
                }
                bracketPlayers.Remove(newGame.Player1);

                if (!bracketPlayers.Any())
                {
                    newGame.Player2 = new TournamentPlayer() { Player = new ApplicationUser { UserName = "BYE" }};
                }
                else
                {
                    if (bracketPlayers.Any(p => p.Bye))
                    {
                        newGame.Player2 = bracketPlayers.Where(p => p.Bye).Random();
                    }
                    else
                    {
                        newGame.Player2 = bracketPlayers.Random();
                    }
                    bracketPlayers.Remove(newGame.Player2);
                }

                newGame.TournamentPlayer1Id = newGame.Player1.PlayerId;
                newGame.TournamentPlayer2Id = newGame.Player2.PlayerId;

                games.Add(newGame);
            }
        }

        return games;

    }
}