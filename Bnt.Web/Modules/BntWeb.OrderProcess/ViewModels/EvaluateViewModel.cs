using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BntWeb.OrderProcess.ViewModels
{
    public class OrderGoodsEvaluateViewModel
    {
        public Models.OrderGoods OrderGoods { get; set; }

        public Evaluate.Models.Evaluate Evaluate { get; set; }
    }

    public class EvaluateReplayViewModel
    {
        public Guid Id { get; set; }
        public string ReplayContent { get; set; }
    }
}