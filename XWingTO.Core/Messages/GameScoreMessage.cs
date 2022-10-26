namespace XWingTO.Core.Messages
{
	public class GameScoreMessage
	{
		public Guid GameId { get; set; }
		public int Player1Points { get; set; }
		public int Player2Points { get; set;}
		public int Turns { get; set; }
		public bool OutOfTime { get; set; }
		public bool Player1Concede { get; set; }
		public bool Player2Concede { get; set; }

		public bool Player1Drop { get; set; }
		public bool Player2Drop { get; set; }
	}
}
