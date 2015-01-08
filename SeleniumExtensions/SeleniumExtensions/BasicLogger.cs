using System;
using SeleniumExtensions.Interfaces;

namespace SeleniumExtensions
{
    public class BasicLogger : ITestLogger
    {

        public void TestAction(string logString)
        {
            Console.WriteLine("  {0}", logString);
        }

        public void TestAction(string format, params object[] arg)
        {
            Log("  " + format, arg);
        }

        public void Error(string logString)
        {
            Console.WriteLine("ERROR: {0}", logString);
        }

        public void Error(string format, params object[] arg)
        {
            Log("ERROR: " + format, arg);
        }

        public void Warning(string logString)
        {
            Console.WriteLine("WARNING: {0}", logString);
        }

        public void Warning(string format, params object[] arg)
        {
            Log("WARNING: " + format, arg);
        }

        public void Comment(string logString)
        {
            Console.WriteLine(logString);
        }

        public void Comment(string format, params object[] arg)
        {
            Log(format, arg);
        }

        public void Debug(string logString)
        {
            System.Diagnostics.Debug.WriteLine(logString);
        }

        private static void Log(string format, params object[] arg)
        {
            if (arg == null)
            {
                Console.WriteLine(format, null, null);
            }
            else
            {
                Console.WriteLine(format, arg);
            }
        }
    }
}
