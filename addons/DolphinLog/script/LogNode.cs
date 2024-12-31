#if TOOLS
using System.Diagnostics;
using System.Text.RegularExpressions;
using GODolphin.BBCode;
using Godot;

namespace GODolphin.Log;

[Tool]
public partial class LogNode : PanelContainer
{
    [Export]
    public RichTextLabel logTextEdit;

    [Export]
    public TextureButton collapseButton;

    [Export]
    public RichTextLabel stackTextEdit;

    [Export]
    public Control stackContainer;

    [Export]
    public Label TimeLabel;

    public override void _EnterTree()
    {
        stackContainer.Visible = false;
        collapseButton.Connect("toggled", new Callable(this, "OnCollapseToggled"));
    }

    public void Setup(LogJson logJson)
    {
        TimeLabel.Text = logJson.LogTime;
        logTextEdit.Text = logJson.LogContent;
        stackTextEdit.Text = logJson.LogStackTrace;
        stackTextEdit.Connect("meta_clicked", new Callable(this, nameof(OnLinkClicked)));
    }

    public void SetMatch(string str)
    {
        if (string.IsNullOrEmpty(str.Trim()))
        {
            Visible = true;
            return;
        }

        Visible = Regex.IsMatch(RemoveBBCode(logTextEdit.Text), str);
    }

    private string RemoveBBCode(string input)
    {
        var decode = new BCDecode(input);
        var output = decode.Item.ToString();
        // 替换 BBCode 标签，保留标签中的内容
        return output;
    }

    public void Collapse()
    {
        collapseButton.ButtonPressed = false;
    }

    private void OnLinkClicked(Variant meta)
    {
        OpenFile(meta);
    }

    private void OpenFile(Variant filePath)
    {
        if (OS.GetName() == "Windows")
        {
            var cmd = $"rider \"{filePath}\"";

            if (filePath.As<string>().StartsWith("/root"))
                return;

            ProcessStartInfo processStartInfo = new ProcessStartInfo()
            {
                FileName = "cmd.exe", // 运行CMD
                Arguments = $"/C {cmd}", // /C让cmd执行命令后关闭
                RedirectStandardOutput = true, // 重定向输出
                UseShellExecute = false, // 不使用操作系统外壳来启动进程
                CreateNoWindow = true, // 不显示命令行窗口
            };
            using (Process process = Process.Start(processStartInfo))
            {
                string output = process.StandardOutput.ReadToEnd();
                GD.Print(output);
            }
        }
    }

    public void OnCollapseToggled(bool toggleOn)
    {
        stackContainer.Visible = toggleOn;
    }

    public override void _ExitTree()
    {
        // stackTextEdit.Disconnect("meta_clicked", new Callable(this, nameof(OnLinkClicked)));
        collapseButton.Disconnect("toggled", new Callable(this, "OnCollapseToggled"));
    }
}
#endif
