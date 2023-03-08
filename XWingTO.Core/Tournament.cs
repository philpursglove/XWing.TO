namespace XWingTO.Core
{
    public class Tournament
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Date Date { get; set; }
        public Guid TOId { get; set; }
        public string? Country { get; set; }
        public string? State { get; set; }
        public string? City { get; set; }
        public string? Venue { get; set; }
        public DateTime CreationDate {get; set;}
        public virtual IEnumerable<TournamentPlayer> Players { get; set; }

        public virtual IEnumerable<TournamentRound> Rounds { get; set; }

        public TournamentFormat Format { get; set; }

        public string Location()
        {
	        List<string> locationElements = new List<string>();
            if (!string.IsNullOrWhiteSpace(Country)) locationElements.Add(Country);
            if (!string.IsNullOrWhiteSpace(State)) locationElements.Add(State);
			if (!string.IsNullOrWhiteSpace(City)) locationElements.Add(City);
			if (!string.IsNullOrWhiteSpace(Venue)) locationElements.Add(Venue);

			if (locationElements.Any())
			{
				return string.Join("/", locationElements);
			}
            return string.Empty;
        }
	}
}
