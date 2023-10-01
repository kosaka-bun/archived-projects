using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

// ReSharper disable InconsistentNaming
// ReSharper disable MemberCanBeMadeStatic.Global
// ReSharper disable StringLiteralTypo

namespace JarLauncher {

internal static class Program {
    
    public static void Main(string[] args) {
        if(args.Length < 1) {
            messageBox("没有提供要打开的文件", MessageBoxIcon.Error);
            return;
        }
        string jarPath = args[0];
        if(!jarPath.Contains("\\")) {
            messageBox("提供的文件路径有误", MessageBoxIcon.Error);
            return;
        }
        var launcher = new Launcher(jarPath);
        if(!launcher.checkVmOptionsFile()) {
            messageBox($"请修改新创建的{launcher.jarName}.vmoptions文件，" +
                "然后重新运行程序", MessageBoxIcon.Information);
            return;
        }
        launcher.launch();
    }

    private static void messageBox(string msg, MessageBoxIcon icon) {
        MessageBox.Show(msg, "JarLauncher",
                        MessageBoxButtons.OK, icon,
                        MessageBoxDefaultButton.Button1, 
                        MessageBoxOptions.DefaultDesktopOnly);
    }
}

internal class Launcher {

    public void launch() {
        checkVmOptionsFile();
        //启动命令行
        var process = new Process {
            StartInfo = {
                //设置要启动的应用程序
                FileName = "cmd.exe",
                //是否使用操作系统shell启动
                UseShellExecute = false,
                //接受来自调用程序的输入信息
                RedirectStandardInput = true,
                //输出信息
                RedirectStandardOutput = true,
                //输出错误
                RedirectStandardError = true,
                //不显示程序窗口
                CreateNoWindow = true
            }
        };
        //启动程序
        process.Start();
        process.StandardInput.WriteLine(
            $"javaw {readVmOptions()} -jar \"{jarPath}\"");
        process.StandardInput.WriteLine("exit");   //需要有这句，不然程序会挂机
        //等待程序执行完退出进程
        process.WaitForExit();
        process.Close();
    }

    /// <summary>
    /// 从vmoptions中读取虚拟机参数
    /// </summary>
    /// <returns></returns>
    private string readVmOptions() {
        var fileStream = new FileStream(vmOptionsPath, FileMode.Open,
            FileAccess.Read);
        var streamReader = new StreamReader(fileStream);
        string content = streamReader.ReadToEnd().Trim();
        streamReader.Close();
        fileStream.Close();
        content = content.Replace("\r", "")
            .Replace("\n", " ");
        return content;
    }

    /// <summary>
    /// 检查vmoptions是否存在，不存在则创建
    /// </summary>
    public bool checkVmOptionsFile() {
        if(File.Exists(vmOptionsPath)) return true;
        var fileStream = new FileStream(vmOptionsPath, FileMode.Create,
            FileAccess.ReadWrite);
        var streamWriter = new StreamWriter(fileStream);
        streamWriter.Write(defaultVmOptions);
        streamWriter.Close();
        fileStream.Close();
        return false;
    }

    public Launcher(string jarPath) {
        this.jarPath = jarPath;
        int lastIndexOfSlash  = jarPath.LastIndexOf("\\", 
            StringComparison.Ordinal);
        jarName = jarPath.Substring(
            lastIndexOfSlash + 1, 
            jarPath.LastIndexOf(".", StringComparison.Ordinal) - 
                lastIndexOfSlash - 1
        );
        vmOptionsPath = jarPath.Substring(0, lastIndexOfSlash) + 
            $"\\{jarName}.vmoptions";
    }

    public readonly string jarName;

    private readonly string jarPath, vmOptionsPath;
    
    private const string defaultVmOptions = "-Dfile.encoding=UTF-8";
}

}