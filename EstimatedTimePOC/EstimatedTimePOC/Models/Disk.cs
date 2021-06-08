using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EstimatedTimePOC.Models
{
    class Disk
    {
        public Disk()
        {
            this.Partition = new List<Partition>();
        }
        public UInt32 BytesPerSector { get; set; }
        public List<Partition> Partition { get; set; }
        public UInt32 Index { get; set; }
        public UInt64 Size { get; set; }
    }
}
