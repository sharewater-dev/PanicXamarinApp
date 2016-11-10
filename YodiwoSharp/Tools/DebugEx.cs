using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Yodiwo
{
    public static class DebugEx
    {
        public enum TraceType
        {
            Log,
            Warning,
            Error,
        }
        public static event Action<TraceType, string> TraceLine;
        public static event Action<YIncident> ThrowYSharpIncident;

        [Conditional("TRACE"), Conditional("TRACE_ASSERT")]
        public static void Assert(bool Condition, string Error_Message, bool reportIt = true, [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0, [CallerMemberNameAttribute] string method = "")
        {
            if (!Condition)
            {
                Debug.Assert(false, Error_Message + Environment.NewLine);
                TraceError(Error_Message, filePath: filePath, lineNumber: lineNumber, method: method);

                var fileName = Path.GetFileName(filePath);
                var failure = new YIncident() { FilePath = filePath, FileName = fileName, LineNumber = lineNumber, Method = method, IsAssert = true };
#if NETFX
                failure.StackTrace = new StackTrace(true);
#endif
                failure.Messages.Add(Error_Message);

                if (reportIt)
                    ThrowYSharpIncident?.Invoke(failure);
            }
        }

        [Conditional("TRACE"), Conditional("TRACE_ASSERT")]
        public static void Assert(string Error_Message, bool reportIt = true, [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0, [CallerMemberNameAttribute] string method = "")
        {
            Debug.Assert(false, Error_Message + Environment.NewLine);
            TraceError(Error_Message, filePath: filePath, lineNumber: lineNumber, method: method);

            var fileName = Path.GetFileName(filePath);
            var failure = new YIncident() { FilePath = filePath, FileName = fileName, LineNumber = lineNumber, Method = method, IsAssert = true };
#if NETFX
            failure.StackTrace = new StackTrace(true);
#endif
            failure.Messages.Add(Error_Message);

            if (reportIt)
                ThrowYSharpIncident?.Invoke(failure);
        }

        [Conditional("TRACE")]
        public static void Assert(Exception ex, bool reportIt = true, [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0, [CallerMemberNameAttribute] string method = "")
        {
            var stack = ex?.StackTrace;
            var failure = new YIncident() { IsAssert = true };
            failure.StackTrace = ex != null ? new StackTrace(ex, true) : null;

            while (ex != null)
            {
                failure.Messages.Add(ex.Message);
                ex = ex.InnerException;
            }
            var msg = "Assert failed: " + Environment.NewLine
                + string.Join(Environment.NewLine, failure.Messages) + Environment.NewLine
                + "StackTrace:" + stack;

            Debug.Assert(false, msg);
            TraceError(msg, filePath: filePath, lineNumber: lineNumber, method: method);

            if (reportIt)
                ThrowYSharpIncident?.Invoke(failure);
        }

        [Conditional("TRACE"), Conditional("TRACE_ASSERT")]
        public static void Assert(Exception ex, string Error_Message, bool reportIt = true, [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0, [CallerMemberNameAttribute] string method = "")
        {
            var stack = ex?.StackTrace;
            var failure = new YIncident();
            failure.IsAssert = true;
            failure.StackTrace = ex != null ? new StackTrace(ex, true) : null;

            while (ex != null)
            {
                failure.Messages.Add(ex.Message);
                ex = ex.InnerException;
            }

            var msg = "Assert failed: " + Environment.NewLine
                + Error_Message + Environment.NewLine
                + string.Join(Environment.NewLine, failure.Messages) + Environment.NewLine
                + "StackTrace:" + stack;

            Debug.Assert(false, msg);
            TraceError(msg, filePath: filePath, lineNumber: lineNumber, method: method);

            if (reportIt)
                ThrowYSharpIncident?.Invoke(failure);
        }

        public static void AssertAndThrow(bool Condition, string Error_Message, [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0, [CallerMemberNameAttribute] string method = "")
        {
            if (!Condition)
            {
#if TRACE
                Debug.Assert(false, Error_Message + Environment.NewLine);
                TraceError(Error_Message, filePath: filePath, lineNumber: lineNumber, method: method);
#endif
                throw new Exception(Error_Message);
            }
        }

        public static void AssertAndThrow(string Error_Message, [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0, [CallerMemberNameAttribute] string method = "")
        {
#if TRACE
            Debug.Assert(false, Error_Message + Environment.NewLine);
            TraceError(Error_Message, filePath: filePath, lineNumber: lineNumber, method: method);
#endif
            throw new Exception(Error_Message);
        }

        [Conditional("TRACE"), Conditional("TRACE_LOG")]
        public static void TraceLog(Exception ex, string message, bool reportIt = true, [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0, [CallerMemberNameAttribute] string method = "")
        {
#if DEBUG
            try { var p = Path.GetFileName(filePath); } catch { Debug.Assert(false, "Invalid Log message"); };
#endif
            //two spaces after info not a typo
            while (ex != null)
            {
                var stack = ex.StackTrace;
#if NETFX
                try { Trace.WriteLine(String.Format("{0} Info  {1} [{2}:{3}] :: {4}", DateTime.Now, Path.GetFileName(filePath), method, lineNumber, message + " (" + ex.Message + ")" + Environment.NewLine + "StackTrace : " + stack)); } catch { }
#elif UNIVERSAL
                try { Debug.WriteLine(String.Format("{0} Info  {1} [{2}:{3}] :: {4}", DateTime.Now, Path.GetFileName(filePath), method, lineNumber, message + " (" + ex.Message + ")" + Environment.NewLine + "StackTrace : " + stack)); } catch { }
#endif
                ex = ex.InnerException;
            }
        }

        [Conditional("TRACE"), Conditional("TRACE_LOG")]
        public static void TraceLog(string message, [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0, [CallerMemberNameAttribute] string method = "")
        {
#if NETFX
#if DEBUG
            try { var p = Path.GetFileName(filePath); } catch { Debug.Assert(false, "Invalid Log message"); };
#endif
            //two spaces after info not a typo
            try { Trace.WriteLine(String.Format("{0} Info  {1} [{2}:{3}] :: {4}", DateTime.Now, Path.GetFileName(filePath), method, lineNumber, message)); } catch { }
#elif UNIVERSAL
            try { Debug.WriteLine(String.Format("{0} Info  {1} [{2}:{3}] :: {4}", DateTime.Now, Path.GetFileName(filePath), method, lineNumber, message)); } catch { }
#endif
            try { TraceLine?.Invoke(TraceType.Log, String.Format("{0} Info  {1} [{2}:{3}] :: {4}", DateTime.Now, Path.GetFileName(filePath), method, lineNumber, message)); } catch { }
        }

        [Conditional("TRACE"), Conditional("TRACE_LOG")]
        public static void TraceLogInlineBegin(string message, [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0, [CallerMemberNameAttribute] string method = "")
        {
#if NETFX
#if DEBUG
            try { var p = Path.GetFileName(filePath); } catch { Debug.Assert(false, "Invalid Log message"); };
#endif
            //two spaces after info not a typo
            try { Trace.Write(String.Format("{0} Info  {1} [{2}:{3}] :: {4}", DateTime.Now, Path.GetFileName(filePath), method, lineNumber, message)); } catch { }
#elif UNIVERSAL
            try { Debug.WriteLine(String.Format("{0} Info  {1} [{2}:{3}] :: {4}", DateTime.Now, Path.GetFileName(filePath), method, lineNumber, message)); } catch { }
#endif
            try { TraceLine?.Invoke(TraceType.Log, String.Format("{0} Info  {1} [{2}:{3}] :: {4}", DateTime.Now, Path.GetFileName(filePath), method, lineNumber, message)); } catch { }
        }

        [Conditional("TRACE"), Conditional("TRACE_LOG")]
        public static void TraceLogInline(string message)
        {
#if NETFX
            try { Trace.Write(message); } catch { }
#elif UNIVERSAL
            try { Debug.WriteLine(message); } catch { }
#endif
            try { TraceLine?.Invoke(TraceType.Log, message); } catch { }
        }

        [Conditional("TRACE"), Conditional("TRACE_LOG")]
        public static void TraceLogInlineEnd(string message)
        {
#if NETFX
            try { Trace.WriteLine(message); } catch { }
#elif UNIVERSAL
            try { Debug.WriteLine(message); } catch { }
#endif
            try { TraceLine?.Invoke(TraceType.Log, message); } catch { }
        }

        [Conditional("TRACE"), Conditional("TRACE_WARNING")]
        public static void TraceWarning(string message, [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0, [CallerMemberNameAttribute] string method = "")
        {
#if NETFX
#if DEBUG
            try { var p = Path.GetFileName(filePath); } catch { Debug.Assert(false, "Invalid Log message"); };
#endif
            //two spaces after warn not a typo
            try { Trace.WriteLine(String.Format("{0} Warn  {1} [{2}:{3}] :: {4}", DateTime.Now, Path.GetFileName(filePath), method, lineNumber, message)); } catch { }
#elif UNIVERSAL
            try { Debug.WriteLine(String.Format("{0} Warn  {1} [{2}:{3}] :: {4}", DateTime.Now, Path.GetFileName(filePath), method, lineNumber, message)); } catch { }
#endif
            try { TraceLine?.Invoke(TraceType.Warning, String.Format("{0} Warn  {1} [{2}:{3}] :: {4}", DateTime.Now, Path.GetFileName(filePath), method, lineNumber, message)); } catch { }
        }

        [Conditional("TRACE"), Conditional("TRACE_WARNING")]
        public static void TraceWarning(Exception ex, string message, [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0, [CallerMemberNameAttribute] string method = "")
        {
#if DEBUG
            try { var p = Path.GetFileName(filePath); } catch { Debug.Assert(false, "Invalid Log message"); };
#endif
            //two spaces after warn not a typo
            while (ex != null)
            {
                var stack = ex.StackTrace;
#if NETFX
                try { Trace.WriteLine(String.Format("{0} Warn  {1} [{2}:{3}] :: {4}", DateTime.Now, Path.GetFileName(filePath), method, lineNumber, message + " (" + ex.Message + ")" + Environment.NewLine + "StackTrace : " + stack)); } catch { }
#elif UNIVERSAL
                try { Debug.WriteLine(String.Format("{0} Warn  {1} [{2}:{3}] :: {4}", DateTime.Now, Path.GetFileName(filePath), method, lineNumber, message + " (" + ex.Message + ")" + Environment.NewLine + "StackTrace : " + stack)); } catch { }
#endif
                try { TraceLine?.Invoke(TraceType.Warning, String.Format("{0} Warn  {1} [{2}:{3}] :: {4}", DateTime.Now, Path.GetFileName(filePath), method, lineNumber, message + " (" + ex.Message + ")" + Environment.NewLine + "StackTrace : " + stack)); } catch { }
                ex = ex.InnerException;
            }
        }

        [Conditional("TRACE"), Conditional("TRACE_ERROR")]
        public static void TraceError(string message, [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0, [CallerMemberNameAttribute] string method = "")
        {
#if NETFX
#if DEBUG
            try { var p = Path.GetFileName(filePath); } catch { Debug.Assert(false, "Invalid Log message"); };
#endif
            try { Trace.WriteLine(String.Format("{0} ERROR {1} [{2}:{3}] :: {4}", DateTime.Now, Path.GetFileName(filePath), method, lineNumber, message)); } catch { }
#elif UNIVERSAL
            try { Debug.WriteLine(String.Format("{0} ERROR {1} [{2}:{3}] :: {4}", DateTime.Now, Path.GetFileName(filePath), method, lineNumber, message)); } catch { }
#endif
            try { TraceLine?.Invoke(TraceType.Error, String.Format("{0} ERROR {1} [{2}:{3}] :: {4}", DateTime.Now, Path.GetFileName(filePath), method, lineNumber, message)); } catch { }
        }

        [Conditional("TRACE"), Conditional("TRACE_ERROR")]
        public static void TraceError(Exception ex, string message, bool reportIt = true, [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0, [CallerMemberNameAttribute] string method = "")
        {
#if DEBUG
            try { var p = Path.GetFileName(filePath); } catch { Debug.Assert(false, "Invalid Log message"); };
#endif
            var fileName = Path.GetFileName(filePath);
            var failure = new YIncident() { FilePath = filePath, FileName = fileName, LineNumber = lineNumber, Method = method };
            failure.StackTrace = new StackTrace(ex, true);

#if NETFX
            try { Trace.WriteLine("Exception caught! Stack trace: " + ex.StackTrace + "" + Environment.NewLine); } catch { }
#elif UNIVERSAL
            try { Debug.WriteLine("Exception caught! Stack trace: " + ex.StackTrace + "" + Environment.NewLine); } catch { }
#endif
            try { TraceLine?.Invoke(TraceType.Error, "Exception caught! Stack trace: " + ex.StackTrace + "" + Environment.NewLine); } catch { }

            while (ex != null)
            {
                failure.Messages.Add(ex.Message);
#if NETFX
                try { Trace.WriteLine(String.Format("{0} ERROR {1} [{2}:{3}] :: {4}", DateTime.Now, Path.GetFileName(filePath), method, lineNumber, message + " (" + ex.Message + ")")); } catch { }
#elif UNIVERSAL
                try { Debug.WriteLine(String.Format("{0} ERROR {1} [{2}:{3}] :: {4}", DateTime.Now, Path.GetFileName(filePath), method, lineNumber, message + " (" + ex.Message + ")")); } catch { }
#endif
                try { TraceLine?.Invoke(TraceType.Error, String.Format("{0} ERROR {1} [{2}:{3}] :: {4}", DateTime.Now, Path.GetFileName(filePath), method, lineNumber, message + " (" + ex.Message + ")")); } catch { }
                ex = ex.InnerException;
            }

            if (reportIt)
                ThrowYSharpIncident?.Invoke(failure);
        }

        [Conditional("TRACE"), Conditional("TRACE_ERROR")]
        public static void TraceErrorException(Exception ex, string message = null, bool reportIt = true, [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0, [CallerMemberNameAttribute] string method = "")
        {
#if DEBUG
            try { var p = Path.GetFileName(filePath); } catch { Debug.Assert(false, "Invalid Log message"); };
#endif
            var fileName = Path.GetFileName(filePath);
            var failure = new YIncident() { FilePath = filePath, FileName = fileName, LineNumber = lineNumber, Method = method };
            failure.StackTrace = new StackTrace(ex, true);
            if (message != null)
                failure.Messages.Add(message);

#if NETFX
            try { Trace.WriteLine("Exception caught!" + (message == null ? "" : " Msg: " + message) + " Stack trace: " + ex.StackTrace + "" + Environment.NewLine); } catch { }
#elif UNIVERSAL
            try { Debug.WriteLine("Exception caught! Stack trace: " + ex.StackTrace + "" + Environment.NewLine); } catch { }
#endif
            try { TraceLine?.Invoke(TraceType.Error, "Exception caught! Stack trace: " + ex.StackTrace + "" + Environment.NewLine); } catch { }

            while (ex != null)
            {
                failure.Messages.Add(ex.Message);
#if NETFX
                try { Trace.WriteLine(String.Format("{0} ERROR {1} [{2}:{3}] :: {4}", DateTime.Now, fileName, method, lineNumber, "Exception : " + ex.Message + "" + Environment.NewLine)); } catch { }
#elif UNIVERSAL
                Debug.WriteLine(String.Format("{0} ERROR {1} [{2}:{3}] :: {4}", DateTime.Now, fileName, method, lineNumber, "Exception : " + ex.Message + "" + Environment.NewLine));
#endif
                try { TraceLine?.Invoke(TraceType.Error, String.Format("{0} ERROR {1} [{2}:{3}] :: {4}", DateTime.Now, fileName, method, lineNumber, "Exception : " + ex.Message + "" + Environment.NewLine)); } catch { }
                ex = ex.InnerException;
            }

            if (reportIt)
                ThrowYSharpIncident?.Invoke(failure);
        }
    }
}