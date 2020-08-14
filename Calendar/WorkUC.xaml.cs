using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Calendar
{
    /// <summary>
    /// Interaction logic for WorkUC.xaml
    /// </summary>
    public partial class WorkUC : UserControl
    {
        public WorkUC(WorkItem work)
        {
            InitializeComponent();

            
            WorkIT = work;

            txtNameWork.Text = work.Work;

            fromtime.SelectedTime = new DateTime(work.Date.Year, work.Date.Month, work.Date.Day, (int)work.FromTime.X, (int)work.FromTime.Y, 00);

            fromtime.Text = fromtime.SelectedTime.Value.ToShortTimeString();

            totime.SelectedTime = new DateTime(work.Date.Year, work.Date.Month, work.Date.Day, (int)work.EndTime.X, (int)work.EndTime.Y, 00);
           
            Status.SelectedIndex = WorkItem.listStatus.IndexOf(work.Status);

            chkstatus.IsChecked = WorkItem.listStatus.IndexOf(work.Status) == (int)EWorkItem.Done ? true : false;

            btnEdit.IsEnabled = false;

        }

        private WorkItem workIT;
        public WorkItem WorkIT { get => workIT; set => workIT = value; }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string newName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(newName));
            }
        }


        private event EventHandler edited;
        public event EventHandler Edited
        {
            add { edited += value; }
            remove { edited -= value; }
        }

        private event EventHandler deleted;
        public event EventHandler Deleted
        {
            add { deleted += value; }
            remove { deleted -= value; }
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            workIT.Work = txtNameWork.Text;

            workIT.FromTime = new Point(fromtime.SelectedTime.Value.Hour,fromtime.SelectedTime.Value.Minute);
            workIT.EndTime = new Point(totime.SelectedTime.Value.Hour, totime.SelectedTime.Value.Minute);

            workIT.Status = Status.Text;

            if (edited != null)
            {
                edited(this, new EventArgs());
            }

            btnEdit.IsEnabled = false;
        }

        private void btnDel_Click(object sender, RoutedEventArgs e)
        {
            if (deleted != null)
            {
                deleted(this, new EventArgs());
            }
        }

        private void chkstatus_Checked(object sender, RoutedEventArgs e)
        {
            Status.SelectedIndex = (int)EWorkItem.Done;  
        }

        private void chkstatus_Unchecked(object sender, RoutedEventArgs e)
        {
            Status.SelectedIndex = (int)EWorkItem.Doing;
        }

        private void Status_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Status.SelectedIndex == (int)EWorkItem.Done) chkstatus.IsChecked = true; else chkstatus.IsChecked = false;
            if (okstatus) okstatus = false; else btnEdit.IsEnabled = true;
        }

        private void txtNameWork_TextChanged(object sender, TextChangedEventArgs e)
        {
            btnEdit.IsEnabled = true;
        }

        bool oktime = true; bool okstatus = true; bool oktime1 = true;
        private void time_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (oktime) oktime = false; else btnEdit.IsEnabled = true;
        }

        bool tg(DateTime? time1, DateTime? time2)
        {
            if (time1 == null || time2 == null) return false;

            string timea = time1.Value.ToShortTimeString();
            string  timeb = time2.Value.ToShortTimeString();

            if (DateTime.Compare(DateTime.Parse(timea),DateTime.Parse(timeb)) > 0)
            {
                return true;
            }

            return false;
        }

        private void time2_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (tg(fromtime.SelectedTime.Value, totime.SelectedTime.Value) == false)
            {
                totime.SelectedTime = new DateTime(WorkIT.Date.Year, WorkIT.Date.Month, WorkIT.Date.Day, fromtime.SelectedTime.Value.Hour, totime.SelectedTime.Value.Minute, 00);
            }

            if (oktime1) oktime1 = false; else btnEdit.IsEnabled = true;
        }

    }
}
