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

    public static void Log(string msg, LogLevel logLevel = LogLevel.Debug)
    {
        if (logLevel == LogLevel.Silent || LabTools.currLogLevel < logLevel)
        {
            return;
        }

        if (logLevel == LogLevel.Error)
        {
            GD.PrintErr($"[ERROR]   {msg}");
        }
        else if (logLevel == LogLevel.Standard)
        {
            GD.Print(msg);
        }
        else
        {
            GD.Print($"[{logLevel.ToString().ToUpper()}]    {msg}");
        }
    }
}
