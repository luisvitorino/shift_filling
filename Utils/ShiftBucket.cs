using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GR.Models;
using GR.Utils;

namespace GR.Utils
{
    public class ShiftBucket
    {
        //Work time and rest variables
        public const int HOURS_PER_SHIFT = 8;
        public const int HOURS_WORKED75 = 75;
        public const int HOURS_WORKED90 = 90;
        public const int HOURS_WORKED100 = 100;
        public const int HOURS_WORKED110 = 110;
        public const int HOURS_WORKED125 = 125;
        public const int five_s = 10;
        public const int rest = 10;
        public const int lunch = 30;
        public DateTime ShiftStart;
        public int shift_number = 0;

        //Create shift time's references
        public ShiftBucket(DateTime shiftStart)
        {
            this.ShiftStart = shiftStart;
            if(shiftStart.Hour.Equals(8))
            {
                shift_number = 1;
            }
            if (shiftStart.Hour.Equals(16))
            {
                shift_number = 2;
            }
            _capacityCuringMap = new Dictionary<Gage_Curing, int>();
        }
        public DateTime ShiftEnd
        {
            get
            {
                return ShiftStart.AddHours(HOURS_PER_SHIFT);
            }
        }        
        public DateTime FirstWork_start
        {
            get
            {
                if (shift_number.Equals(1)) { return ShiftStart.AddMinutes(five_s); }
                return ShiftStart;
            }
        }
        public DateTime FirstWork_end
        {
            get
            {
                if (shift_number.Equals(1)) { return FirstWork_start.AddMinutes(HOURS_WORKED100); }
                return FirstWork_start.AddMinutes(HOURS_WORKED110);
            }
        }
        public DateTime MiddleWork_start
        {
            get
            {
                if (shift_number.Equals(1)) { return FirstWork_end.AddMinutes(rest); }
                return FirstWork_end.AddMinutes(lunch);
            }
        }
        public DateTime MiddleWork_end
        {
            get
            {
                if (shift_number.Equals(1)) { return MiddleWork_start.AddMinutes(HOURS_WORKED75); }
                return MiddleWork_start.AddMinutes(HOURS_WORKED90);
            }
        }
        public DateTime Middle2Work_start
        {
            get
            {
                if (shift_number.Equals(1)) { return MiddleWork_end.AddMinutes(lunch); }
                return MiddleWork_end.AddMinutes(rest);
            }
        }
        public DateTime Middle2Work_end
        {
            get
            {
                if (shift_number.Equals(1)) { return Middle2Work_start.AddMinutes(HOURS_WORKED125); }
                return Middle2Work_start.AddMinutes(HOURS_WORKED110);
            }
        }
        public DateTime EndWork_start
        {
            get
            {
                return Middle2Work_end.AddMinutes(rest);
            }
        }
        public DateTime EndWork_end
        {
            get
            {
                return EndWork_start.AddMinutes(HOURS_WORKED110);
            }
        }


        //Create CapacityBuchets
        public Dictionary<string, IList<CapacityBucket>> capacitybuchets { get; set; }

        public Dictionary<string, IList<CapacityBucket>> createTimeCapacities(string[] productionLines)
        {            
            Dictionary<string, IList<CapacityBucket>> result = new Dictionary<string, IList<CapacityBucket>>();
            foreach (string productionLine in productionLines)
            {
                result.Add(productionLine, createInternalBuckets());
            }
            capacitybuchets = result;
            return result;
        }

        public IList<CapacityBucket> createInternalBuckets()
        {
            List<CapacityBucket> result = new List<CapacityBucket>();
            CapacityBucket firstwork = new CapacityBucket(FirstWork_start, FirstWork_end);
            result.Add(firstwork);
            CapacityBucket middlework = new CapacityBucket(MiddleWork_start, MiddleWork_end);
            result.Add(middlework);
            CapacityBucket middle2work = new CapacityBucket(Middle2Work_start, Middle2Work_end);
            result.Add(middle2work);
            result.Add(new CapacityBucket(EndWork_start, EndWork_end));
            return result;
        }

        //Schedule calculation auxiliary methods 
        public double GetTimeCapacity(Gage_Capacity pl)
        {
            int resultMinutes = 0;
            foreach(CapacityBucket bucket in this.capacitybuchets[pl.Production_Line])
            {
                resultMinutes = resultMinutes + bucket.TimeCapacityLeft();               
            }
            return ((resultMinutes * pl.CAPACITY) / 60);
        }

        public double TimeRequired(Dictionary<string, float> mult, double Capacity_Hour, int quantity, string productionlines)
        {
            double mult_select = mult[productionlines];
            double Capacity_hour = mult_select*Capacity_Hour;
            int Quantity = quantity;
            double timerequired = ((Quantity * 60) / (Capacity_hour));
            double TimeRequired = timerequired;
            IList<CapacityBucket> capacityBuchet_list = capacitybuchets[productionlines];
            foreach (CapacityBucket capacity in capacityBuchet_list)
            {
                timerequired = capacity.AddTime(timerequired);
            }
            return TimeRequired;
        }

        public DateTime GetEndTime(string productionlines)
        {
            IList<CapacityBucket> capacityBucket_list = capacitybuchets[productionlines];
            foreach (CapacityBucket capacity in capacityBucket_list)
            {
                if (!capacity.IsFull())
                {
                    DateTime endTime;
                    endTime = capacity.GetCurrentTime();
                    return endTime;
                }
            }
            return capacityBucket_list[capacityBucket_list.Count - 1].GetCurrentTime();
        }

        //Fixture logic 
        public Dictionary<Gage_Curing, int> _capacityCuringMap;
        public int GetFixtureCapacity(Gage_Curing fixture)
        {
            int Result = 0;
            if (!_capacityCuringMap.ContainsKey(fixture))
            {
                _capacityCuringMap.Add(fixture, 0);
            }
            Result = fixture.Fixture_capacity - this._capacityCuringMap[fixture];
            return Result;
        }

        public void AddFixture(int quantity, Gage_Curing curing)
        {
            int quantityToStore = quantity;
            if (_capacityCuringMap.ContainsKey(curing))
            {
                int alredyUssed = _capacityCuringMap[curing] + quantityToStore;
                _capacityCuringMap[curing] = alredyUssed;
            }
            else
            {
                _capacityCuringMap.Add(curing, quantityToStore);
            }
        }
    }
}
