using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EstimatedTimePOC.Models
{
    class Partition
    {
        public int Index { get; set; }
        public string Type { get; set; }
        public UInt64 StartSector { get; set; }
        public UInt64 EndSector { get; set; }
        public bool Erasable { get; set; }
    }
}
