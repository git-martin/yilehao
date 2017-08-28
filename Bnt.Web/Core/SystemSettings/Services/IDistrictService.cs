/* 
    ======================================================================== 
        File name：        IDistrictService
        Module:                
        Author：            罗嗣宝
        Create Time：    2016/6/2 17:51:08
        Modify By:        
        Modify Date:    
    ======================================================================== 
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BntWeb.Core.SystemSettings.Models;
using BntWeb.Mvc;

namespace BntWeb.Core.SystemSettings.Services
{
    public interface IDistrictService : IDependency
    {
        DataJsonResult Save(District district, bool isNew);
        DataJsonResult Delete(string districtId);
    }
}