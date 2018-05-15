﻿using System;
using Microsoft.AspNetCore.Http;

namespace Hellang.Middleware.ProblemDetails
{
    internal class ExceptionProblemDetails : StatusCodeProblemDetails
    {
        private static readonly string[] LineSeparators = { Environment.NewLine };

        public ExceptionProblemDetails(Exception error) : base(StatusCodes.Status500InternalServerError)
        {
            Detail = error.Message;
            Instance = GetHelpLink(error);
            StackTrace = GetStackTraceLines(error.StackTrace);
            Title = error.GetType().FullName;
        }

        public string[] StackTrace { get; set; }

        private static string GetHelpLink(Exception error)
        {
            var link = error.HelpLink;

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

        private static string[] GetStackTraceLines(string stackTrace)
        {
            if (string.IsNullOrEmpty(stackTrace))
            {
                return null;
            }

            return stackTrace.Split(LineSeparators, StringSplitOptions.RemoveEmptyEntries);
        }
    }
}
