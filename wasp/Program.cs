using System;
using System.Windows.Forms;

[assembly: log4net.Config.XmlConfigurator(Watch = true, ConfigFile = "log4net.config")]

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
            model.InitTimer();

            topController.Show();
            Application.Run(appCtx);
        }
    }
}