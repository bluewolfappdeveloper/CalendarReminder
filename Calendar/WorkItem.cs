using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Calendar
{
    public class WorkItem
    {
        private DateTime date;
        public DateTime Date { get => date; set => date = value; }

        private string work;
        public string Work { get => work; set => work = value; }


        private Point fromTime;
        public Point FromTime { get => fromTime; set => fromTime = value; }

        private Point endTime;
        public Point EndTime { get => endTime; set => endTime = value; }

        private string status;
        public string Status { get => status; set => status = value; }


        public static List<string> listStatus = new List<string>() { "Done", "Doing", "Comming", "Missed" };
    }

    public enum EWorkItem
    {
        Done,
        Doing,
        Coming,
        Missed,
    }
}
