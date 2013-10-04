using System;
using System.Threading;
using System.Windows.Forms;
using TaskLeader.GUI;
using TaskLeader.BLL;
using System.ServiceModel.Web; // Nécessite le .NET framework 4.0 (pas client profile)
using TaskLeader.Server;

namespace TaskLeader
{
    static class Program
    {
        /// <summary>
        /// Point d'entrée principal de l'application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            using (Mutex mutex = new Mutex(false, @"Global\6fd1e7cc-9bb5-4154-9798-a36e6239d34d"))
            {
                if (!mutex.WaitOne(0, false))
                {
                    MessageBox.Show("Application déjà lancée", "TaskLeader");
                    return;
                }

                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);

                //Hook d'outlook si possible
                OutlookIF.Instance.tryHook(false);

                //Lancement du serveur d'IHM
                try
                {
                    Uri baseAddress = new Uri("http://localhost:80/Temporary_Listen_Addresses/");
                    WebServiceHost host = new WebServiceHost(typeof(GuiService), baseAddress);
                    host.Open();
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message,"Erreur");
                }

                //Affichage de la TrayIcon
                Application.Run(new TrayIcon());
            }
        }
    }
}
