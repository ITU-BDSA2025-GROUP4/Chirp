
# How to use the Logger

The Logger serves to eliminate the need for print debugging. It is used to
track what the program is doing and when it's doing it.

By default the logger will output to a file, note that the file stream which the
logger is writing to is buffered, which means the changes will only appear
after a sufficient amount of logs have been made or the program is terminated.

The logger is apart of the `Utils` namespace, ensure to include `Using Utils;`
the top of the file where the logger will be used.

There are three different log commands which determine the type of log.

Different log types:
```csharp
// Most generic log type. Used for general info
Logger.get.Log("General info message here");

// Used to warn against something that could cause issues
Logger.get.LogWarn("Warning message here");

// Used to indicate that something went wrong
Logger.get.LogError("Error message here");
```

The resulting log file will look something like seen here. Each log is placed
on a new line and starts off with the type. Hereafter, the file name, the name
of the function is log call was made from, the line number and the time the log
was made. The actual log message itself appears at the end.
```
[INFO]	Program.cs:main:2 @ 03/09/25 14:28:42 | General info message here
[WARN]	Program.cs:main:5 @ 03/09/25 14:28:42 | Warning message here
[ERROR]	Program.cs:main:8 @ 03/09/25 14:28:43 | Error message here
```


It is possible to change the output of the logger from the file to std output or std error.

```csharp
Logger.get.Log("This goes to the file");

Logget.get.SetOutput(Output.STDOUT);
Logger.get.Log("This goes to std output");

Logget.get.SetOutput(Output.STDERR);
Logger.get.Log("This goes to std error");

Logget.get.SetOutput(Output.FILE);
Logger.get.Log("This also goes to the file");
```

Should one need to disable logging, i.e. to improve performance, it can be done like so:
```csharp
// Disable the logger
Logger.get.Disable();

// Enable the logger
Logger.get.Enable();

// Check whether if the logger is enabled or not
bool isEnabled = Logger.IsEnabled();
```

