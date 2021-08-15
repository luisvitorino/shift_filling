using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GR.Models
{
    public class Gage_Schedule
    {
        
        public int ID { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:HH:mm}", ApplyFormatInEditMode = true)]
        public DateTime END_Fi { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:HH:mm}", ApplyFormatInEditMode = true)]
        public DateTime END_Se { get; set; }
        [DisplayFormat(DataFormatString = "{0:HH:mm}", ApplyFormatInEditMode = true)]
        public DateTime END_Th { get; set; }
        public int timerequired { get; set; }
        public double TR_Fi { get; set; }
        public double TR_Se { get; set; }
        public double TR_Th { get; set; }      
        public int 早 { get; set; }
        public int 中 { get; set; }
        public int 晚 { get; set; }
        public int fixture { get; set; }

        public Gage_Demand demand { get; set; }
        public Gage_Capacity capacity { get; set; }
        public Gage_Curing curing { get; set; }
        public Gage_Straingage straingage { get; set; }



        //private Demand _demand;
        //public Demand demand { get => _demand; set => _demand = value; }


    }
}
