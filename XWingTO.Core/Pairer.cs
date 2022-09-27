namespace XWingTO.Core;

public class Pairer
{
    private readonly IPairingStrategy pairingStrategy;

    public Pairer(IPairingStrategy strategy)
    {
        pairingStrategy = strategy;
    }


    public List<Game> Pair(List<TournamentPlayer> players)
    {
        players = players.Where(p => !p.Dropped).ToList();

        List<Game> games = new List<Game>();

        games = pairingStrategy.Pair(players).ToList();

        return games;
    }
}