using splatheap;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Diagnostics;



namespace splatheap
{

    public class ProcessWrapper
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool WriteProcessMemory(
            IntPtr hProcess,
            IntPtr lpBaseAddress,
            byte[] lpBuffer,
            UIntPtr nSize,
            out UIntPtr lpNumberOfBytesWritten
        );
        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr OpenProcess(
             uint processAccess,
             bool bInheritHandle,
             uint processId
        );
        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool ReadProcessMemory(
            IntPtr hProcess,
            IntPtr lpBaseAddress,
            [Out] byte[] lpBuffer,
            int dwSize,
            out IntPtr lpNumberOfBytesRead
        );
        [DllImport("kernel32.dll", SetLastError = true)]
        static extern string GetMainModuleFileName();

        public IntPtr openProcess(uint processAccess, bool bInheritHandle)
        {
            Process[] process = Process.GetProcessesByName("Cemu");

            if (process.Length != 1)
                return new IntPtr(0);

            return OpenProcess(processAccess, bInheritHandle, (uint)process[0].Id);
        }

        public bool writeProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, Int32 nSize, UIntPtr lpNumberOfBytesWritten)
        {
            return WriteProcessMemory(hProcess, lpBaseAddress, lpBuffer, (UIntPtr)nSize, out lpNumberOfBytesWritten);
        }

        public bool readProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, [Out] byte[] lpBuffer, int dwSize, IntPtr lpNumberOfBytesRead)
        {
            return ReadProcessMemory(hProcess, lpBaseAddress, lpBuffer, dwSize, out lpNumberOfBytesRead);
        }

        public string getMainModuleFileName()
        {
            var process = Process.GetProcessesByName("cemu").First();
            return process.MainModule.FileName.ToString();
        }

        internal void readProcessMemory(IntPtr processPtr, IntPtr intPtr1, int v, IntPtr intPtr2)
        {
            throw new NotImplementedException();
        }
    }
}
