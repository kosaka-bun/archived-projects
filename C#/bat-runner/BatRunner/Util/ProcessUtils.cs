using System.Diagnostics;
using System.Runtime.InteropServices;

// ReSharper disable InconsistentNaming

namespace BatRunner.Util {

public static class ProcessUtils {
    
    private delegate bool ConsoleCtrlDelegate(CtrlTypes type);
 
    //控制消息
    private enum CtrlTypes : uint {
        CTRL_C_EVENT = 0,
        CTRL_BREAK_EVENT,
        CTRL_CLOSE_EVENT,
        CTRL_LOGOFF_EVENT = 5,
        CTRL_SHUTDOWN_EVENT
    }
    
    //导入Win32 Console函数
    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool AttachConsole(uint dwProcessId);
 
    [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
    private static extern bool FreeConsole();
 
    [DllImport("kernel32.dll")]
    private static extern bool SetConsoleCtrlHandler(
        ConsoleCtrlDelegate handler, bool add);
    
    [DllImport("kernel32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool GenerateConsoleCtrlEvent(
        CtrlTypes dwCtrlEvent, uint dwProcessGroupId);

    /// <summary>
    /// 向指定进程的控制台发送TERM信号
    /// </summary>
    /// <param name="proc"></param>
    public static void sendSigTerm(Process proc) {
        //以防父进程已经attach到另一个Console，先调一次FreeConsole
        FreeConsole();
        //一个进程最多只能attach到一个Console，否则失败，返回0
        if(!AttachConsole((uint) proc.Id)) return;
        //设置父进程属性，忽略Ctrl-C信号
        SetConsoleCtrlHandler(null, true);
        //发出一个Ctrl-C到共享该控制台的所有进程中
        GenerateConsoleCtrlEvent(CtrlTypes.CTRL_C_EVENT, 0);
        //父进程与控制台分离，此时子进程控制台收到Ctrl-C关闭
        FreeConsole();
        //恢复父进程处理Ctrl-C信号
        SetConsoleCtrlHandler(null, false);
    }
}

}