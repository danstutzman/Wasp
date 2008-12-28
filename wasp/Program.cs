using System;
using System.Windows.Forms;

namespace Wasp {
    class Program {
        [STAThread]
        static void Main() {
            Program program = new Program();
            program.execute();
        }

        public void execute() {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            ApplicationContext appCtx = new ApplicationContext();

            TopModel model = new TopModel();
            TopController topController = new TopController(model);
            topController.FormDestroyed += delegate(Object sender, EventArgs args) {
                appCtx.ExitThread();
            };
            topController.Show();
            model.InitTimer();

            Application.Run(appCtx);
        }
    }
}