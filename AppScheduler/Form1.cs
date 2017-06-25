using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AppScheduler
{
    public partial class Form1 : Form
    {
        List<ScheduleItem> scheduleItems = new List<ScheduleItem>();
        bool starttimeSelected = false;
        bool endtimeSelected = false;
        bool daySelected = false;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
        // https://stackoverflow.com/questions/17121313/c-sharp-starting-a-exe-file-from-current-file-path-folder-exe-file
        private void startProgram(ScheduleItem item)
        {
            System.Diagnostics.Process.Start(@item.getProgramPath());
        }
        private void Schedule_Monitor()
        {
            for (int i = 0; i < scheduleItems.Count; i++)
            {
                DateTime curDay = new DateTime();
                if (scheduleItems[i].curDayValid(curDay))
                {
                    TimeSpan start = new TimeSpan(scheduleItems[i].getStartHour()
                        , scheduleItems[i].getStartMinute()
                        , scheduleItems[i].getStartSecond());
                    TimeSpan end = new TimeSpan(scheduleItems[i].getEndHour()
                        , scheduleItems[i].getEndMinute()
                        , scheduleItems[i].getEndSecond());
                    TimeSpan now = DateTime.Now.TimeOfDay;

                    if ((now > start) && (now < end))
                    {
                        //match found
                        bool isRunning = System.Diagnostics.Process
                            .GetProcessesByName(scheduleItems[i].GetName())
                            .FirstOrDefault(p => p.MainModule.FileName
                            .StartsWith(scheduleItems[i].getProgramPath())) != default(System
                            .Diagnostics.Process);
                        if (!isRunning)
                        {
                            startProgram(scheduleItems[i]);
                        }
                        else
                        {
                            System.Diagnostics.Process theProcess = System.Diagnostics.Process.GetProcessesByName(scheduleItems[i].GetName())
                                .FirstOrDefault(p => p.MainModule.FileName
                                .StartsWith(scheduleItems[i].getProgramPath()));
                            WindowHelper.BringProcessToFront(theProcess);
                        }
                    }
                }
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.WindowState = FormWindowState.Minimized;
            if (scheduleItems.Count > 0)
            {
                Init_Timer();
            }
            //MinimizeToTray();
        }

        private void Init_Timer()
        {
            Timer timer1 = new Timer();
            timer1.Tick += new EventHandler(timer1_Tick);
            timer1.Interval = 2000; // in milliseconds (2 seconds)
            timer1.Start();
        }
        
        private void timer1_Tick(object sender, EventArgs e)
        {
            //MessageBox.Show("test");
            Schedule_Monitor();
        }

        private string scheduleItemStringBuilder(string programName, ScheduleItem item)
        {
            string time = " / ";
            if (starttimeSelected && endtimeSelected)
            {
                time = time + dateTimePicker1.Value.Hour.ToString() + ":" 
                    + dateTimePicker1.Value.Minute.ToString();
                time = time + " to ";
                time = time + dateTimePicker2.Value.Hour.ToString() + ":"
                    + dateTimePicker2.Value.Minute.ToString();
            }
            string days = " / ";
            if (daySelected)
            {
                string[] selectedDays = new string[chklstbxDays.CheckedItems.Count];
                CheckedListBox.CheckedItemCollection items = chklstbxDays.CheckedItems;
                for (int i = 0; i < items.Count; i++)
                {
                    days = days + items[i] + "|";
                    selectedDays[i] = items[i].ToString();
                }
                item.setSelectedDays(selectedDays);
            }
            string outputString = programName + time + days;
            return outputString;
        }

        private ScheduleItem newScheduleItem(string fileP, string fileN)
        {
            bool isURL = false;
            if (fileN == "chrome" || fileN == "firefox")
            {
                isURL = true;
            }
            string startTime = "";
            string endTime = "";
            if (starttimeSelected && endtimeSelected)
            {
                startTime = startTime + dateTimePicker1.Value.Hour.ToString() + ":"
                    + dateTimePicker1.Value.Minute.ToString() + ":" 
                    + dateTimePicker1.Value.Second.ToString();
                endTime = endTime + dateTimePicker2.Value.Hour.ToString() + ":"
                    + dateTimePicker2.Value.Minute.ToString() + ":"
                    + dateTimePicker2.Value.Second.ToString();
            }
            ScheduleItem newScheduleItem = new ScheduleItem(isURL, fileP, fileN, startTime, endTime);
            return newScheduleItem;
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            DialogResult result = openFileDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                char[] delimitedChars = { ' ', ',', '\\', '.', '\t' };
                string file = openFileDialog1.FileName;
                String[] pathSplit = file.Split(delimitedChars);
                string fileN = pathSplit.GetValue(pathSplit.Length - 2).ToString();
                ScheduleItem newItem = newScheduleItem(file, fileN);
                string scheduleString = scheduleItemStringBuilder(fileN, newItem);
                lstbxScheduled.Items.Add(scheduleString);
                scheduleItems.Add(newItem);
            }
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            starttimeSelected = true;
        }

        private void dateTimePicker2_ValueChanged(object sender, EventArgs e)
        {
            endtimeSelected = true;
        }

        private void chklstbxDays_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (chklstbxDays.CheckedItems.Count > 0)
            {
                daySelected = true;
            }
        }
    }

    public static class WindowHelper
    {
        public static void BringProcessToFront(System.Diagnostics.Process process)
        {
            IntPtr handle = process.MainWindowHandle;
            if (IsIconic(handle))
            {
                ShowWindow(handle, SW_RESTORE);
            }

            SetForegroundWindow(handle);
        }

        const int SW_RESTORE = 9;

        [System.Runtime.InteropServices.DllImport("User32.dll")]
        private static extern bool SetForegroundWindow(IntPtr handle);
        [System.Runtime.InteropServices.DllImport("User32.dll")]
        private static extern bool ShowWindow(IntPtr handle, int nCmdShow);
        [System.Runtime.InteropServices.DllImport("User32.dll")]
        private static extern bool IsIconic(IntPtr handle);
    }
}
