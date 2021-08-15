using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GR.Models;
using GR.Pages;
using GR.Utils;

namespace GR.Utils
{
    public class CapacityBucket
    {
        DateTime StartWork;
        DateTime EndWork;
        TimeSpan TimeWork;
        TimeSpan TimeCapacity;

        //CapacityBucket constructor
        public CapacityBucket(DateTime start, DateTime end)
        {
            this.StartWork = start;
            this.EndWork = end;
            this.TimeCapacity = new TimeSpan(this.EndWork.Ticks - this.StartWork.Ticks);
            this.TimeWork = new TimeSpan();
        }

        //CapacityBucket methods 
        public int TimeCapacityLeft()
        {
            TimeSpan timeLeft = this.TimeCapacity - this.TimeWork;
            return (int)timeLeft.TotalMinutes;
        }

        public int AddTime(double minutes)
        {
            TimeSpan TimeLeft = TimeCapacity - TimeWork;  
            int MinutesLeft = (int) TimeLeft.TotalMinutes;
            double plannedInThisShift = Math.Min(MinutesLeft, minutes);
            int int_plannedInThisShift = (int)Math.Floor(plannedInThisShift);
            if(int_plannedInThisShift.Equals(0)){int_plannedInThisShift = 1;}
            this.TimeWork = TimeWork.Add(new TimeSpan(0,int_plannedInThisShift,0));
            double diference = minutes - plannedInThisShift;
            return (int) diference;
        }

        public bool IsFull()
        {
            return (this.TimeWork >= TimeCapacity);
        }

        public DateTime GetCurrentTime()
        {   
            return StartWork.Add(TimeWork);
        }   

    }
}
