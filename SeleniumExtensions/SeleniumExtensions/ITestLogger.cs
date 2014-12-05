namespace Common.Interfaces
{
    public interface ITestLogger
    {
        void TestAction(string logString);
        void TestAction(string format, params object[] arg);
        void Error(string logString);
        void Error(string format, params object[] arg);
        void Warning(string logString);
        void Warning(string format, params object[] arg);
        void Comment(string logString);
        void Comment(string format, params object[] arg);
        void Debug(string logString);

    }
}
