using System.Runtime.CompilerServices;
using System.Text;

namespace Utils {

    public class StringUtils {
        public static bool IsInteger(string x) {
            if(x.Length == 0) return false;
            return x.Select(x => Char.IsDigit(x)).Aggregate((x, y) => x && y);
        }
    }

    // NOT FINISHED, DON'T USE YET
    //
    // Thread safe Singleton pattern
    // https://inthetechpit.com/2021/04/25/singleton-pattern-c/
    public sealed class Logger {

        private static Logger? instance = null;
        private static Mutex mut = new Mutex();

        public static Logger get {
            get {
                lock(Logger.mut) {
                    if(Logger.instance == null) {
                        Logger.instance = new Logger();
                    }
                    return Logger.instance;
                }
            }
        }

        private FileStream logFile;

        Logger() {
            long timestamp = DateTimeOffset.Now.ToUnixTimeSeconds();

            logFile = File.Create("./chirp_logs_" + timestamp + ".txt");
        }

        ~Logger() {
            logFile.Flush();
            logFile.Close();
        }

        private void internalLog(string label, string text, string file, string member, int line) {
            string msg = String.Format("[{0}] {1}:{2}:{3} | {4}", label, file, member, line, text);

            logFile.Write(Encoding.UTF8.GetBytes(msg), 0, msg.Length);
        }

        // Basic log
        public void Log(string text,
                        [CallerFilePath] string file = "",
                        [CallerMemberName] string member = "",
                        [CallerLineNumber] int line = 0) {

            internalLog("LOG", text, file, member, line);
        }

        // Warning log
        public void LogWarn(string text,
                        [CallerFilePath] string file = "",
                        [CallerMemberName] string member = "",
                        [CallerLineNumber] int line = 0) {

            internalLog("WARN", text, file, member, line);
        }

        // Error log
        public void LogError(string text,
                        [CallerFilePath] string file = "",
                        [CallerMemberName] string member = "",
                        [CallerLineNumber] int line = 0) {

            internalLog("ERROR", text, file, member, line);
        }


    }

}