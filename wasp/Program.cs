using System;
using System.Windows.Forms;

namespace Wasp {
    class Program {
        [STAThread]
        static void Main() {
            Program program = new Program();
            program.execute();
        }

        private ApplicationContext appCtx;

        public void execute() {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            this.appCtx = new ApplicationContext();

            TopForm form = new TopForm();
            form.Visible = false;
            form.HandleDestroyed += new EventHandler(CustomHandleDestroyed);
            ApplicationDesktopToolbar toolbar = new ApplicationDesktopToolbar(
                form, ApplicationDesktopToolbar.AppBarEdges.Top);
            toolbar.AutoHide = true;

            Application.Run(this.appCtx);
        }

        private void CustomHandleDestroyed(Object sender, EventArgs e) {
            this.appCtx.ExitThread();
        }
    }
}