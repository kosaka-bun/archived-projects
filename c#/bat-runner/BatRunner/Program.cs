using System;
using System.Windows.Forms;
using BatRunner.Util;

// ReSharper disable InconsistentNaming
// ReSharper disable MemberCanBePrivate.Global

namespace BatRunner {

public static class Program {

    public static MainForm mainForm;

    public static string inputFilePath;

    public static Launcher launcher;
    
    /// <summary>
    /// The main entry point for the application.
    /// </summary>
    [STAThread]
    public static void Main(string[] args) {
        if(args.Length < 1) {
            Utils.messageBox("没有提供要打开的文件", MessageBoxIcon.Error);
            return;
        }
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        mainForm = new MainForm();
        inputFilePath = args[0];
        Application.Run(mainForm);
    }
}

}