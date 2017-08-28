using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BntWeb.Core.SystemSettings.Models
{
    public class CaptchaContent
    {

        public string Key { get; set; }

        public Dictionary<string, object> KeyValues { get; set; }

        public string Image;

        public static CaptchaContent Empty => null;

        public static CaptchaContent Create(string key, string code)
        {
            CaptchaContent captchaContent = new CaptchaContent();
            Dictionary<string, object> keyValues = new Dictionary<string, object> { { "Code", code } };
            captchaContent.Key = key;
            captchaContent.KeyValues = keyValues;
            return captchaContent;
        }


    }
}