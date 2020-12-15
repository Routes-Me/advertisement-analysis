using AnalyticsService.Abstraction;
using AnalyticsService.Helper;
using AnalyticsService.Models;
using AnalyticsService.Models.Common;
using AnalyticsService.Models.DBModels;
using AnalyticsService.Models.ResponseModel;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace AnalyticsService.Repository
{
    public class IncludedRepository : IIncludedRepository
    {

        private readonly AppSettings _appSettings;
        private readonly Dependencies _dependencies;
        private readonly analyticsserviceContext _context;

        public IncludedRepository(IOptions<AppSettings> appSettings, analyticsserviceContext context, IOptions<Dependencies> dependencies)
        {
            _appSettings = appSettings.Value;
            _context = context;
            _dependencies = dependencies.Value;
        }
        public dynamic GetAdvertisementsIncludedData(List<PromotionAnalyticsModel> analyticsModelList)
        {
            List<AdvertisementsModel> lstAdvertisements = new List<AdvertisementsModel>();
            foreach (var item in analyticsModelList)
            {
                var client = new RestClient(_appSettings.Host + _dependencies.AdvertisementsUrl + item.AdvertisementId);
                var request = new RestRequest(Method.GET);
                IRestResponse response = client.Execute(request);
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var result = response.Content;
                    var advertisementsData = JsonConvert.DeserializeObject<AdvertisementData>(result);
                    lstAdvertisements.AddRange(advertisementsData.data);
                }
            }
            var advertisementsList = lstAdvertisements.GroupBy(x => x.AdvertisementId).Select(a => a.First()).ToList();
            return Common.SerializeJsonForIncludedRepo(advertisementsList.Cast<dynamic>().ToList());
        }

        public dynamic GetInstitutionsIncludedData(List<PromotionAnalyticsModel> analyticsModelList)
        {
            List<InstitutionsModel> lstInstitutions = new List<InstitutionsModel>();
            foreach (var item in analyticsModelList)
            {
                var client = new RestClient(_appSettings.Host + _dependencies.InstitutionUrl + item.InstitutionId);
                var request = new RestRequest(Method.GET);
                IRestResponse response = client.Execute(request);
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var result = response.Content;
                    var institutionsData = JsonConvert.DeserializeObject<InstitutionsData>(result);
                    lstInstitutions.AddRange(institutionsData.data);
                }
            }
            var institutionsList = lstInstitutions.GroupBy(x => x.InstitutionId).Select(a => a.First()).ToList();
            return Common.SerializeJsonForIncludedRepo(institutionsList.Cast<dynamic>().ToList());
        }
    }
}
