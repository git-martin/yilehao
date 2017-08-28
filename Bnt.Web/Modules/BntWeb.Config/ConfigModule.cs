/* 
    ======================================================================== 
        File name：        ConfigModule
        Module:                
        Author：            罗嗣宝
        Create Time：    2016/5/27 8:57:07
        Modify By:        
        Modify Date:    
    ======================================================================== 
*/

using BntWeb.Environment;

namespace BntWeb.Config
{
    public class ConfigModule : IBntWebModule
    {
        public static readonly ConfigModule Instance = new ConfigModule();

        public string InnerKey => "BntWeb-Config";
        public static string Key => Instance.InnerKey;
        public string InnerDisplayName => "配置管理";
        public static string DisplayName => Instance.InnerDisplayName;
        public string InnerArea => "Config";
        public static string Area => Instance.InnerArea;
        public int InnerPosition => 10000;
        public static int Position => Instance.InnerPosition;
    }
}