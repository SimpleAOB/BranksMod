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

        public SettingsFrm()
        {
            InitializeComponent();
        }

        private void SettingsFrm_Load(object sender, EventArgs e)
        {
            LoadSettings();
        }

        public void LoadSettings()
        {
            TimeoutBox.Text = Properties.Settings.Default.Timeout.ToString();

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
            if (Properties.Settings.Default.AutoUpdate == true)
            {
                AutoUpdateBox.Checked = true;
            }
            else if (Properties.Settings.Default.AutoUpdate == false)
            {
                AutoUpdateBox.Checked = false;
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
            if (Properties.Settings.Default.InjectionType == "Automatic")
            {
                AutomaticBox.Checked = true;
                ManualBox.Checked = false;
            }
            else if (Properties.Settings.Default.InjectionType == "Manual")
            {
                ManualBox.Checked = true;
                AutomaticBox.Checked = false;
            }
            if (Properties.Settings.Default.AutoSave == true)
            {
                AutoSaveBox.Checked = true;
            }
            else if (Properties.Settings.Default.AutoSave == false)
            {
                AutoSaveBox.Checked = false;
            }
            if (Properties.Settings.Default.RunOnStart == true)
            {
                StartupBox.Checked = true;
            }
            else if (Properties.Settings.Default.RunOnStart == false)
            {
                StartupBox.Checked = false;
            }
            if (Properties.Settings.Default.AutoInject == true)
            {
                AutoRunBox.Checked = true;
            }
            else if (Properties.Settings.Default.AutoInject == false)
            {
                AutoRunBox.Checked = false;
            }

            RLVersionLbl.Text = "Rocket League Build: " + Properties.Settings.Default.RLVersion;
            InjectorVersionLbl.Text = "Injector Version: " + Properties.Settings.Default.InjectorVersion;
            ModVersionLbl.Text = "Mod Version: " + Properties.Settings.Default.ModVersion;
            RLLauncher.WriteToLog(Properties.Settings.Default.FolderPath, Time + "[LoadSettings] All settings loaded.");
        }


        private void SettingsFrm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (AutoSaveBox.Checked == true)
            {
                SaveChanges();
            }
        }

        private void SaveBtn_Click(object sender, EventArgs e)
        {
            SaveChanges();
        }

        private void ResetBtn_Click(object sender, EventArgs e)
        {
            SafeBox.Checked = true;
            WarningsBox.Checked = false;
            AutoUpdateBox.Checked = true;
            AutoSaveBox.Checked = false;
            StartupBox.Checked = false;
            AutoRunBox.Checked = false;
            MiniStartupBox.Checked = false;
            MiniHideBox.Checked = false;
            AutomaticBox.Checked = true;
            ManualBox.Checked = false;        
            TimeoutBox.Text = "2500";
            SaveChanges();
            RLLauncher.WriteToLog(Properties.Settings.Default.FolderPath, Time + "[ResetBtn] Reset settings to default.");
        }

        private void CloseBtn_Click(object sender, EventArgs e)
        {
            if (AutoSaveBox.Checked == true)
            {
                SaveChanges();
            }
            this.Hide();
        }

        private void Icons8Link_Click(object sender, EventArgs e)
        {
            Process P = new Process();
            P.StartInfo.FileName = "www.icons8.com";
            P.Start();
            RLLauncher.WriteToLog(Properties.Settings.Default.FolderPath, Time + "[Icons8Link] Opened link.");
        }

        public void CheckAutoInjector()
        {
            if (AutoRunBox.Checked == true)
            {
                StreamWriter INIFile = new StreamWriter(Properties.Settings.Default.FolderPath + "\\X3DAudio1_7.ini");
                INIFile.Write(Properties.Settings.Default.FolderPath + "\\BakkesMod\\bakkesmod.dll");
                INIFile.Close();
                RLLauncher.WriteToLog(Properties.Settings.Default.FolderPath, Time + "[CheckAutoInjector] Created X3DAudio1_7.ini.");

                StreamWriter DLLFile = new StreamWriter(Properties.Settings.Default.FolderPath + "\\X3DAudio1_7.dll");
                DLLFile.Close();
                File.WriteAllBytes(Properties.Settings.Default.FolderPath + "\\X3DAudio1_7.dll", Properties.Resources.X3DAudio1_7);
                RLLauncher.WriteToLog(Properties.Settings.Default.FolderPath, Time + "[CheckAutoInjector] Created X3DAudio1_7.dll.");
            } else if (AutoRunBox.Checked == false)
            {
                 try
                {
                    File.Delete(Properties.Settings.Default.FolderPath + "\\X3DAudio1_7.ini");
                    RLLauncher.WriteToLog(Properties.Settings.Default.FolderPath, Time + "[CheckAutoInjector] Deleted X3DAudio1_7.ini.");
                    File.Delete(Properties.Settings.Default.FolderPath + "X3DAudio1_7.dll");
                    RLLauncher.WriteToLog(Properties.Settings.Default.FolderPath, Time + "[CheckAutoInjector] Deleted X3DAudio1_7.dll.");
                } catch (Exception Ex)
                {
                    RLLauncher.WriteToLog(Properties.Settings.Default.FolderPath, Time + "[CheckAutoInjector] " + Ex);
                }
            }
        }

        public void SaveChanges()
        {
            CheckAutoInjector();
            string InjectionType = "";
            if (AutomaticBox.Checked == true)
            {
                InjectionType = "Automatic";
            }
            else if (AutomaticBox.Checked == false)
            {
                InjectionType = "Manual";
            }

            int Val = int.Parse(TimeoutBox.Text);
            if (TimeoutBox.Text == "0")
            {
                TimeoutBox.Text = "2500";
                Properties.Settings.Default.Timeout = Val;
            } else
            {
                Properties.Settings.Default.Timeout = Val;
            }

            RegistryKey rk = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
            if (StartupBox.Checked == true)
            {
                rk.SetValue("BranksMod", Application.ExecutablePath);
            }
            if (StartupBox.Checked == false)
            {
                rk.DeleteValue("BranksMod", false);
            }

            Properties.Settings.Default.EnableSafeMode = SafeBox.Checked;
            Properties.Settings.Default.DisableWarnings = WarningsBox.Checked;
            Properties.Settings.Default.AutoUpdate = AutoUpdateBox.Checked;
            Properties.Settings.Default.AutoInject = AutoRunBox.Checked;
            Properties.Settings.Default.AutoSave = AutomaticBox.Checked;
            Properties.Settings.Default.MinimizeStartup = MiniStartupBox.Checked;
            Properties.Settings.Default.MinimizeHide = MiniHideBox.Checked;
            Properties.Settings.Default.InjectionType = InjectionType;
            Properties.Settings.Default.AutoSave = AutoSaveBox.Checked;
            Properties.Settings.Default.RunOnStart = StartupBox.Checked;
            Properties.Settings.Default.Save();
            RLLauncher.WriteToLog(Properties.Settings.Default.FolderPath, Time + "[SaveSettings] All settings saved.");
        }

        private void TimeoutBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
        }
    }
}
