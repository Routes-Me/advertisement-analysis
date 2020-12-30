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
        private readonly analyticsserviceContext _context;
        public AnalyticsController(IAnalyticsRepository analyticsRepository, analyticsserviceContext context)
        {
            _analyticsRepository = analyticsRepository;
            _context = context;
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

        [HttpPost]
        [Route("analytics/devices/{deviceId}/playbacks")]
        public async Task<IActionResult> PostPlaybacks(string deviceId, List<PlaybackDto> playbackDtoList)
        {
            try
            {
                List<Playback> playbacksList = _analyticsRepository.InsertPlaybacks(deviceId, playbackDtoList);
                _context.Playbacks.AddRange(playbacksList);
                await _context.SaveChangesAsync();
            }
            catch (ArgumentNullException ex)
            {
                return StatusCode(StatusCodes.Status422UnprocessableEntity, ex.Message);
            }
            catch (Exception ex)
            {
                dynamic errorResponse = ReturnResponse.ExceptionResponse(ex);
                return StatusCode((int)errorResponse.statusCode, errorResponse);
            }
            dynamic response = ReturnResponse.SuccessResponse(CommonMessage.AnalyticsInsert, true);
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
        [Route("analytics/promotions/{id?}")]
        public IActionResult GetAnalyticsData(string id, string start_at, string end_at, string include,[FromQuery] Pagination pageInfo)
        {
            dynamic response = _analyticsRepository.GetAnalyticsData(id, start_at, end_at, include, pageInfo);
            return StatusCode((int)response.statusCode, response);
        }
    }
}
