using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GR.Models
{
    public class Gage_Capacity
    {

        public int ID { get; set; }
        public string Model { get; set; }
        public string Production_Line { get; set; }
        public double CAPACITY { get; set; }
        public string Fixture { get; set; }
        public int Priority { get; set; }
        public int Into_oven_hour { get; set; }
        public int Out_oven_hour { get; set; }
   
    }
}
