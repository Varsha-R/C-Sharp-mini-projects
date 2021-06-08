using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FetchUserProfiles
{
    public class Account
    {
        public string DisplayName { get; set; }
        public string AccountPicture { get; set; }
        public bool? LoggedInUser { get; set; }
        public string UserName { get; set; }
    }
}
