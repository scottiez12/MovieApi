//using Microsoft.AspNetCore.Mvc.Filters;
//using Microsoft.Extensions.Logging;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;

//namespace MovieApi.Filters
//{
//    public class MyActionFilter : IActionFilter
//    {
//        private readonly ILogger<MyActionFilter> _logger;

//        public MyActionFilter(ILogger<MyActionFilter> logger)
//        {
//            _logger = logger;
//        }

//        public void OnActionExecuting(ActionExecutingContext context)
//        {
//            _logger.LogWarning("on action executing");
//        }

//        public void OnActionExecuted(ActionExecutedContext context)
//        {
//            _logger.LogWarning("on action executed");
//        }

//    }
//}
