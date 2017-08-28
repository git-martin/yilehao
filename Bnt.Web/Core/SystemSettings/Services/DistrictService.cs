/* 
    ======================================================================== 
        File name：        DistrictService
        Module:                
        Author：            罗嗣宝
        Create Time：    2016/6/2 17:57:27
        Modify By:        
        Modify Date:    
    ======================================================================== 
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BntWeb.Core.SystemSettings.Models;
using BntWeb.Data.Services;
using BntWeb.Logging;
using BntWeb.Mvc;
using BntWeb.Utility;

namespace BntWeb.Core.SystemSettings.Services
{
    public class DistrictService : IDistrictService
    {
        private readonly ICurrencyService _currencyService;
        public DistrictService(ICurrencyService currencyService)
        {
            _currencyService = currencyService;

            Logger = NullLogger.Instance;
        }

        public ILogger Logger { get; set; }

        public DataJsonResult Save(District district, bool isNew)
        {
            var result = new DataJsonResult();
            try
            {

                var nameChanged = true;
                if (!isNew)
                {
                    var oldDistrict = _currencyService.GetSingleById<District>(district.Id);
                    if (oldDistrict != null)
                    {
                        //判断名称是否变更，如果变更，则需要递归更新子节点
                        if (oldDistrict.FullName.Equals(district.FullName, StringComparison.OrdinalIgnoreCase) &&
                            oldDistrict.ShortName.Equals(district.ShortName, StringComparison.OrdinalIgnoreCase))
                        {
                            nameChanged = false;
                        }

                        var parentDistrict = _currencyService.GetSingleById<District>(district.ParentId);
                        if (parentDistrict != null)
                        {
                            district.Position = $"{parentDistrict.Position} tr_{parentDistrict.Id}".Trim();
                            if (parentDistrict.Id != "0")
                            {
                                district.MergerName = $"{parentDistrict.MergerName},{district.FullName}".Trim(',');
                                district.MergerShortName = $"{parentDistrict.MergerShortName},{district.ShortName}".Trim(',');
                            }
                            else
                            {
                                district.MergerName = district.FullName;
                                district.MergerShortName = district.ShortName;
                            }
                        }
                        else
                        {
                            district.Position = $"tr_{district.Id}";
                            district.MergerName = district.FullName;
                            district.MergerShortName = district.ShortName;
                        }
                    }
                    else
                    {
                        //转为新增
                        isNew = true;
                    }

                }

                if (district.Level < 3 && nameChanged)
                {
                    district.PinYin = PinYinConverter.Get(district.FullName);
                    district.JianPin = PinYinConverter.GetFirst(district.ShortName);
                    district.FirstChar = PinYinConverter.GetFirst(district.ShortName.First());
                }

                if (!isNew)
                {
                    result.Success = _currencyService.Update(district);
                    if (nameChanged)
                    {
                        //更新所有子节点
                        UpdateChildsInfo(district);
                    }
                }
                else
                {
                    result.Success = _currencyService.Create(district);
                }
            }
            catch (Exception ex)
            {
                result.ErrorMessage = "出现异常，保存行政区失败";

                Logger.Error(ex, "保存行政区失败");
            }

            return result;
        }

        private void UpdateChildsInfo(District parentDictrict)
        {
            var childs =
                _currencyService.GetList<District>(
                    d => d.ParentId.Equals(parentDictrict.Id, StringComparison.OrdinalIgnoreCase));

            foreach (var district in childs)
            {
                //更新结点
                district.MergerName = (parentDictrict.MergerName + "," + district.FullName).Trim(',');
                district.MergerShortName = (parentDictrict.MergerShortName + "," + district.ShortName).Trim(',');
                _currencyService.Update(district);

                UpdateChildsInfo(district);
            }
        }

        public DataJsonResult Delete(string districtId)
        {
            var result = new DataJsonResult();
            if (!string.IsNullOrWhiteSpace(districtId))
            {
                var position = $"tr_{districtId} ";
                //删除节点
                _currencyService.DeleteByConditon<District>(d => d.Id.Equals(districtId, StringComparison.OrdinalIgnoreCase));
                //批量删除子节点
                _currencyService.DeleteByConditon<District>(d => d.Position.Contains(position));
            }
            else
            {
                result.ErrorMessage = "要删除的行政区编号为空";
            }

            return result;
        }
    }
}