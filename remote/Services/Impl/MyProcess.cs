using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace remote
{
    public class MyProcess : IProcess
    {

        [DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();


        [DllImport("User32.dll")]
        static extern int SetForegroundWindow(IntPtr point);

        public IList<Process> GetProcessesByName(string player)
        {
            return Process.GetProcessesByName(player);
        }

        public void Start(string currentPath)
        {
            Process.Start(currentPath);
        }

        public Process GetCurrentProcess()
        {
            return Process.GetCurrentProcess();
        }

        public IList<Process> GetProcesses()
        {
            return Process.GetProcesses();
        }

        public void Kill(Process process)
        {
            process.Kill();
        }

        public void WaitForInputIdle(Process process)
        {
            process.WaitForInputIdle();
        }

        public IntPtr GetForegroundWindowA()
        {
            return GetForegroundWindow();
        }

        public void SetForegroundWindowA(IntPtr intPtr)
        {
            SetForegroundWindow(intPtr);
        }
    }
}
