using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AnalyticsService.Abstraction;
using AnalyticsService.Models;
using AnalyticsService.Models.DBModels;
using AnalyticsService.Models.ResponseModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AnalyticsService.Controllers
{
    [Route("api")]
    [ApiController]
    public class AnalyticsController : ControllerBase
    {
        private readonly IAnalyticsRepository _analyticsRepository;
        public AnalyticsController(IAnalyticsRepository analyticsRepository)
        {
            _analyticsRepository = analyticsRepository;
        }

        [HttpPost]
        [Route("analytics/promotions")]
        public IActionResult Post(AnalyticsModel model)
        {
            dynamic response = _analyticsRepository.InsertAnalytics(model);
            return StatusCode((int)response.statusCode, response);
        }


        [HttpPost]
        [Route("analytics/linkslog")]
        public IActionResult PostLinksLog(LinkLogsModel model)
        {
            dynamic response = _analyticsRepository.InsertLinksLog(model);
            return StatusCode((int)response.statusCode, response);
        }

        [HttpGet]
        [Route("analytics/promotions/lastdate")]
        public IActionResult Get(string Include, string type, [FromQuery] Pagination pageInfo)
        {
            dynamic response = _analyticsRepository.GetAnalytics(Include, type, pageInfo);
            return StatusCode((int)response.statusCode, response);
        }

        [HttpGet]
        [Route("analytics/promotions")]
        public IActionResult GetAnalyticsData(string analyticsId, string start_at, string end_at, [FromQuery] Pagination pageInfo)
        {
            dynamic response = _analyticsRepository.GetAnalyticsData(analyticsId, start_at, end_at, pageInfo);
            return StatusCode((int)response.statusCode, response);
        }
    }
}
