using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace remote
{
    public interface IProcess
    {
        IList<Process> GetProcessesByName(string player);
        void Start(string currentPath);
        Process GetCurrentProcess();
        IList<Process> GetProcesses();
        void Kill(Process process);
        void WaitForInputIdle(Process process);
        IntPtr GetForegroundWindowA();
        void SetForegroundWindowA(IntPtr intPtr);
    }
}