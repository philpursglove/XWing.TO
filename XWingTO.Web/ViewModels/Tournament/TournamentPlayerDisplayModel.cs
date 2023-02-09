namespace XWingTO.Web.ViewModels.Tournament;

public class TournamentPlayerDisplayModel
{
	public string Name { get; set; }
	public int Points { get; set; }

	public bool Dropped { get; set; }

	public Guid TournamentPlayerId { get; set; }

	public int MissionPoints { get; set; }

}