using System;
using System.Collections.Generic;
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
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace EmbeddSSHShell
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        const int WS_BORDER = 8388608;
        const int WS_DLGFRAME = 4194304;
        const int WS_CAPTION = WS_BORDER | WS_DLGFRAME;
        const int WS_SYSMENU = 524288;
        const int WS_THICKFRAME = 262144;
        const int WS_MINIMIZE = 536870912;
        const int WS_MAXIMIZEBOX = 65536;
        const int GWL_STYLE = -16;
        const int GWL_EXSTYLE = -20;
        const int WS_EX_DLGMODALFRAME = 0x1;
        const int SWP_NOMOVE = 0x2;
        const int SWP_NOSIZE = 0x1;
        const int SWP_FRAMECHANGED = 0x20;
        const uint MF_BYPOSITION = 0x400;
        const uint MF_REMOVE = 0x1000;

        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr SetParent(IntPtr child, IntPtr newParent);

        private const int SW_MAXIMIZE = 3;
        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        // This static method is required because legacy OSes do not support
        // SetWindowLongPtr 
        public static IntPtr SetWindowLongPtr(IntPtr hWnd, int nIndex, IntPtr dwNewLong)
        {
            if (IntPtr.Size == 8)
                return SetWindowLongPtr64(hWnd, nIndex, dwNewLong);
            else
                return new IntPtr(SetWindowLong32(hWnd, nIndex, dwNewLong.ToInt32()));
        }

        [DllImport("user32.dll", EntryPoint = "SetWindowLong")]
        private static extern int SetWindowLong32(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll", EntryPoint = "SetWindowLongPtr")]
        private static extern IntPtr SetWindowLongPtr64(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]
        public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, int uFlags);


        System.Diagnostics.Process cmdProcess = null;
        System.Windows.Forms.Panel panel;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void BtnRun_Click(object sender, RoutedEventArgs e)
        {
            if (cmdProcess == null)
            {
                //Run console app
                string exe = @"cmd.exe";
                var processStartInfo = new System.Diagnostics.ProcessStartInfo(exe);
                processStartInfo.WorkingDirectory = System.IO.Path.GetDirectoryName(exe);
                processStartInfo.WindowStyle = ProcessWindowStyle.Minimized;
                cmdProcess = Process.Start(processStartInfo);

                panel = new System.Windows.Forms.Panel();


                panel.BackColor = System.Drawing.Color.Green;

                winFormsHost.Child = panel;

                /* Adds the event and the event handler for the method that will 
               process the timer event to the timer. */
                myTimer.Tick += new EventHandler(TimerEventProcessor);

                // Sets the timer interval to 5 seconds.
                myTimer.Interval = 1000;
                myTimer.Start();
            }
            else
            {
                
                //SetParent(cmdProcess.MainWindowHandle, new IntPtr(0));
            }
        }

        [DllImport("user32.dll", SetLastError = true)]
        internal static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);

        private void MyPanel_SizeChanged(object sender, System.EventArgs e)
        {
            MoveWindow(cmdProcess.MainWindowHandle, 0, 0, panel.Width, panel.Height, true);

            //SetParent(cmdProcess.MainWindowHandle, panel.Handle);
            //ShowWindow(cmdProcess.MainWindowHandle, SW_MAXIMIZE);
            //MakeExternalWindowBorderless(cmdProcess.MainWindowHandle);
        }

        [DllImport("user32.dll", EntryPoint = "GetWindowLong")]
        private static extern IntPtr GetWindowLongPtr32(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll", EntryPoint = "GetWindowLongPtr")]
        private static extern IntPtr GetWindowLongPtr64(IntPtr hWnd, int nIndex);

        // This static method is required because Win32 does not support
        // GetWindowLongPtr directly
        public static int GetWindowLongPtr(IntPtr hWnd, int nIndex)
        {
            if (IntPtr.Size == 8)
                return (int)GetWindowLongPtr64(hWnd, nIndex);
            else
                return (int)GetWindowLongPtr32(hWnd, nIndex);
        }

        public void MakeExternalWindowBorderless(IntPtr MainWindowHandle)
        {
            int Style = 0;
            Style = GetWindowLongPtr(MainWindowHandle, GWL_STYLE);
            Style = Style & ~WS_CAPTION;
            Style = Style & ~WS_SYSMENU;
            Style = Style & ~WS_THICKFRAME;
            Style = Style & ~WS_MINIMIZE;
            Style = Style & ~WS_MAXIMIZEBOX;
            SetWindowLongPtr(MainWindowHandle, GWL_STYLE, (IntPtr)Style);
            Style = GetWindowLongPtr(MainWindowHandle, GWL_EXSTYLE);
            SetWindowLongPtr(MainWindowHandle, GWL_EXSTYLE, (IntPtr)(Style | WS_EX_DLGMODALFRAME));
            //SetWindowPos(MainWindowHandle, panel.Handle, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE | SWP_FRAMECHANGED);

            MoveWindow(cmdProcess.MainWindowHandle, 0, 0, panel.Width, panel.Height, true);
        }

        private void Embedding()
        {
            SetParent(cmdProcess.MainWindowHandle, panel.Handle);
            //ShowWindow(cmdProcess.MainWindowHandle, SW_MAXIMIZE);
            MakeExternalWindowBorderless(cmdProcess.MainWindowHandle);
        }

        private void BtnSet_Click(object sender, RoutedEventArgs e)
        {
            Embedding();
        }

        static System.Windows.Forms.Timer myTimer = new System.Windows.Forms.Timer();
        
        // This is the method to run when the timer is raised.
        private void TimerEventProcessor(Object myObject,
                                                EventArgs myEventArgs)
        {
            myTimer.Stop();
            myTimer.Enabled = false;

            Embedding();

            panel.SizeChanged += new EventHandler(this.MyPanel_SizeChanged);
        }

    }
}
