using System;
using System.Windows.Forms;

namespace GLGUI.Example
{
	public static class Program
	{
        [STAThread]
		public static int Main(string[] args)
		{
            System.Windows.Forms.Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            System.Windows.Forms.Application.ThreadException += new System.Threading.ThreadExceptionEventHandler(OnGuiUnhandedException);
            AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;

			Application.Run(new MainForm());
			return 0;
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

