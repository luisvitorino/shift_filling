using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GR.Models
{
    public class Gage_Curing
    {
        public int ID { get; set; }
        public string Fixture { get; set; }
        public int Fixture_capacity { get; set; }
        public int Wait_time { get; set; }     

    }
}
