using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using Microsoft.Win32;
using System.IO;

namespace BranksMod
{
    public partial class SettingsFrm : Form
    {
        string Time = DateTime.Now.ToString("[HH:mm:ss] ");
        Color ThemeBackground = Color.FromArgb(0, 0, 0);
        Color ThemeHighlight = Color.FromArgb(0, 0, 0);
        Color ThemeFontColor = Color.Black;

        public SettingsFrm()
        {
            InitializeComponent();
        }

        private void SettingsFrm_Load(object sender, EventArgs e)
        {
            LoadSettings();
            LoadPlugins();
            CheckTheme();
        }

        private void SettingsFrm_FormClosing(object sender, FormClosingEventArgs e)
        {
            SaveChanges();
        }

        public void CheckAutoInjector()
        {
            //if (AutoInjectBox.Checked == true)
            //{
            //    StreamWriter INIFile = new StreamWriter(Properties.Settings.Default.FolderPath + "\\X3DAudio1_7.ini");
            //    INIFile.Write(Properties.Settings.Default.FolderPath + "\\BakkesMod\\bakkesmod.dll");
            //    INIFile.Close();
            //    Controller.WriteToLog(Properties.Settings.Default.FolderPath, Time + "[CheckAutoInjector] Created X3DAudio1_7.ini.");

            //    StreamWriter DLLFile = new StreamWriter(Properties.Settings.Default.FolderPath + "\\X3DAudio1_7.dll");
            //    DLLFile.Close();
            //    File.WriteAllBytes(Properties.Settings.Default.FolderPath + "\\X3DAudio1_7.dll", Properties.Resources.winrnr);
            //    Controller.WriteToLog(Properties.Settings.Default.FolderPath, Time + "[CheckAutoInjector] Created X3DAudio1_7.dll.");
            //}
            //else if (AutoInjectBox.Checked == false)
            //{
            //    try
            //    {
            //        File.Delete(Properties.Settings.Default.FolderPath + "\\X3DAudio1_7.ini");
            //        Controller.WriteToLog(Properties.Settings.Default.FolderPath, Time + "[CheckAutoInjector] Deleted X3DAudio1_7.ini.");
            //        File.Delete(Properties.Settings.Default.FolderPath + "\\X3DAudio1_7.dll");
            //        Controller.WriteToLog(Properties.Settings.Default.FolderPath, Time + "[CheckAutoInjector] Deleted X3DAudio1_7.dll.");
            //    }
            //    catch (Exception Ex)
            //    {
            //        Controller.WriteToLog(Properties.Settings.Default.FolderPath, Time + "[CheckAutoInjector] " + Ex);
            //    }
            //}
        }

        private void TimerBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
        }

        #region "Saving & Loading Events"
        public void SaveChanges()
        {
            CheckAutoInjector();

            RegistryKey RK = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
            if (StartupBox.Checked == true)
            {
                RK.SetValue("BranksMod", Application.ExecutablePath);
            }
            if (StartupBox.Checked == false)
            {
                RK.DeleteValue("BranksMod", false);
            }

            if (SettingsTopBox.Checked == true)
            {
                this.TopMost = true;
            }
            else if (SettingsTopBox.Checked == true)
            {
                this.TopMost = false;
            }

            string Theme = "";
            if (NightBox.Checked == true)
            {
                Theme = "Night";
            }
            else if (NightBox.Checked == false)
            {
                Theme = "Light";
            }

            string InjectionType = "";
            if (AutoInjectBox.Checked == true)
            {
                InjectionType = "AutoInject";
            }
            else if (TimeoutBox.Checked == true)
            {
                InjectionType = "Timeout";
            }
            else if (ManualBox.Checked == true)
            {
                InjectionType = "Manual";
            }

            int Timeout = int.Parse(TimerBox.Text);
            if (TimerBox.Text == "0")
            {
                TimerBox.Text = "2500";
            }
            else
            {

            }

            Properties.Settings.Default.AutoUpdate = AutoUpdateBox.Checked;
            Properties.Settings.Default.RunOnStart = StartupBox.Checked;
            Properties.Settings.Default.MinimizeStartup = MiniStartupBox.Checked;
            Properties.Settings.Default.MinimizeHide = MiniHideBox.Checked;
            Properties.Settings.Default.BrankTopmost = BrankTopBox.Checked;
            Properties.Settings.Default.SettingsTopmost = SettingsTopBox.Checked;
            Properties.Settings.Default.Theme = Theme;
            Properties.Settings.Default.EnableSafeMode = SafeBox.Checked;
            Properties.Settings.Default.DisableWarnings = WarningsBox.Checked;
            Properties.Settings.Default.InjectionType = InjectionType;
            Properties.Settings.Default.Timeout = Timeout;
            Properties.Settings.Default.Save();
            Controller.WriteToLog(Properties.Settings.Default.FolderPath, Time + "[SaveSettings] All settings saved.");
        }

