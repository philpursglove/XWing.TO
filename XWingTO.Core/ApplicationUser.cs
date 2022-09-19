using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace XWingTO.Core
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        public string Country { get; set; }
        public string State { get; set; }
        public string City { get; set; }
        public string Venue { get; set; }
    }
}
