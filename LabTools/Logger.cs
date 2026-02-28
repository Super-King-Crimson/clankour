using System.Diagnostics;
using System.IO;
using Godot;

public enum LogLevel
{
    Debug,
    Standard,
    Error,
    Silent,
}

public static partial class LabTools
{
    public static readonly LogLevel currLogLevel = LogLevel.Debug;
    public static readonly string projectRoot = ProjectSettings.GlobalizePath("res://");

    public static void Log(string msg, LogLevel logLevel = LogLevel.Debug)
    {
        if (logLevel == LogLevel.Silent || LabTools.currLogLevel < logLevel)
        {
            return;
        }

        if (logLevel == LogLevel.Error)
        {
            GD.PushError($"[ERROR]   {msg}");
        }
        else if (logLevel == LogLevel.Standard)
        {
            GD.Print(msg);
        }
        else
        {
            StackFrame sf = new StackFrame(1, true);
            var fullPath = sf.GetFileName();
            var file = Path.GetRelativePath(projectRoot, fullPath!);

            var line = sf.GetFileLineNumber();

            GD.Print($"[{logLevel.ToString().ToUpper()} ({file} line {line})]    {msg}    -    {Time.GetTimeStringFromSystem()}");
        }
    }
}