        public void LoadSettings()
        {
            TimerBox.Text = Properties.Settings.Default.Timeout.ToString();

            if (Properties.Settings.Default.AutoUpdate == true)
            {
                AutoUpdateBox.Checked = true;
            }
            else if (Properties.Settings.Default.AutoUpdate == false)
            {
                AutoUpdateBox.Checked = false;
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
                MiniStartupBox.Checked = true;
            }
            else if (Properties.Settings.Default.MinimizeStartup == false)
            {
                MiniStartupBox.Checked = false;
            }
            if (Properties.Settings.Default.MinimizeHide == true)
            {
                MiniHideBox.Checked = true;
            }
            else if (Properties.Settings.Default.MinimizeHide == false)
            {
                MiniHideBox.Checked = false;
            }
            if (Properties.Settings.Default.BrankTopmost == true)
            {
                BrankTopBox.Checked = true;
            }
            else if (Properties.Settings.Default.BrankTopmost == false)
            {
                BrankTopBox.Checked = false;
            }
            if (Properties.Settings.Default.SettingsTopmost == true)
            {
                SettingsTopBox.Checked = true;
            }
            else if (Properties.Settings.Default.SettingsTopmost == false)
            {
                SettingsTopBox.Checked = false;
            }
            if (Properties.Settings.Default.Theme == "Light")
            {
                NightBox.Checked = false;
            }
            else if (Properties.Settings.Default.Theme == "Night")
            {
                NightBox.Checked = true;
            }
            if (Properties.Settings.Default.EnableSafeMode == true)
            {
                SafeBox.Checked = true;
            }
            else if (Properties.Settings.Default.EnableSafeMode == false)
            {
                SafeBox.Checked = false;
            }
            if (Properties.Settings.Default.DisableWarnings == true)
            {
                WarningsBox.Checked = true;
            }
            else if (Properties.Settings.Default.DisableWarnings == false)
            {
                WarningsBox.Checked = false;
            }
            if (Properties.Settings.Default.InjectionType == "AutoInject")
            {
                AutoInjectBox.Checked = true;
            }
            else if (Properties.Settings.Default.InjectionType == "Timeout")
            {
                TimeoutBox.Checked = true;
            }
            else if (Properties.Settings.Default.InjectionType == "Manual")
            {
                ManualBox.Checked = true;
            }

            string Timeout = Properties.Settings.Default.Timeout.ToString();
            TimerBox.Text = Timeout;

            RLVersionLbl.Text = "Rocket League Build: " + Properties.Settings.Default.RLVersion;
            InjectorVersionLbl.Text = "Injector Version: " + Properties.Settings.Default.InjectorVersion;
            ModVersionLbl.Text = "Mod Version: " + Properties.Settings.Default.ModVersion;
            Controller.WriteToLog(Properties.Settings.Default.FolderPath, Time + "[LoadSettings] All settings loaded.");
        }

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
        }

        public void ResetSettings()
        {
            AutoUpdateBox.Checked = true;
            StartupBox.Checked = false;
            MiniStartupBox.Checked = false;
            MiniHideBox.Checked = false;
            BrankTopBox.Checked = false;
            SettingsTopBox.Checked = true;
            NightBox.Checked = false;
            SafeBox.Checked = true;
            WarningsBox.Checked = false;
            AutoInjectBox.Checked = false;
            TimeoutBox.Checked = false;
            ManualBox.Checked = false;
            TimerBox.Text = "2500";
            SaveChanges();
            Controller.WriteToLog(Properties.Settings.Default.FolderPath, Time + "[ResetBtn] Reset settings to default.");
        }
        #endregion

        #region "GUI Events"
        public void RefreshTabs()
        {
            GeneralBtn.BackColor = ThemeBackground;
            InjectorBtn.BackColor = ThemeBackground;
            PluginsBtn.BackColor = ThemeBackground;
            AboutBtn.BackColor = ThemeBackground;
            GeneralIcon.BackColor = ThemeBackground;
            InjectorIcon.BackColor = ThemeBackground;
            PluginsIcon.BackColor = ThemeBackground;
            AboutIcon.BackColor = ThemeBackground;
        }

        private void GeneralBtn_Click(object sender, EventArgs e)
        {
            SettingsTabCtrl.SelectedTab = GeneralTab;
            RefreshTabs();
            GeneralBtn.BackColor = ThemeHighlight;
            GeneralTab.BackColor = ThemeHighlight;
            GeneralIcon.BackColor = ThemeHighlight;
        }

        private void InjectorBtn_Click(object sender, EventArgs e)
        {
            SettingsTabCtrl.SelectedTab = InjectorTab;
            RefreshTabs();
            InjectorBtn.BackColor = ThemeHighlight;
            InjectorTab.BackColor = ThemeHighlight;
            InjectorIcon.BackColor = ThemeHighlight;
        }

        private void PluginsBtn_Click(object sender, EventArgs e)
        {
            SettingsTabCtrl.SelectedTab = PluginsTab;
            RefreshTabs();
            PluginsBtn.BackColor = ThemeHighlight;
            PluginsTab.BackColor = ThemeHighlight;
            PluginsIcon.BackColor = ThemeHighlight;
        }

        private void AboutBtn_Click(object sender, EventArgs e)
        {
            SettingsTabCtrl.SelectedTab = AboutTab;
            RefreshTabs();
            AboutBtn.BackColor = ThemeHighlight;
            AboutTab.BackColor = ThemeHighlight;
            AboutIcon.BackColor = ThemeHighlight;
        }

        private void GeneralIcon_Click(object sender, EventArgs e)
        {
            SettingsTabCtrl.SelectedTab = GeneralTab;
            RefreshTabs();
            GeneralBtn.BackColor = ThemeHighlight;
            GeneralTab.BackColor = ThemeHighlight;
            GeneralIcon.BackColor = ThemeHighlight;
        }

        private void InjectorIcon_Click(object sender, EventArgs e)
        {
            SettingsTabCtrl.SelectedTab = InjectorTab;
            RefreshTabs();
            InjectorBtn.BackColor = ThemeHighlight;
            InjectorTab.BackColor = ThemeHighlight;
            InjectorIcon.BackColor = ThemeHighlight;
        }

        private void PluginsIcon_Click(object sender, EventArgs e)
        {
            SettingsTabCtrl.SelectedTab = PluginsTab;
            RefreshTabs();
            PluginsBtn.BackColor = ThemeHighlight;
            PluginsTab.BackColor = ThemeHighlight;
            PluginsIcon.BackColor = ThemeHighlight;
        }

        private void AboutIcon_Click(object sender, EventArgs e)
        {
            SettingsTabCtrl.SelectedTab = AboutTab;
            RefreshTabs();
            AboutBtn.BackColor = ThemeHighlight;
            AboutTab.BackColor = ThemeHighlight;
            AboutIcon.BackColor = ThemeHighlight;
        }

        private void Icons8Link_Click(object sender, EventArgs e)
        {
            Process P = new Process();
            P.StartInfo.FileName = "www.icons8.com";
            P.Start();
            Controller.WriteToLog(Properties.Settings.Default.FolderPath, Time + "[Icons8Link] Opened link.");
        }

        private void DiscordLink_Click(object sender, EventArgs e)
        {
            Process.Start("https://discordapp.com/invite/HsM6kAR");
            Controller.WriteToLog(Properties.Settings.Default.FolderPath, Time + "[DiscordLink] Opened link.");
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
                ThemeBackground = Color.FromArgb(35, 35, 35);
                ThemeHighlight = Color.FromArgb(45, 45, 45);
                ThemeFontColor = Color.FromArgb(235, 235, 235);
                LoadNight();
                Controller.WriteToLog(Properties.Settings.Default.FolderPath, Time + "[CheckTheme] Loaded Night Theme. ");
            }
            GeneralTab.BackColor = ThemeHighlight;
            InjectorTab.BackColor = ThemeHighlight;
            PluginsTab.BackColor = ThemeHighlight;
            AboutTab.BackColor = ThemeHighlight;
            GeneralBtn.BackColor = ThemeHighlight;
            InjectorBtn.BackColor = ThemeBackground;
            PluginsBtn.BackColor = ThemeBackground;
            AboutBtn.BackColor = ThemeBackground;
            GeneralIcon.BackColor = ThemeHighlight;
            InjectorIcon.BackColor = ThemeBackground;
            PluginsIcon.BackColor = ThemeBackground;
            AboutIcon.BackColor = ThemeBackground;
            TimerBox.BackColor = ThemeBackground;
            PluginsList.BackColor = ThemeHighlight;
            PluginsList.ForeColor = ThemeFontColor;
            GeneralBtn.ForeColor = ThemeFontColor;
            InjectorBtn.ForeColor = ThemeFontColor;
            PluginsBtn.ForeColor = ThemeFontColor;
            AboutBtn.ForeColor = ThemeFontColor;
            AutoUpdateBox.ForeColor = ThemeFontColor;
            StartupBox.ForeColor = ThemeFontColor;
            MiniStartupBox.ForeColor = ThemeFontColor;
            MiniHideBox.ForeColor = ThemeFontColor;
            BrankTopBox.ForeColor = ThemeFontColor;
            SettingsTopBox.ForeColor = ThemeFontColor;
            NightBox.ForeColor = ThemeFontColor;
            SafeBox.ForeColor = ThemeFontColor;
            WarningsBox.ForeColor = ThemeFontColor;
            AutoInjectBox.ForeColor = ThemeFontColor;
            TimeoutBox.ForeColor = ThemeFontColor;
            ManualBox.ForeColor = ThemeFontColor;
            TimerLbl.ForeColor = ThemeFontColor;
            TimerBox.ForeColor = ThemeFontColor;
            RLVersionLbl.ForeColor = ThemeFontColor;
            InjectorVersionLbl.ForeColor = ThemeFontColor;
            ModVersionLbl.ForeColor = ThemeFontColor;
            DiscordLbl.ForeColor = ThemeFontColor;
            Icons8Lbl.ForeColor = ThemeFontColor;
            DevelopersLbl.ForeColor = ThemeFontColor;
        }

        public void LoadLight()
        {
            UpdateImg.BackgroundImage = Properties.Resources.Update_Light;
            StartupImg.BackgroundImage = Properties.Resources.Run_Light;
            MiniStartupImg.BackgroundImage = Properties.Resources.Minimize_Light;
            MiniHideImg.BackgroundImage = Properties.Resources.Hide_Light;
            BrankTopImg.BackgroundImage = Properties.Resources.Topmost_Light;
            SettingsTopImg.BackgroundImage = Properties.Resources.Topmost_Light;
            NightImg.BackgroundImage = Properties.Resources.Night_Light;
            SafeModeImg.BackgroundImage = Properties.Resources.Safe_Light;
            WarningsImg.BackgroundImage = Properties.Resources.Warning_Light;
            AutoInjectImg.BackgroundImage = Properties.Resources.RL_Light;
            TimeoutImg.BackgroundImage = Properties.Resources.Automatic_Light;
            ManualImg.BackgroundImage = Properties.Resources.Manual_Light;
            TimerImg.BackgroundImage = Properties.Resources.Timeout_Light;
            GeneralIcon.BackgroundImage = Properties.Resources.Settings_Light;
            InjectorIcon.BackgroundImage = Properties.Resources.Inject_Light;
            PluginsIcon.BackgroundImage = Properties.Resources.Plugins_Light;
            AboutIcon.BackgroundImage = Properties.Resources.Help_Light;
        }

        public void LoadNight()
        {
            UpdateImg.BackgroundImage = Properties.Resources.Update_Dark;
            StartupImg.BackgroundImage = Properties.Resources.Run_Dark;
            MiniStartupImg.BackgroundImage = Properties.Resources.Minimize_Dark;
            MiniHideImg.BackgroundImage = Properties.Resources.Hide_Dark;
            BrankTopImg.BackgroundImage = Properties.Resources.Topmost_Dark;
            SettingsTopImg.BackgroundImage = Properties.Resources.Topmost_Dark;
            NightImg.BackgroundImage = Properties.Resources.Night_Dark;
            SafeModeImg.BackgroundImage = Properties.Resources.Safe_Dark;
            WarningsImg.BackgroundImage = Properties.Resources.Warning_Dark;
            AutoInjectImg.BackgroundImage = Properties.Resources.RL_Dark;
            TimeoutImg.BackgroundImage = Properties.Resources.Automatic_Dark;
            ManualImg.BackgroundImage = Properties.Resources.Manual_Dark;
            TimerImg.BackgroundImage = Properties.Resources.Timeout_Dark;
            GeneralIcon.BackgroundImage = Properties.Resources.Settings_Dark;
            InjectorIcon.BackgroundImage = Properties.Resources.Inject_Dark;
            PluginsIcon.BackgroundImage = Properties.Resources.Plugins_Dark;
            AboutIcon.BackgroundImage = Properties.Resources.Help_Dark;
        }
        #endregion

    }
}
