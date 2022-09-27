namespace XWingTO.Core
{
    public class Tournament
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public DateOnly Date { get; set; }
        public Guid TOId { get; set; }
        public string Country { get; set; }
        public string State { get; set; }
        public string City { get; set; }
        public string Venue { get; set; }
        public virtual IEnumerable<TournamentPlayer> Players { get; set; }

        public virtual IEnumerable<TournamentRound> Rounds { get; set; }
    }
}
