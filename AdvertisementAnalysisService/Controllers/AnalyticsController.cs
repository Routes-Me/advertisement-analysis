using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AdvertisementAnalysisService.Abstraction;
using AdvertisementAnalysisService.Models;
using AdvertisementAnalysisService.Models.DBModels;
using AdvertisementAnalysisService.Models.ResponseModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AdvertisementAnalysisService.Controllers
{
    [ApiController]
    [ApiVersion( "1.0" )]
    [Route("v{version:apiVersion}/")]
    public class AnalyticsController : ControllerBase
    {
        private readonly IAnalyticsRepository _analyticsRepository;
        private readonly AnalyticsContext _context;
        public AnalyticsController(IAnalyticsRepository analyticsRepository, AnalyticsContext context)
        {
            _analyticsRepository = analyticsRepository;
            _context = context;
        }

        [HttpPost]
        [Route("analytics/promotions")]
        public IActionResult Post(AnalyticsModel model)
        {
            dynamic response = _analyticsRepository.InsertAnalytics(model);
            return StatusCode(response.statusCode, response);
        }


        [HttpPost]
        [Route("analytics/linkslog")]
        public IActionResult PostLinksLog(LinkLogsModel model)
        {
            dynamic response = _analyticsRepository.InsertLinksLog(model);
            return StatusCode(response.statusCode, response);
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
                return StatusCode(StatusCodes.Status400BadRequest, errorResponse.message);
            }
            dynamic response = ReturnResponse.SuccessResponse(CommonMessage.AnalyticsInsert, true);
            return StatusCode(response.statusCode, response);
        }

        [HttpGet]
        [Route("analytics/playbacks")]
        public IActionResult GetPlaybacks(string startAt, string endAt, [FromQuery] Pagination pageInfo)
        {
            PlaybacksGetResponse response = new PlaybacksGetResponse();
            try
            {
                response = _analyticsRepository.GetPlaybacks(startAt, endAt, pageInfo);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new ErrorResponse{ Error = ex.Message });
            }
            return StatusCode(StatusCodes.Status200OK, response);
        }

        [HttpGet]
        [Route("analytics/linklogs")]
        public IActionResult GetLinkLogs(string startAt, string endAt, [FromQuery] Pagination pageInfo)
        {
            LinkLogsGetResponse response = new LinkLogsGetResponse();
            try
            {
                response = _analyticsRepository.GetLinkLogs(startAt, endAt, pageInfo);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new ErrorResponse{ Error = ex.Message });
            }
            return StatusCode(StatusCodes.Status200OK, response);
        }

        [HttpGet]
        [Route("analytics/promotions/lastdate")]
        public IActionResult Get(string Include, string type, [FromQuery] Pagination pageInfo)
        {
            dynamic response = _analyticsRepository.GetAnalytics(Include, type, pageInfo);
            return StatusCode(response.statusCode, response);
        }

        [HttpGet]
        [Route("analytics/promotions/{analyticId?}")]
        public IActionResult GetAnalyticsData(string analyticsId, string institutionId, List<SearchDto> groupBy, string start_at, string end_at, string include,[FromQuery] Pagination pageInfo)
        {
            dynamic response = _analyticsRepository.GetAnalyticsData(analyticsId, institutionId, groupBy, start_at, end_at, include, pageInfo);
            return StatusCode(response.statusCode, response);
        }

        [HttpGet]
        [Route("analytics/drivers/{driverId}/linklogs")]
        public IActionResult GetDriversLinkLog(string driverId,  string startAt, string endAt, [FromQuery] Pagination pageInfo)
        {
            try
            {
                dynamic response = _analyticsRepository.GetDriversLinkLogs(driverId, startAt,  endAt, pageInfo);
                return StatusCode(response.statusCode, response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new ErrorResponse { Error = ex.Message });
            }
        }
    }
}
