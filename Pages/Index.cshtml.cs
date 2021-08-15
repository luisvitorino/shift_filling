using GR.Models;
using GR.Utils;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GR.Pages
{
    public class ScheduleModel : PageModel
    {

        //Database constructor
        private readonly GR.DatabaseContext _context;
        public ScheduleModel(GR.DatabaseContext context)
        {
            _context = context;
        }

        //Lists
        public IList<Gage_Demand> demand { get; set; }
        public IList<Gage_Schedule> schedule { get; set; }
        public IList<Gage_Capacity> capacity { get; set; }
        public IList<string> weekend { get; set; }

        //Input dictionaries 
        public Dictionary<string, float> Morning_dictionary = new Dictionary<string, float>();        
        public Dictionary<string, float> Middle_dictionary = new Dictionary<string, float>();
        public Dictionary<string, float> Night_dictionary = new Dictionary<string, float>();
        public Dictionary<string, string> Week_dictionary = new Dictionary<string, string>();

        //Initialize page
        public void OnGet()
        {
            if (!Morning_dictionary.ContainsKey("独立组1"))
            {
                Morning_dictionary.Add("独立组1", 1);
            }
            if (!Morning_dictionary.ContainsKey("独立组2"))
            {
                Morning_dictionary.Add("独立组2", 1);
            }
            if (!Morning_dictionary.ContainsKey("连续流组"))
            {
                Morning_dictionary.Add("连续流组", 1);
            }

            if (!Middle_dictionary.ContainsKey("独立组1"))
            {
                Middle_dictionary.Add("独立组1", 1);
            }
            if (!Middle_dictionary.ContainsKey("独立组2"))
            {
                Middle_dictionary.Add("独立组2", 1);
            }
            if (!Middle_dictionary.ContainsKey("连续流组"))
            {
                Middle_dictionary.Add("连续流组", 1);
            }

            if (!Night_dictionary.ContainsKey("独立组1"))
            {
                Night_dictionary.Add("独立组1", 1);
            }
            if (!Night_dictionary.ContainsKey("独立组2"))
            {
                Night_dictionary.Add("独立组2", 1);
            }
            if (!Night_dictionary.ContainsKey("连续流组"))
            {
                Night_dictionary.Add("连续流组", 1);
            }
            if (!Week_dictionary.ContainsKey("saturday"))
            {
                Week_dictionary.Add("saturday", "");
            }
            if (!Week_dictionary.ContainsKey("sunday"))
            {
                Week_dictionary.Add("sunday", "");
            }

            if (_context.Gage_Schedule.Count() == 0)
            {   
                demand = _context.Gage_Demand.ToList();
                foreach (Gage_Demand de in demand)
                {
                    Gage_Capacity cap = _context.Gage_Capacity.Where((c) => c.Model.Equals(de.Model)).FirstOrDefault();
                                  
                    Gage_Straingage st = _context.Gage_Straingage.Where((s) => s.Model.Equals(de.Model)).FirstOrDefault();

                    Gage_Curing cur = _context.Gage_Curing.Where((cu) => cu.Fixture.Equals(cap.Fixture)).FirstOrDefault(); 

                    Gage_Schedule schedule = new Gage_Schedule();
                    schedule.straingage = st;
                    schedule.demand = de;
                    schedule.capacity = cap;
                    schedule.curing = cur;

                    _context.Gage_Schedule.Add(schedule);
                }
                _context.SaveChanges();
            }
            schedule = _context.Gage_Schedule.Include(s => s.demand).Include(s => s.capacity).Include(cu => cu.curing).Include(s => s.straingage).ToList();
        }

        //Shifts creation
        private ShiftBucket MorningShift;
        private ShiftBucket LateShift;
        public void InitShifts()
        {
            string[] productionLines = { "独立组1", "独立组2", "连续流组" };
            MorningShift = new ShiftBucket(DateTime.Today.AddHours(8));
            LateShift = new ShiftBucket(DateTime.Today.AddHours(16));

            MorningShift.createTimeCapacities(productionLines);
            LateShift.createTimeCapacities(productionLines);
        }

        //Input dictionaries
        public float LastForm(string name)
        {
            float value = 0;
            if (name.Equals("txtPL1_Fi"))
            {
                value = Morning_dictionary["独立组1"];
            }
            if (name.Equals("txtPL2_Fi"))
            {
                value = Morning_dictionary["独立组2"];
            }
            if (name.Equals("txtPLC_Fi"))
            {
                value = Morning_dictionary["连续流组"];
            }
            if (name.Equals("txtPL1_Se"))
            {
                value = Middle_dictionary["独立组1"];
            }
            if (name.Equals("txtPL2_Se"))
            {
                value = Middle_dictionary["独立组2"];
            }
            if (name.Equals("txtPLC_Se"))
            {
                value = Middle_dictionary["连续流组"];
            }
            if (name.Equals("txtPL1_Th"))
            {
                value = Night_dictionary["独立组1"];
            }
            if (name.Equals("txtPL2_Th"))
            {
                value = Night_dictionary["独立组2"];
            }
            if (name.Equals("txtPLC_Th"))
            {
                value = Night_dictionary["连续流组"];
            }
            return value;
        }

        //Schedule Day
        DateTime Date = DateTime.Today.AddDays(1);
        public string SaturdayChecked;
        public string SundayChecked;
        public DateTime TakeDate(string name)
        {
            DateTime date = Date;
            return date;
        }

        //Button Logic  
        public void OnPostOnCalculateSchedule()
        {
            List<Gage_Schedule> schedules = _context.Gage_Schedule.Include(s => s.demand).Include(s => s.capacity).Include(cu => cu.curing).Include(s => s.straingage).ToList();     
            InitShifts();

            float PLC_first = float.Parse(this.Request.Form["txtPLC_Fi"]);
            float PLC_second = float.Parse(this.Request.Form["txtPLC_Se"]);
            float PLC_third = float.Parse(this.Request.Form["txtPLC_Th"]);
            float PL1_first = float.Parse(this.Request.Form["txtPL1_Fi"]);
            float PL1_second = float.Parse(this.Request.Form["txtPL1_Se"]);
            float PL1_third = float.Parse(this.Request.Form["txtPL1_Th"]);
            float PL2_first = float.Parse(this.Request.Form["txtPL2_Fi"]);
            float PL2_second = float.Parse(this.Request.Form["txtPL2_Se"]);
            float PL2_third = float.Parse(this.Request.Form["txtPL2_Th"]);

            Morning_dictionary.Add("连续流组", PLC_first);
            Morning_dictionary.Add("独立组1", PL1_first);
            Morning_dictionary.Add("独立组2", PL2_first);

            Middle_dictionary.Add("连续流组", PLC_second);
            Middle_dictionary.Add("独立组1", PL1_second);
            Middle_dictionary.Add("独立组2", PL2_second);

            Night_dictionary.Add("连续流组", PLC_third);
            Night_dictionary.Add("独立组1", PL1_third);
            Night_dictionary.Add("独立组2", PL2_third);

            string saturday = this.Request.Form["saturday"];
            string sunday = this.Request.Form["sunday"];
            Week_dictionary.Add("saturday", saturday);
            Week_dictionary.Add("sunday", sunday);

            if (Week_dictionary["saturday"] == null)
            {
                Week_dictionary["saturday"] = "off";
                SaturdayChecked = "";
            }
            if (Week_dictionary["sunday"] == null)
            {
                Week_dictionary["sunday"] = "off";
                SundayChecked = "";
            }

            if (Week_dictionary["saturday"] == "on")
            {
                Week_dictionary["saturday"] = "on";
                SaturdayChecked = "checked";
            }
            if (Week_dictionary["sunday"] == "on")
            {
                Week_dictionary["sunday"] = "on";
                SundayChecked = "checked";
            }

            if (Date.DayOfWeek == DayOfWeek.Saturday && Week_dictionary["saturday"] == "on")
            {
                Date = Date.AddDays(1);
            }
            if (Date.DayOfWeek == DayOfWeek.Sunday && Week_dictionary["sunday"].Equals("on"))
            {
                Date = Date.AddDays(1);
            }
            
            foreach (Gage_Schedule s in schedules.OrderBy(s => s.demand.Priority))
            {
                int quantityToDistribute = s.demand.Quantity;
                int cTm = (int)Math.Floor(MorningShift.GetTimeCapacity(s.capacity));
                int cFm = MorningShift.GetFixtureCapacity(s.curing);
                int cMorning = Math.Min(cTm, cFm);
                s.fixture = 0;
                if (quantityToDistribute > cFm) { s.fixture = 1; }
                
                if (cMorning > 0 && quantityToDistribute > 0)
                {
                    int actualQuantityDistributed = Math.Min(cMorning, quantityToDistribute);
                    s.早 = actualQuantityDistributed;
                    s.TR_Fi = Math.Floor(MorningShift.TimeRequired(Morning_dictionary, s.capacity.CAPACITY, actualQuantityDistributed, s.capacity.Production_Line));
                    if (s.TR_Fi.Equals(0)){s.TR_Fi = 1;}
                    MorningShift.AddFixture(actualQuantityDistributed, s.curing);
                    s.END_Fi = MorningShift.GetEndTime(s.capacity.Production_Line);
                    quantityToDistribute = Math.Max(quantityToDistribute - cMorning,0);
                }

                int cTl = (int)Math.Floor(LateShift.GetTimeCapacity(s.capacity));
                int cFl = LateShift.GetFixtureCapacity(s.curing);
                int cLate = Math.Min(cTl, cFl);
                if (quantityToDistribute > cFl) { s.fixture = 1; }

                if (cLate > 0 && quantityToDistribute > 0)
                {
                    int actualQuantityDistributed = Math.Min(cLate, quantityToDistribute);
                    s.中 = actualQuantityDistributed;
                    s.TR_Se = Math.Floor(LateShift.TimeRequired(Middle_dictionary, s.capacity.CAPACITY, actualQuantityDistributed, s.capacity.Production_Line));
                    if (s.TR_Se.Equals(0)) { s.TR_Se = 1; }
                    LateShift.AddFixture(actualQuantityDistributed, s.curing);
                    s.END_Se = LateShift.GetEndTime(s.capacity.Production_Line);
                    quantityToDistribute = Math.Max(quantityToDistribute - cLate, 0);
                }
                _context.Gage_Schedule.Update(s);
                _context.SaveChanges();               
            }
            OnGet();
        }

        //Clear button logic 
        public void OnPostOnClearTable()
        {
            List<Gage_Schedule> schedules = _context.Gage_Schedule.ToList();
            foreach (Gage_Schedule s in schedules)
            {
                _context.Gage_Schedule.Remove(s);
                _context.SaveChanges();
            }
            float PLC_first = float.Parse(this.Request.Form["txtPLC_Fi"]);
            float PLC_second = float.Parse(this.Request.Form["txtPLC_Se"]);
            float PLC_third = float.Parse(this.Request.Form["txtPLC_Th"]);
            float PL1_first = float.Parse(this.Request.Form["txtPL1_Fi"]);
            float PL1_second = float.Parse(this.Request.Form["txtPL1_Se"]);
            float PL1_third = float.Parse(this.Request.Form["txtPL1_Th"]);
            float PL2_first = float.Parse(this.Request.Form["txtPL2_Fi"]);
            float PL2_second = float.Parse(this.Request.Form["txtPL2_Se"]);
            float PL2_third = float.Parse(this.Request.Form["txtPL2_Th"]);

            Morning_dictionary.Add("连续流组", PLC_first);
            Morning_dictionary.Add("独立组1", PL1_first);
            Morning_dictionary.Add("独立组2", PL2_first);

            Middle_dictionary.Add("连续流组", PLC_second);
            Middle_dictionary.Add("独立组1", PL1_second);
            Middle_dictionary.Add("独立组2", PL2_second);

            Night_dictionary.Add("连续流组", PLC_third);
            Night_dictionary.Add("独立组1", PLC_third);
            Night_dictionary.Add("独立组2", PL2_third);


            string saturday = this.Request.Form["saturday"];
            string sunday = this.Request.Form["sunday"];
            Week_dictionary.Add("saturday", saturday);
            Week_dictionary.Add("sunday", sunday);
            if (Week_dictionary["saturday"] == "on")
            {
                Week_dictionary["saturday"] = "on";
                SaturdayChecked = "checked";
            }
            if (Week_dictionary["sunday"] == "on")
            {
                Week_dictionary["sunday"] = "on";
                SundayChecked = "checked";
            }

            OnGet();
        }
    }
}
