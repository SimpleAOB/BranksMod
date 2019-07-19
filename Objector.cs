using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

    public enum Feedback
    {
        FILE_NOT_FOUND,
        PROCESS_NOT_FOUND,
        FAIL,
        SUCCESS
    }

    public sealed class Objector
{
        static readonly IntPtr IntPtr_Zero = IntPtr.Zero;
        static readonly uint desiredAccess = (0x2 | 0x8 | 0x10 | 0x20 | 0x400);
        static Objector instance;

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern int WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] buffer, uint size, int lpNumberOfBytesWritten);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr GetProcAddress(IntPtr hModule, string lpProcName);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr CreateRemoteThread(IntPtr hProcess, IntPtr lpThreadAttribute, IntPtr dwStackSize, IntPtr lpStartAddress,
            IntPtr lpParameter, uint dwCreationFlags, IntPtr lpThreadId);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr VirtualAllocEx(IntPtr hProcess, IntPtr lpAddress, IntPtr dwSize, uint flAllocationType, uint flProtect);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr GetModuleHandle(string lpModuleName);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr OpenProcess(uint dwDesiredAccess, int bInheritHandle, uint dwProcessId);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern int CloseHandle(IntPtr hObject);

        public static Objector Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Objector();
                }
                return instance;
            }
        }

        private Objector() { }

        public Feedback Object(string Name, string Path)
        {
            if (!File.Exists(Path))
            {
                return Feedback.FILE_NOT_FOUND;
            }

            uint ProcessID = 0;

            Process[] processes = Process.GetProcesses();
            foreach (Process p in processes)
            {
                if (p.ProcessName == Name)
                {
                    ProcessID = (uint)p.Id;
                }
            }
            if (ProcessID == 0) return Feedback.PROCESS_NOT_FOUND;
            if (!ObjectDLL(ProcessID, Path)) return Feedback.FAIL;
            return Feedback.SUCCESS;
        }

        bool ObjectDLL(uint processToInject, string dllPath)
        {
            IntPtr processHandle = OpenProcess(desiredAccess, 1, processToInject);

            if (processHandle == IntPtr_Zero) return false;

            IntPtr loadLibraryAddress = GetProcAddress(GetModuleHandle("kernel32.dll"), "LoadLibraryA");

            if (loadLibraryAddress == IntPtr_Zero) return false;

            IntPtr argAddress = VirtualAllocEx(processHandle, (IntPtr)null, (IntPtr)dllPath.Length, (0x1000 | 0x2000), 0X40);

            if (argAddress == IntPtr_Zero) return false;

            byte[] bytes = Encoding.ASCII.GetBytes(dllPath);

            if (WriteProcessMemory(processHandle, argAddress, bytes, (uint)bytes.Length, 0) == 0)
                return false;

            if (CreateRemoteThread(processHandle, (IntPtr)null, IntPtr_Zero, loadLibraryAddress, argAddress, 0, (IntPtr)null) == IntPtr_Zero)
            {
                return false;
            }

            CloseHandle(processHandle);
            return true;
        }
    }