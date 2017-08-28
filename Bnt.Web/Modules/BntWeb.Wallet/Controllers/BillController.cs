using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using BntWeb.Logging;
using BntWeb.Wallet.Services;
using BntWeb.Mvc;
using BntWeb.Security;
using BntWeb.SystemMessage.Models;
using BntWeb.SystemMessage.Services;
using BntWeb.Utility.Extensions;
using BntWeb.Wallet.Models;
using BntWeb.Wallet.ViewModel;
using BntWeb.Web.Extensions;
using BntWeb.WebApi.Models;

namespace BntWeb.Wallet.Controllers
{
    public class BillController : Controller
    {
        private readonly IWalletService _walletService;
        private readonly ISystemMessageService _systemMessageService;

        public BillController(IWalletService walletService, ISystemMessageService systemMessageService)
        {
            _walletService = walletService;
            _systemMessageService = systemMessageService;

            Logger = NullLogger.Instance;
        }

        public ILogger Logger { get; set; }

        [AdminAuthorize]
        public ActionResult List(WalletType walletType, string memberId)
        {
            ViewBag.WalletType = walletType;
            ViewBag.MemberId = memberId;

            var wallet = _walletService.GetWalletByMemberId(memberId, walletType) ??
                         new Models.Wallet { WalletType = walletType, MemberId = memberId };
            return View(wallet);
        }

        [AdminAuthorize]
        public ActionResult ListOnPage(WalletType walletType, string memberId)
        {
            var result = new DataTableJsonResult();

            //取参数值
            int draw, pageIndex, pageSize, totalCount;
            string sortColumn;
            bool isDesc;
            Request.GetDatatableParameters(out draw, out pageIndex, out pageSize, out sortColumn, out isDesc);
            result.draw = draw;

            //分页查询
            var list = _walletService.GetWalletBillByMemberId(memberId, pageIndex, pageSize, out totalCount, walletType);

            result.data = list;
            result.recordsTotal = totalCount;
            result.recordsFiltered = totalCount;
            return Json(result, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// 添加积分
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [AdminAuthorize(PermissionsArray = new[] { Permissions.ManageMemberIntegralKey })]
        public ActionResult AppendIntegral(string memberId, IntegralWalletViewModel postModel)
        {
            var result = new DataJsonResult();
            if (string.IsNullOrWhiteSpace(postModel.Remark))
            {
                result.ErrorMessage = "备注不能为空";
            }
            else if (postModel.Integral < 1)
            {
                result.ErrorMessage = "积分不得少于1";
            }
            else
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    string error;
                    _walletService.Deposit(memberId, WalletType.Integral, postModel.Integral, postModel.Remark, out error);
                    if (string.IsNullOrWhiteSpace(error))
                    {
                        Logger.Operation($"平台向用户{memberId}添加{postModel.Integral}积分", WalletModule.Instance, SecurityLevel.Warning);
                    }

                    scope.Complete();
                }
            }
            
            return Json(result);
        }

        /// <summary>
        /// 扣除积分
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [AdminAuthorize(PermissionsArray = new[] { Permissions.ManageMemberIntegralKey })]
        public ActionResult DeductIntegral(string memberId,IntegralWalletViewModel postModel)
        {
            var result = new DataJsonResult();

            if (string.IsNullOrWhiteSpace(postModel.Remark))
            {
                result.ErrorMessage = "备注不能为空";
            }
            else 
            {
                var wallet = _walletService.GetWalletByMemberId(memberId, WalletType.Integral) ??new Models.Wallet();
                if (postModel.Integral > wallet.Available)
                    result.ErrorMessage = "扣除积分不得大于用户可用余额";
                else
                {
                    using (TransactionScope scope = new TransactionScope())
                    {
                        string error;
                        _walletService.Draw(memberId, WalletType.Integral, postModel.Integral, postModel.Remark, out error);
                        if (string.IsNullOrWhiteSpace(error))
                        {
                            Logger.Operation($"平台从用户{memberId}扣除{postModel.Integral}积分", WalletModule.Instance, SecurityLevel.Warning);
                        }

                        scope.Complete();
                    }
                }
            }
            return Json(result);
        }
    }
}