using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BntWeb.Logistics.ViewModels
{

    public class DistrictViewModel
    {
        public string Id { get; set; }

        public string ParentId { get; set; }

        public string ShortName { get; set; }

        public int Sort { get; set; }
    }

    public class DistrictWithChilViewModel
    {
        public string Id { get; set; }

        public string ParentId { get; set; }

        public string ShortName { get; set; }

        public int Sort { get; set; }

        public List<DistrictViewModel> DistrictChil { get; set; }
    }
}