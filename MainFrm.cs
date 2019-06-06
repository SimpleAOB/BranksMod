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
        Color ThemeFontColor = Color.FromArgb(0, 0, 0);
        Boolean IsInjected = false;

        public MainFrm()
        {
            InitializeComponent();
        }

        private void MainFrm_Load(object sender, EventArgs e)
        {
            GetFolderPath();
            CheckUpdaterFile();
            CheckWarnings();
            CheckSafeMode();
            CheckTopmost();
            CheckBuffered();
            LoadSettings();
            CheckTheme();
            LoadChangelog();
        }

        #region "Injector & Timers"
        private void ProcessTmr_Tick(object sender, EventArgs e)
        {
            Process[] Name = Process.GetProcessesByName("RocketLeague");
            if (Name.Length == 0)
            {
                RLLbl.Text = "Rocket League is not running.";
                StatusLbl.Text = "Uninjected, waiting for user to start Rocket League.";
                IsInjected = false;
            }
            else
            {
                RLLbl.Text = "Rocket League is running.";
                if (IsInjected == false)
                {
                    StatusLbl.Text = "Process found, attempting injection.";
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
                    StatusLbl.Text = "Process found, waiting for user to manually inject.";
                    InjectionTmr.Stop();
                    InjectBtn.Visible = true;
                }
            }
        }

        private void InjectBtn_Click(object sender, EventArgs e)
        {
            Controller.WriteToLog(LogPath, Time + "[InjectDLL] Manually injecting DLL.");
            InjectDLL();
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
                    StatusLbl.Text = "Uninjected, waiting for user to start Rocket League.";
                    break;
                case InjectionResult.INJECTION_FAILED:
                    Controller.WriteToLog(LogPath, Time + "[InjectDLL] Injection failed.");
                    StatusLbl.Text = "Injection failed, possible data corruption.";
                    IsInjected = false;
                    break;
                case InjectionResult.SUCCESS:
                    Controller.WriteToLog(LogPath, Time + "[InjectDLL] Successfully injected.");
                    StatusLbl.Text = "Successfully injected, changes applied in-game.";
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

        public void CheckUpdaterFile()
        {
            if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "\\BranksModUpdater.exe"))
            {
                Controller.WriteToLog(LogPath, Time + "[CheckUpdaterFile] File found.");
                try
                {
                    File.Delete(AppDomain.CurrentDomain.BaseDirectory + "\\BranksModUpdater.exe");
                    Controller.WriteToLog(LogPath, Time + "[CheckUpdaterFile] Successfully deleted BranksModUpdater.");
                }
                catch (Exception)
                {
                    Controller.WriteToLog(LogPath, Time + "[CheckUpdaterFile] Failed to delete BranksModUpdater.exe");
                }
            }
            else
            {
                Controller.WriteToLog(LogPath, Time + "[CheckUpdaterFile] No file found.");
            }
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
                LoadVersions();
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
                try
                {
                    Client.DownloadFile(URL, "bakkesmod.zip");
                } catch (Exception Ex)
                {
                    Controller.WriteToLog(LogPath, Time + "[Install] Downloading Failed.");
                    Controller.WriteToLog(LogPath, Time + "[Install] " + Ex.ToString());
                }
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
            if (Properties.Settings.Default.OfflineMode == false)
            {
                Controller.WriteToLog(LogPath, Time + "[CheckForUpdates] Checking injector version.");
                string InjectorVersion = HttpDownloader("https://pastebin.com/raw/91j3JaZM", "(\"([^ \"]|\"\")*\")", "InjectorVersion");
                Controller.WriteToLog(LogPath, Time + "[CheckInjectorUpdate] Latest Injector Version: " + InjectorVersion);

                StatusLbl.Text = "Checking for updates...";
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
            else
            {
                Controller.WriteToLog(LogPath, Time + "[CheckForUpdates] Offline Mode Enabled, aborting.");
                MessageBox.Show("Offline Mode is enabled, cannot check for updates at this time.", "BranksMod", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        public void InstallInjector()
        {
            try
            {
                byte[] BMU = Properties.Resources.BranksModUpdater;
                File.WriteAllBytes(AppDomain.CurrentDomain.BaseDirectory + "\\BranksModUpdater.exe", BMU);
                Process P = new Process();
                P.StartInfo.FileName = AppDomain.CurrentDomain.BaseDirectory + "\\BranksModUpdater.exe";
                P.Start();
                this.Close();
            }
            catch (Exception Ex)
            {
                Controller.WriteToLog(LogPath, Time + "[InstallInjector] " + Ex.ToString());
            }
        }

        public void InstallUpdate()
        {
            string Version = HttpDownloader("https://pastebin.com/raw/BzZiKdZh", "(\"([^ \"]|\"\")*\")", "ModVersion");
            string URL = "http://149.210.150.107/static/versions/bakkesmod_" + Version + ".zip";

            using (WebClient Client = new WebClient())
            {
                Controller.WriteToLog(LogPath, Time + "[InstallUpdate] Downloading Archive.");
                try
                {
                    Client.DownloadFile(URL, "bakkesmod.zip");
                }
                catch (Exception Ex)
                {
                    Controller.WriteToLog(LogPath, Time + "[InstallUpdate] Download Failed.");
                    Controller.WriteToLog(LogPath, Time + "[InstallUpdate] " + Ex.ToString());
                    MessageBox.Show(Ex.ToString(), "BranksMod", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                Client.DownloadFile(URL, "bakkesmod.zip");
            }

            StatusLbl.Text = "Update found, installing updates...";
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
                    Controller.WriteToLog(LogPath, Time + "[InstallUpdate] Installation Failed.");
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
            StatusLbl.Text = "Uninjected, waiting for user to start Rocket League.";
        }
        #endregion

        #region "Loading Events"
        public void GetFolderPath()
        {
            string Return = Controller.GetDirFromLog();
            if (Return == "Null")
            {
                Controller.WriteToLog(LogPath, Time + "[GetFolderPath] Return Null, calling GetFolderPathOverride.");
                GetFolderPathOverride();
            }
            else
            {
                Controller.WriteToLog(LogPath, Time + "[GetFolderPath] Return " + Return);
                Properties.Settings.Default.FolderPath = Return;
                Properties.Settings.Default.Save();
                CreateLogger();
                CheckInstall();
                LoadPlugins();
            }
        }
        
        public void GetFolderPathOverride()
        {
            Controller.WriteToLog(LogPath, Time + "[GetFolderPathOverride] ");
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
                Controller.WriteToLog(LogPath, Time + "[GetFolderPathOverride] User selected location " + FilePath);
                CreateLogger();
                CheckInstall();
                LoadPlugins();
            }
            else
            {
                Controller.WriteToLog(LogPath, Time + "[GetFolderPathOverride] Canceled by user.");
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
            if (Properties.Settings.Default.Topmost == true)
            {
                this.TopMost = true;
            }
            else if (Properties.Settings.Default.Topmost == false)
            {
                this.TopMost = false;
            }
        }

        public void CheckBuffered()
        {
            if (Properties.Settings.Default.DoubleBuffered == true)
            {
                this.DoubleBuffered = true;
            }
            else if (Properties.Settings.Default.DoubleBuffered == false)
            {
                this.DoubleBuffered = false;
            }
        }

        public void CheckAutoUpdates()
        {
            if (Properties.Settings.Default.OfflineMode == false)
            {
                if (Properties.Settings.Default.AutoUpdate == true)
                {
                    CheckForUpdates();
                }
            }
            else
            {
                
            }
        }

        public void CheckSafeMode()
        {
            if (Properties.Settings.Default.SafeMode == true)
            {
                CheckRL();
            }
            else if (Properties.Settings.Default.SafeMode == false)
            {
                ProcessTmr.Start();
            }
        }

        public void CheckWarnings()
        {
            if (Properties.Settings.Default.Warnings == true)
            {

            }
            else if (Properties.Settings.Default.Warnings == false)
            {
                CheckD3D9();
            }
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
                if (Properties.Settings.Default.SafeMode == true)
                {
                    ActivateSafeMode();
                }
            }
        }

        public void LoadVersions()
        {
            Properties.Settings.Default.RLVersion = Controller.GetRLVersion(Properties.Settings.Default.FolderPath + "/../../../../");
            Properties.Settings.Default.ModVersion = Controller.GetModVersion(Properties.Settings.Default.FolderPath);
            Properties.Settings.Default.Save();
            Controller.WriteToLog(LogPath, Time + "[CheckVersion] Rocket League Version: " + Properties.Settings.Default.RLVersion);
            Controller.WriteToLog(LogPath, Time + "[CheckVersion] Mod Version: " + Properties.Settings.Default.ModVersion);
            RocketversionLbl.Text = "Rocket League Build: " + Properties.Settings.Default.RLVersion;
            InjectorversionLbl.Text = "Injector Version: " + Properties.Settings.Default.InjectorVersion;
            ModversionLbl.Text = "Mod Version: " + Properties.Settings.Default.ModVersion;
            CheckAutoUpdates();
        }

        public void LoadChangelog()
        {
            string Changelog = new WebClient().DownloadString("https://pastebin.com/raw/6bHkFtWp");
            if (Properties.Settings.Default.JustUpdated == true)
            {
                if (OfflinemodeBox.Checked == false)
                {
                    Controller.WriteToLog(LogPath, Time + "[LoadChangelog] Downloading latest Changelog.");
                    ChangelogBox.Visible = true;
                    TitleLbl.Location = new Point(12, 72);
                    ChangelogBox.Text = Changelog;
                    Properties.Settings.Default.JustUpdated = false;
                    Properties.Settings.Default.Save();
                }
                else
                {
                    Controller.WriteToLog(LogPath, Time + "[LoadChangelog] Offline Mode Eanbled, canceled download.");
                    ChangelogBox.Visible = false;
                    TitleLbl.Location = new Point(12, 292);
                    ChangelogBox.Text = "Offline Mode Enabled";
                }
            }
            else
            {
                if (Properties.Settings.Default.Collapsed == true)
                {
                    ChangelogBox.Visible = false;
                    TitleLbl.Location = new Point(12, 292);
                    ChangelogBox.Text = Changelog;
                }
                else
                {
                    ChangelogBox.Visible = true;
                    TitleLbl.Location = new Point(12, 72);
                    ChangelogBox.Text = Changelog;
                }
            }
        }

        public void ActivateSafeMode()
        {
            Controller.WriteToLog(LogPath, Time + "[ActivateSafeMode] Loading into Safe Mode.");
            RLLbl.Text = "Safe Mode Enabled.";
            StatusLbl.Text = "Mod out of date, please wait for an update.";
        }
        #endregion

        #region "Plugins"
        public void LoadPlugins()
        {
            PluginsList.Clear();
            try
            {
                string[] Files = Directory.GetFiles(Properties.Settings.Default.FolderPath + "\\bakkesmod\\plugins");

                foreach (string File in Files)
                {
                    PluginsList.Items.Add(Path.GetFileName(File));
                }
                Controller.WriteToLog(Properties.Settings.Default.FolderPath, Time + "[LoadPlugins] All plugins loaded.");
            }
            catch (Exception)
            {
                Controller.WriteToLog(Properties.Settings.Default.FolderPath, Time + "[LoadPlugins] Failed to load plugins.");
            }
            PluginsList.Items[0].Selected = true;
            PluginsList.Select();
        }

        private void ReinstallpluginBtn_Click(object sender, EventArgs e)
        {
            //DialogResult Result = MessageBox.Show("Are you sure you want to reinstall this plugin? This will reset all settings you may have changed.", "BranksMod", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            //if (Result == DialogResult.Yes)
            //{
            //    string Dll = Properties.Settings.Default.FolderPath + "bakkesmod\\plugins\\" + PluginsList.SelectedItems[0].Text;
            //    string Set = PluginsList.SelectedItems[0].Text.Replace(".dll", ".set");
            //    string Settings = Properties.Settings.Default.FolderPath + "bakkesmod\\plugins\\settings\\" + Set;
            //    if (File.Exists(Dll))
            //    {

            //    }
            //    LoadPlugins();
            //}
        }

        private void UninstallpluginBtn_Click(object sender, EventArgs e)
        {
            DialogResult Result = MessageBox.Show("Are you sure you want to uninstall this plugin? This action can not be undone.", "BranksMod", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (Result == DialogResult.Yes)
            {
                string Dll = Properties.Settings.Default.FolderPath + "bakkesmod\\plugins\\" + PluginsList.SelectedItems[0].Text;
                string Set = PluginsList.SelectedItems[0].Text.Replace(".dll", ".set");
                string Settings = Properties.Settings.Default.FolderPath + "bakkesmod\\plugins\\settings\\" + Set;
                try
                {
                    Controller.WriteToLog(LogPath, Time + "[UninstallpluginBtn] Attemping to delete " + Dll + "&" + Set);
                    if (File.Exists(Dll))
                    {
                        File.Delete(Dll);
                        File.Delete(Settings);
                    }
                } catch (Exception Ex)
                {
                    Controller.WriteToLog(LogPath, Time + "[UninstallpluginBtn] " + Ex.ToString());
                }
                LoadPlugins();
            }
        }

        private void DocumentationBtn_Click(object sender, EventArgs e)
        {
            string SelectedItem = PluginsList.SelectedItems[0].Text.Replace(".dll", "");
            bool DocAvalible = Controller.Plugins.Contains<String>(SelectedItem);
            if (DocAvalible == true)
            {
                try
                {
                    Controller.Documentation = SelectedItem;
                    DocFrm DF = new DocFrm();
                    DF.Show();
                }
                catch
                {

                }
            }
            else
            {
                MessageBox.Show("Sorry, it seems I have not yet implemented the documentation for this plugin yet.", "BranksMod", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        #endregion

        #region "Settings"
        public void LoadSettings()
        {
            if (Properties.Settings.Default.AutoUpdate == true)
            {
                AutoupdateBox.Checked = true;
            }
            else if (Properties.Settings.Default.AutoUpdate == false)
            {
                AutoupdateBox.Checked = false;
            }
            if (Properties.Settings.Default.SafeMode == true)
            {
                SafemodeBox.Checked = true;
            }
            else if (Properties.Settings.Default.SafeMode == false)
            {
                SafemodeBox.Checked = false;
            }
            if (Properties.Settings.Default.OfflineMode == true)
            {
                OfflinemodeBox.Checked = true;
            }
            else if (Properties.Settings.Default.OfflineMode == false)
            {
                OfflinemodeBox.Checked = false;
            }
            if (Properties.Settings.Default.Warnings == true)
            {
                WarningsBox.Checked = true;
            }
            else if (Properties.Settings.Default.Warnings == false)
            {
                WarningsBox.Checked = false;
            }
            if (Properties.Settings.Default.RunOnStart == true)
            {
                StartupBox.Checked = true;
            }
            else if (Properties.Settings.Default.RunOnStart == false)
            {
                StartupBox.Checked = false;
            }
            if (Properties.Settings.Default.MinimizeStartup == true)
            {
                MinistartupBox.Checked = true;
            }
            else if (Properties.Settings.Default.MinimizeStartup == false)
            {
                MinistartupBox.Checked = false;
            }
            if (Properties.Settings.Default.MinimizeHide == true)
            {
                MinihideBox.Checked = true;
            }
            else if (Properties.Settings.Default.MinimizeHide == false)
            {
                MinihideBox.Checked = false;
            }
            if (Properties.Settings.Default.Topmost == true)
            {
                TopmostBox.Checked = true;
            }
            else if (Properties.Settings.Default.Topmost == false)
            {
                TopmostBox.Checked = false;
            }
            if (Properties.Settings.Default.Topmost == true)
            {
                TopmostBox.Checked = true;
            }
            else if (Properties.Settings.Default.Topmost == false)
            {
                TopmostBox.Checked = false;
            }
            if (Properties.Settings.Default.DoubleBuffered == true)
            {
                BufferedBox.Checked = true;
            }
            else if (Properties.Settings.Default.Topmost == false)
            {
                BufferedBox.Checked = false;
            }
            if (Properties.Settings.Default.Theme == "Light")
            {
                NightmodeBox.Checked = false;
            }
            else if (Properties.Settings.Default.Theme == "Night")
            {
                NightmodeBox.Checked = true;
            }
            else if (Properties.Settings.Default.InjectionType == "Timeout")
            {
                TimeoutBox.Checked = true;
            }
            else if (Properties.Settings.Default.InjectionType == "Manual")
            {
                ManualBox.Checked = true;
            }
            TimerBox.Text = Properties.Settings.Default.Timeout.ToString();
            Controller.WriteToLog(Properties.Settings.Default.FolderPath, Time + "[LoadSettings] All settings loaded.");
        }

        public void ResetSettings()
        {
            Properties.Settings.Default.AutoUpdate = true;
            Properties.Settings.Default.SafeMode = true;
            Properties.Settings.Default.OfflineMode = false;
            Properties.Settings.Default.Warnings = false;
            Properties.Settings.Default.RunOnStart = false;
            Properties.Settings.Default.MinimizeStartup = false;
            Properties.Settings.Default.MinimizeHide = false;
            Properties.Settings.Default.Topmost = false;
            Properties.Settings.Default.DoubleBuffered = false;
            Properties.Settings.Default.Theme = "Light";
            Properties.Settings.Default.InjectionType = "Timeout";
            Properties.Settings.Default.Timeout = 2500;
            Controller.WriteToLog(Properties.Settings.Default.FolderPath, Time + "[ResetSettings] Reset settings to default.");
            Properties.Settings.Default.Save();
            LoadSettings();
        }

        private void AutoUpdateBox_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.AutoUpdate = AutoupdateBox.Checked;
            Properties.Settings.Default.Save();
        }

        private void SafemodeBox_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.SafeMode = SafemodeBox.Checked;
            Properties.Settings.Default.Save();
        }

        private void OfflinemodeBox_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.OfflineMode = OfflinemodeBox.Checked;
            Properties.Settings.Default.Save();
        }

        private void WarningsBox_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.Warnings = WarningsBox.Checked;
            Properties.Settings.Default.Save();
        }

        private void StartupBox_CheckedChanged(object sender, EventArgs e)
        {
            RegistryKey RK = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
            if (StartupBox.Checked == true)
            {
                RK.SetValue("BranksMod", Application.ExecutablePath);
            }
            if (StartupBox.Checked == false)
            {
                RK.DeleteValue("BranksMod", false);
            }
            Properties.Settings.Default.RunOnStart = StartupBox.Checked;
            Properties.Settings.Default.Save();
        }

        private void MiniStartupBox_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.MinimizeStartup = MinistartupBox.Checked;
            Properties.Settings.Default.Save();
        }

        private void MiniHideBox_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.MinimizeHide = MinihideBox.Checked;
            Properties.Settings.Default.Save();
        }

        private void TopmostBox_CheckedChanged(object sender, EventArgs e)
        {
            if (TopmostBox.Checked == true)
            {
                this.TopMost = true;
            }
            else
            {
                this.TopMost= false;
            }
            Properties.Settings.Default.Topmost = TopmostBox.Checked;
            Properties.Settings.Default.Save();
        }

        private void NightmodeBox_CheckedChanged(object sender, EventArgs e)
        {
            if (NightmodeBox.Checked == true)
            {
                Properties.Settings.Default.Theme = "Night";
            }
            else
            {
                Properties.Settings.Default.Theme = "Light";
            }
            Properties.Settings.Default.Save();
            CheckTheme();
            ResetTabs();
            SettingsBtn.BackColor = ThemeHighlight;
            SettingsImg.BackColor = ThemeHighlight;
        }

        private void BufferedBox_CheckedChanged(object sender, EventArgs e)
        {
            if (BufferedBox.Checked == true)
            {
                this.DoubleBuffered = true;
            }
            else
            {
                this.DoubleBuffered = false;
            }
            Properties.Settings.Default.DoubleBuffered = BufferedBox.Checked;
            Properties.Settings.Default.Save();
        }

        private void TimeoutBox_CheckedChanged(object sender, EventArgs e)
        {
            if (TimeoutBox.Checked == true)
            {
                Properties.Settings.Default.InjectionType = "Timeout";
            }
            else
            {
                Properties.Settings.Default.InjectionType = "Manual";
            }
            Properties.Settings.Default.Save();
        }

        private void ManualBox_CheckedChanged(object sender, EventArgs e)
        {
            if (ManualBox.Checked == true)
            {
                Properties.Settings.Default.InjectionType = "Manual";
            }
            else
            {
                Properties.Settings.Default.InjectionType = "Timeout";
            }
            Properties.Settings.Default.Save();
        }

        private void TimerBox_ValueChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.Timeout = Convert.ToInt32(TimerBox.Value);
            Properties.Settings.Default.Save();
        }

        private void UpdateBtn_Click(object sender, EventArgs e)
        {
            CheckForUpdates();
        }

        private void FolderBtn_Click(object sender, EventArgs e)
        {
            string BranksModDirectory = Properties.Settings.Default.FolderPath + "bakkesmod";
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

        private void ResetBtn_Click(object sender, EventArgs e)
        {
            ResetSettings();
        }

        private void ReinstallBtn_Click(object sender, EventArgs e)
        {
            DialogResult Result = MessageBox.Show("This will fully remove all BakkesMod files, are you sure you want to continue?", "BranksMod", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (Result == DialogResult.Yes)
            {
                string Path = Properties.Settings.Default.FolderPath + "bakkesmod";
                if (Directory.Exists(Path))
                {
                    Directory.Delete(Path, true);
                    Install();
                }
            }
        }

        private void UninstallBtn_Click(object sender, EventArgs e)
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

        private void MainFrm_Resize(object sender, EventArgs e)
        {
            CheckMiniHide();
        }

        private void OpenTrayBtn_Click(object sender, EventArgs e)
        {
            this.Show();
        }

        private void ExitTrayBtn_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void TrayIcon_MouseClick(object sender, MouseEventArgs e)
        {
            this.Show();
        }

        private void TitleLbl_Click(object sender, EventArgs e)
        {
            if (TitleLbl.Top == 72)
            {
                ChangelogBox.Visible = false;
                TitleLbl.Location = new Point(12, 292);
                Properties.Settings.Default.Collapsed = true;
            }
            else
            {
                ChangelogBox.Visible = true;
                TitleLbl.Location = new Point(12, 72);
                Properties.Settings.Default.Collapsed = false;
            }
            Properties.Settings.Default.Save();
        }

        private void Icons8Link_Click(object sender, EventArgs e)
        {
            Process.Start("https://icons8.com/");
        }

        private void DiscordLink_Click(object sender, EventArgs e)
        {
            Process.Start("https://discordapp.com/invite/HsM6kAR");
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
                Controller.WriteToLog(Properties.Settings.Default.FolderPath, Time + "[CheckTheme] Loaded Light Theme. ");
            }
            else if (Properties.Settings.Default.Theme == "Night")
            {
                ThemeBackground = Color.FromArgb(40, 40, 40);
                ThemeHighlight = Color.FromArgb(55, 55, 55);
                ThemeFontColor = Color.FromArgb(250, 250, 250);
                LoadNight();
                Controller.WriteToLog(Properties.Settings.Default.FolderPath, Time + "[CheckTheme] Loaded Night Theme. ");
            }

            this.BackColor = ThemeBackground;
            HomeTab.BackColor = ThemeHighlight;
            PluginsBtn.BackColor = ThemeHighlight;
            SettingsTab.BackColor = ThemeHighlight;
            AboutTab.BackColor = ThemeHighlight;
            HomeBtn.BackColor = ThemeHighlight;
            PluginsBtn.BackColor = ThemeBackground;
            SettingsBtn.BackColor = ThemeBackground;
            AboutBtn.BackColor = ThemeBackground;
            HomeImg.BackColor = ThemeHighlight;
            PluginsImg.BackColor = ThemeBackground;
            SettingsImg.BackColor = ThemeBackground;
            AboutImg.BackColor = ThemeBackground;
            TimerBox.BackColor = ThemeBackground;
            HomeBtn.ForeColor = ThemeFontColor;
            PluginsBtn.ForeColor = ThemeFontColor;
            SettingsBtn.ForeColor = ThemeFontColor;
            AboutBtn.ForeColor = ThemeFontColor;
            RLLbl.ForeColor = ThemeFontColor;
            StatusLbl.ForeColor = ThemeFontColor;
            TitleLbl.BackColor = ThemeBackground;
            TitleLbl.ForeColor = ThemeFontColor;
            ChangelogBox.BackColor = ThemeBackground;
            ChangelogBox.ForeColor = ThemeFontColor;
            PluginsList.BackColor = ThemeHighlight;
            PluginsList.ForeColor = ThemeFontColor;
            ReinstallpluginBtn.BackColor = ThemeBackground;
            ReinstallpluginBtn.ForeColor = ThemeFontColor;
            UninstallpluginBtn.BackColor = ThemeBackground;
            UninstallpluginBtn.ForeColor = ThemeFontColor;
            DocumentationBtn.ForeColor = ThemeFontColor;
            DocumentationBtn.BackColor = ThemeBackground;
            AutoupdateBox.ForeColor = ThemeFontColor;
            SafemodeBox.ForeColor = ThemeFontColor;
            OfflinemodeBox.ForeColor = ThemeFontColor;
            WarningsBox.ForeColor = ThemeFontColor;
            StartupBox.ForeColor = ThemeFontColor;
            MinistartupBox.ForeColor = ThemeFontColor;
            MinihideBox.ForeColor = ThemeFontColor;
            TopmostBox.ForeColor = ThemeFontColor;
            BufferedBox.ForeColor = ThemeFontColor;
            NightmodeBox.ForeColor = ThemeFontColor;
            TimeoutBox.ForeColor = ThemeFontColor;
            ManualBox.ForeColor = ThemeFontColor;
            TimerLbl.ForeColor = ThemeFontColor;
            TimerBox.BackColor = ThemeBackground;
            TimerBox.ForeColor = ThemeFontColor;
            UpdateBtn.BackColor = ThemeBackground;
            FolderBtn.BackColor = ThemeBackground;
            ResetBtn.BackColor = ThemeBackground;
            ReinstallBtn.BackColor = ThemeBackground;
            UninstallBtn.BackColor = ThemeBackground;
            UpdateBtn.ForeColor = ThemeFontColor;
            FolderBtn.ForeColor = ThemeFontColor;
            ResetBtn.ForeColor = ThemeFontColor;
            ReinstallBtn.ForeColor = ThemeFontColor;
            UninstallBtn.ForeColor = ThemeFontColor;
            RocketversionLbl.ForeColor = ThemeFontColor;
            InjectorversionLbl.ForeColor = ThemeFontColor;
            ModversionLbl.ForeColor = ThemeFontColor;
            DevelopersLbl.ForeColor = ThemeFontColor;
            Icons8Lbl.ForeColor = ThemeFontColor;
            DiscordLbl.ForeColor = ThemeFontColor;
        }

        public void LoadLight()
        {
            HomeImg.BackgroundImage = Properties.Resources.Home_Black;
            PluginsImg.BackgroundImage = Properties.Resources.Plugin_Black;
            SettingsImg.BackgroundImage = Properties.Resources.Settings_Black;
            AboutImg.BackgroundImage = Properties.Resources.About_Black;
            RLImg.BackgroundImage = Properties.Resources.Rocket_Black;
            StatusImg.BackgroundImage = Properties.Resources.Status_Black;
            InjectBtn.BackgroundImage = Properties.Resources.Inject_Black;
            AutoupdateImg.BackgroundImage = Properties.Resources.Update_Black;
            SafemodeImg.BackgroundImage = Properties.Resources.Safe_Black;
            OfflinemodeImg.BackgroundImage = Properties.Resources.Offline_Black;
            WarningsImg.BackgroundImage = Properties.Resources.Warning_Black;
            StartupImg.BackgroundImage = Properties.Resources.Run_Black;
            MinistartupImg.BackgroundImage = Properties.Resources.Minimize_Black;
            MinihideImg.BackgroundImage = Properties.Resources.Hide_Black;
            TopmostImg.BackgroundImage = Properties.Resources.Top_Black;
            BufferedImg.BackgroundImage = Properties.Resources.Double_Black;
            NightmodeImg.BackgroundImage = Properties.Resources.Night_Black;
            TimeoutImg.BackgroundImage = Properties.Resources.Hourglass_Black;
            ManualImg.BackgroundImage = Properties.Resources.Click_Black;
            TimerImg.BackgroundImage = Properties.Resources.Stopwatch_Black;
            RocketversionImg.BackgroundImage = Properties.Resources.Rocket_Black;
            InjectorversionImg.BackgroundImage = Properties.Resources.Inject_Black;
            ModversionImg.BackgroundImage = Properties.Resources.Console_Black;
            DevelopersImg.BackgroundImage = Properties.Resources.Crown_Black;
            Icons8Img.BackgroundImage = Properties.Resources.Icons8_Black;
        }

        public void LoadNight()
        {
            HomeImg.BackgroundImage = Properties.Resources.Home_White;
            PluginsImg.BackgroundImage = Properties.Resources.Plugin_White;
            SettingsImg.BackgroundImage = Properties.Resources.Settings_White;
            AboutImg.BackgroundImage = Properties.Resources.About_White;
            RLImg.BackgroundImage = Properties.Resources.Rocket_White;
            StatusImg.BackgroundImage = Properties.Resources.Status_White;
            InjectBtn.BackgroundImage = Properties.Resources.Inject_White;
            AutoupdateImg.BackgroundImage = Properties.Resources.Update_White;
            SafemodeImg.BackgroundImage = Properties.Resources.Safe_White;
            OfflinemodeImg.BackgroundImage = Properties.Resources.Offline_White;
            WarningsImg.BackgroundImage = Properties.Resources.Warning_White;
            StartupImg.BackgroundImage = Properties.Resources.Run_White;
            MinistartupImg.BackgroundImage = Properties.Resources.Minimize_White;
            MinihideImg.BackgroundImage = Properties.Resources.Hide_White;
            TopmostImg.BackgroundImage = Properties.Resources.Top_White;
            BufferedImg.BackgroundImage = Properties.Resources.Double_White;
            NightmodeImg.BackgroundImage = Properties.Resources.Night_White;
            TimeoutImg.BackgroundImage = Properties.Resources.Hourglass_White;
            ManualImg.BackgroundImage = Properties.Resources.Click_White;
            TimerImg.BackgroundImage = Properties.Resources.Stopwatch_White;
            RocketversionImg.BackgroundImage = Properties.Resources.Rocket_White;
            InjectorversionImg.BackgroundImage = Properties.Resources.Inject_White;
            ModversionImg.BackgroundImage = Properties.Resources.Console_White;
            DevelopersImg.BackgroundImage = Properties.Resources.Crown_White;
            Icons8Img.BackgroundImage = Properties.Resources.Icons8_White;
        }

        public void ResetTabs()
        {
            HomeBtn.BackColor = ThemeBackground;
            PluginsBtn.BackColor = ThemeBackground;
            SettingsBtn.BackColor = ThemeBackground;
            AboutBtn.BackColor = ThemeBackground;
            HomeImg.BackColor = ThemeBackground;
            PluginsImg.BackColor = ThemeBackground;
            SettingsImg.BackColor = ThemeBackground;
            AboutImg.BackColor = ThemeBackground;
        }

        private void HomeBtn_Click(object sender, EventArgs e)
        {
            ResetTabs();
            ControlTabs.SelectedTab = HomeTab;
            HomeBtn.BackColor = ThemeHighlight;
            HomeTab.BackColor = ThemeHighlight;
            HomeImg.BackColor = ThemeHighlight;
        }

        private void PluginsBtn_Click(object sender, EventArgs e)
        {
            ResetTabs();
            ControlTabs.SelectedTab = PluginsTab;
            PluginsBtn.BackColor = ThemeHighlight;
            PluginsTab.BackColor = ThemeHighlight;
            PluginsImg.BackColor = ThemeHighlight;
        }

        private void SettingsBtn_Click(object sender, EventArgs e)
        {
            ResetTabs();
            ControlTabs.SelectedTab = SettingsTab;
            SettingsBtn.BackColor = ThemeHighlight;
            SettingsTab.BackColor = ThemeHighlight;
            SettingsImg.BackColor = ThemeHighlight;
        }

        private void AboutBtn_Click(object sender, EventArgs e)
        {
            ResetTabs();
            ControlTabs.SelectedTab = AboutTab;
            AboutBtn.BackColor = ThemeHighlight;
            AboutTab.BackColor = ThemeHighlight;
            AboutImg.BackColor = ThemeHighlight;
        }

        private void HomeImg_Click(object sender, EventArgs e)
        {
            ResetTabs();
            ControlTabs.SelectedTab = HomeTab;
            //Lol this is the 1337th line
            HomeBtn.BackColor = ThemeHighlight;
            HomeTab.BackColor = ThemeHighlight;
            HomeImg.BackColor = ThemeHighlight;
        }

        private void PluginsImg_Click(object sender, EventArgs e)
        {
            ResetTabs();
            ControlTabs.SelectedTab = PluginsTab;
            PluginsBtn.BackColor = ThemeHighlight;
            PluginsTab.BackColor = ThemeHighlight;
            PluginsImg.BackColor = ThemeHighlight;
        }

        private void SettingsImg_Click(object sender, EventArgs e)
        {
            ResetTabs();
            ControlTabs.SelectedTab = SettingsTab;
            SettingsBtn.BackColor = ThemeHighlight;
            SettingsTab.BackColor = ThemeHighlight;
            SettingsImg.BackColor = ThemeHighlight;
        }

        private void AboutImg_Click(object sender, EventArgs e)
        {
            ResetTabs();
            ControlTabs.SelectedTab = AboutTab;
            AboutBtn.BackColor = ThemeHighlight;
            AboutTab.BackColor = ThemeHighlight;
            AboutImg.BackColor = ThemeHighlight;
        }
        #endregion

    }
}