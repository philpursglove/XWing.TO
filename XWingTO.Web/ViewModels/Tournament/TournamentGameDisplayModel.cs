namespace XWingTO.Web.ViewModels.Tournament;

public class TournamentGameDisplayModel
{
	public string Player1 { get; set; }
	public string Player2 { get; set; }
	public int Player1Score { get; set; }
	public int Player2Score { get; set; }

	public Guid GameId { get; set; }
}