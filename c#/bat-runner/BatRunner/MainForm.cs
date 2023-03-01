using System;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using BatRunner.Util;

// ReSharper disable InconsistentlySynchronizedField
// ReSharper disable InvertIf
// ReSharper disable UnassignedField.Global
// ReSharper disable VirtualMemberCallInConstructor
// ReSharper disable MemberCanBeMadeStatic.Local
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable InconsistentNaming

namespace BatRunner {

public partial class MainForm : Form {

    private readonly object printLock = new object();

    public MenuItem appNameMenuItem;

    public string systemTrayIconText {
        get => notifyIcon1.Text;
        set => notifyIcon1.Text = value;
    }

    public MainForm() {
        InitializeComponent();
        customInitializeComponent();
        //将默认宽度高度的比例设置为最小宽度高度
        var minSize = new Size {
            Width = (int) (Size.Width * 0.7),
            Height = (int) ( Size.Height * 0.7)
        };
        MinimumSize = minSize;
    }

    private void customInitializeComponent() {
        Icon = new Icon(Utils.getResource("cmd.ico"));
        notifyIcon1.Icon = new Icon(Utils.getResource("cmd.ico"));
    }

    public void invokeLambda(Action action, bool ignoreException = false) {
        try {
            Invoke(action);
        } catch(Exception) {
            if(!ignoreException) throw;
        }
    }

    public void print(object str) {
        lock(printLock) {
            invokeLambda(() => {
                console.AppendText(str.ToString());
            }, true);
        }
    }

    public void println(object str) {
        print(str + "\r\n");
    }

    public void println() {
        println("");
    }

    public void clear() {
        lock(printLock) {
            invokeLambda(() => {
                console.Clear();
            }, true);
        }
    }

    private void initTrayIcon() {
        notifyIcon1.ContextMenu = new ContextMenu();
        Menu.MenuItemCollection menuItems = notifyIcon1.ContextMenu.MenuItems;
        //添加托盘图标菜单项
        appNameMenuItem = new MenuItem(Constant.APP_NAME) {
            Enabled = false
        };
        menuItems.Add(appNameMenuItem);
        menuItems.Add(new MenuItem("-"));
        var exitMenu = new MenuItem("退出");
        exitMenu.Click += (sender, e) => {
            DialogResult result = MessageBox.Show("确定退出吗？", "退出",
                MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            if(result != DialogResult.OK) return;
            new Thread(doExit).Start();
        };
        menuItems.Add(exitMenu);
    }

    private void initClearThread() {
        const int maxLineCount = 200;
        //单位为秒
        const int checkTimePeriod = 3;
        new Thread(() => {
            int getLineCount() {
                MatchCollection matches = Regex.Matches(console.Text,
                    "\\r\\n");
                return matches.Count;
            }
            for(; ; ) {
                if((Program.launcher?.processClosed).Equals(true)) return;
                if(getLineCount() > maxLineCount) {
                    lock(printLock) {
                        int lineCount = getLineCount();
                        if(lineCount <= maxLineCount) continue;
                        int index = 0, count = 0;
                        while(count < lineCount - maxLineCount) {
                            index = console.Text.IndexOf("\r\n", index,
                                StringComparison.Ordinal) + 2;
                            count++;
                        }
                        string content = console.Text.Substring(index);
                        console.Clear();
                        console.AppendText(content);
                    }
                } else {
                    Thread.Sleep(checkTimePeriod * 1000);
                }
            }
        }).Start();
    }

    private void doExit() {
        try {
            Program.mainForm.println("正在退出……");
            onExit();
        } catch(Exception ex) {
            println(ex);
            DialogResult result = MessageBox.Show("退出时出现了异常，" +
                "是否强制退出？", "退出", MessageBoxButtons.OKCancel, 
                MessageBoxIcon.Question);
            if(result != DialogResult.OK) return;
        }
        invokeLambda(() => {
            Application.Exit();
            Application.ExitThread();
        });
    }

    private void onExit() {
        Program.launcher.stopProcess();
    }

    //窗口被加载时要执行的内容
    private void Form1_Load(object sender, EventArgs e) {
        initTrayIcon();
        initClearThread();
        new Thread(() => {
            Program.launcher = new Launcher(Program.inputFilePath);
            Program.launcher.launch();
        }).Start();
    }

    private void MainForm_FormClosing(object sender, FormClosingEventArgs e) {
        Hide();
        e.Cancel = true;
    }

    private void notifyIcon1_MouseClick(object sender, MouseEventArgs e) {
        if(e.Button == MouseButtons.Left && !Visible) Show();
    }
}

}