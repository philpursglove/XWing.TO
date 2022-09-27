using XWingTO.Core;

namespace XWingTO.Tests
{
    [TestFixture]
    public class PointsStrategyTests
    {
        private Pairer pairer;

        [SetUp]
        public void Setup()
        {
            pairer = new Pairer(new PointsPairingStrategy());
        }

        [Test]
        public void For_An_Odd_Number_Of_Players_The_Last_Player_Is_A_Bye()
        {
            TournamentPlayer alice = new TournamentPlayerBuilder().WithName("Alice");
            TournamentPlayer bob = new TournamentPlayerBuilder().WithName("Bob");
            TournamentPlayer charlie = new TournamentPlayerBuilder().WithName("Charlie");

            List<TournamentPlayer> players = new List<TournamentPlayer>() { alice, bob, charlie };

            var result = pairer.Pair(players);

            var game = result.Last();

            Assert.That(game.Player2.Player.UserName, Is.EqualTo("BYE"));
        }

        [TestCase(2, 1)]
        [TestCase(3, 2)]
        [TestCase(4, 2)]
        [TestCase(5, 3)]
        public void Number_Of_Games_Generated(int numberOfPlayers, int expectedNumberOfGames)
        {
            List<TournamentPlayer> players = new List<TournamentPlayer>();

            for (int i = 1; i <= numberOfPlayers; i++)
            {
                TournamentPlayer player = new TournamentPlayer { Player = new ApplicationUser { UserName = $"Player{i}" }};
                players.Add(player);
            }

            var result = pairer.Pair(players);

            Assert.That(result.Count, Is.EqualTo(expectedNumberOfGames));
        }

        [Test]
        public void A_Dropped_Player_Is_Not_Paired()
        {
            TournamentPlayer alice = new TournamentPlayerBuilder().WithName("Alice");
            TournamentPlayer bob = new TournamentPlayerBuilder().WithName("Bob");
            TournamentPlayer charlie = new TournamentPlayerBuilder().WithName("Charlie");
            TournamentPlayer dave = new TournamentPlayer { Player = new ApplicationUser { UserName = "Dave"}, Dropped = true };

            List<TournamentPlayer> players = new List<TournamentPlayer>() { alice, bob, charlie, dave };

            var result = pairer.Pair(players);

            var game = result.Last();

            Assert.That(game.Player2.Player.UserName, Is.EqualTo("BYE"));
        }

        [Test]
        public void Players_Get_Matched_With_Players_On_Equal_Points()
        {
            TournamentPlayer alice = new TournamentPlayerBuilder().WithName("Alice").WithPoints(3);
            TournamentPlayer bob = new TournamentPlayerBuilder().WithName("Bob").WithPoints(3);
            TournamentPlayer charlie = new TournamentPlayerBuilder().WithName("Charlie");
            TournamentPlayer dave = new TournamentPlayer { Player = new ApplicationUser { UserName = "Dave" }, Points = 0 };

            List<TournamentPlayer> players = new List<TournamentPlayer>() { alice, bob, charlie, dave };

            var result = pairer.Pair(players);

            var game = result.First(g => g.Player1 == alice || g.Player2 == alice);
            if (game.Player1 == alice)
            {
                Assert.That(game.Player2, Is.EqualTo(bob));
            }
            else
            {
                Assert.That(game.Player1, Is.EqualTo(bob));
            }
            game = result.First(g => g.Player1 == charlie || g.Player2 == charlie);
            if (game.Player1 == charlie)
            {
                Assert.That(game.Player2, Is.EqualTo(dave));
            }
            else
            {
                Assert.That(game.Player1, Is.EqualTo(dave));
            }
        }

        [Test]
        public void Where_A_Bracket_Has_An_Odd_Number_Of_Players_A_Random_Player_From_The_Next_Bracket_Is_Paired_Up()
        {
            TournamentPlayer alice = new TournamentPlayerBuilder().WithName("Alice").WithPoints(3);
            TournamentPlayer bob = new TournamentPlayerBuilder().WithName("Bob").WithPoints(1);
            TournamentPlayer charlie = new TournamentPlayerBuilder().WithName("Charlie").WithPoints(1);
            TournamentPlayer dave = new TournamentPlayer { Player = new ApplicationUser { UserName = "Dave" }, Points = 0 };

            List<TournamentPlayer> players = new List<TournamentPlayer> { alice, bob, charlie, dave };

            var result = pairer.Pair(players);

            // One of bob or charlie must be paired with alice
            // One of bob or charlie must be paired with dave

            IEnumerable<TournamentPlayer> middlePlayers = players.Where(p => p.Player.UserName == "Bob" || p.Player.UserName == "Charlie");

            Game firstGame = result.First(g => g.Player1.Player.UserName == "Alice" || g.Player2.Player.UserName == "Alice");
            if (firstGame.Player1.Player.UserName == "Alice")
            {
                Assert.That(middlePlayers.Contains(firstGame.Player2));
            }
            else
            {
                Assert.That(middlePlayers.Contains(firstGame.Player1));
            }
            Game lastGame = result.First(g => g.Player1.Player.UserName == "Dave" || g.Player2.Player.UserName == "Dave");
            if (lastGame.Player1.Player.UserName == "Dave")
            {
                Assert.That(middlePlayers.Contains(lastGame.Player2));
            }
            else
            {
                Assert.That(middlePlayers.Contains(lastGame.Player1));
            }
        }

        [Test]
        public void Where_Brackets_Finish_With_An_Odd_Number_In_The_Last_Group_The_Last_Game_Is_A_Bye()
        {
            List<TournamentPlayer> players = new List<TournamentPlayer>()
            {
                new TournamentPlayerBuilder().WithName("Alice").WithPoints(3),
                new TournamentPlayerBuilder().WithName("Bob").WithPoints(1),
                new TournamentPlayerBuilder().WithName("Charlie").WithPoints(1),
                new TournamentPlayerBuilder().WithName("Dave").WithPoints(0),
                new TournamentPlayerBuilder().WithName("Ed").WithPoints(0),
                new TournamentPlayerBuilder().WithName("Fred").WithPoints(0).WithDropped(true)
            };

            var result = pairer.Pair(players);

            // One of bob or charlie must be paired with alice
            // One of bob or charlie must be paired with dave

            Game lastGame = result.Last();
            Assert.That(lastGame.Player2.Player.UserName, Is.EqualTo("BYE"));
        }

        [Test]
        public void In_The_Last_Bracket_Assign_The_Bye_To_A_Player_Who_Has_Not_Had_A_Bye()
        {
            List<TournamentPlayer> players = new List<TournamentPlayer>()
            {
                new TournamentPlayerBuilder().WithName("Alice").WithPoints(3),
                new TournamentPlayerBuilder().WithName("Bob").WithPoints(3),
                new TournamentPlayerBuilder().WithName("Charlie").WithPoints(1),
                new TournamentPlayerBuilder().WithName("Dave").WithPoints(1),
                new TournamentPlayerBuilder().WithName("Ed").WithPoints(0).WithBye(true),
                new TournamentPlayerBuilder().WithName("Fred").WithPoints(0).WithBye(true),
                new TournamentPlayerBuilder().WithName("George").WithPoints(0).WithBye(false)
            };

            var result = pairer.Pair(players);

            Game lastGame = result.Last();
            Assert.That(lastGame.Player1.Player.UserName, Is.EqualTo("George"));
            Assert.That(lastGame.Player2.Player.UserName, Is.EqualTo("BYE"));
        }
    }
}