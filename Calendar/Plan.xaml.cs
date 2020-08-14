using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Xml.Serialization;

namespace Calendar
{
    /// <summary>
    /// Interaction logic for Plan.xaml
    /// </summary>
    public partial class Plan : Window
    {
        public Plan(DateTime date, PlanData data)
        {
            InitializeComponent();

            this.Visibility = Visibility.Visible;
            this.WindowState = WindowState.Normal;

            try
            {
                Job = data;

            }
            catch (Exception e)
            {
                //SetDefaltJob();
            }

            
            datepicker.DisplayDate = date;
            datepicker.Text = datepicker.DisplayDate.ToString();
            LoadWork(date);
            
        }

        #region Biến
        string FilePath = "data.xml";
        private PlanData job;
        public PlanData Job { get => job; set => job = value; }
        public PlanData JobDate { get => jobDate; set => jobDate = value; }

        private PlanData jobDate;
        #endregion


        #region Method
        void LoadWork(DateTime date)
        {
            ListWork.Children.Clear();
            this.WindowState = WindowState.Normal;
            if (Job != null && Job.Work != null && GetJobByDate(date).Count > 0)
            {
                //Job.Work = GetJobByDate(date);
                for (int i = 0; i < GetJobByDate(date).Count; i++)
                {
                    AddWork(GetJobByDate(date)[i]);
                }
            }

            GetStatusFromDate();
        }

        void AddWork(WorkItem item)
        {
            DockPanel panel = new DockPanel();
            panel.Margin = new Thickness(5, 5, 5, 10);

            WorkUC work = new WorkUC(item);
            work.Edited += work_Edited;
            work.Deleted += work_Deleted;

            panel.Children.Add(work);

            ListWork.Children.Add(panel);

            GetStatusFromDate();
        }

        List<WorkItem> GetJobByDate(DateTime date)
        {
            if (Job == null || Job.Work == null) return null;
            return Job.Work.Where(p => p.Date.Year == date.Year && p.Date.Month == date.Month && p.Date.Day == date.Day).ToList();
        }

        void SetDefaultDate()
        {
            datepicker.DisplayDate = DateTime.Now;

            datepicker.Text = datepicker.DisplayDate.ToString();
        }

        void GetStatusFromDate()
        {
            JobDate = new PlanData();
            JobDate.Work = GetJobByDate(datepicker.DisplayDate);
            if (JobDate == null || JobDate.Work == null ||JobDate.Work.Count <= 0 )
            {
                SumWork.Text = "0";
                DoneWork.Text = "0";
                Missed.Text = "0";
                DoingWork.Text = "0";
                ComingWork.Text = "0";
                NoStatus.Text = "0";
                return;
            }

            int[] a = new int[20];

            foreach (var item in JobDate.Work)
            {
                a[0]++;
                if (item.Status == EWorkItem.Done.ToString()) a[1]++;
                else
                  if (item.Status == EWorkItem.Missed.ToString()) a[2]++;
                else
                  if (item.Status == EWorkItem.Doing.ToString()) a[3]++;
                else
                  if (item.Status == EWorkItem.Coming.ToString()) a[4]++;
                else a[5]++;

            }

            SumWork.Text = a[0].ToString();
            DoneWork.Text = a[1].ToString();
            Missed.Text = a[2].ToString();
            DoingWork.Text = a[3].ToString();
            ComingWork.Text = a[4].ToString();
            NoStatus.Text = a[5].ToString();
        }

        #endregion

        #region Event

        private void btnToday_Click(object sender, RoutedEventArgs e)
        {
            SetDefaultDate();
        }

        private void btnMonthNext_Click(object sender, RoutedEventArgs e)
        {
            datepicker.DisplayDate = datepicker.DisplayDate.AddDays(1);
            datepicker.Text = datepicker.DisplayDate.ToString();
        }

        private void btnMonthPrevious_Click(object sender, RoutedEventArgs e)
        {
            datepicker.DisplayDate = datepicker.DisplayDate.AddDays(-1);
            datepicker.Text = datepicker.DisplayDate.ToString();
        }

        private void work_Deleted(object sender, EventArgs e)
        {
            WorkUC workuc = sender as WorkUC;
            WorkItem workitem = workuc.WorkIT;

            DockPanel pnl = (DockPanel)workuc.Parent;
            ListWork.Children.Remove(pnl);

            Job.Work.Remove(workitem);


            GetStatusFromDate();
        }

        private void work_Edited(object sender, EventArgs e)
        {
            GetStatusFromDate();
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            WorkItem work = new WorkItem() { Date = datepicker.DisplayDate };
            if (Job == null || Job.Work == null)
            {
                Job = new PlanData();
                Job.Work = new List<WorkItem>();
            }

            Job.Work.Add(work);

            AddWork(work);

        }

        private void datepicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectdate = datepicker.SelectedDate;

            if (selectdate != null) datepicker.DisplayDate = selectdate.Value;

            LoadWork(datepicker.DisplayDate);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            MainWindow.data =new PlanData();
            MainWindow.data = Job;
            this.Hide();
        }
        #endregion

        



       

    }
}
