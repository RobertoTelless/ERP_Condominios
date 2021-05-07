using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntitiesServices.WorkClasses
{
    public class FileQueue
    {
        public String Name { get; set; }
        public String ContentType { get; set; }
        public byte[] Contents { get; set; }
        public Int32? Profile { get; set; }
    }
}
