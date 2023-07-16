namespace XWingTO.Tests;

public class TournamentPlayerBuilder
{
	private TournamentPlayer _player = new TournamentPlayer {Player = new ApplicationUser()};

    public static implicit operator TournamentPlayer(TournamentPlayerBuilder builder)
    {
        return builder.Build();
    }

    public TournamentPlayerBuilder WithName(string name)
    {
        _player.Player.UserName = name;
        return this;
    }

    public TournamentPlayerBuilder WithDropped(bool dropped)
    {
        _player.Dropped = dropped;
        return this;
    }

    public TournamentPlayerBuilder WithPoints(int points)
    {
        _player.Points = points;
        return this;
    }

    public TournamentPlayerBuilder WithBye(bool bye)
    {
        _player.Bye = bye;
        return this;
    }

    public TournamentPlayer Build()
    {
        return _player;
    }
}