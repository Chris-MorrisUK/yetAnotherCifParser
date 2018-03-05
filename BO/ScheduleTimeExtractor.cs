using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScheduleVis.BO
{
    public static class ScheduleTimeExtractor
    {
        public static DateTime? GetTime(string line, int offset)
        { 
            string timePart = line.Substring(offset, 4);
            if (string.IsNullOrWhiteSpace(timePart))
                return null;
            DateTime result = DateTime.ParseExact(timePart, TimeFormat, ProgramState.Provider);
            if (line[offset + 5] == 'h')
                result.AddSeconds(30);
            return result;
        }
        public static DateTime? GetTimeNoHalf(string line, int offset)
        {
            string timePart = line.Substring(offset, 4);
            if (string.IsNullOrWhiteSpace(timePart))
                return null;
            DateTime result = DateTime.ParseExact(timePart, TimeFormat, ProgramState.Provider);
            return result;
        }

        public static TimeSpan? TwoCharacterDigitTime(string line, int offset)
        {            
            if (!char.IsDigit(line[offset + 1]))
            {
                string timeCode = line.Substring(offset, 1);
                if (string.IsNullOrEmpty(timeCode))
                    return null;
                if (string.IsNullOrWhiteSpace(timeCode))
                    return null;
                double nMinutes = double.Parse(timeCode);
                if (line[offset + 1] == 'h')
                {
                    TimeSpan res = TimeSpan.FromMinutes(nMinutes + 0.5);
                    return res;                    
                }
                return TimeSpan.FromMinutes(nMinutes);
            }
            else
            {  
                //No half minutes above 9
                string timeCode = line.Substring(offset, 2);
                int nMinutes = int.Parse(timeCode);
                return TimeSpan.FromMinutes(nMinutes);
            }

        }
        const string TimeFormat = "HHmm";
    }
}
