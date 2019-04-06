using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace BranksMod
{
    public partial class MainFrm : Form
    {
        string Time = DateTime.Now.ToString("[HH:mm:ss] ");
        string LogPath = Properties.Settings.Default.FolderPath + "\\bakkesmod\\branksmod.log";
        Color ThemeBackground = Color.FromArgb(0, 0, 0);
        Color ThemeHighlight = Color.FromArgb(0, 0, 0);
        Color ThemeFontColor = Color.Black;
        Boolean IsInjected = false;

        public MainFrm()
        {
            InitializeComponent();
        }

        private void MainFrm_Load(object sender, EventArgs e)
        {
            GetFolderPath();
            CheckVersion();
            CheckInstall();
            CheckAutoUpdates();
            //CheckSafeMode();
            CheckWarnings();
            CheckTopmost();
            CheckInstall();
            CheckTheme();
            ProcessTmr.Start();
        }

        private void MainFrm_Resize(object sender, EventArgs e)
        {
            CheckMiniHide();
        }

        private void ProcessTmr_Tick(object sender, EventArgs e)
        {
            Process[] Name = Process.GetProcessesByName("RocketLeague");
            if (Name.Length == 0)
            {
                RLLbl.Text = "Rocket League is not running.";
                StatusLbl.Text = "Status: Uninjected.";
            }
            else
            {
                RLLbl.Text = "Rocket League is running.";
                if (Properties.Settings.Default.AutoInject == true)
                {
                    StatusLbl.Text = "Status: AutoInjector Enabled.";
                }
                else if (Properties.Settings.Default.AutoInject == false)
                {
                    InjectionTmr.Interval = Properties.Settings.Default.Timeout;
                    InjectionTmr.Start();
                }
            }
        }

        private void InjectionTmr_Tick(object sender, EventArgs e)
        {
            if (IsInjected == false)
            {
                if (Properties.Settings.Default.InjectionType == "Timeout")
                {
                    InjectBtn.Visible = false;
                    InjectionTmr.Stop();
                    InjectDLL();
                }
                else if (Properties.Settings.Default.InjectionType == "Manual")
                {
                    InjectionTmr.Stop();
                    InjectBtn.Visible = true;
                }
            } else
            {

            }
        }

        void InjectDLL()
        {
            Controller.WriteToLog(LogPath, Time + "[InjectDLL] Attempting injection.");
            InjectionResult Result = Injector.Instance.Inject("RocketLeague", Properties.Settings.Default.FolderPath + "\\BakkesMod\\bakkesmod.dll");
            switch (Result)
            {
                case InjectionResult.DLL_NOT_FOUND:
                    Controller.WriteToLog(LogPath, Time + "[InjectDLL] Could Not Find DLL File.");
                    StatusLbl.Text = "Status: Could Not Find DLL File.";
                    break;
                case InjectionResult.GAME_PROCESS_NOT_FOUND:
                    Controller.WriteToLog(LogPath, Time + "[InjectDLL] Uninjected, Rocket League is not running.");
                    StatusLbl.Text = "Status: Uninjected.";
                    break;
                case InjectionResult.INJECTION_FAILED:
                    Controller.WriteToLog(LogPath, Time + "[InjectDLL] Injection Failed.");
                    StatusLbl.Text = "Status: Injection Failed.";
                    break;
                case InjectionResult.SUCCESS:
                    Controller.WriteToLog(LogPath, Time + "[InjectDLL] Successfully Injected.");
                    StatusLbl.Text = "Status: Successfully Injected.";
                    IsInjected = true;
                    break;
            }
        }

        #region "Installers & Updaters"
        public static string HttpDownloader(String URL, String Pattern, String Contents)
        {
            string Return = "";
            string Download = "";

            HttpWebRequest Request = (HttpWebRequest)WebRequest.Create(URL);
            HttpWebResponse Response = (HttpWebResponse)Request.GetResponse();
            StreamReader SR = new StreamReader(Response.GetResponseStream());
            Download = SR.ReadToEnd();
            SR.Close();

            if (Download.Contains(Contents))
            {
                Return = Regex.Match(Download, Pattern, RegexOptions.IgnoreCase | RegexOptions.RightToLeft).Groups[1].Value.Replace("\"", "");
            }
            return Return;
        }

        public void CheckForUpdates()
        {
            Controller.WriteToLog(LogPath, Time + "[CheckForUpdates] Checking injector version.");
            string InjectorVersion = HttpDownloader("https://pastebin.com/raw/91j3JaZM", "(\"([^ \"]|\"\")*\")", "InjectorVersion");
            Controller.WriteToLog(LogPath, Time + "[CheckInjectorUpdate] Latest Injector Version: " + InjectorVersion);


            if (Properties.Settings.Default.InjectorVersion == InjectorVersion)
            {
                Controller.WriteToLog(LogPath, Time + "[CheckForUpdates] No injector update found.");
            }
            else
            {
                Controller.WriteToLog(LogPath, Time + "[CheckForUpdates] Injector Update Found.");
                DialogResult Result = MessageBox.Show("A new BranksMod version was detected, would you like to download it?", "BranksMod", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                if (Result == DialogResult.Yes)
                {
                    //Install Injector
                }
            }

            Controller.WriteToLog(LogPath, Time + "[CheckForUpdates] Checking for updates.");
            string ModVersion = HttpDownloader("https://pastebin.com/raw/BzZiKdZh", "(\"([^ \"]|\"\")*\")", "ModVersion");
            Controller.WriteToLog(LogPath, Time + "[CheckForUpdates] Latest Mod Version: " + ModVersion);

            if (Properties.Settings.Default.ModVersion == ModVersion)
            {
                Controller.WriteToLog(LogPath, Time + "[CheckForUpdates] No Mod Update Found.");
            }
            else
            {
                Controller.WriteToLog(LogPath, Time + "[CheckForUpdates] Mod Update Found.");
                DialogResult Result = MessageBox.Show("A new BakkesMod.dll was detected, would you like to download it?", "BranksMod", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                if (Result == DialogResult.Yes)
                {
                    //Install DLL
                }
            }
        }

        public void CheckInstall()
        {
            string Path = (Properties.Settings.Default.FolderPath);

            if (!Directory.Exists(Path))
            {
                Controller.WriteToLog(LogPath, Time + "[CheckInstall] Could not find Win32 folder.");
                MessageBox.Show("Error: Could not find Win32 folder, do you have Rocket League installed on another drive?", "BranksMod", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.Close();
            }
            else if (Directory.Exists(Path))
            {
                Controller.WriteToLog(LogPath, Time + "[CheckInstall] Found Win32 folder.");
            }

            if (!Directory.Exists(Path + "\\bakkesmod"))
            {
                Controller.WriteToLog(LogPath, Time + "[CheckInstall] Could not find BakkesMod folder.");
                DialogResult DialogResult = MessageBox.Show("Error: Could not find BakkesMod folder, would you like to install it?", "BakkesMod", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (DialogResult == DialogResult.Yes)
                {
                    Install();
                }
            }
            else if (Directory.Exists(Path + "\\bakkesmod"))
            {
                Controller.WriteToLog(LogPath, Time + "[CheckInstall] Found BakkesMod folder.");
            }
        }

        public void Install()
        {
            string Path = Properties.Settings.Default.FolderPath;
            string Version = HttpDownloader("https://pastebin.com/raw/BzZiKdZh", "(\"([^ \"]|\"\")*\")", "ModVersion");
            string URL = "http://149.210.150.107/static/versions/bakkesmod_" + Version + ".zip";

            if (!Directory.Exists(Path + "\\bakkesmod"))
            {
                Directory.CreateDirectory(Path + "\\bakkesmod");
            }

            using (WebClient Client = new WebClient())
            {
                Client.DownloadFile(URL, "bakkesmod.zip");
            }

            if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "bakkesmod.zip"))
            {
                try
                {
                    ZipFile.ExtractToDirectory(AppDomain.CurrentDomain.BaseDirectory + "\\bakkesmod.zip", Path + "\\bakkesmod\\");
                    File.Delete(AppDomain.CurrentDomain.BaseDirectory + "\\bakkesmod.zip");
                }
                catch (Exception Ex)
                {
                    MessageBox.Show(Ex.ToString(), "BranksMod", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        #endregion

        #region "Form Events"
        private void TrayIcon_DoubleClick(object sender, EventArgs e)
        {
            this.Show();
        }

        private void OpenTrayBtn_Click(object sender, EventArgs e)
        {
            this.Show();
        }

        private void ExitTrayBtn_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void OpenFolderMenuBtn_Click(object sender, EventArgs e)
        {
            string BranksModDirectory = Properties.Settings.Default.FolderPath + "\\bakkesmod";
            if (!Directory.Exists(BranksModDirectory))
            {
                CheckInstall();
            }
            else
            {
                Controller.WriteToLog(Properties.Settings.Default.FolderPath, Time + "[OpenFolder] Found BakkesMod folder.");
                Process.Start(BranksModDirectory);
            }
        }

        private void CheckUpdatesMenuBtn_Click(object sender, EventArgs e)
        {
            CheckForUpdates();
        }

        private void ReinstallMenuBtn_Click(object sender, EventArgs e)
        {
            //DialogResult Result = MessageBox.Show("This will fully remove all BakkesMod files, are you sure you want to continue?", "BranksMod", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            //if (Result == DialogResult.Yes)
            //{
            //    string Path = Properties.Settings.Default.FolderPath + "\\bakkesmod";
            //    if (Directory.Exists(Path))
            //    {
            //        Directory.Delete(Path, true);
            //        Install();
            //    }
            //}
        }

        private void UninstallMenuBtn_Click(object sender, EventArgs e)
        {
            DialogResult Result = MessageBox.Show("This will fully remove all BakkesMod files, are you sure you want to continue?", "BranksMod", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (Result == DialogResult.Yes)
            {
                string Path = Properties.Settings.Default.FolderPath + "\\bakkesmod";
                if (Directory.Exists(Path))
                {
                    Directory.Delete(Path, true);
                    this.Close();
                }
            }
        }

        private void ExitMenuBtn_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void SettingsMenuBtn_Click(object sender, EventArgs e)
        {
            SettingsFrm Settings = new SettingsFrm();
            Settings.Show();

        }

        private void TroubleshootingMenuBtn_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Help is not complete yet.", "BranksMod", MessageBoxButtons.OK, MessageBoxIcon.Information);
            //HelpFrm HF = new HelpFrm();
            //HF.Show();
        }

        private void InjectBtn_Click(object sender, EventArgs e)
        {
            Controller.WriteToLog(LogPath, Time + "[InjectDLL] Manually injecting DLL.");
            InjectDLL();
        }
        #endregion

        #region "Loading Events"
        public void CheckMiniHide()
        {
            if (Properties.Settings.Default.MinimizeHide == true)
            {
                this.Hide();
                TrayIcon.Visible = true;
            }
            else if (Properties.Settings.Default.MinimizeHide == false)
            {
                TrayIcon.Visible = false;
            }
        }

        public void CheckTopmost()
        {
            if (Properties.Settings.Default.BrankTopmost == true)
            {
                this.TopMost = true;
            }
            else if (Properties.Settings.Default.BrankTopmost == false)
            {
                this.TopMost = false;
            }
        }

        public void GetFolderPath()
        {
            string Path = Controller.GetDirFromLog();
            Properties.Settings.Default.FolderPath = Path;
            Properties.Settings.Default.Save();
            CreateLogger();
        }

        public void CreateLogger()
        {
            try
            {
                StreamWriter LogFile = new StreamWriter(LogPath);
                LogFile.Close();
                Controller.WriteToLog(LogPath, Time + "[CreateLogger] Initialized logging.");
            } catch
            {
                CheckInstall();
            }
        }

        public void CheckAutoUpdates()
        {
            if (Properties.Settings.Default.AutoUpdate == true)
            {
                CheckForUpdates();
            }
            else if (Properties.Settings.Default.AutoUpdate == false)
            {

            }
        }

        public void CheckSafeMode()
        {
            if (Properties.Settings.Default.EnableSafeMode == true)
            {
                CheckVersion();
            }
            else if (Properties.Settings.Default.EnableSafeMode == false)
            {

            }
        }

        public void CheckWarnings()
        {
            if (Properties.Settings.Default.DisableWarnings == true)
            {

            }
            else if (Properties.Settings.Default.DisableWarnings == false)
            {
                CheckD3D9();
            }
        }

        public void CheckVersion()
        {
            Properties.Settings.Default.RLVersion = Controller.GetRLVersion(Properties.Settings.Default.FolderPath + "/../../../../");
            Properties.Settings.Default.ModVersion = Controller.GetModVersion(Properties.Settings.Default.FolderPath);
            Properties.Settings.Default.Save();
            Controller.WriteToLog(LogPath, Time + "[CheckVersion] Rocket League Version: " + Properties.Settings.Default.RLVersion);
            Controller.WriteToLog(LogPath, Time + "[CheckVersion] Mod Version: " + Properties.Settings.Default.ModVersion);
        }

        public void CheckD3D9()
        {
            if (File.Exists(Properties.Settings.Default.FolderPath + "d3d9.dll"))
            {
                Controller.WriteToLog(LogPath, Time + "[CheckD3D9] D3D9.dll found.");
                DialogResult DialogResult = MessageBox.Show("Warning: d3d9.dll detected. This file is used by ReShade/uMod and might prevent the GUI from working. Would you like BranksMod to remove this file?", "BakkesMod", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (DialogResult == DialogResult.Yes)
                {
                    File.Delete(Properties.Settings.Default.FolderPath + "d3d9.dll");
                    Controller.WriteToLog(LogPath, Time + "[CheckD3D9] Deleted D3D9.dll.");
                }
                else if (DialogResult == DialogResult.No)
                {
                    Controller.WriteToLog(LogPath, Time + "[CheckD3D9] D3D9.dll not found.");
                }
            }
        }
        #endregion

        #region "Theme Events"
        public void CheckTheme()
        {
            if (Properties.Settings.Default.Theme == "Light")
            {
                ThemeBackground = Color.FromArgb(240, 240, 240);
                ThemeHighlight = Color.FromArgb(255, 255, 255);
                ThemeFontColor = Color.Black;
                LoadLight();
                Controller.WriteToLog(LogPath, Time + "[CheckTheme] Loaded Light Theme. ");
            }
            else if (Properties.Settings.Default.Theme == "Night")
            {
                ThemeBackground = Color.FromArgb(25, 25, 25);
                ThemeHighlight = Color.FromArgb(35, 35, 35);
                ThemeFontColor = Color.White;
                LoadNight();
                Controller.WriteToLog(LogPath, Time + "[CheckTheme] Loaded Night Theme. ");
            }
            this.BackColor = ThemeBackground;
            MenuStrip.BackColor = ThemeHighlight;
            MenuStrip.ForeColor = ThemeFontColor;
            RLLbl.ForeColor = ThemeFontColor;
            StatusLbl.ForeColor = ThemeFontColor;
            OpenFolderMenuBtn.ForeColor = ThemeFontColor;
            CheckUpdatesMenuBtn.ForeColor = ThemeFontColor;
            ReinstallMenuBtn.ForeColor = ThemeFontColor;
            UninstallMenuBtn.ForeColor = ThemeFontColor;
            ExitMenuBtn.ForeColor = ThemeFontColor;
            OpenFolderMenuBtn.BackColor = ThemeHighlight;
            CheckUpdatesMenuBtn.BackColor = ThemeHighlight;
            ReinstallMenuBtn.BackColor = ThemeHighlight;
            UninstallMenuBtn.BackColor = ThemeHighlight;
            ExitMenuBtn.BackColor = ThemeHighlight;
        }

        public void LoadLight()
        {
            RLImg.BackgroundImage = Properties.Resources.RL_Light;
            StatusImg.BackgroundImage = Properties.Resources.Status_Light;
            InjectBtn.BackgroundImage = Properties.Resources.Inject_Light;
            FileMenuBtn.Image = Properties.Resources.Menu_Light;
            OpenFolderMenuBtn.Image = Properties.Resources.Open_Light;
            CheckUpdatesMenuBtn.Image = Properties.Resources.Update_Light;
            ReinstallMenuBtn.Image = Properties.Resources.Reinstall_Light;
            UninstallMenuBtn.Image = Properties.Resources.Delete_Light;
            ExitMenuBtn.Image = Properties.Resources.Exit_Light;
            SettingsMenuBtn.Image = Properties.Resources.Settings_Light;
            HelpMenuBtn.Image = Properties.Resources.Help_Light;
        }

        public void LoadNight()
        {
            RLImg.BackgroundImage = Properties.Resources.RL_Dark;
            StatusImg.BackgroundImage = Properties.Resources.Status_Dark;
            InjectBtn.BackgroundImage = Properties.Resources.Inject_Dark;
            FileMenuBtn.Image = Properties.Resources.Menu_Dark;
            OpenFolderMenuBtn.Image = Properties.Resources.Open_Dark;
            CheckUpdatesMenuBtn.Image = Properties.Resources.Update_Dark;
            ReinstallMenuBtn.Image = Properties.Resources.Reinstall_Dark;
            ExitMenuBtn.Image = Properties.Resources.Exit_Dark;
            UninstallMenuBtn.Image = Properties.Resources.Delete_Dark;
            SettingsMenuBtn.Image = Properties.Resources.Settings_Dark;
            HelpMenuBtn.Image = Properties.Resources.Help_Dark;
        }
        #endregion
    }
}