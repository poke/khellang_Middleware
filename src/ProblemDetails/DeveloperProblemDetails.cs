using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Internal;
using Microsoft.Extensions.StackTrace.Sources;

namespace Hellang.Middleware.ProblemDetails
{
    public static class DeveloperProblemDetails
    {
        internal static IEnumerable<ErrorDetails> GetErrors(IEnumerable<ExceptionDetails> details)
        {
            foreach (var detail in details)
            {
                yield return new ErrorDetails(detail);
            }
        }

        internal static string GetHelpLink(Exception exception)
        {
            var link = exception.HelpLink;

            if (string.IsNullOrEmpty(link))
            {
                return null;
            }

            if (Uri.TryCreate(link, UriKind.Absolute, out var result))
            {
                return result.ToString();
            }

            return null;
        }
        
        public class ErrorDetails
        {
            internal ErrorDetails(ExceptionDetails detail)
            {
                Raw = detail.Error.ToString();
                Message = detail.ErrorMessage ?? detail.Error.Message;
                Type = TypeNameHelper.GetTypeDisplayName(detail.Error.GetType());
                StackFrames = GetStackFrames(detail.StackFrames).ToList();
            }

            public string Message { get; }

            public string Type { get; }
            
            public string Raw { get; }
            
            public IReadOnlyCollection<StackFrame> StackFrames { get; }
            
            private static IEnumerable<StackFrame> GetStackFrames(IEnumerable<StackFrameSourceCodeInfo> stackFrames)
            {
                foreach (var stackFrame in stackFrames)
                {
                    yield return new StackFrame
                    {
                        FilePath = stackFrame.File,
                        FileName = string.IsNullOrEmpty(stackFrame.File) ? null : Path.GetFileName(stackFrame.File),
                        Function = stackFrame.Function,
                        Line = GetLineNumber(stackFrame.Line),
                        PreContextLine = GetLineNumber(stackFrame.PreContextLine),
                        PreContextCode = GetCode(stackFrame.PreContextCode),
                        ContextCode = GetCode(stackFrame.ContextCode),
                        PostContextCode = GetCode(stackFrame.PostContextCode),
                    };
                }
            }

            private static int? GetLineNumber(int lineNumber)
            {
                if (lineNumber == 0)
                {
                    return null;
                }

                return lineNumber;
            }

            private static IReadOnlyCollection<string> GetCode(IEnumerable<string> code)
            {
                var list = code.ToList();
                return list.Count > 0 ? list : null;
            }

            public class StackFrame
            {
                public string FilePath { get; set; }

                public string FileName { get; set; }
                
                public string Function { get; set; }
                
                public int? Line { get; set; }
                
                public int? PreContextLine { get; set; }
                
                public IReadOnlyCollection<string> PreContextCode { get; set; }
                
                public IReadOnlyCollection<string> ContextCode { get; set; }
                
                public IReadOnlyCollection<string> PostContextCode { get; set; }
            }
        }
    }
}
