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
using System.Threading;

namespace BranksMod
{
    public partial class MainFrm : Form
    {
        string Time = DateTime.Now.ToString("[HH:mm:ss] ");
        string LogPath = Path.GetTempPath() + "branksmod.log";
        Color ThemeBackground = Color.FromArgb(0, 0, 0);
        Color ThemeHighlight = Color.FromArgb(0, 0, 0);
        Color ThemeFontColor = Color.Black;
        Boolean IsInjected = false;

        public MainFrm()
        {
            InitializeComponent();
        }

        private void MainFrm_Shown(object sender, EventArgs e)
        {
            if (Properties.Settings.Default.MinimizeStartup == true)
            {
                TrayIcon.Visible = true;
                this.Visible = false;
            }
            else
            {
               
            }
        }

        private void MainFrm_Load(object sender, EventArgs e)
        {
            GetFolderPath();
            LoadVersions();
            CheckWarnings();
            CheckSafeMode();
            CheckTopmost();
            CheckTheme();
        }

        private void MainFrm_Resize(object sender, EventArgs e)
        {
            CheckMiniHide();
        }

        #region "Injector & Timers"
        private void ProcessTmr_Tick(object sender, EventArgs e)
        {
            Process[] Name = Process.GetProcessesByName("RocketLeague");
            if (Name.Length == 0)
            {
                RLLbl.Text = "Rocket League is not running.";
                StatusLbl.Text = "Status: Uninjected.";
                IsInjected = false;
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
            }
            else
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
                    Controller.WriteToLog(LogPath, Time + "[InjectDLL] Could not find DLL.");
                    StatusLbl.Text = "Status: Could not find DLL file.";
                    IsInjected = false;
                    break;
                case InjectionResult.GAME_PROCESS_NOT_FOUND:
                    Controller.WriteToLog(LogPath, Time + "[InjectDLL] Rocket League is not running.");
                    IsInjected = false;
                    StatusLbl.Text = "Status: Uninjected.";
                    break;
                case InjectionResult.INJECTION_FAILED:
                    Controller.WriteToLog(LogPath, Time + "[InjectDLL] Injection failed.");
                    StatusLbl.Text = "Status: Injection failed.";
                    IsInjected = false;
                    break;
                case InjectionResult.SUCCESS:
                    Controller.WriteToLog(LogPath, Time + "[InjectDLL] Successfully injected.");
                    StatusLbl.Text = "Status: Successfully injected.";
                    IsInjected = true;
                    break;
            }
        }
        #endregion

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

