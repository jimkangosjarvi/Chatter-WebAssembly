using System.Collections.Generic;

namespace Chatter.Shared.Models.Register
{
    public class RegisterResult
    {
        public bool Successful { get; set; }
        public IEnumerable<string> Errors { get; set; }
    }
}
