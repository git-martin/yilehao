/* 
    ======================================================================== 
        File name：        ConfigService
        Module:                
        Author：            罗嗣宝
        Create Time：    2016/7/27 11:40:29
        Modify By:        
        Modify Date:    
    ======================================================================== 
*/

using System;
using BntWeb.Caching;
using BntWeb.Config.Models;
using BntWeb.Data;
using BntWeb.Data.Services;
using BntWeb.Services;
using BntWeb.Utility.Extensions;

namespace BntWeb.Config.Service
{
    public class ConfigService : IConfigService
    {
        private readonly ICacheManager _cacheManager;
        private readonly ISignals _signals;
        private readonly ICurrencyService _currencyService;

        public ConfigService(ICacheManager cacheManager, ISignals signals, ICurrencyService currencyService)
        {
            _cacheManager = cacheManager;
            _signals = signals;
            _currencyService = currencyService;
        }

        public T Get<T>() where T : new()
        {
            var key = typeof(T).FullName;
            var config = _cacheManager.Get($"config_{key}", ctx =>
            {
                ctx.Monitor(_signals.When($"{ctx.Key}-changed"));

                return _currencyService.GetSingleByConditon<ConfigData>(
                         c => c.Key.Equals(key, StringComparison.OrdinalIgnoreCase));
            });

            if (config == null || string.IsNullOrWhiteSpace(config.Value))
            {
                return new T();
            }

            var obj = config.Value.DeserializeJsonToObject<T>();
            if (obj == null) return new T();
            return obj;
        }

        public bool Save(object configValue)
        {
            var key = configValue.GetType().FullName;
            var config = new ConfigData
            {
                Id = KeyGenerator.GetGuidKey(),
                Key = key,
                Value = configValue.ToJson()
            };

            _currencyService.DeleteByConditon<ConfigData>(c => c.Key.Equals(key, StringComparison.OrdinalIgnoreCase));
            if (_currencyService.Create(config))
            {
                _signals.Trigger($"config_{key}-changed");

                return true;
            }

            return false;
        }
    }
}