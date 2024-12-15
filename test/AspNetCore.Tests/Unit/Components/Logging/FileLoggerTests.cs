using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace AspNetCore.Components.Logging;

[Collection("File access")]
public class FileLoggerTests
{
    private FileLogger logger;

    public FileLoggerTests()
    {
        logger = new FileLogger("file-log.txt", 300);

        if (Directory.Exists("FileLogger"))
            Directory.Delete("FileLogger", true);

        foreach (String file in Directory.GetFiles(Directory.GetCurrentDirectory(), "file-log*"))
            File.Delete(file);
    }

    [Fact]
    public void IsEnabled_DisabledForNoneLevel()
    {
        Assert.False(logger.IsEnabled(LogLevel.None));
    }

    [Fact]
    public void IsEnabled_ForNotEmptyTypes()
    {
        foreach (LogLevel level in Enum.GetValues<LogLevel>().Where(level => level != LogLevel.None))
            Assert.True(logger.IsEnabled(level));
    }

    [Fact]
    public void BeginScope_Null()
    {
        Assert.Null(logger.BeginScope<Object?>(null));
    }

    [Fact]
    public void Log_None()
    {
        logger.Log(LogLevel.None, 0, new Object(), null, (_, _) => "Test");

        Assert.False(File.Exists("file-log.txt"));
    }

    [Fact]
    public void Log_Id()
    {
        HttpContext context = new HttpContextAccessor().HttpContext = new DefaultHttpContext();
        Claim[] claims = { new(ClaimTypes.NameIdentifier, "124") };

        context.User = new ClaimsPrincipal(new ClaimsIdentity(claims, "Password"));
        context.TraceIdentifier = "TraceId";

        logger.Log(LogLevel.Warning, 0, "Message", null, (state, _) => state);

        Assert.Contains("Id         : TraceId [124]", File.ReadAllText("file-log.txt"));
    }

    [Fact]
    public void Log_System()
    {
        new HttpContextAccessor().HttpContext = null;

        logger.Log(LogLevel.Warning, 0, "Message", null, (state, _) => state);

        Assert.Contains("Id         : System []", File.ReadAllText("file-log.txt"));
    }

    [Fact]
    public void Log_Message()
    {
        logger.Log(LogLevel.Warning, 0, "Message from above", null, (state, _) => state);

        Assert.Contains($"{LogLevel.Warning}    : Message from above", File.ReadAllText("file-log.txt"));
    }

    [Fact]
    public void Log_Exception()
    {
        Exception stackTrace = Substitute.ForPartsOf<Exception>("Lower exception message");
        Exception exception = new InvalidOperationException("Exception from below", stackTrace);

        stackTrace.StackTrace.Returns("Test trace");

        logger.Log(LogLevel.Error, 0, "Test", exception, (state, _) => $"{state}-{exception.Message}");

        String actual = File.ReadAllText("file-log.txt");

        Assert.Contains($"{nameof(InvalidOperationException)}: Exception from below", actual);
        Assert.Contains("ExceptionProxy: Lower exception message", actual);
        Assert.Contains($"{LogLevel.Error}      : Test", actual);
        Assert.Contains("Stack trace:", actual);
        Assert.Contains("Test trace", actual);
    }

    [Fact]
    public void Log_ToNestedPath()
    {
        logger = new FileLogger("FileLogger/InnerPath/log.txt", 300);

        logger.Log(LogLevel.Trace, 0, "Nested message", null, (state, _) => state);

        Assert.Contains($"{LogLevel.Trace}      : Nested message", File.ReadAllText("FileLogger/InnerPath/log.txt"));
    }

    [Fact]
    public void Log_RollsLargeFiles()
    {
        logger.Log(LogLevel.Debug, 0, "Test", null, (_, _) => new String('A', 301));

        String actual = File.ReadAllText(Directory.GetFiles(Directory.GetCurrentDirectory(), "file-log*").Single());

        Assert.Contains(new String('A', 301), actual);
        Assert.True(!File.Exists("file-log.txt"));
    }
}
