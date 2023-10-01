using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using BatRunner.Util;
using Newtonsoft.Json.Linq;

// ReSharper disable MemberCanBeMadeStatic.Local
// ReSharper disable InconsistentNaming

namespace BatRunner {

public class Launcher {
    
    /// <summary>
    /// 专有文件的后缀名
    /// </summary>
    private const string suffix = ".hbat";
    
    private string filePath;

    private Process process;

    public bool processClosed;

    public Launcher(string filePath) {
        this.filePath = filePath;
    }
    
    public void launch() {
        //启动命令行
        initProcess();
        //启动程序
        Program.mainForm.println($"Starting {Program.inputFilePath}...\r\n");
        if(filePath.EndsWith(suffix)) {
            executeHbat();
        } else {
            changeTitleOfWindowAndMenu();
            process.Start();
            process.StandardInput.WriteLine($"call {filePath} & exit");
        }
        //监听标准输出，并转移
        redirectOutput();
        //等待程序执行完退出进程
        process.WaitForExit();
        int exitCode = process.ExitCode;
        processClosed = true;
        //等待线程将输出流中的文本输出完成
        Thread.Sleep(3 * 1000);
        Program.mainForm.println("\r\nProcess has exited with code " +
            $"{exitCode}.");
    }
    
    private void initProcess() {
        process = new Process {
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
    }

    /// <summary>
    /// 读取专有文件，向控制台输出命令
    /// </summary>
    private void executeHbat() {
        string content = Utils.readFileToString(filePath);
        //如果文件为空，则直接调用该文件所在目录下，与该文件同名的，后缀名为.bat的文件
        if(content.Length <= 0) {
            filePath = filePath.Substring(0, filePath.Length - 
                suffix.Length) + ".bat";
            process.Start();
            process.StandardInput.WriteLine($"call {filePath} & exit");
            return;
        }
        //解析hbat json文件
        JObject jo = JObject.Parse(content);
        string name = jo["name"]?.Value<string>().Trim();
        string command = jo["command"]?.Value<string>().Trim();
        string encoding = jo["encoding"]?.Value<string>().Trim().ToUpper();
        changeTitleOfWindowAndMenu(name);
        if(encoding != null && encoding.Equals("UTF-8")) {
            Program.mainForm.println("Use UTF-8 encoding...");
            process.StartInfo.StandardOutputEncoding = Encoding.UTF8;
            process.StartInfo.StandardErrorEncoding = Encoding.UTF8;
        }
        //调用
        if(string.IsNullOrEmpty(command)) {
            Program.mainForm.println("没有提供要执行的命令，将默认执行该文件所在目录下" +
                "与该文件同名的bat文件。");
            string batPath = filePath.Substring(0, filePath.Length - 
                suffix.Length) + ".bat";
            command = $"call {batPath}";
        }
        process.Start();
        string baseDir = filePath.Substring(0, filePath.LastIndexOf(
            "\\", StringComparison.Ordinal));
        process.StandardInput.WriteLine($"cd /d {baseDir}");
        process.StandardInput.WriteLine($"{command} & exit");
    }
    
    private void changeTitleOfWindowAndMenu(string title = null) {
        string windowName;
        if(Program.inputFilePath.Contains("\\")) {
            windowName = title ?? Program.inputFilePath.Substring(
                Program.inputFilePath.LastIndexOf("\\", 
                    StringComparison.Ordinal) + 1
            );
        } else {
            windowName = title ?? Program.inputFilePath;
        }
        Program.mainForm.invokeLambda(() => {
            Program.mainForm.Text = windowName;
            Program.mainForm.systemTrayIconText = windowName;
        });
        Program.mainForm.appNameMenuItem.Text = windowName;
    }

    private void redirectOutput() {
        void redirect(TextReader sr, string threadName) {
            string line;
            while(!processClosed) {
                line = sr.ReadLine();
                if(line != null) {
                    Program.mainForm.println(line);
                } else {
                    Thread.Sleep(1000);
                }
            }
            while((line = sr.ReadLine()) != null) {
                Program.mainForm.println(line);
            }
            Program.mainForm.println($"{threadName} monitor thread has stopped.");
        }
        new Thread(() => {
            redirect(process.StandardOutput, "StdOut");
        }).Start();
        new Thread(() => {
            redirect(process.StandardError, "StdErr");
        }).Start();
    }

    public void stopProcess() {
        try {
            if(processClosed) {
                Utils.stopProcessTree(process.Id);
                return;
            }
            Program.launcher.processClosed = true;
            ProcessUtils.sendSigTerm(process);
            process.StandardInput.WriteLine("y\r\nexit");
            process.WaitForExit(20 * 1000);
            Utils.stopProcessTree(process.Id);
        } catch(Exception) {
            //ignore
        }
    }
}

}