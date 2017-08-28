/* 
    ======================================================================== 
        File name：        AlipayConfig
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
    public class AlipayConfig
    {
        private string _sellerId;
        public string SellerId
        {
            get { return _sellerId; }
            set { _sellerId = value.Trim(); }
        }

        private string _accountName;
        public string AccountName
        {
            get { return _accountName; }
            set { _accountName = value.Trim(); }
        }

        private string _partner;
        public string Partner
        {
            get { return _partner; }
            set { _partner = value.Trim(); }
        }

        private string _md5Key;
        public string MD5Key
        {
            get { return _md5Key; }
            set { _md5Key = value.Trim(); }
        }

    }
}