using AnalyticsService.Models;
using AnalyticsService.Models.ResponseModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AnalyticsService.Abstraction
{
    public interface IAnalyticsRepository
    {
        dynamic InsertAnalytics(AnalyticsModel model);
        dynamic GetAnalytics(string include, string type, Pagination pageInfo);
    }
}
