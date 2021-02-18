using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;


namespace Chatter.Server.Model
{
    public class ApplicationUser : IdentityUser
    {
        public string Name { get; set; }
        public string Avatar { get; set; }
        public ICollection<Room> Rooms { get; set; }
        public ICollection<Message> Messages { get; set; }
    }
}
