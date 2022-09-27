namespace XWingTO.Core;

public interface IPairingStrategy
{
    IEnumerable<Game> Pair(List<TournamentPlayer> players);
}