using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Serialization;
using System.Windows.Threading;
using Microsoft.Win32;
using System.Runtime.InteropServices;
using System.Diagnostics;
using KAutoHelper;

namespace Calendar
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
      

        private bool KTON = false;
        public MainWindow()
        {
            InitializeComponent();

            LoginRegistryKey();

            LoadWindowFirst();

            TakeScreenFirst();
        }

        #region khởi tạo phần tử

        public void LoginRegistryKey()
        {
            RegistryKey regkey = Registry.CurrentUser.CreateSubKey("Software\\Calendar");
            //mo registry khoi dong cung win
            RegistryKey regstart = Registry.CurrentUser.CreateSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run");
            string keyvalue = "1";
            //string subkey = "Software\\ManhQuyen";
            try
            {
                //chen gia tri key
                regkey.SetValue("Index", keyvalue);
                //regstart.SetValue("taoregistrytronghethong", "E:\\Studing\\Bai Tap\\CSharp\\Channel 4\\bai temp\\tao registry trong he thong\\tao registry trong he thong\\bin\\Debug\\tao registry trong he thong.exe");
                regstart.SetValue("Calendar", System.IO.Directory.GetCurrentDirectory() + @"\Calendar.exe");
                ////dong tien trinh ghi key
                regkey.Close();
            }
            catch (System.Exception ex)
            {
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string newName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(newName));
            }
        }

        DispatcherTimer timer = new DispatcherTimer();
        System.Windows.Forms.NotifyIcon notifyIcon = new System.Windows.Forms.NotifyIcon();
        #endregion

        #region Biến
        private List<List<Button>> matrixDate;

        public List<List<Button>> MatrixDate
        {
            get => matrixDate;
            set => matrixDate = value;
        }


        private List<string> DateOfWeek = new List<string>() { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday" };

        List<int> ListMinute = new List<int>();

        public List<int> ListMin { get => ListMinute; set => ListMinute = value; }

        private PlanData job;
        public PlanData Job { get => job; set => job = value; }

        public static PlanData data;

        string FilePath = "data.xml";
        Button TodayButton = new Button();

        #endregion

        #region LoadFromXML
        private void SerializeFromXML(object data, string FilePath)
        {
            FileStream fs = new FileStream(FilePath, FileMode.Create, FileAccess.Write);
            XmlSerializer sr = new XmlSerializer(typeof(PlanData));

            if (data == null && KTON == false) MessageBox.Show("Không có công việc nào! :)");

            sr.Serialize(fs, data);

            fs.Close();
        }

        private object DeSerializeFromXML(string FilePath)
        {
            FileStream fs = new FileStream(FilePath, FileMode.Open, FileAccess.Read);
            try
            {
                XmlSerializer sr = new XmlSerializer(typeof(PlanData));


                object result = sr.Deserialize(fs);
                fs.Close();

                return result;
            }
            catch (Exception e)
            {
                fs.Close();
                System.Windows.MessageBox.Show(e.ToString());
                throw new NotImplementedException();
            }

        }
        #endregion

        #region Method
        void TakeScreenFirst()
        {
            IntPtr hWnd = IntPtr.Zero;

            hWnd = Process.GetCurrentProcess().MainWindowHandle;

            AutoControl.BringToFront(hWnd);
        }

        void LoadWindowFirst()
        {
            STT();
            LoadTableDate();

            try
            {
                Job = (PlanData)DeSerializeFromXML(FilePath);
                if (Job != null && Job.Work != null) data = Job;
                DateWork();
            }
            catch
            {
            }
            //SetDefaltJob();

            timer = new DispatcherTimer();
            notifyIcon = new System.Windows.Forms.NotifyIcon();
            notifyIcon.Visible = false;
            notifyIcon.BalloonTipClicked += NotifyIcon_BalloonTipClicked;

            this.notifyIcon.Icon = new System.Drawing.Icon("icon\\calender.ico");
            notifyIcon.MouseDoubleClick += notifyIcon_mouseDoubleClick;

            System.Windows.Forms.ContextMenuStrip menu = new System.Windows.Forms.ContextMenuStrip();
            System.Windows.Forms.ToolStripMenuItem Open = new System.Windows.Forms.ToolStripMenuItem("Mở",System.Drawing.Image.FromFile("icon\\Show.ico"),Show_Click);
            System.Windows.Forms.ToolStripMenuItem Closed = new System.Windows.Forms.ToolStripMenuItem("Đóng", System.Drawing.Image.FromFile("icon\\Stop.ico"), Close_Click);



            menu.Items.Add(Open);
            menu.Items.Add(Closed);

            notifyIcon.ContextMenuStrip = menu;

            timer.Tick += timer_Tick;
            timer.Interval = new TimeSpan(0, 1, 0);
        }


        void STT()
        {
            for (int i = 1; i <= 3600; i++)
                ListMinute.Add(i);
            Minute.ItemsSource = ListMinute;
        }

        void Adddate(DateTime date)
        {
            ClearTableDate();
            DateTime useDate = new DateTime(date.Year, date.Month, 1);


            int line = 0;

            for (int i = 1; i <= DaysInMonth﻿(date.Month, date.Year); i++)
            {
                int column = DateOfWeek.IndexOf(useDate.DayOfWeek.ToString());

                Button btn = MatrixDate[line][column];
                btn.Content = i.ToString();

                btn.Tag = i;
                if (IsDateSame(useDate, DateTime.Now))
                {
                    btn.Background = new SolidColorBrush(Color.FromRgb(255, 255, 0));
                    btn.BorderBrush = btn.Background;
                    TodayButton.Click += Btn_Click;
                    TodayButton.Tag = i;
                }

                if (IsDateSame(useDate, DateTime.Now) == false && IsDateSame(useDate, datepicker.DisplayDate) == true)
                {
                    btn.Background = new SolidColorBrush(Color.FromRgb(255, 0, 0));
                    btn.BorderBrush = btn.Background;
                }

                if (column == 6) line++;

                useDate = useDate.AddDays(1);
            }

            for (int i = 0; i < Cons.DayOfWeek - 1; i++)
            {
                for (int j = 0; j <= Cons.DayOfColumn; j++)
                {
                    Button btn = MatrixDate[i][j];
                    if (btn.Content == "")
                    { btn.IsEnabled = false; }
                }
            }

            DateWork();
        }


        void LoadTableDate()
        {
            MatrixDate = new List<List<Button>>();

            for (int i = 0; i < Cons.DayOfWeek - 1; i++)
            {
                MatrixDate.Add(new List<Button>());

                for (int j = 0; j <= Cons.DayOfColumn; j++)
                {
                    Button btn = new Button();

                    btn.Width = Cons.ButtonWidth;
                    btn.Height = Cons.ButtonHeight;
                    btn.Margin = new Thickness(Cons.ButtonMargin);
                    btn.Tag = null;
                    btn.FontSize = 16;
                    btn.Click += Btn_Click;
                    date.Children.Add(btn);

                    MatrixDate[i].Add(btn);

                    //Button btn1 = MatrixDate[i][j];
                    //btn1.Content = i.ToString() + " " + j.ToString();

                }
            }


            SetDefaultDate();
            Adddate(datepicker.DisplayDate);

        }

        void ClearTableDate()
        {
            for (int i = 0; i < Cons.DayOfWeek - 1; i++)
            {
                for (int j = 0; j <= Cons.DayOfColumn; j++)
                {
                    Button btn = MatrixDate[i][j];
                    btn.Content = "";

                    btn.Tag = null;
                    btn.IsEnabled = true;
                    btn.Background = new SolidColorBrush(Color.FromRgb(103, 58, 183));
                    btn.BorderBrush = btn.Background;
                    //MessageBox.Show(btn.Background.ToString());
                }
            }
        }

        void DateWork()
        {
            for (int i = 0; i < Cons.DayOfWeek - 1; i++)
            {
                for (int j = 0; j <= Cons.DayOfColumn; j++)
                {
                    Button btn = MatrixDate[i][j];
                    if (btn.IsEnabled == true && GetJobByDate(new DateTime(datepicker.DisplayDate.Year, datepicker.DisplayDate.Month, Convert.ToInt32(btn.Tag))))
                    {
                        btn.Foreground = new SolidColorBrush(Color.FromRgb(4, 249, 4));
                        btn.FontWeight = FontWeights.Bold;
                    }
                    else
                    {
                        btn.Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255));
                        btn.FontWeight = FontWeights.Normal;
                    }
                    //MessageBox.Show(btn.Background.ToString());
                }
            }
        }

        private int DaysInMonth(int month, int year)
        {
            /* 1 - 3 - 5 - 7 - 8 - 10 - 12 có 31 ngày
             4 - 6 - 9 - 11 có 30 ngày
             tháng 2 thì 28 hoặc 29.*/

            switch (month)
            {
                case 1:
                case 3:
                case 5:
                case 7:
                case 8:
                case 10:
                case 12:
                    return 31;
                case 2:
                    if ((year % 4 == 0 && year % 100 != 0) || year % 400 == 0)
                        return 29;
                    else return 28;

                default: return 30;
            }
        }

        bool IsDateSame(DateTime date1, DateTime date2)
        {
            return date1.Date == date2.Date && date1.Month == date2.Month && date1.Year == date2.Year;
        }

        void SetDefaultDate()
        {
            datepicker.DisplayDate = DateTime.Now;

            datepicker.Text = datepicker.DisplayDate.ToString();
        }

        int CountJobByDate(DateTime date)
        {
            if (Job == null || Job.Work == null) return -1;
            var result = Job.Work.Where(p => p.Date.Year == date.Year && p.Date.Month == date.Month && p.Date.Day == date.Day);
            if (result != null)
            {
                return result.ToList().Count();
            }
            else return -1;
        }

        bool GetJobByDate(DateTime date)
        {
            if (Job == null || Job.Work == null) return false;
            var result = Job.Work.Where(p => p.Date.Year == date.Year && p.Date.Month == date.Month && p.Date.Day == date.Day);
            if (result != null)
            {
                return result.ToList().Count() > 0;
            }
            else return false;
        }
        
        #endregion

        #region Event

        void notifyIcon_mouseDoubleClick(object sender, EventArgs e)
        {
            TakeScreenFirst();
            this.Visibility = Visibility.Visible;
            this.WindowState = WindowState.Normal;
            if (chkNotify.IsChecked == false) notifyIcon.Visible = false; else notifyIcon.Visible = true;
            this.ShowInTaskbar = true;
        }


        void Btn_Click(object sender, RoutedEventArgs e)
        {

            datepicker.DisplayDate = new DateTime(datepicker.DisplayDate.Year, datepicker.DisplayDate.Month, Convert.ToInt32((sender as Button).Tag));
            datepicker.Text = datepicker.DisplayDate.ToString();
            Plan plandate = new Plan(new DateTime(datepicker.DisplayDate.Year, datepicker.DisplayDate.Month, Convert.ToInt32((sender as Button).Tag)),Job);
            plandate.ShowDialog();
            Job = data;
            DateWork();
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            string tooltiptext;
            balloontipclick = true;
            if (CountJobByDate(DateTime.Now) > 0)
                tooltiptext = "Bạn có " + CountJobByDate(DateTime.Now) + " công việc ngày hôm nay";
            else tooltiptext = "Hôm Nay là ngày rãnh của bạn";

            notifyIcon.ShowBalloonTip(Cons.NotifyTimeOut, "Thông Báo", tooltiptext, System.Windows.Forms.ToolTipIcon.Info);

            notifyIcon.BalloonTipClicked += NotifyIcon_BalloonTipClicked;
        }

        bool balloontipclick = true;
        private void NotifyIcon_BalloonTipClicked(object sender, EventArgs e)
        {
            if (balloontipclick == true && CountJobByDate(DateTime.Now) >= 1)
            {
                TakeScreenFirst();
                this.Visibility = Visibility.Visible;
                this.WindowState = WindowState.Normal;

                Btn_Click(TodayButton, new RoutedEventArgs());
                balloontipclick = false;
            }
            
        }

        private void datepicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectdate = datepicker.SelectedDate;

            if (selectdate != null) datepicker.DisplayDate = selectdate.Value;

            Adddate((sender as DatePicker).DisplayDate);
        }

        private void btnToday_Click(object sender, RoutedEventArgs e)
        {
            SetDefaultDate();
        }

        private void btnMonthNext_Click(object sender, RoutedEventArgs e)
        {
            datepicker.DisplayDate =  datepicker.DisplayDate.AddMonths(1);
            datepicker.Text = datepicker.DisplayDate.ToString();
        }

        private void btnMonthPrevious_Click(object sender, RoutedEventArgs e)
        {
            datepicker.DisplayDate = datepicker.DisplayDate.AddMonths(-1) ;
            datepicker.Text = datepicker.DisplayDate.ToString();
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
           
            if (KTON == false)
            {
                 e.Cancel = true;
                Job = data;
                SerializeFromXML(Job, FilePath);
                this.ShowInTaskbar = false;
                notifyIcon.Visible = true;
                this.Hide();
            }


        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            Minute.IsEnabled = true;
            if (Minute.SelectedIndex == -1) return;
            notifyIcon.Visible = true;
            if (chkNotify.IsChecked == true) timer.Start();



        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            timer.Stop();
            Minute.IsEnabled = false;
            notifyIcon.Visible = false;
        }

        private void Minute_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            timer.Stop();
            timer.Interval = new TimeSpan(0, Minute.SelectedIndex+1, 0);
            if (chkNotify.IsChecked == true) timer.Start();
        }

        #endregion

        private void Window_StateChanged(object sender, EventArgs e)
        {
            TakeScreenFirst();
            notifyIcon.Visible = false;
            /*if (this.WindowState == WindowState.Minimized)
            {
                this.ShowInTaskbar = false;
                notifyIcon.Visible = true;
            }
            else if (this.WindowState == WindowState.Normal)
            {
                
                this.ShowInTaskbar = true;
            }*/
        }

        private void Show_Click(object sender, EventArgs e)
        {
            this.Visibility = Visibility.Visible;
            this.WindowState = WindowState.Normal;
            if (chkNotify.IsChecked == false) notifyIcon.Visible = false; else notifyIcon.Visible = true;
            this.ShowInTaskbar = true;
            TakeScreenFirst();
        }

        private void Close_Click(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

    }
}
