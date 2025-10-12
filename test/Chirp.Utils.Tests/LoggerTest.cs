namespace Utils.Tests;

using Chirp.Core.Utils;

public class LoggerUnitTest
{
    private bool logIsCorrectlyFormatted(string log, string label, string message)
    {
        string start = string.Format("[{0}]\t", label);
        if (!log.StartsWith(start))
        {
            return false;
        }

        string[] splitLog = log.Split("|");

        if (splitLog[1].Substring(1) == message)
        {
            return false;
        }

        return true;
    }

    [Fact]
    public void GeneralUse()
    {
        string logFile = Logger.get.GetLogFileName();
        string message = "testing";

        Assert.Contains("chirp_logs_", logFile);
        Assert.EndsWith(".txt", logFile);

        Logger.get.Log(message);
        Logger.get.Dispose();

        Assert.True(logIsCorrectlyFormatted(File.ReadAllText(logFile), "INFO", message));
    }
}