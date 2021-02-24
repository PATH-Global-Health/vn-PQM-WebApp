﻿using Mapster;
using PQM_WebApp.Data;
using PQM_WebApp.Data.Entities;
using PQM_WebApp.Data.Models;
using PQM_WebApp.Data.ViewModels;
using System;
using System.Linq;
using System.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;



namespace PQM_WebApp.Service
{
    public interface IIndicatorService
    {
        public ResultModel Create(IndicatorCreateModel model);
        public ResultModel Get();
        public ResultModel ImportExcel(IFormFile file, string type = "");
    }

    public class IndicatorService : IIndicatorService
    {
        private readonly AppDBContext _dbContext;

        public IndicatorService(AppDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public ResultModel Create(IndicatorCreateModel model)
        {
            var rs = new ResultModel();
            try
            {
                var indicator = model.Adapt<Indicator>();
                indicator.Id = Guid.NewGuid();
                _dbContext.Indicators.Add(indicator);
                rs.Succeed = _dbContext.SaveChanges() > 0;
                if (rs.Succeed)
                {
                    rs.Data = indicator.Adapt<IndicatorViewModel>();
                    return rs;
                }
                return rs;
            }
            catch (Exception e)
            {
                rs.Succeed = false;
                rs.ErrorMessage = e.InnerException != null ? e.InnerException.Message : e.Message;
                return rs;
            }
        }

        public ResultModel Get()
        {
            var rs = new ResultModel();
            try
            {
                rs.Succeed = true;
                rs.Data = _dbContext.Indicators.Select(s => s.Adapt<IndicatorViewModel>()).ToList();
                return rs;
            }
            catch (Exception e)
            {
                rs.Succeed = false;
                rs.ErrorMessage = e.InnerException != null ? e.InnerException.Message : e.Message;
                return rs;
            }
        }

        public ResultModel ImportIndicator(List<IndicatorImportModel> importValues, string type = "")
        {
            if (type == "VL")
            {
                for (var i = 0; i < importValues.Count; i++)
                {
                    var value = importValues[i];
                    var month = _dbContext.DimMonths.FirstOrDefault(m => m.Year.Year == value.Year && m.MonthNumOfYear == value.Month);
                    var gender = _dbContext.Sex.FirstOrDefault(s => s.Name.Equals(value.Gender));
                    var ageGroup = _dbContext.AgeGroups.FirstOrDefault(a => a.Name.Equals(value.AgeGroup));
                    var keyPopulation = _dbContext.KeyPopulations.FirstOrDefault(k => k.Name.Equals(value.KeyPopulation));
                    var site = _dbContext.Sites.FirstOrDefault(s => s.Name.Equals(value.Site));
                    var indicator = _dbContext.Indicators.FirstOrDefault(i => i.Name.Equals(value.Indicator));
                    var cIndicator = _dbContext.Indicators.FirstOrDefault(i => i.Name.Equals("TX_Curr"));

                    if (month != null && gender != null && ageGroup != null && keyPopulation != null && site != null && indicator != null)
                    {
                        var _v = _dbContext.AggregatedValues
                            .FirstOrDefault(
                            s => s.IndicatorId == indicator.Id
                            && s.AgeGroupId == ageGroup.Id
                            && s.KeyPopulationId == keyPopulation.Id
                            && s.SexId == gender.Id
                            && s.SiteId == site.Id
                            && s.MonthId == month.Id);
                        var _c = _dbContext.AggregatedValues
                            .FirstOrDefault(
                            s => s.IndicatorId == cIndicator.Id
                            && s.AgeGroupId == ageGroup.Id
                            && s.KeyPopulationId == keyPopulation.Id
                            && s.SexId == gender.Id
                            && s.SiteId == site.Id
                            && s.MonthId == month.Id);
                        if (_c == null)
                        {
                            continue;
                        }
                        if (_v == null)
                        {
                            var _value = new AggregatedValue
                            {
                                Id = Guid.NewGuid(),
                                AgeGroupId = ageGroup.Id,
                                SexId = gender.Id,
                                KeyPopulationId = keyPopulation.Id,
                                SiteId = site.Id,
                                Numerator = value.Numerator,
                                Denominator = _c.Value,
                                Value = value.Value,
                                DataType = value.ValueType == 1 ? Data.Enums.DataType.Number : Data.Enums.DataType.Percent,
                                CreatedBy = "",
                                DateId = null,
                                MonthId = month.Id,
                                IsDeleted = false,
                                DateCreated = DateTime.Now,
                                IndicatorId = indicator.Id,
                            };
                            _dbContext.AggregatedValues.Add(_value);
                        }
                        else
                        {
                            _v.Value = value.Value;
                            _v.Numerator = value.Numerator;
                            _v.Denominator = _c.Value;
                            _dbContext.AggregatedValues.Update(_v);
                        }
                    }
                    else
                    {
                        Console.WriteLine(JsonConvert.SerializeObject(value));
                    }
                }
                _dbContext.SaveChanges();
                return new ResultModel();
            }
            else
            {
                for (var i = 0; i < importValues.Count; i++)
                {
                    var value = importValues[i];
                    var month = _dbContext.DimMonths.FirstOrDefault(m => m.Year.Year == value.Year && m.MonthNumOfYear == value.Month);
                    var gender = _dbContext.Sex.FirstOrDefault(s => s.Name.Equals(value.Gender));
                    var ageGroup = _dbContext.AgeGroups.FirstOrDefault(a => a.Name.Equals(value.AgeGroup));
                    var keyPopulation = _dbContext.KeyPopulations.FirstOrDefault(k => k.Name.Equals(value.KeyPopulation));
                    var site = _dbContext.Sites.FirstOrDefault(s => s.Name.Equals(value.Site));
                    var indicator = _dbContext.Indicators.FirstOrDefault(i => i.Name.Equals(value.Indicator));

                    if (month != null && gender != null && ageGroup != null && keyPopulation != null && site != null && indicator != null)
                    {
                        var _v = _dbContext.AggregatedValues
                            .FirstOrDefault(
                            s => s.IndicatorId == indicator.Id
                            && s.AgeGroupId == ageGroup.Id
                            && s.KeyPopulationId == keyPopulation.Id
                            && s.SexId == gender.Id
                            && s.SiteId == site.Id
                            && s.MonthId == month.Id);
                        if (_v == null)
                        {
                            var _value = new AggregatedValue
                            {
                                Id = Guid.NewGuid(),
                                AgeGroupId = ageGroup.Id,
                                SexId = gender.Id,
                                KeyPopulationId = keyPopulation.Id,
                                SiteId = site.Id,
                                Numerator = value.Numerator,
                                Denominator = value.Denominator,
                                Value = value.Value,
                                DataType = value.ValueType == 1 ? Data.Enums.DataType.Number : Data.Enums.DataType.Percent,
                                CreatedBy = "",
                                DateId = null,
                                MonthId = month.Id,
                                IsDeleted = false,
                                DateCreated = DateTime.Now,
                                IndicatorId = indicator.Id,
                            };
                            _dbContext.AggregatedValues.Add(_value);
                        }
                        else
                        {
                            _v.Value = value.Value;
                            _v.Numerator = value.Numerator;
                            _v.Denominator = value.Denominator;
                            _dbContext.AggregatedValues.Update(_v);
                        }
                    }
                    else
                    {
                        Console.WriteLine(JsonConvert.SerializeObject(value));
                    }
                }
                _dbContext.SaveChanges();
                return new ResultModel();
            }
        }

        public ResultModel ImportExcel(IFormFile file, string type = "")
        {
            var importedValues = new List<IndicatorImportModel>();
            using (var stream = new MemoryStream())
            {
                file.CopyTo(stream);
                stream.Position = 0;
                XSSFWorkbook xssWorkbook = new XSSFWorkbook(stream);
                ISheet sheet;
                sheet = xssWorkbook.GetSheetAt(0);
                for (var i = 0; i <= sheet.LastRowNum; i++)
                {
                    var row = sheet.GetRow(i);
                    var year = int.Parse(row.GetCell(0).ToString());
                    var month = int.Parse(row.GetCell(1).ToString());
                    var indicator = row.GetCell(2).ToString();
                    var gender = row.GetCell(3).ToString();
                    var ageGroup = row.GetCell(4).ToString();
                    var keyPopulation = row.GetCell(5).ToString();
                    var site = row.GetCell(6).ToString();
                    var valueType = row.GetCell(7) != null && row.GetCell(7).ToString().Length > 0 ? int.Parse(row.GetCell(7).ToString()) : 0;
                    var value = row.GetCell(8) != null && row.GetCell(8).ToString().Length > 0 ? int.Parse(row.GetCell(8).ToString()) : 0;
                    var numerator = row.GetCell(9) != null && row.GetCell(9).ToString().Length > 0 ? int.Parse(row.GetCell(9).ToString()) : 0;
                    var denominator = row.GetCell(10) != null && row.GetCell(10).ToString().Length > 0 ? int.Parse(row.GetCell(10).ToString()) : 0;
                    importedValues.Add(new IndicatorImportModel
                    {
                        Year = year,
                        Month = month,
                        Indicator = indicator,
                        Gender = gender,
                        AgeGroup = ageGroup,
                        KeyPopulation = keyPopulation,
                        Site = site,
                        ValueType = valueType,
                        Value = value,
                        Numerator = numerator,
                        Denominator = denominator,
                    });
                }
            }

            return ImportIndicator(importedValues, type);
        }
    }
}
