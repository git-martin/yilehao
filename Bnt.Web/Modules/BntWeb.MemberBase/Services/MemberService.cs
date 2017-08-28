/* 
    ======================================================================== 
        File name：        MemberService
        Module:                
        Author：            Luce
        Create Time：    2016/6/7 14:31:11
        Modify By:        
        Modify Date:    
    ======================================================================== 
*/

using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using BntWeb.Caching;
using BntWeb.Data;
using BntWeb.Data.Services;
using BntWeb.FileSystems.Media;
using BntWeb.Logging;
using BntWeb.MemberBase.Models;
using BntWeb.Security;
using BntWeb.Security.Identity;
using BntWeb.Utility.Extensions;
using BntWeb.Validation;
using LinqKit;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace BntWeb.MemberBase.Services
{
    public class MemberService : IMemberService
    {
        private readonly ICurrencyService _currencyService;
        private readonly IStorageFileService _storageFileService;
        private readonly DefaultUserManager _userManager;
        private readonly IUserContainer _userContainer;
        private readonly ICacheManager _cacheManager;
        private readonly ISignals _signals;

        public MemberService(ICurrencyService currencyService, IStorageFileService storageFileService, DefaultUserManager userManager, IUserContainer userContainer, ICacheManager cacheManager, ISignals signals)
        {
            _currencyService = currencyService;
            _storageFileService = storageFileService;
            _userManager = userManager;
            _userContainer = userContainer;
            _cacheManager = cacheManager;
            _signals = signals;


            Logger = NullLogger.Instance;
        }

        public ILogger Logger { get; set; }


        public IdentityResult CreateMember(Member member, string password)
        {
            //不是最佳做法，没有事务回滚
            member.UserType = UserType.Member;
            member.DynamicToken = KeyGenerator.GetGuidKey().ToString();
            var user = member.GetUser();
            var result = _userManager.Create(user, password);

            if (result.Succeeded)
            {
                member.Id = user.Id;
                //插入扩展数据
                _currencyService.Create(member.GetMemberExtension());

                result = _userManager.AddToRole(member.Id, "member");
                Logger.Operation($"添加会员-{member.UserName}:{member.Id}", MemberBaseModule.Instance, SecurityLevel.Warning);
                //更新缓存
                _signals.Trigger(_userContainer.UserChangedSignalName);
                _signals.Trigger($"member_{member.Id}_changed");
            }

            return result;
        }

        public IdentityResult UpdateMember(Member oldMember, string oldPassword = null, string newPassword = null)
        {
            //不是最佳做法，没有事务回滚
            //更新
            IdentityResult result;
            if (!string.IsNullOrEmpty(oldPassword) && !string.IsNullOrEmpty(newPassword))
            {
                result = _userManager.ChangePassword(oldMember.Id, oldPassword, newPassword);
                if (!result.Succeeded)
                    return result;
            }

            result = _userManager.Update(oldMember.GetUser(_userManager));
            if (!result.Succeeded)
                return result;

            //更新扩展数据
            _currencyService.Update(oldMember.GetMemberExtension());

            Logger.Operation($"修改会员-{oldMember.UserName}:{oldMember.Id}", MemberBaseModule.Instance, SecurityLevel.Warning);
            //更新缓存
            _signals.Trigger(_userContainer.UserChangedSignalName);
            _signals.Trigger($"member_{oldMember.Id}_changed");

            return result;
        }


        public User FindUserByName(string name)
        {
            return _userContainer.FindUser(name);
        }

        public Member FindMemberById(string id)
        {
            return _cacheManager.Get($"member_{id}", ctx =>
            {
                ctx.Monitor(_signals.When($"member_{id}_changed"));

                var member = _currencyService.GetSingleById<Member>(id);
                if (member == null) return null;
                member.Avatar =
                    _storageFileService.GetFiles(id.ToGuid(), MemberBaseModule.Key, "Avatar").FirstOrDefault();
                return member;
            });
        }

        public Member FindMemberByInvitationCode(string code)
        {
            return _currencyService.GetSingleByConditon<Member>(m => m.InvitationCode.Equals(code, StringComparison.OrdinalIgnoreCase));
        }

        public bool NickNameIsExists(string nickName)
        {
            return _currencyService.Count<Member>(m => m.NickName.Equals(nickName, StringComparison.OrdinalIgnoreCase)) > 0;
        }

        public Member FindMember(User user)
        {
            if (user.UserType != UserType.Member) return null;
            return FindMemberById(user.Id);
        }

        public List<Member> GetListPaged<TKey>(int pageIndex, int pageSize, Expression<Func<Member, bool>> expression, Expression<Func<Member, TKey>> orderByExpression, bool isDesc, out int totalCount)
        {
            using (var dbContext = new MemberDbContext())
            {
                totalCount = dbContext.Members.AsExpandable().Where(expression).Count();//获取总数
                                                                                        //需要增加AsExpandable(),否则查询的是所有数据到内存，然后再排序  AsExpandable是linqkit.dll中的方法
                var data = dbContext.Members.AsExpandable().Where(expression);
                var orderedData = isDesc ? data.OrderByDescending(orderByExpression) : data.OrderBy(orderByExpression);
                return orderedData.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            }
        }

        /// <summary>
        /// 会员后台管理获取用户信息
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="expression"></param>
        /// <param name="orderByExpression"></param>
        /// <param name="isDesc"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public List<MemberFull> GetMemberFullListPaged<TKey>(int pageIndex, int pageSize, Expression<Func<MemberFull, bool>> expression, Expression<Func<MemberFull, TKey>> orderByExpression, bool isDesc, out int totalCount)
        {
            using (var dbContext = new MemberDbContext())
            {
                totalCount = dbContext.MembersFull.AsExpandable().Where(expression).Count();//获取总数
                                                                                            //需要增加AsExpandable(),否则查询的是所有数据到内存，然后再排序  AsExpandable是linqkit.dll中的方法
                var data = dbContext.MembersFull.AsExpandable().Where(expression);
                var orderedData = isDesc ? data.OrderByDescending(orderByExpression) : data.OrderBy(orderByExpression);
                return orderedData.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            }
        }



        public IdentityResult SetLockoutEnabled(string userId, bool enabled)
        {
            var result = _userManager.SetLockoutEnabled(userId, enabled);
            if (result.Succeeded)
            {
                var user = _userManager.FindById(userId);
                if (!enabled)
                {
                    //修改令牌，屏蔽app访问
                    user.DynamicToken = KeyGenerator.GetGuidKey().ToString();
                    _userManager.Update(user);
                }
                Logger.Operation($"{(enabled ? "启用" : "禁用")}会员-{user.UserName}:{user.Id}", MemberBaseModule.Instance, SecurityLevel.Warning);
                //更新缓存
                _signals.Trigger(_userContainer.UserChangedSignalName);
                _signals.Trigger($"member_{userId}_changed");
            }
            return result;
        }

        public async Task<IdentityResult> Delete(Member member)
        {
            var result = await _userManager.SetLockoutEnabledAsync(member.Id, true);
            if (result.Succeeded)
            {
                var user = _userManager.FindById(member.Id);
                user.UserType = UserType.DeletedMember;
                user.UserName = user.UserName + "del" + DateTime.Now.ToString("yyyyMMddHHmmss");
                user.MobileDevice = null;
                user.DynamicToken = null;
                user.PhoneNumber = null;
                await _userManager.UpdateAsync(user);
                Logger.Operation($"删除会员-{member.UserName}:{member.Id}", MemberBaseModule.Instance, SecurityLevel.Warning);
                //删除第三方关联
                _currencyService.DeleteByConditon<UserOAuth>(
                    uo => uo.MemberId.Equals(member.Id, StringComparison.OrdinalIgnoreCase));

                //更新缓存
                _signals.Trigger(_userContainer.UserChangedSignalName);
                _signals.Trigger($"member_{member.Id}_changed");
            }
            return result;
        }

        public async void ResetPassword(User user, string newPassword)
        {
            user = _userManager.FindById(user.Id);
            var hashedNewPassword = _userManager.PasswordHasher.HashPassword(newPassword);
            user.PasswordHash = hashedNewPassword;
            await _userManager.UpdateAsync(user);
        }

        public StorageFile GetAvatarFile(string userId)
        {
            return _storageFileService.GetFiles(userId.ToGuid(), MemberBaseModule.Key, "Avatar").FirstOrDefault();
        }

        /// <summary>
        /// 设置默认地址
        /// </summary>
        /// <param name="memberId"></param>
        /// <param name="addressId"></param>
        public void SetDefaultAddress(string memberId, Guid addressId)
        {
            using (var dbContext = new MemberDbContext())
            {
                var myAddress = dbContext.MemberAddresses.Where(me => me.MemberId == memberId);
                foreach (var addr in myAddress)
                {
                    addr.IsDefault = addr.Id == addressId;
                }
                dbContext.SaveChanges();
            }
        }
    }
}
