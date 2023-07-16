namespace XWingTO.Tests;

public class RandomStrategyTests
{
    private Pairer pairer;

    [SetUp]
    public void SetUp()
    {
        pairer = new Pairer(new RandomPairingStrategy());
    }

    [Test]
    public void Players_Are_Matched_Randomly()
    {
        TournamentPlayer alice = new TournamentPlayerBuilder().WithName("Alice");
        TournamentPlayer bob = new TournamentPlayerBuilder().WithName("Bob");
        TournamentPlayer charlie = new TournamentPlayerBuilder().WithName("Charlie");
        TournamentPlayer dave = new TournamentPlayerBuilder().WithName("Dave");

        List<TournamentPlayer> players = new List<TournamentPlayer>() { alice, bob, charlie, dave };

        var result = pairer.Pair(players);

        List<TournamentPlayer> seenPlayers = new List<TournamentPlayer>();

        foreach (Game game in result)
        {
            Assert.That(players.Except(seenPlayers).Contains(game.Player1));
            seenPlayers.Add(game.Player1);
            Assert.That(players.Except(seenPlayers).Contains(game.Player2));
            seenPlayers.Add(game.Player2);
        }
    }

    [Test]
    public void Games_Are_Assigned_Table_Numbers()
    {
	    TournamentPlayer alice = new TournamentPlayerBuilder().WithName("Alice");
	    TournamentPlayer bob = new TournamentPlayerBuilder().WithName("Bob");
	    TournamentPlayer charlie = new TournamentPlayerBuilder().WithName("Charlie");
	    TournamentPlayer dave = new TournamentPlayerBuilder().WithName("Dave");

	    List<TournamentPlayer> players = new List<TournamentPlayer>() { alice, bob, charlie, dave };

	    var result = pairer.Pair(players);

        Assert.Multiple(() =>
        {
	        Game game = result.First();
	        Assert.That(game.TableNumber, Is.EqualTo(1));
            game = result.Last();
            Assert.That(game.TableNumber, Is.EqualTo(2));
        });
    }

}