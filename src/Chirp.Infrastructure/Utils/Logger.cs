using System.Runtime.CompilerServices;
using System.Text;

namespace Chirp.Infrastructure.Utils;

// Possible improvement:
// Add queue with mutex which receives the messages.
// A different thread can then perform the writes as items are added to the queue.
// Prevents the calling thread from getting locked.
//
// Thread safe Singleton pattern
// https://inthetechpit.com/2021/04/25/singleton-pattern-c/
public sealed class Logger : IDisposable
{
    public enum Output : ushort
    {
        FILE,
        STDERR,
        STDOUT
    }

    private static bool enabled = true;
    private static Output output = Output.FILE;
    private static Logger? instance;
    private static readonly Mutex mut = new();

    private readonly FileStream logFile;

    private Logger()
    {
        long timestamp = DateTimeOffset.Now.ToUnixTimeSeconds();

        // This does nothing if the dir already exists according to .NET docs
        Directory.CreateDirectory("./logs");
        logFile = File.Create("./logs/chirp_logs_" + timestamp + ".txt");
    }

    public static Logger get
    {
        get
        {
            lock (mut)
            {
                if (instance == null)
                {
                    instance = new Logger();
                    AppDomain.CurrentDomain.UnhandledException
                        += CrashHandler;
                }

                return instance;
            }
        }
    }

    public void Dispose()
    {
        logFile.Flush();
        logFile.Close();
    }

    private static void CrashHandler(object sender, UnhandledExceptionEventArgs args)
    {
        Exception e = (Exception)args.ExceptionObject;
        get.LogError("Unhandled exception crash: " + e.Message);
        get.Dispose();
    }

    private void internalLog(string label, string text, string file, string member, int line)
    {
        string timestamp = StringUtils.TimeToString(DateTimeOffset.Now.ToLocalTime());

        string msg = string.Format("[{0}]\t{1}:{2}:{3} @ {4} | {5}\n",
            label,
            StringUtils.GetFileName(file),
            member,
            line,
            timestamp,
            text
        );

        switch (output)
        {
            case Output.FILE:
                byte[] bytes = Encoding.UTF8.GetBytes(msg);
                logFile.Write(bytes, 0, bytes.Length);
                break;
            case Output.STDERR:
                Console.Error.Write(msg);
                break;
            case Output.STDOUT:
                Console.Write(msg);
                break;
        }
    }

    // Basic log
    public void Log(string text,
        [CallerFilePath] string file = "",
        [CallerMemberName] string member = "",
        [CallerLineNumber] int line = 0)
    {
        internalLog("INFO", text, file, member, line);
    }

    // Warning log
    public void LogWarn(string text,
        [CallerFilePath] string file = "",
        [CallerMemberName] string member = "",
        [CallerLineNumber] int line = 0)
    {
        internalLog("WARN", text, file, member, line);
    }

    // Error log
    public void LogError(string text,
        [CallerFilePath] string file = "",
        [CallerMemberName] string member = "",
        [CallerLineNumber] int line = 0)
    {
        internalLog("ERROR", text, file, member, line);
    }

    public void SetOutput(Output newOutput)
    {
        output = newOutput;
    }

    public void Disable()
    {
        enabled = false;
    }

    public void Enable()
    {
        enabled = true;
    }

    public bool IsEnabled()
    {
        return enabled;
    }

    public string GetLogFileName()
    {
        return logFile.Name;
    }
}