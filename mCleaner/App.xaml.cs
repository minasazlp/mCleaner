﻿using mCleaner.Helpers;
using mCleaner.Logics.Clam;
using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows;

namespace mCleaner
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        //public static string testcleaner = mCleaner.Properties.Resources.testcleaner;
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // force one instance of this application
            bool force_terminate = false;
            Process curr = Process.GetCurrentProcess();
            Process[] procs = Process.GetProcessesByName(curr.ProcessName);
            foreach (Process p in procs)
            {
                if ((p.Id != curr.Id) && (p.MainModule.FileName == curr.MainModule.FileName))
                {
                    force_terminate = true;
                    curr = p;
                    break;
                }
            }
            if (force_terminate)
            {
                MessageBox.Show("mCleaner is already running", "mCleaner", MessageBoxButton.OK, MessageBoxImage.Information);
                SetForegroundWindow(curr.MainWindowHandle);
                Process.GetCurrentProcess().Kill();
            }

            // check permission
            if (!Permissions.IsUserAdministrator)
            {
                MessageBox.Show("You must be an administrator to run this program", "mCleaner", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                Process.GetCurrentProcess().Kill();
                return;
            }

            // Enable needed privileges
            Permissions.SetPrivileges(true);

            Version version = Assembly.GetEntryAssembly().GetName().Version;
            string s_version = version.ToString();

            if (s_version != mCleaner.Properties.Settings.Default.Version)
            {
                mCleaner.Properties.Settings.Default.Upgrade();
                mCleaner.Properties.Settings.Default.Version = s_version;
                mCleaner.Properties.Settings.Default.Save();
            }

            // check for clamwin installation and
            // decide which database to use
            CommandLogic_Clam.I.CheckClamWinInstallation();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);

            // Disable needed privileges
            Permissions.SetPrivileges(false);
        }
    }
}
