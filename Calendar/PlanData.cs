using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calendar
{
    [Serializable]
    public class PlanData
    {
        private List<WorkItem> work;

        public List<WorkItem> Work { get => work; set => work = value; }


    }
}
