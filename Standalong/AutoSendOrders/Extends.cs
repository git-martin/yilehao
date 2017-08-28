using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AutoSendOrders
{
    public static class Extends
    {
        public static string Description(this Enum enumName)
        {
            string _description = string.Empty;
            FieldInfo _fieldInfo = enumName.GetType().GetField(enumName.ToString());
            DescriptionAttribute[] _attributes = _fieldInfo.GetDescriptAttr();
            if (_attributes != null && _attributes.Length > 0)
                _description = _attributes[0].Description;
            else
                _description = enumName.ToString();
            return _description;
        }
        /// <summary>  
        /// 获取字段Description  
        /// </summary>  
        /// <param name="fieldInfo">FieldInfo</param>  
        /// <returns>DescriptionAttribute[] </returns>  
        public static DescriptionAttribute[] GetDescriptAttr(this FieldInfo fieldInfo)
        {
            if (fieldInfo != null)
            {
                return (DescriptionAttribute[])fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);
            }
            return null;
        } 
    }
}
