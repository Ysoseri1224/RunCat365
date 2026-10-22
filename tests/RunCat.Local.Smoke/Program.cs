using System.Collections;
using System.Globalization;
using System.Reflection;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using System.Diagnostics;

internal static class Smoke
{
    private const BindingFlags Members = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;
    private static readonly Assembly App = Assembly.Load("RunCat Local");
    private static object Field(object obj, string name) => obj.GetType().GetField(name, Members)!.GetValue(obj)!;
    private static object? Call(object obj, string name, params object?[] args) => obj.GetType().GetMethod(name, Members)!.Invoke(obj, args);
    private static Type Type(string name) => App.GetType("RunCat365." + name, true)!;
    private static void Check(bool condition, string message)
    {
        if (!condition) throw new Exception(message);
        Console.WriteLine("PASS " + message);
    }

    [STAThread]
    private static void Main(string[] args)
    {
        CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("zh-CN");
        CultureInfo.CurrentUICulture = CultureInfo.CurrentCulture;
        Application.SetHighDpiMode(HighDpiMode.PerMonitorV2);
        Application.EnableVisualStyles();
        Console.WriteLine(RuntimeInformation.FrameworkDescription);
        Check(Environment.Version == new Version(6, 0, 36), "Uses installed .NET 6.0.36");
        var settingsType = Type("Properties.UserSettings");
        var settings = settingsType.GetProperty("Default", Members)!.GetValue(null)!;
        settingsType.GetProperty("FirstLaunch", Members)!.SetValue(settings, false);
        Call(settings, "Save");
        using var context = (ApplicationContext)Activator.CreateInstance(Type("RunCat365ApplicationContext"), true)!;
        var menu = Field(context, "contextMenuManager");
        var tray = (NotifyIcon)Field(menu, "notifyIcon");
        var top = tray.ContextMenuStrip!.Items.OfType<ToolStripMenuItem>().ToList();
        Console.WriteLine("MENU " + string.Join(" | ", top.Select(x => x.Text)));
        Check(top.Count == 4, "Only system information, settings, information and exit menus");
        Check(top.Any(x => x.Text == "设置"), "Simplified Chinese resources loaded");
        var themes = top[1].DropDownItems.OfType<ToolStripMenuItem>().First();
        Check(themes.DropDownItems.Count == 2, "Only two cat themes");
        foreach (ToolStripMenuItem theme in themes.DropDownItems)
        {
            theme.PerformClick();
            for (var i = 0; i < 20; i++) Call(menu, "AdvanceFrame");
            Check(((IList)Field(menu, "icons")).Count == 5 && tray.Icon is not null, "Cat animation after theme " + theme.Text);
        }
        Call(settings, "Reload");
        Check((string)settingsType.GetProperty("Theme", Members)!.GetValue(settings)! == "Dark", "Theme persisted");
        var cpu = Activator.CreateInstance(Type("CPUInfo"))!;
        var memory = Activator.CreateInstance(Type("MemoryInfo"))!;
        Type("CPUInfo").GetProperty("Total", Members)!.SetValue(cpu, 0f);
        var idle = (int)Call(context, "CalculateInterval", cpu, null, memory)!;
        Type("CPUInfo").GetProperty("Total", Members)!.SetValue(cpu, 100f);
        var busy = (int)Call(context, "CalculateInterval", cpu, null, memory)!;
        Check(busy > 0 && busy < idle, "CPU load speeds up animation");
        Type("MemoryInfo").GetProperty("MemoryLoad", Members)!.SetValue(memory, 100u);
        Check((int)Call(context, "CalculateInterval", cpu, null, memory)! == busy, "Memory load does not drive animation");
        var deadline = DateTime.UtcNow.AddSeconds(8);
        while (DateTime.UtcNow < deadline)
        {
            Application.DoEvents();
            Thread.Sleep(20);
        }
        var text = ((ToolStripMenuItem)Field(menu, "systemInfoMenu")).Text;
        Console.WriteLine("METRICS\n" + text);
        Check(text.Contains("CPU") && text.Contains("内存"), "Live CPU and memory indicators populated");
        foreach (var name in new[] { "cpuRepository", "gpuRepository", "memoryRepository", "networkRepository" })
        {
            var value = Call(Field(context, name), "Get");
            Console.WriteLine(name + ": " + (value is null ? "unavailable on this host" : "sample returned"));
        }
        var startupType = Type("LaunchAtStartupManager");
        var command = (string)startupType.GetMethod("GetCommand", Members)!.Invoke(null, null)!;
        Check(command == "\"" + Environment.ProcessPath + "\"", "Startup command quotes executable path");
        var startup = Field(context, "launchAtStartupManager");
        using (var key = Registry.CurrentUser.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run"))
        {
            var previous = key.GetValue("RunCat Local");
            var previousKind = previous is null ? RegistryValueKind.String : key.GetValueKind("RunCat Local");
            try
            {
                Call(startup, "ToggleStartup", false);
                Check((bool)Call(startup, "GetStartup")!, "Startup entry enabled and recognized");
                Check((string)key.GetValue("RunCat Local")! == command, "Startup entry contains the quoted current executable");
                Call(startup, "ToggleStartup", true);
                Check(!(bool)Call(startup, "GetStartup")!, "Startup entry disabled");
            }
            finally
            {
                if (previous is null) key.DeleteValue("RunCat Local", false);
                else key.SetValue("RunCat Local", previous, previousKind);
            }
        }
        var strings = Type("Properties.Strings");
        var resourceManager = (System.Resources.ResourceManager)strings.GetProperty("ResourceManager", Members)!.GetValue(null)!;
        Check(resourceManager.GetString("Menu_Settings", CultureInfo.GetCultureInfo("zh-TW")) == "設定", "Traditional Chinese resource fallback");
        if (args.Contains("--task-manager") && Process.GetProcessesByName("Taskmgr").Length == 0)
        {
            typeof(NotifyIcon).GetMethod("OnDoubleClick", Members)!.Invoke(tray, new object[] { EventArgs.Empty });
            var taskDeadline = DateTime.UtcNow.AddSeconds(5);
            Process[] tasks;
            do
            {
                Thread.Sleep(100);
                tasks = Process.GetProcessesByName("Taskmgr");
            } while (tasks.Length == 0 && DateTime.UtcNow < taskDeadline);
            Check(tasks.Length > 0, "Tray double-click launches Task Manager");
            foreach (var task in tasks)
            {
                using (task)
                {
                    try
                    {
                        task.WaitForInputIdle(5000);
                        task.CloseMainWindow();
                    }
                    catch (System.ComponentModel.Win32Exception exception)
                    {
                        Console.WriteLine("Task Manager launched; automatic window cleanup unavailable: " + exception.Message);
                    }
                }
            }
        }
        Call(menu, "SetNotifyIconText", new string('x', 200));
        Check(tray.Text.Length == 127, "Long tooltip stays within Windows limit");
        Call(context, "FetchTick", null, EventArgs.Empty);
        context.Dispose();
        Application.DoEvents();
        Check(!tray.Visible, "Tray removed safely during background sampling");
        Console.WriteLine("All smoke checks passed.");
    }
}
