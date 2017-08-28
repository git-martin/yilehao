
using BntWeb.Environment;

namespace BntWeb.Discovery
{
    public class DiscoveryModule : IBntWebModule
    {

        public static readonly DiscoveryModule Instance = new DiscoveryModule();

        public string InnerKey => "BntWeb-Discovery";
        public static string Key => Instance.InnerKey;
        public string InnerDisplayName => "发现管理";
        public static string DisplayName => Instance.InnerDisplayName;
        public string InnerArea => "Discovery";
        public static string Area => Instance.InnerArea;
        public int InnerPosition => 7000;
        public static int Position => Instance.InnerPosition;
    }
}