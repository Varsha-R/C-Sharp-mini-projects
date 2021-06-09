using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonDeserialize
{
    public class SettingsInformation
    {
        public string Version { get; set; }
        public List<Settings> Settings { get; set; }
    }
}
