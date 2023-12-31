﻿namespace Paradigm.Server.Handlers
{
    using System;    
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;
    using Microsoft.Extensions.Logging;

    using Paradigm.Server.Model;

    public class GlobalExceptionFilter : IExceptionFilter
    {
        private readonly ILogger logger;

        public GlobalExceptionFilter(ILogger<GlobalExceptionFilter> logger)
        {
            if (logger == null)
            {
                throw new ArgumentNullException(nameof(logger));
            }

            this.logger = logger;
        }

        public void OnException(ExceptionContext context)
        {
            var response = new ErrorResponse()
            {
                Message = context.Exception.Message,
                StackTrace = context.Exception.StackTrace
            };

            context.Result = new ObjectResult(response)
            {
                StatusCode = 500,
                DeclaredType = typeof(ErrorResponse)
            };

            this.logger.LogError(context.Exception, context.Exception.Message);
        }
    }
}
