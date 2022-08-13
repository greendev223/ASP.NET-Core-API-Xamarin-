using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Xamarin.Forms;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace MukaiTablet2.Util
{
    public class Logger
    {
        private static Logger mInstance = null;
        private readonly string LOGDIR;
        private readonly IDepend mDep = null;
        private readonly Object mLockObj = new Object();

        public static Logger Inst
        {
            get
            {
                if (mInstance == null) { mInstance = new Logger(); }
                return mInstance;
            }
        }

        private Logger()
        {
            mDep = DependencyService.Get<IDepend>();
            LOGDIR = mDep.GetLogDirPath();
            if (Directory.Exists(LOGDIR) == false) Directory.CreateDirectory(LOGDIR);
        }

        public static void Info(string log, int skipFrame = 2, [CallerLineNumber]int lineNo = 0)
        {
            Inst.WriteLine(log, skipFrame, lineNo);
        }

        public void WriteLine(string log, int skipFrame = 1, [CallerLineNumber]int lineNo = 0)
        {
            lock (mLockObj)
            {

                string filePath = LOGDIR + DateTime.Now.ToString("yyyyMMdd") + "_log.txt";
/*
                StackFrame callerFrame = new StackFrame(skipFrame, true);
                string className = callerFrame.GetMethod().ReflectedType.FullName;
                string methodName = callerFrame.GetMethod().Name;
                string dateTime = DateTime.Now.ToString("yyMMdd_hhmmss");
*/
                string className = "";
                string methodName = "";
                string dateTime = DateTime.Now.ToString("yyMMdd_hhmmss");

                log = dateTime + ":" + className + ":" + methodName + ":" + lineNo + ":" + log;
                try
                {
                    using (StreamWriter writer = new StreamWriter(filePath, true))
                    {
                        writer.WriteLine(log);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
                Console.WriteLine(log);
                            }
        }

        public void Assert(bool isSuccess, string message = "", [CallerLineNumber]int lineNo = 0)
        {
            if (isSuccess) return;
            WriteLine("[ASSERT]:" + message, 2, lineNo);
        }
    }
}
