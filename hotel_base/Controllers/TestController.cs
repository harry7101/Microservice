﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace hotel_base.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TestController : ControllerBase
    {
        private readonly ILogger _logger;
        IConfiguration _configuration;
        public TestController(ILoggerFactory loggerFactory, IConfiguration configuration)
        {
            _logger = loggerFactory.CreateLogger<TestController>();
            _configuration = configuration;
        }

        [HttpGet("TestLogSeq")]
        public string TestLogSeq()
        {
            _logger.LogTrace("this is a test log for trace level .");
            _logger.LogDebug("this is a test log for debug level .");
            _logger.LogInformation("this is a test log for info level .");
            _logger.LogWarning("this is a test log for warning level .");
            _logger.LogError(new Exception("this is a ex for seq log ."), "this is a test log for error level .");

            return _configuration["serviceInfo:port"];
        }
    }
}
