using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieApi.Filters
{
    public class MyGlobalExceptionFilter: ExceptionFilterAttribute
    {
        private readonly ILogger<MyGlobalExceptionFilter> _logger;

        public MyGlobalExceptionFilter(ILogger<MyGlobalExceptionFilter> logger)
        {
            _logger = logger;
        }

        public override void OnException(ExceptionContext context)
        {
            
            _logger.LogError(context.Exception, context.Exception.Message);
            base.OnException(context);
        }

    }
}
