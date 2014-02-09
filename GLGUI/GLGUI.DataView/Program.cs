using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace GLGUI.DataView
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            System.Windows.Forms.Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            System.Windows.Forms.Application.ThreadException += new System.Threading.ThreadExceptionEventHandler(OnGuiUnhandedException);
            AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }

        private static void HandleUnhandledException(Object o)
        {
            Exception e = o as Exception;

            if (e != null)
            {
                MessageBox.Show(e.ToString());
            }
        }

        private static void OnUnhandledException(Object sender, UnhandledExceptionEventArgs e)
        {
            HandleUnhandledException(e.ExceptionObject);
        }

        private static void OnGuiUnhandedException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            HandleUnhandledException(e.Exception);
        }
    }
}
