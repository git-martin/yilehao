using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Transactions;
using System.Web;
using System.Web.Http;
using BntWeb.Caching;
using BntWeb.Data;
using BntWeb.Data.Services;
using BntWeb.FileSystems.Media;
using BntWeb.MemberBase;
using BntWeb.MemberBase.Models;
using BntWeb.MemberBase.Services;
using BntWeb.MemberCenter.ApiModels;
using BntWeb.OrderProcess;
using BntWeb.OrderProcess.Services;
using BntWeb.Security;
using BntWeb.Security.Identity;
using BntWeb.Services;
using BntWeb.Utility.Extensions;
using BntWeb.Validation;
using BntWeb.Wallet.Services;
using BntWeb.WebApi.Filters;
using BntWeb.WebApi.Models;
using Microsoft.AspNet.Identity;

namespace BntWeb.MemberCenter.ApiControllers
{
    public class PartnerController : BaseApiController
    {
        private readonly IMemberService _memberService;
        private readonly DefaultUserManager _userManager;
        private readonly IFileService _fileService;
        private readonly IStorageFileService _storageFileService;
        private readonly ISmsService _smsService;
        private readonly ICurrencyService _currencyService;
        private readonly ISignals _signals;
        private readonly IUserContainer _userContainer;
        private readonly IWalletService _walletService;
        private readonly IConfigService _configService;

        public PartnerController(IMemberService memberService, DefaultUserManager userManager, IFileService fileService, IStorageFileService storageFileService, ISmsService smsService, ICurrencyService currencyService, ISignals signals, IUserContainer userContainer, IWalletService walletService, IConfigService configService)
        {
            _memberService = memberService;
            _userManager = userManager;
            _fileService = fileService;
            _storageFileService = storageFileService;
            _smsService = smsService;
            _currencyService = currencyService;
            _signals = signals;
            _userContainer = userContainer;
            _walletService = walletService;
            _configService = configService;
        }

        [HttpGet]
        [BasicAuthentication]
        public ApiResult GetCommission(int pageNo = 1, int limit = 10)
        {
            var result = new ApiResult();
            int totalCount;
            var walletBills = _walletService.GetWalletBillByMemberId(AuthorizedUser.Id, pageNo, limit, out totalCount, Wallet.Models.WalletType.Cash, null, "commision");

            var list = (from walletBill in walletBills
                        let member = _memberService.FindMemberById(walletBill.FromMemberId.ToString())
                        select new CommissionModel
                        {
                            Remark = walletBill.Remark,
                            CreateTime = walletBill.CreateTime,
                            Money = walletBill.Money,
                            MemberName = member?.NickName ?? "匿名用户"
                        }).ToList();

            result.SetData(new
            {
                TotalCount = totalCount,
                WalletBills = list
            });
            return result;
        }

        [HttpGet]
        [BasicAuthentication]
        public ApiResult GetPartner(int pageNo = 1, int limit = 10)
        {
            var result = new ApiResult();
            int totalCount;

            var members = _memberService.GetListPaged(pageNo, limit, m => m.ParentIds.Contains(AuthorizedUser.Id), m => m.CreateTime, true, out totalCount);

            var list = new List<PartnerModel>();

            foreach (var member in members)
            {
                var money = _walletService.Sum(AuthorizedUser.Id, Wallet.Models.WalletType.Cash, null, "commision", null, null, member.Id);
                list.Add(new PartnerModel
                {
                    MemberName = member.NickName,
                    Avatar = member.Avatar?.Simplified() ?? _storageFileService.GetFiles(member.Id.ToGuid(), MemberBaseModule.Key, "Avatar").FirstOrDefault()?.Simplified(),
                    Money = money
                });
            }
            
            result.SetData(new
            {
                TotalCount = totalCount,
                Partners = list
            });

            return result;
        }


    }
}
