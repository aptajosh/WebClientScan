using CorsEnabledService;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ScanClient.Service;

namespace ScanClient
{
    class ScanClientContext: ApplicationContext
    {
        private NotifyIcon TrayIcon;
        private ContextMenuStrip TrayIconContextMenu;
        private ToolStripMenuItem CloseMenuItem;
        private CorsEnabledService.CorsEnabledServiceHost host;
        private static bool IsInitialized=false;
        public ScanClientContext()
        {

            Application.ApplicationExit += new EventHandler(this.OnApplicationExit);
            if (!Program.IsRunAsAdministrator())
            {
                var processInfo = new ProcessStartInfo(Assembly.GetExecutingAssembly().CodeBase);

                // The following properties run the new process as administrator
                processInfo.UseShellExecute = true;
                processInfo.Verb = "runas";
                var status = false;
                // Start the new process
                try
                {
                    //Process.Start(processInfo);
                    //InitializeComponent();
                    //var epAddress = "http://localhost:1122/ScanService";
                    //Uri baseAddress = new Uri(epAddress);
                    //host = new CorsEnabledServiceHost(typeof(ScannerService), baseAddress);
                    //host.Open();
                    //TrayIcon.Visible = true;
                    //status = true;
                }
                catch (Exception ex)
                {
                    status = false;
                    // The user did not allow the application to run as administrator
                    
                }
                if (!status)
                {
                    if (MessageBox.Show("Kindly close the application and run as Administrator",
                                "Information", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation,
                                MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                    {
                        Application.Exit();
                    }
                }

                
                //Application.Exit();
                // Shut down the current process
                //.Shutdown();
            }
            else
            {
                InitializeComponent();
                var epAddress = "http://localhost:1122/ScanService";
                Uri baseAddress = new Uri(epAddress);
                host = new CorsEnabledServiceHost(typeof(ScannerService), baseAddress);
                host.Open();
                TrayIcon.Visible = true;
            }

            
        }
        private void OnApplicationExit(object sender, EventArgs e)
        {
            //Cleanup so that the icon will be removed when the application is closed
            if(IsInitialized)
                TrayIcon.Visible = false;
            if (host != null)
            {
                if (host.State != System.ServiceModel.CommunicationState.Closed)
                {
                    if (host.State == System.ServiceModel.CommunicationState.Faulted)
                        host.Abort();
                    else
                        host.Close();
                }
            }
           //Application.Exit();

        }
        private void InitializeComponent()
        {
            TrayIcon = new NotifyIcon();
            IsInitialized = true;
            TrayIcon.BalloonTipIcon = ToolTipIcon.Info;
            TrayIcon.BalloonTipText = "I noticed that you double-clicked me! This application can not be invoked directly";
            TrayIcon.BalloonTipTitle = "You called Master?";
            TrayIcon.Text = "Scanner Application";

            //The icon is added to the project resources. Here I assume that the name of the file is 'TrayIcon.ico'
            TrayIcon.Icon = Properties.Resources.favicon;

            //Optional - handle doubleclicks on the icon:
            TrayIcon.DoubleClick += TrayIcon_DoubleClick;

            //Optional - Add a context menu to the TrayIcon:
            TrayIconContextMenu = new ContextMenuStrip();
            CloseMenuItem = new ToolStripMenuItem();
            TrayIconContextMenu.SuspendLayout();

            // 
            // TrayIconContextMenu
            // 
            this.TrayIconContextMenu.Items.AddRange(new ToolStripItem[] {
            this.CloseMenuItem});
            this.TrayIconContextMenu.Name = "TrayIconContextMenu";
            this.TrayIconContextMenu.Size = new Size(153, 70);
            // 
            // CloseMenuItem
            // 
            this.CloseMenuItem.Name = "CloseMenuItem";
            this.CloseMenuItem.Size = new Size(152, 22);
            this.CloseMenuItem.Text = "Close the Scanner Application";
            this.CloseMenuItem.Click += new EventHandler(this.CloseMenuItem_Click);

            TrayIconContextMenu.ResumeLayout(false);

            TrayIcon.ContextMenuStrip = TrayIconContextMenu;
        }

        private void TrayIcon_DoubleClick(object sender, EventArgs e)
        {
            //Here you can do stuff if the tray icon is doubleclicked
            TrayIcon.ShowBalloonTip(10000);
            
        }
        private void CloseMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Do you really want to close me?",
                                "Are you sure?", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation,
                                MessageBoxDefaultButton.Button2) == DialogResult.Yes)
            {
                Application.Exit();
            }
        }
    }
}
