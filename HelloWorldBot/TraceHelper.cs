using Microsoft.ApplicationInsights;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelloWorldBot
{
    [Flags]
    public enum LogOptions
    {
        /// <summary>
        /// Log entry into the method
        /// </summary>
        Entry = 0x01,
        /// <summary>
        /// Log exit from the method
        /// </summary>
        Exit = 0x02,
        /// <summary>
        /// The error
        /// </summary>
        Error = 0x03,
        /// <summary>
        /// Log the execution time of the method
        /// </summary>
        ExecutionTime = 0x04,
        /// <summary> 
        /// Log all data 
        /// </summary> 
        All = 0xFF
    }

    public class TraceHelper : IDisposable
    {
        public static IDisposable Trace(string methodName, IDialogContext context, LogOptions options)
        {
            // If logging off then return null, else
            return new TraceHelper(methodName, options, context);
        }

        public static IDisposable Trace(string methodName, Activity activity, LogOptions options)
        {
            // If logging off then return null, else
            return new TraceHelper(methodName, options, null, activity);
        }

        public static IDisposable Trace(string methodName, IDialogContext context, Exception ex)
        {
            // If logging off then return null, else
            return new TraceHelper(methodName, LogOptions.Error, context, null, ex);
        }

        public static IDisposable Trace(string methodName, LogOptions options)
        {
            // If logging off then return null, else
            return new TraceHelper(methodName, options);
        }

        /// <summary>
        /// Ctor now private - just called from the static Log method
        /// </summary>
        /// <param name="methodName">The name of the method being logged</param>
        /// <param name="options">The log options</param>
        private TraceHelper(string methodName, LogOptions options, IDialogContext context = null, Activity activity = null, Exception ex = null)
        {
            _methodName = methodName;
            _options = options;
            _context = context;
            _activity = context != null && context.Activity != null ? context.Activity as Activity: null;
            _activityId = _activity != null ? _activity.Id : string.Empty;
            _exception = ex;

            if ((_options & LogOptions.ExecutionTime) == LogOptions.ExecutionTime)
            {
                _sw = new Stopwatch();
                _sw.Start();
            }

            if ((_options & LogOptions.Entry) == LogOptions.Entry)
            {
                telemetry.TrackTrace(
                    $"Entering {_methodName}",
                    Microsoft.ApplicationInsights.DataContracts.SeverityLevel.Information);
            }
        }

        /// <summary>
        /// Tidy up
        /// </summary>
        public void Dispose()
        {
            if ((_options & LogOptions.Error) == LogOptions.Error && _exception != null)
            {
                var dic = _activity == null ? new Dictionary<string, string>() :
                    new Dictionary<string, string> {
                        { "Identifier", $"{_methodName}_{_activityId}" },
                        { "UserQuery", _activity.Text },
                        { "ConversationId", _activity.Conversation.Id },
                        { "MessageId", _activity.Id ?? _activity.ReplyToId },
                        { "UserId", _activity.From.Id },
                        { "ElapsedTime", _sw.ElapsedMilliseconds.ToString() }
                    };
                dic.Add("Exception", _exception.Message);
                dic.Add("Stack Trace", _exception.StackTrace);

                telemetry.TrackTrace(
                    $"Exception @ {_methodName}",
                    Microsoft.ApplicationInsights.DataContracts.SeverityLevel.Error, dic);
            }

            if ((_options & LogOptions.ExecutionTime) == LogOptions.ExecutionTime)
            {
                _sw.Stop();

                var dic = _activity == null ? new Dictionary<string, string>() :
                     new Dictionary<string, string> {
                        { "Identifier", $"{_methodName}_{_activityId}" },
                        { "UserQuery", _activity.Text },
                        { "ConversationId", _activity.Conversation.Id },
                        { "MessageId", _activity.Id ?? _activity.ReplyToId },
                        { "UserId", _activity.From.Id },
                        { "ElapsedTime", _sw.ElapsedMilliseconds.ToString() }
                     };

                telemetry.TrackTrace(
                    $"Completing {_methodName}",
                    Microsoft.ApplicationInsights.DataContracts.SeverityLevel.Information, dic);
            }

            if ((_options & LogOptions.Exit) == LogOptions.Exit)
            {
                telemetry.TrackTrace(
                    $"Exiting {_methodName}",
                    Microsoft.ApplicationInsights.DataContracts.SeverityLevel.Information);
            }
        }

        private string _methodName;
        private LogOptions _options;
        private Stopwatch _sw;
        private Activity _activity;
        private IDialogContext _context;
        private string _activityId;
        private readonly TelemetryClient telemetry = new TelemetryClient();
        private Exception _exception;
    }
}
