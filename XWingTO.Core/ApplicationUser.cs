using Microsoft.AspNetCore.Identity;

namespace XWingTO.Core
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        public string Country { get; set; }
        public string State { get; set; }
        public string City { get; set; }
        public string Venue { get; set; }

        public IEnumerable<TournamentPlayer> TournamentPlayers { get; set; }

        public string DisplayName { get; set; }
    }
}
