namespace Utils;

using System.Runtime.CompilerServices;
using System.Text;

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
    };

    private static bool enabled = true;
    private static Output output = Output.FILE;
    private static Logger? instance = null;
    private static Mutex mut = new Mutex();

    public static Logger get 
    {
        get 
        {
            lock(Logger.mut) 
            {
                if(Logger.instance == null) 
                {
                    Logger.instance = new Logger();
                    AppDomain.CurrentDomain.UnhandledException 
                        += new UnhandledExceptionEventHandler(CrashHandler);
                }
                return Logger.instance;
            }
        }
    }

    private FileStream logFile;

    private Logger() 
    {
        long timestamp = DateTimeOffset.Now.ToUnixTimeSeconds();

        logFile = File.Create("./chirp_logs_" + timestamp + ".txt");
    }

    /// <summary>
    /// Only called when an unhandled exception occurs.
    /// Forces loggger to flush and close file handle before program is terminated.
    /// </summary>
    private static void CrashHandler(object sender, UnhandledExceptionEventArgs args) 
    {
        Exception e = (Exception) args.ExceptionObject;
        Logger.get.LogError("Unhandled exception crash: " + e.Message);
        Logger.get.Dispose();
    }

    /// <summary>
    /// Forces the logger to flush any content currently in the filestream to the file.
    /// </summary>
    public void flush() 
    {
        logFile.Flush();
    }

    /// <summary>
    /// Efficetively disables the logger by flushing and closing the file handle.
    /// Any subsequent logs made after dispose is called which result in an exception
    /// </summary>
    public void Dispose() 
    {
        logFile.Flush();
        logFile.Close();
    }

    /// <summary>
    /// The logger's internal method for creating logs.
    /// </summary>
    /// <param name="label">The label placed in square brackets at the front of the log</param>
    /// <param name="text">The primary message provided in the log</param>
    /// <param name="file">The file name which the log call is being made from</param>
    /// <param name="member">The method name which the log call is being made from</param>
    /// <param name="line">The line number which the log call is being made from</param>
    private void internalLog(string label, string text, string file, string member, int line)
    {
        string timestamp = StringUtils.TimeToString(DateTimeOffset.Now.ToLocalTime());

        string msg = String.Format("[{0}]\t{1}:{2}:{3} @ {4} | {5}\n",
                label,
                StringUtils.GetFileName(file),
                member,
                line, 
                timestamp,
                text
        );

        switch(output) 
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

    /// <summary>
    /// Generates a new log with the "[INFO]" tag.
    /// </summary>
    /// <param name="text">The log message</param>
    public void Log(string text,
                    [CallerFilePath] string file = "",
                    [CallerMemberName] string member = "",
                    [CallerLineNumber] int line = 0)
    {
        if(!enabled) return;
        internalLog("INFO", text, file, member, line);
    }

    /// <summary>
    /// Generates a new log with the "[WARN]" tag.
    /// </summary>
    /// <param name="text">The log message</param>
    public void LogWarn(string text,
                    [CallerFilePath] string file = "",
                    [CallerMemberName] string member = "",
                    [CallerLineNumber] int line = 0)
    {
        if(!enabled) return;
        internalLog("WARN", text, file, member, line);
    }

    /// <summary>
    /// Generates a new log with the "[ERROR]" tag.
    /// </summary>
    /// <param name="text">The log message</param>
    public void LogError(string text,
                    [CallerFilePath] string file = "",
                    [CallerMemberName] string member = "",
                    [CallerLineNumber] int line = 0)
    {
        if(!enabled) return;
        internalLog("ERROR", text, file, member, line);
    }

    /// <summary>
    /// Changes the current output source of the logger.
    /// If the same output source is given then nothing occurs.
    /// </summary>
    /// <param name="newOutput">The new output source of the logger</param>
    public void SetOutput(Output newOutput)
    {
        output = newOutput;
    }

    /// <summary>
    /// Disabler the logger
    /// </summary>
    public void Disable()
    {
        enabled = false;
    }

    /// <summary>
    /// Enable the logger
    /// </summary>
    public void Enable()
    {
        enabled = true;
    }

    /// <summary>
    /// Check if the logger is currently enabled.
    /// </summary>
    public bool IsEnabled()
    {
        return enabled;
    }


}