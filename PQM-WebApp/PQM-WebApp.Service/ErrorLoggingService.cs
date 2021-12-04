using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Nest;
using PQM_WebApp.Data.Models;

namespace PQM_WebApp.Service
{

    public interface IErrorLoggingService
    {
        public ResultModel Create(ErrorLoggingModel error);
        public bool CreateFromResultModel(ResultModel result, object raw, ErrorDetailLogging detail = null, List<ErrorDetailLogging> errorDetails = null);
        public ResultModel Get(int pageIndex = 0, int pageSize = 10, DateTime? from = null, DateTime? to = null, string code = null);
    }

    public class ErrorLoggingService : IErrorLoggingService
    {
        private IConfiguration Configuration { get; }
        private readonly ElasticClient _elasticClient;
        private string _index;

        public ErrorLoggingService(ElasticClient elasticClient, IConfiguration configuration)
        {
            _elasticClient = elasticClient;
            Configuration = configuration;
            _index = Configuration["elasticsearch:error_logging_index"];
            CreateIndex();
        }

        private void CreateIndex()
        {
            var getResponse = _elasticClient.Indices.Get(_index);
            if (!getResponse.IsValid)
            {
                var createIndexResponse = _elasticClient.Indices.Create(_index,
                                                                    index => index.Map<ErrorLoggingModel>(x => x.AutoMap())
                                                            );
                _elasticClient.Indices.UpdateSettings(_index, u => u
                                .IndexSettings(di => di
                                    .Setting("index.max_result_window", int.MaxValue)
                                )
                            );
            }
        }

        public ResultModel Create(ErrorLoggingModel error)
        {
            var rs = new ResultModel()
            {
                Succeed = true
            };
            try
            {
                var response = _elasticClient.Index<ErrorLoggingModel>(error, s => s.Index(_index));
                if (!response.IsValid)
                {
                    rs.Succeed = false;
                    rs.Error.ErrorMessage = response.DebugInformation;
                }
            }
            catch (Exception ex)
            {
                rs.Succeed = false;
                rs.Error.ErrorMessage = ex.Message;
            }
            return rs;
        }

        public bool CreateFromResultModel(ResultModel result, object raw, ErrorDetailLogging detail = null, List<ErrorDetailLogging> errorDetails = null)
        {
            //check case error from root object
            if (result.Succeed == false && (result.Error.Code == "01" || result.Error.Code == "02" || result.Error.Code == "03"))
            {
                var error = new ErrorLoggingModel
                {
                    Id = Guid.NewGuid(),
                    DateTime = DateTime.Now,
                    RawData = raw,
                    Result = result,
                    Detail = detail
                };
                Create(error);
            }
            else
            {
                //error from data (array of aggregated value)
                errorDetails.ForEach(detail =>
                {
                    var error = new ErrorLoggingModel
                    {
                        Id = Guid.NewGuid(),
                        DateTime = DateTime.Now,
                        RawData = raw,
                        Result = result,
                        Detail = detail
                    };
                    Create(error);
                });
            }
            return true;
        }

        public ResultModel Get(int pageIndex = 0, int pageSize = 10, DateTime? from = null, DateTime? to = null, string code = null)
        {
            var rs = new ResultModel { Succeed = true };
            try
            {
                var queryContainers = new List<QueryContainer>();
                var filterContainers = new List<QueryContainer>();
                var shouldContainers = new List<QueryContainer>();
                if (from != null)
                {
                    queryContainers.Add((new QueryContainerDescriptor<ErrorLoggingModel>()).DateRange(s => s.Field(f => f.DateTime).GreaterThanOrEquals(from)));
                }
                if (to != null)
                {
                    queryContainers.Add((new QueryContainerDescriptor<ErrorLoggingModel>()).DateRange(s => s.Field(f => f.DateTime).LessThanOrEquals(to)));
                }
                if (!string.IsNullOrEmpty(code))
                {
                    queryContainers.Add((new QueryContainerDescriptor<ErrorLoggingModel>()).Term(s => s.Field("detail.code").Value(code)));
                }
                var searchRequest = new SearchRequest<ErrorLoggingModel>(_index)
                {
                    From = pageIndex * pageSize,
                    Size = pageSize,
                    Query = new BoolQuery { Must = queryContainers, Filter = filterContainers, Should = shouldContainers },
                };
                var response = _elasticClient.Search<ErrorLoggingModel>(searchRequest);
                rs.Data = response.Documents;
            }
            catch (Exception ex)
            {
                rs.Succeed = false;
                rs.Error.ErrorMessage = ex.Message;
            }
            return rs;
        }
    }
}
