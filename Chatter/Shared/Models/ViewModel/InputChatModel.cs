using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chatter.Shared.Models.ViewModel
{
    public class InputChatModel
    {
        public string NewRoom { get; set; } = "";
        public string Room { get; set; } = "";
        public string CurrentRoom { get; set; } = "";
        public string Message { get; set; } = "";
        public string Recipent { get; set; } = "";
    }
}
