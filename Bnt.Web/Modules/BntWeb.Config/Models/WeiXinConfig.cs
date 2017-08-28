/* 
    ======================================================================== 
        File name：        WeiXinConfig
        Module:                
        Author：            罗嗣宝
        Create Time：    2016/7/27 13:42:49
        Modify By:        
        Modify Date:    
    ======================================================================== 
*/

using System;
using System.Collections.Generic;
using BntWeb.FileSystems.Media;

namespace BntWeb.Config.Models
{
    public class WeiXinConfig
    {
        private string _appId;
        public string AppId
        {
            get { return _appId; }
            set { _appId = value.Trim(); }
        }

        private string _appSecret;
        public string AppSecret
        {
            get { return _appSecret; }
            set { _appSecret = value.Trim(); }
        }

        private string _mchId;
        public string MchId
        {
            get { return _mchId; }
            set { _mchId = value.Trim(); }
        }

        private string _key;
        public string Key
        {
            get { return _key; }
            set { _key = value.Trim(); }
        }

    }
}