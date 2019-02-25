using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Internal;
using Microsoft.Extensions.StackTrace.Sources;

namespace Hellang.Middleware.ProblemDetails
{
    public class ExceptionProblemDetails : StatusCodeProblemDetails
    {
        private readonly Exception _exception;

        public ExceptionProblemDetails(Exception error) : this(error, StatusCodes.Status500InternalServerError)
        {
        }

        public ExceptionProblemDetails(Exception error, int statusCode) : base(statusCode)
        {
            _exception = error ?? throw new ArgumentNullException(nameof(error));

            Detail = _exception.Message;
            Title = TypeNameHelper.GetTypeDisplayName(_exception.GetType());
            Instance = DeveloperProblemDetails.GetHelpLink(_exception);
        }

        public Exception Error => _exception;

        public IReadOnlyCollection<DeveloperProblemDetails.ErrorDetails> Errors { get; private set; }

        internal ExceptionProblemDetails WithDetails(IEnumerable<ExceptionDetails> details)
        {
            Errors = DeveloperProblemDetails.GetErrors(details).ToList();
            return this;
        }
    }
}
