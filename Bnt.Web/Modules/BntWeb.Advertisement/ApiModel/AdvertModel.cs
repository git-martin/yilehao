using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Autofac;
using BntWeb.Advertisement.Models;
using BntWeb.Environment;
using BntWeb.FileSystems.Media;

namespace BntWeb.Advertisement.ApiModel
{
    public class AdvertModel
    {
        public string Key { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public SimplifiedStorageFile AdvertImage { get; set; }

        public string ShotUrl { get; set; }

        public AdvertModel(Advert model)
        {
            Key = model.Key;
            Name = model.Name;
            Description = model.Description ?? "";
            var fileService = HostConstObject.Container.Resolve<IStorageFileService>();
            var mainImage = fileService.GetFiles(model.Id, AdvertisementModule.Key, "AdvertImage").FirstOrDefault();
            AdvertImage = mainImage?.Simplified();
            ShotUrl = model.ShotUrl;
        }
    }
}