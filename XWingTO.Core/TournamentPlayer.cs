namespace XWingTO.Core;

public class TournamentPlayer
{
    public Guid Id { get; set; }
    public Guid TournamentId { get; set; }
    public Guid PlayerId { get; set; }
    public Tournament Tournament { get; set; }
    public int Points { get; set; }
    public int MissionPoints { get; set; }
    public bool Bye { get; set; }
    public bool Dropped { get; set; }

    public ApplicationUser Player { get; set; }
}