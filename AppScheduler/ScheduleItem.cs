using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppScheduler
{
    class ScheduleItem
    {
        bool isURL = false;
        string filePath = "";
        string programName = "";
        string[] days = new string[7];
        string startTime = "";
        string endTime = "";
        public ScheduleItem(bool isURL, string filePath
            , string programName, string startTime, string endTime)
        {
            this.isURL = isURL;
            this.filePath = filePath;
            this.programName = programName;
            this.startTime = startTime;
            this.endTime = endTime;
        }

        public string GetName()
        {
            return programName;
        }

        public bool curDayValid(DateTime curDay)
        {
            int[] dayInts = new int[days.Length];
            for (int i = 0; i < days.Length; i++)
            {
                DayOfWeek tempCurDay = curDay.DayOfWeek - 1;
                if (tempCurDay.ToString() == days[i])
                {
                    return true;
                }

            }
            return false;
        }

        public int getStartHour()
        {
            return Convert.ToInt32(startTime.Split(':')[0]);
        }

        public int getEndHour()
        {
            return Convert.ToInt32(endTime.Split(':')[0]);
        }

        public int getStartMinute()
        {
            return Convert.ToInt32(startTime.Split(':')[1]);
        }

        public int getEndMinute()
        {
            return Convert.ToInt32(endTime.Split(':')[1]);
        }

        public int getStartSecond()
        {
            return Convert.ToInt32(startTime.Split(':')[2]);
        }

        public int getEndSecond()
        {
            return Convert.ToInt32(endTime.Split(':')[2]);
        }

        public string getProgramPath()
        {
            return filePath;
        }

        public void setSelectedDays(string[] selectedDays)
        {
            days = selectedDays;
        }
    }
}
