using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace WindowsFormsApplication1
{
    static class Program
    {
        static TopForm form;
        static ApplicationContext appCtx;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            appCtx = new ApplicationContext();

            form = new TopForm();
            form.Visible = false;
            form.HandleDestroyed += new EventHandler(CustomHandleDestroyed);
            ApplicationDesktopToolbar toolbar = new ApplicationDesktopToolbar(
                form, ApplicationDesktopToolbar.AppBarEdges.Top);
            toolbar.AutoHide = true;

            //Timer timer = new Timer();
            //timer.Interval = 3000;
            //timer.Tick += new EventHandler(Timer_Tick);
            //timer.Start();
            Application.Run(appCtx);
        }

        //static void Timer_Tick(object sender, EventArgs e)
        //{
        //    Console.Error.WriteLine("setting form visible TO TRUE");
        //    form.Visible = true;
        //}


        static private void CustomHandleDestroyed(Object sender, EventArgs e)
        {
            Console.Error.WriteLine("handledestroyed");
            appCtx.ExitThread();
        }
    }
}