        public void CheckInstall()
        {
            string Path = (Properties.Settings.Default.FolderPath);

            if (!Directory.Exists(Path))
            {
                Controller.WriteToLog(LogPath, Time + "[CheckInstall] Could not find Win32 folder.");
                GetFolderPathOverride();
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
                CheckAutoUpdates();
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
                Controller.WriteToLog(LogPath, Time + "[Install] Downloading Archive.");
                Client.DownloadFile(URL, "bakkesmod.zip");
            }

            if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "\\bakkesmod.zip"))
            {
                try
                {
                    ZipFile.ExtractToDirectory(AppDomain.CurrentDomain.BaseDirectory + "\\bakkesmod.zip", Path + "\\bakkesmod\\");
                    File.Delete(AppDomain.CurrentDomain.BaseDirectory + "\\bakkesmod.zip");
                }
                catch (Exception Ex)
                {
                    Controller.WriteToLog(LogPath, Time + "[Install] " + Ex.ToString());
                    MessageBox.Show(Ex.ToString(), "BranksMod", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        public void CheckForUpdates()
        {
            Controller.WriteToLog(LogPath, Time + "[CheckForUpdates] Checking injector version.");
            string InjectorVersion = HttpDownloader("https://pastebin.com/raw/91j3JaZM", "(\"([^ \"]|\"\")*\")", "InjectorVersion");
            Controller.WriteToLog(LogPath, Time + "[CheckInjectorUpdate] Latest Injector Version: " + InjectorVersion);

            StatusLbl.Text = "Status: Checking injector version...";
            if (Properties.Settings.Default.InjectorVersion == InjectorVersion)
            {
                Controller.WriteToLog(LogPath, Time + "[CheckForUpdates] No injector update found.");
            }
            else
            {
                Controller.WriteToLog(LogPath, Time + "[CheckForUpdates] Injector Update Found.");
                DialogResult Result = MessageBox.Show("A new version of BranksMod was detected, would you like to download it?", "BranksMod", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                if (Result == DialogResult.Yes)
                {
                    InstallInjector();
                }
            }

            Controller.WriteToLog(LogPath, Time + "[CheckForUpdates] Checking for updates.");
            string ModVersion = HttpDownloader("https://pastebin.com/raw/BzZiKdZh", "(\"([^ \"]|\"\")*\")", "ModVersion");
            Controller.WriteToLog(LogPath, Time + "[CheckForUpdates] Latest Mod Version: " + ModVersion);

            StatusLbl.Text = "Status: Checking mod version...";
            if (Properties.Settings.Default.ModVersion == ModVersion)
            {
                Controller.WriteToLog(LogPath, Time + "[CheckForUpdates] No Mod Update Found.");
            }
            else
            {
                Controller.WriteToLog(LogPath, Time + "[CheckForUpdates] Mod Update Found.");
                DialogResult Result = MessageBox.Show("A new version of BakkesMod.dll was detected, would you like to download it?", "BranksMod", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                if (Result == DialogResult.Yes)
                {
                    InstallUpdate();
                }
            }
        }

        public void InstallInjector()
        {
            Process.Start("https://github.com/ItsBranK/BranksMod/releases");
            this.Close();
        }

        public void InstallUpdate()
        {
            string Version = HttpDownloader("https://pastebin.com/raw/BzZiKdZh", "(\"([^ \"]|\"\")*\")", "ModVersion");
            string URL = "http://149.210.150.107/static/versions/bakkesmod_" + Version + ".zip";

            using (WebClient Client = new WebClient())
            {
                Controller.WriteToLog(LogPath, Time + "[InstallUpdate] Downloading Archive.");
                Client.DownloadFile(URL, "bakkesmod.zip");
            }

            StatusLbl.Text = "Status: Installing update...";
            if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "\\bakkesmod.zip"))
            {
                try
                {
                    using (ZipArchive Archive = ZipFile.OpenRead(AppDomain.CurrentDomain.BaseDirectory + "\\bakkesmod.zip"))
                    {
                        foreach (ZipArchiveEntry Entry in Archive.Entries)
                        {
                            string DestinationPath = Path.GetFullPath(Path.Combine(Properties.Settings.Default.FolderPath + "\\bakkesmod\\", Entry.FullName));
                            if (Entry.Name == "")
                            {
                                Directory.CreateDirectory(Path.GetDirectoryName(DestinationPath));
                                continue;
                            }
                            Controller.WriteToLog(LogPath, Time + "[InstallUpdate] Checking existing installed files.");
                            if (DestinationPath.ToLower().EndsWith(".cfg") || DestinationPath.ToLower().EndsWith(".json"))
                            {
                                if (File.Exists(DestinationPath))
                                    continue;
                            }
                            Controller.WriteToLog(LogPath, Time + "[InstallUpdate] Extracting files.");
                            Entry.ExtractToFile(DestinationPath, true);
                        }
                    }
                }
                catch (Exception Ex)
                {
                    Controller.WriteToLog(LogPath, Time + "[InstallUpdate] " + Ex.ToString());
                    MessageBox.Show(Ex.ToString(), "BranksMod", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            try
            {
                Controller.WriteToLog(LogPath, Time + "[InstallUpdate] Removing Archive.");
                File.Delete(AppDomain.CurrentDomain.BaseDirectory + "\\bakkesmod.zip");
            }
            catch
            {
                Controller.WriteToLog(LogPath, Time + "[InstallUpdate] Failed to remove Archive.");
                MessageBox.Show("Failed to remove bakkesmod.zip, try running as administrator if you aren't arlready.", "BranksMod", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            StatusLbl.Text = "Status: Uninjected.";
        }
        #endregion

        #region "Uninstaller"
            Boolean DirectoryError = false;
            Boolean RegistryError = false;
            Boolean LogError = false;
            Boolean PickDirectory = false;
            string DirectoryPath;
            string DirectoryEx;
            string RegistryEx;
            string LogEx;

            public void GetDirectory()
            {
                string MyDocuments = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                string LogDir = MyDocuments + "\\My Games\\Rocket League\\TAGame\\Logs\\";
                string LogFile = LogDir + "launch.log";
                if (PickDirectory == true)
                {
                    OpenFileDialog OFD = new OpenFileDialog
                    {
                        Title = "Select RocketLeague.exe",
                        Filter = "EXE Files (*.exe)|*.exe"
                    };

                    if (OFD.ShowDialog() == DialogResult.OK)
                    {
                        string FilePath = OFD.FileName;
                        FilePath = FilePath.Replace("RocketLeague.exe", "");
                        DirectoryPath = FilePath;
                    }
                }
                else
                {
                    if (File.Exists(LogFile))
                    {
                        string Line;
                        using (FileStream Stream = File.Open(LogFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                        {
                            StreamReader File = new StreamReader(Stream);
                            while ((Line = File.ReadLine()) != null)
                            {
                                if (Line.Contains("Init: Base directory: "))
                                {
                                    Line = Line.Replace("Init: Base directory: ", "");
                                    DirectoryPath = Line;
                                    break;
                                }
                            }
                        }
                    }
                }
                RemoveDirectory();
            }

            public void RemoveDirectory()
            {
                string FolderPath = DirectoryPath + "bakkesmod";
                if (!Directory.Exists(FolderPath))
                {
                    MessageBox.Show("Could not find the directory, please manually point to where your RocketLeague.exe is located.", "BakkesMod Uninstaller", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    PickDirectory = true;
                    GetDirectory();
                }
                else
                {
                    try
                    {
                        Directory.Delete(FolderPath, true);
                    }
                    catch (Exception Ex)
                    {
                        DirectoryError = true;
                        DirectoryEx = Ex.ToString();
                    }
                }
                RemoveLog();
            }

            public void RemoveLog()
            {
                string LogPath = Path.GetTempPath() + "branksmod.log";
                if (File.Exists(LogPath))
                {
                    try
                    {
                        File.Delete(LogPath);
                    }
                    catch (Exception Ex)
                    {
                        LogError = true;
                        LogEx = Ex.ToString();
                    }
                }
                RemoveRegistry();
            }

            public void RemoveRegistry()
            {
                RegistryKey Key = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
                try
                {
                    Key.DeleteValue("BranksMod", false);
                }
                catch (Exception Ex)
                {
                    RegistryError = true;
                    RegistryEx = Ex.ToString();
                }
                ConfirmUninstall();
            }

            public void ConfirmUninstall()
            {
                if (DirectoryError == true || RegistryError == true || LogError == true)
                {
                    if (DirectoryError == true)
                    {
                        DialogResult DirectoryResult = MessageBox.Show("There was an error trying to remove the directory, would you like to try again?", "BakkesMod Uninstaller", MessageBoxButtons.YesNo, MessageBoxIcon.Error);
                        if (DirectoryResult == DialogResult.Yes)
                        {
                            RemoveDirectory();
                        }
                    }
                    else if (RegistryError == true)
                    {
                        DialogResult RegistryResult = MessageBox.Show("There was an error trying to remove the startup registry, would you like to try again?", "BakkesMod Uninstaller", MessageBoxButtons.YesNo, MessageBoxIcon.Error);
                        if (RegistryResult == DialogResult.Yes)
                        {
                            RemoveRegistry();
                        }
                    }
                    else if (LogError == true)
                    {
                        DialogResult RegistryResult = MessageBox.Show("There was an error trying to remove the log files, would you like to try again?", "BakkesMod Uninstaller", MessageBoxButtons.YesNo, MessageBoxIcon.Error);
                        if (RegistryResult == DialogResult.Yes)
                        {
                            RemoveLog();
                        }
                    }
                    else
                    {
                        this.Close();
                    }
                }
                else
                {
                    MessageBox.Show("BranksMod has successfully been uninstalled.", "BakkesMod Uninstaller", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.Close();
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
                Controller.WriteToLog(LogPath, Time + "[OpenFolder] Found BakkesMod folder.");
                Process.Start(BranksModDirectory);
            }
        }

        private void CheckUpdatesMenuBtn_Click(object sender, EventArgs e)
        {
            CheckForUpdates();
        }

        private void ReinstallMenuBtn_Click(object sender, EventArgs e)
        {
            DialogResult Result = MessageBox.Show("This will fully remove all BakkesMod files, are you sure you want to continue?", "BranksMod", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (Result == DialogResult.Yes)
            {
                string Path = Properties.Settings.Default.FolderPath + "\\bakkesmod";
                if (Directory.Exists(Path))
                {
                    Directory.Delete(Path, true);
                    Install();
                }
            }
        }

        private void UninstallMenuBtn_Click(object sender, EventArgs e)
        {
            DialogResult Result = MessageBox.Show("Are you sure you want to uninstall BakkesMod? This will remove all BakkesMod & BranksMod files and registry keys.", "Uninstaller", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (Result == DialogResult.Yes)
            {
                GetDirectory();
            }
            else if (Result == DialogResult.No)
            {
                
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
            Process.Start("https://bakkesmod.fandom.com/wiki/Troubleshooting");
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
        public void GetFolderPath()
        {
            string Return = Controller.GetDirFromLog();
            if (Return == "Null")
            {
                Controller.WriteToLog(LogPath, Time + "[GetFolderPath] Return Null.");
                GetFolderPathOverride();
            }
            else
            {
                Controller.WriteToLog(LogPath, Time + "[GetFolderPath] Return " + Return);
                Properties.Settings.Default.FolderPath = Return;
                Properties.Settings.Default.Save();
            }
            CreateLogger();
            CheckInstall();
        }
        
        public void GetFolderPathOverride()
        {
            MessageBox.Show("Error: Could not find Win32 folder, please manually select where your RocketLeague.exe is located.", "BranksMod", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            OpenFileDialog OFD = new OpenFileDialog
            {
                Title = "Select RocketLeague.exe",
                Filter = "EXE Files (*.exe)|*.exe"
            };
            if (OFD.ShowDialog() == DialogResult.OK)
            {
                string FilePath = OFD.FileName;
                FilePath = FilePath.Replace("RocketLeague.exe", "");
                Properties.Settings.Default.FolderPath = FilePath;
                Properties.Settings.Default.Save();
                CreateLogger();
                CheckInstall();
            }
        }

        public void CreateLogger()
        {
            try
            {
                StreamWriter LogFile = new StreamWriter(LogPath);
                LogFile.Close();
                Controller.WriteToLog(LogPath, Time + "[CreateLogger] Initialized logging.");
            }
            catch
            {
               
            }
        }

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
                CheckRL();
            }
            else if (Properties.Settings.Default.EnableSafeMode == false)
            {
                ProcessTmr.Start();
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

        public void LoadVersions()
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

        public void CheckRL()
        {
            string Path = Properties.Settings.Default.FolderPath;
            string Version = HttpDownloader("https://pastebin.com/raw/Hs2e6nM1", "(\"([^ \"]|\"\")*\")", "RL");

            if (Properties.Settings.Default.RLVersion == Version)
            {
                Controller.WriteToLog(LogPath, Time + "[CheckRL] Latest Rocket League build.");
                ProcessTmr.Start();
            }
            else
            {
                Controller.WriteToLog(LogPath, Time + "[CheckRL] Incorrect Rocket League build.");
                if (Properties.Settings.Default.EnableSafeMode == true)
                {
                    ActivateSafeMode();
                }
            }
        }
        public void ActivateSafeMode()
        {
            Controller.WriteToLog(LogPath, Time + "[ActivateSafeMode] Loading into SafeMode.");
            RLLbl.Text = "Mod out of date, please wait for an update.";
            StatusLbl.Text = "Status: SafeMode enabled.";
        }
        #endregion

        #region "Theme Events"
        public void CheckTheme()
        {
            if (Properties.Settings.Default.Theme == "Light")
            {
                ThemeBackground = Color.FromArgb(240, 240, 240);
                ThemeHighlight = Color.FromArgb(255, 255, 255);
                ThemeFontColor = Color.FromArgb(5, 5, 5);
                LoadLight();
                Controller.WriteToLog(LogPath, Time + "[CheckTheme] Loaded Light Theme. ");
            }
            else if (Properties.Settings.Default.Theme == "Night")
            {
                ThemeBackground = Color.FromArgb(35, 35, 35);
                ThemeHighlight = Color.FromArgb(45, 45, 45);
                ThemeFontColor = Color.FromArgb(235, 235, 235);
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