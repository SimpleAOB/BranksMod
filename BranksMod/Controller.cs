using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

    class Controller
    {
        public static void WriteToLog(String Path, String Data)
        {
        if (File.Exists(Path))
        {
            File.AppendAllText(Path, Environment.NewLine + Data);
        }
    }

        public static string GetDirFromLog()
        {
            string MyDocuments = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string LogDir = MyDocuments + @"\My Games\Rocket League\TAGame\Logs\";
            string LogFile = LogDir + "launch.log";
            string ReturnDir = "";

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
                        ReturnDir = Line;
                            break;
                        }
                    }
                }
            }
            return ReturnDir;
        }

        public static string GetRLVersion(String Path)
        {
            string AppInfo = Path + "\\appmanifest_252950.acf";
            string Version = "0";
            string Pattern = "(\"([^ \"]|\"\")*\")";

            if (File.Exists(AppInfo))
            {
                string Line;
                using (FileStream Stream = File.Open(AppInfo, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    StreamReader File = new StreamReader(Stream);
                    while ((Line = File.ReadLine()) != null)
                    {
                        if (Line.Contains("buildid"))
                        {
                            Version = Regex.Match(Line, Pattern, RegexOptions.IgnoreCase | RegexOptions.RightToLeft).Groups[1].Value.Replace("\"", "");
                            break;
                        }
                    }
                }
            }
            return Version;
        }

    public static string GetModVersion(String Path)
    {
        string AppInfo = Path + "\\bakkesmod\\version.txt";
        string Version = "0";

        if (File.Exists(AppInfo))
        {
            string Line;
            using (FileStream Stream = File.Open(AppInfo, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                StreamReader File = new StreamReader(Stream);
                while ((Line = File.ReadLine()) != null)
                {
                    Version = Line;
                }
            }
        }
        return Version;
    }
}