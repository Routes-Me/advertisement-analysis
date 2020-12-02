using AnalyticsService.Abstraction;
using AnalyticsService.Models;
using AnalyticsService.Models.ResponseModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AnalyticsService.Repository
{
    public class AnalyticsRepository : IAnalyticsRepository
    {
        public dynamic GetAnalytics(string include, Pagination pageInfo)
        {
            throw new NotImplementedException();
        }

        public dynamic InsertAnalytics(AnalyticsModel model)
        {
            throw new NotImplementedException();
        }
    }
}
