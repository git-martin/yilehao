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
using BntWeb.Config.Models;
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
    public class MemberController : BaseApiController
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
        private readonly ISecurityService _securityService;

        public MemberController(IMemberService memberService, DefaultUserManager userManager, IFileService fileService, IStorageFileService storageFileService, ISmsService smsService, ICurrencyService currencyService, ISignals signals, IUserContainer userContainer, IWalletService walletService, IConfigService configService, ISecurityService securityService)
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
            _securityService = securityService;
        }

        [HttpGet]
        [BasicAuthentication]
        public ApiResult GetCenterInfo()
        {
            var money = _walletService.GetWalletByMemberId(AuthorizedUser.Id, Wallet.Models.WalletType.Cash)?.Available ?? 0;
            var integral = _walletService.GetWalletByMemberId(AuthorizedUser.Id, Wallet.Models.WalletType.Integral)?.Available ?? 0;

            var pendingPayment = 0;
            var waitingForReceiving = 0;
            var waitingForEvaluate = 0;
            var hasRefund = 0;
            using (var orderDbContext = new OrderProcessDbContext())
            {
                pendingPayment = orderDbContext.Orders.Count(o => o.OrderStatus == OrderProcess.Models.OrderStatus.PendingPayment && o.MemberId.Equals(AuthorizedUser.Id, StringComparison.OrdinalIgnoreCase));
                waitingForReceiving = orderDbContext.Orders.Count(o => (o.OrderStatus == OrderProcess.Models.OrderStatus.WaitingForDelivery || o.OrderStatus == OrderProcess.Models.OrderStatus.WaitingForReceiving) && o.MemberId.Equals(AuthorizedUser.Id, StringComparison.OrdinalIgnoreCase));
                waitingForEvaluate = orderDbContext.Orders.Count(o => o.OrderStatus == OrderProcess.Models.OrderStatus.Completed && o.EvaluateStatus == OrderProcess.Models.EvaluateStatus.NotEvaluated && o.RefundStatus == OrderProcess.Models.OrderRefundStatus.NoRefund && o.MemberId.Equals(AuthorizedUser.Id, StringComparison.OrdinalIgnoreCase));
                hasRefund = orderDbContext.Orders.Count(o => o.RefundStatus == OrderProcess.Models.OrderRefundStatus.Refunding && o.MemberId.Equals(AuthorizedUser.Id, StringComparison.OrdinalIgnoreCase));
            }

            var todayIncome = _walletService.Sum(AuthorizedUser.Id, Wallet.Models.WalletType.Cash, Wallet.Models.BillType.TakeIn, "commision", DateTime.Now.DayZero(), DateTime.Now.DayEnd());
            var totalIncome = _walletService.Sum(AuthorizedUser.Id, Wallet.Models.WalletType.Cash, Wallet.Models.BillType.TakeIn, "commision");

            var teamCount = _currencyService.Count<Member>(m => m.ParentIds.Contains(AuthorizedUser.Id));
            //var maxLevel = _configService.Get<SystemConfig>().MaxLevel;
            //if (maxLevel > 3) maxLevel = 3;
            //using (var memberDbContext = new MemberDbContext())
            //{
            //    CountChilds(memberDbContext, AuthorizedUser.Id, maxLevel, ref teamCount);
            //}//end

            var member = _memberService.FindMemberById(AuthorizedUser.Id);
            var result = new ApiResult();
            result.SetData(new
            {
                Money = money,
                Integral = integral,

                PendingPayment = pendingPayment,
                WaitingForReceiving = waitingForReceiving,
                WaitingForEvaluate = waitingForEvaluate,
                HasRefund = hasRefund,

                TodayIncome = todayIncome,
                TotalIncome = totalIncome,
                TeamCount = teamCount,

                MemberType = member?.MemberType ?? MemberType.General
            });
            return result;
        }

        //private void CountChilds(MemberDbContext memberDbContext, string referrerId, int maxLevel, ref int total)
        //{
        //    List<MemberExtension> childs = memberDbContext.MemberExtensions.Where(m => m.ReferrerId.Equals(referrerId)).ToList();
        //    total += childs.Count;
        //    maxLevel--;

        //    if (maxLevel > 0)
        //        foreach (var child in childs)
        //        {
        //            CountChilds(memberDbContext, child.Id, maxLevel, ref total);
        //        }
        //}

        /// <summary>
        /// 会员注册
        /// </summary>
        /// <param name="member"></param>
        /// <returns></returns>
        [HttpPost]
        public ApiResult Create([FromBody]MemberRegisterModel member)
        {
            Argument.ThrowIfNullOrEmpty(member.PhoneNumber, "手机号码");
            Argument.ThrowIfNullOrEmpty(member.Password, "密码");
            //Argument.ThrowIfNullOrEmpty(member.NickName, "昵称");
            Argument.ThrowIfNullOrEmpty(member.SmsVerifyCode, "短信验证码");

            var result = new ApiResult();

            User user = _memberService.FindUserByName(member.PhoneNumber);
            if (user != null) throw new WebApiInnerException("0001", "此手机号已经注册");

            //if (_memberService.NickNameIsExists(member.NickName)) throw new WebApiInnerException("0003", "昵称已经被占用");


            if (!_smsService.VerifyCode(member.PhoneNumber, member.SmsVerifyCode, MemberCenterModule.Instance, SmsRequestType.Register.ToString()))
                throw new WebApiInnerException("0002", "短信验证码验证失败");

            string referrerId = null;
            string parentIds = null;
            if (!string.IsNullOrWhiteSpace(member.InvitationCode))
            {
                var referrer = _memberService.FindMemberByInvitationCode(member.InvitationCode);
                if (referrer != null && referrer.MemberType == MemberType.Partner)
                {
                    referrerId = referrer.Id;
                    if (string.IsNullOrWhiteSpace(referrer.ParentIds))
                        parentIds = referrer.Id;
                    else
                    {
                        var maxLevel = _configService.Get<SystemConfig>().MaxLevel;
                        var ids = referrer.ParentIds.Split(',');
                        if (ids.Count() >= maxLevel)
                        {
                            //只取系统设置的层级数量
                            parentIds = "";
                            for (int i = maxLevel - 3; i < ids.Count(); i++)
                            {
                                if (string.IsNullOrWhiteSpace(parentIds))
                                    parentIds = ids[i];
                                else
                                    parentIds += "," + ids[i];
                            }

                            parentIds += "," + referrer.Id;

                        }
                        else
                            parentIds = referrer.ParentIds + "," + referrer.Id;
                    }
                }
            }

            var code = KeyGenerator.GenerateRandom(6);
            while (_memberService.FindMemberByInvitationCode(code) != null)
            {
                code = KeyGenerator.GenerateRandom(6);
            }

            var newMember = new Member
            {
                UserName = member.PhoneNumber,
                PhoneNumber = member.PhoneNumber,
                LockoutEnabled = false,
                Sex = SexType.UnKonw,
                NickName = member.PhoneNumber,
                Birthday = DateTime.Parse("1949-10-1"),
                ChannelId = member.ChannelId,
                ReferrerId = referrerId,
                ParentIds = parentIds,
                CreateTime = DateTime.Now,
                InvitationCode = code
            };

            using (TransactionScope scope = new TransactionScope())
            {
                var identityResult = _memberService.CreateMember(newMember, member.Password);

                if (!identityResult.Succeeded)
                {
                    throw new WebApiInnerException("0003", identityResult.Errors.FirstOrDefault());
                    //result.msg = identityResult.Errors.FirstOrDefault();
                }
                else
                {
                    //添加推广积分
                    if (!string.IsNullOrWhiteSpace(newMember.ReferrerId))
                    {
                        var systemConfig = _configService.Get<SystemConfig>();
                        string error;
                        _walletService.Deposit(newMember.ReferrerId, Wallet.Models.WalletType.Integral, systemConfig.RecommendIntegral, "推荐新用户奖励", out error);
                    }

                    //绑定微信
                    if (!string.IsNullOrWhiteSpace(member.OpenId))
                    {
                        _currencyService.DeleteByConditon<UserOAuth>(uo => uo.OAuthType == OAuthType.WeiXin && uo.OAuthId.Equals(member.OpenId, StringComparison.OrdinalIgnoreCase));

                        _currencyService.Create(new UserOAuth()
                        {
                            Id = KeyGenerator.GetGuidKey(),
                            MemberId = newMember.Id,
                            OAuthId = member.OpenId,
                            OAuthType = OAuthType.WeiXin
                        });
                    }

#if DEBUG
                    //添加测试余额
                    string errorMessage;
                    _walletService.Deposit(newMember.Id, Wallet.Models.WalletType.Cash, 100000, "测试现金", out errorMessage);
                    _walletService.Deposit(newMember.Id, Wallet.Models.WalletType.Integral, 100000, "测试积分", out errorMessage);

#endif
                    var createdMember = _memberService.FindMemberById(newMember.Id);
                    result.SetData(createdMember.Simplified());
                    //提交
                    scope.Complete();
                }
            }

            return result;
        }

        /// <summary>
        /// 找回/重置密码
        /// </summary>
        /// <param name="member"></param>
        /// <returns></returns>
        [HttpPatch]
        public ApiResult ResetPassword([FromBody] ResetPasswordModel member)
        {
            Argument.ThrowIfNullOrEmpty(member.PhoneNumber, "手机号码");
            Argument.ThrowIfNullOrEmpty(member.Password, "密码");
            Argument.ThrowIfNullOrEmpty(member.SmsVerifyCode, "短信验证码");

            if (!_smsService.VerifyCode(member.PhoneNumber, member.SmsVerifyCode, MemberCenterModule.Instance, SmsRequestType.FindPassword.ToString()))
                throw new WebApiInnerException("0002", "短信验证码验证失败");

            var result = new ApiResult();

            User user = _memberService.FindUserByName(member.PhoneNumber);
            if (user == null) throw new WebApiInnerException("0001", "此手机号未注册");

            _memberService.ResetPassword(user, member.Password);

            return result;
        }

        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="id"></param>
        /// <param name="member"></param>
        /// <returns></returns>
        [HttpPatch]
        [BasicAuthentication]
        public ApiResult ChangePassword(string id, [FromBody] ChangePasswordModel member)
        {
            Argument.ThrowIfNullOrEmpty(member.Password, "旧密码");
            Argument.ThrowIfNullOrEmpty(member.NewPassword, "新密码");

            if (!AuthorizedUser.Id.Equals(id, StringComparison.OrdinalIgnoreCase))
                throw new WebApiInnerException("0001", "只允许修改自己密码");

            var result = new ApiResult();
            if (Platform != Platform.Web)
            {
                member.Password = _securityService.Decrypt(member.Password);
                member.NewPassword = _securityService.Decrypt(member.NewPassword);
            }

            var identityResult = _userManager.ChangePasswordAsync(AuthorizedUser.Id, member.Password, member.NewPassword).Result;
            if (!identityResult.Succeeded) throw new WebApiInnerException("0002", identityResult.Errors.FirstOrDefault());

            return result;
        }

        /// <summary>
        /// 更新会员头像
        /// </summary>
        /// <param name="id"></param>
        /// <param name="avatar"></param>
        /// <returns></returns>
        [HttpPatch]
        [BasicAuthentication]
        public ApiResult UpdateAvatar(string id, [FromBody]Base64Image avatar)
        {
            Argument.ThrowIfNullOrEmpty(id, "会员Id");
            Argument.ThrowIfNullOrEmpty(avatar.Data, "头像图片数据");
            Argument.ThrowIfNullOrEmpty(avatar.FileName, "头像图片文件名");
            if (string.IsNullOrWhiteSpace(Path.GetExtension(avatar.FileName)))
                throw new WebApiInnerException("0003", "文件名称没有包含扩展名");
            if (!AuthorizedUser.Id.Equals(id, StringComparison.OrdinalIgnoreCase))
                throw new WebApiInnerException("0001", "只允许修改自己的头像");
            var result = new ApiResult();

            StorageFile storageFile;
            if (_fileService.SaveFile(avatar.Data, avatar.FileName, false, out storageFile, 128, 128, 64, 64, ThumbnailType.TakeCenter))
            {
                _storageFileService.DisassociateFile(AuthorizedUser.Id.ToGuid(), MemberBaseModule.Key, "Avatar");
                if (_storageFileService.AssociateFile(AuthorizedUser.Id.ToGuid(), MemberBaseModule.Key, MemberBaseModule.DisplayName, storageFile.Id, "Avatar"))
                {
                    //更新缓存
                    _signals.Trigger(_userContainer.UserChangedSignalName);
                    _signals.Trigger($"member_{AuthorizedUser.Id}_changed");
                    result.SetData(storageFile.Simplified());
                }
                else
                {
                    throw new WebApiInnerException("0002", "头像上传失败");
                }
            }
            else
            {
                throw new WebApiInnerException("0002", "头像上传失败");
            }

            return result;
        }

        /// <summary>
        /// 更新会员资料
        /// </summary>
        /// <param name="id"></param>
        /// <param name="member"></param>
        /// <returns></returns>
        [HttpPut]
        [BasicAuthentication]
        public ApiResult Update(string id, [FromBody]ModifyMemberModel member)
        {
            Argument.ThrowIfNullOrEmpty(id, "会员Id");
            if (!AuthorizedUser.Id.Equals(id, StringComparison.OrdinalIgnoreCase))
                throw new WebApiInnerException("0001", "只允许修改自己的信息");

            var oldMember = _memberService.FindMemberById(id);
            if (oldMember == null)
                throw new WebApiInnerException("0002", "会员信息不存在");

            if (!string.IsNullOrWhiteSpace(member.NickName))
                oldMember.NickName = member.NickName;
            if (member.Birthday != null)
                oldMember.Birthday = member.Birthday;
            if (member.Sex != SexType.UnKonw)
                oldMember.Sex = member.Sex;
            var identityResult = _memberService.UpdateMember(oldMember);
            if (!identityResult.Succeeded)
            {
                throw new WebApiInnerException("0003", "更新失败:" + identityResult.Errors.FirstOrDefault());
            }
            var result = new ApiResult();
            result.SetData(oldMember.Simplified());
            return result;
        }

        /// <summary>
        /// 修改手机
        /// </summary>
        /// <param name="id"></param>
        /// <param name="member"></param>
        /// <returns></returns>
        [HttpPatch]
        [BasicAuthentication]
        public ApiResult ChangePhoneNumber(string id, [FromBody] ChangePhoneNumberModel member)
        {
            Argument.ThrowIfNullOrEmpty(member.PhoneNumber, "旧手机");
            Argument.ThrowIfNullOrEmpty(member.SmsVerifyCode, "旧手机验证码");
            Argument.ThrowIfNullOrEmpty(member.NewPhoneNumber, "新手机");
            Argument.ThrowIfNullOrEmpty(member.NewSmsVerifyCode, "新手机验证码");

            if (!AuthorizedUser.Id.Equals(id, StringComparison.OrdinalIgnoreCase))
                throw new WebApiInnerException("0001", "只允许修改自己手机");

            if (!_smsService.VerifyCode(member.PhoneNumber, member.SmsVerifyCode, MemberCenterModule.Instance, SmsRequestType.ChangePhoneNumber.ToString()))
                throw new WebApiInnerException("0002", "旧手机短信验证码验证失败");

            if (!_smsService.VerifyCode(member.NewPhoneNumber, member.NewSmsVerifyCode, MemberCenterModule.Instance, SmsRequestType.ChangePhoneNumber.ToString()))
                throw new WebApiInnerException("0003", "新手机短信验证码验证失败");

            //判断是否已经存在此手机号
            var testUser = _userContainer.FindUser(member.NewPhoneNumber);
            if (testUser != null)
            {
                throw new WebApiInnerException("0004", "新手机已经注册，无法更改");
            }
            var user = _userManager.FindById(id);
            user.UserName = member.NewPhoneNumber;
            user.PhoneNumber = member.NewPhoneNumber;

            var identityResult = _userManager.Update(user);
            if (identityResult.Succeeded)
            {
                //更新缓存
                _signals.Trigger(_userContainer.UserChangedSignalName);
                _signals.Trigger($"member_{AuthorizedUser.Id}_changed");
            }
            else
            {
                throw new WebApiInnerException("0005", "手机号修改失败");
            }
            var result = new ApiResult();
            return result;
        }

        /// <summary>
        /// 修改手机
        /// </summary>
        /// <param name="id"></param>
        /// <param name="member"></param>
        /// <returns></returns>
        [HttpPatch]
        [BasicAuthentication]
        public ApiResult BoundPhoneNumber(string id, [FromBody] BoundPhoneNumberModel member)
        {
            Argument.ThrowIfNullOrEmpty(member.PhoneNumber, "手机号");
            Argument.ThrowIfNullOrEmpty(member.SmsVerifyCode, "手机验证码");
            Argument.ThrowIfNullOrEmpty(member.Password, "密码");

            if (!_smsService.VerifyCode(member.PhoneNumber, member.SmsVerifyCode, MemberCenterModule.Instance, SmsRequestType.BoundPhoneNumber.ToString()))
                throw new WebApiInnerException("0001", "手机短信验证码验证失败");

            var user = _userManager.Find(AuthorizedUser.UserName, Platform != Platform.Web ? _securityService.Decrypt(member.Password) : member.Password);
            if (user != null && user.UserType == UserType.Member)
            {
                if (user.LockoutEnabled)
                    throw new WebApiInnerException("0002", "此用户已经禁止登录");

                //判断是否已经存在此手机号
                var testUser = _userContainer.FindUser(member.PhoneNumber);
                if (testUser != null)
                {
                    throw new WebApiInnerException("0004", "手机号已经注册，无法绑定");
                }

                user.UserName = member.PhoneNumber;
                user.PhoneNumber = member.PhoneNumber;

                if (!string.IsNullOrWhiteSpace(member.InvitationCode))
                {
                    var referrer = _memberService.FindMemberByInvitationCode(member.InvitationCode);
                    if (referrer != null)
                    {
                        var oldMember = _memberService.FindMemberById(id);
                        if (oldMember == null)
                            throw new WebApiInnerException("0005", "会员信息不存在");
                        oldMember.ReferrerId = referrer.Id;

                        var midentityResult = _memberService.UpdateMember(oldMember);
                        if (!midentityResult.Succeeded)
                        {
                            throw new WebApiInnerException("0006", "更新失败:" + midentityResult.Errors.FirstOrDefault());
                        }
                    }
                }
            }
            else
            {
                throw new WebApiInnerException("0003", "密码不正确");
            }

            var identityResult = _userManager.Update(user);
            if (identityResult.Succeeded)
            {
                //更新缓存
                _signals.Trigger(_userContainer.UserChangedSignalName);
                _signals.Trigger($"member_{AuthorizedUser.Id}_changed");
            }
            else
            {
                throw new WebApiInnerException("0007", "手机号绑定失败");
            }
            var result = new ApiResult();
            return result;
        }

    }
}
