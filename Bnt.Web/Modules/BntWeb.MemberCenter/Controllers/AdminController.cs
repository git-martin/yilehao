using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using BntWeb.Data;
using BntWeb.Data.Services;
using BntWeb.FileSystems.Media;
using BntWeb.MemberBase;
using BntWeb.MemberBase.Models;
using BntWeb.MemberBase.Services;
using BntWeb.MemberCenter.ViewModels;
using BntWeb.Mvc;
using BntWeb.Security;
using BntWeb.Security.Identity;
using BntWeb.Utility.Extensions;
using BntWeb.Validation;
using BntWeb.Web.Extensions;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;

namespace BntWeb.MemberCenter.Controllers
{
    public class AdminController : Controller
    {
        private readonly IMemberService _memberService;
        private readonly IStorageFileService _storageFileService;

        public AdminController(IMemberService memberService, IStorageFileService storageFileService)
        {
            _memberService = memberService;
            _storageFileService = storageFileService;
        }


        [AdminAuthorize(PermissionsArray = new[] { Permissions.EditMemberKey })]
        public ActionResult Edit(string id)
        {
            Argument.ThrowIfNullOrEmpty(id, "会员Id不能为空");
            var member = _memberService.FindMemberById(id);

            ViewBag.AvatarFile =
                _storageFileService.GetFiles(member.Id.ToGuid(), MemberBaseModule.Key, "Avatar").FirstOrDefault();
            return View(member);
        }

        [HttpPost]
        [AdminAuthorize(PermissionsArray = new[] { Permissions.EditMemberKey })]
        public ActionResult EditOnPost(EditMemberViewModel editMember)
        {
            var result = new DataJsonResult();
            Member oldMember = null;
            if (!string.IsNullOrWhiteSpace(editMember.MemberId))
                oldMember = _memberService.FindMemberById(editMember.MemberId);

            if (oldMember == null)
            {
                //新建用户
                User user = _memberService.FindUserByName(editMember.UserName);
                if (user == null)
                {
                    var member = new Member
                    {
                        UserName = editMember.UserName,
                        Email = editMember.Email,
                        PhoneNumber = editMember.PhoneNumber,
                        LockoutEnabled = false,
                        NickName = editMember.NickName,
                        Birthday = editMember.Birthday,
                        Sex = editMember.Sex,
                        Province = editMember.Member_Province,
                        City = editMember.Member_City,
                        District = editMember.Member_District,
                        Street = editMember.Member_Street,
                        Address = editMember.Address
                    };

                    var identityResult = _memberService.CreateMember(member, editMember.Password);

                    if (!identityResult.Succeeded)
                    {
                        result.ErrorMessage = identityResult.Errors.FirstOrDefault();
                    }
                    else
                    {
                        _storageFileService.AssociateFile(member.Id.ToGuid(), MemberBaseModule.Key, MemberBaseModule.DisplayName, editMember.Avatar.ToGuid(), "Avatar");
                    }
                }
                else
                {
                    result.ErrorMessage = "此用户名的账号已经存在！";
                }
            }
            else
            {
                //编辑用户
                oldMember.Email = editMember.Email;
                oldMember.PhoneNumber = editMember.PhoneNumber;

                oldMember.NickName = editMember.NickName;
                oldMember.Birthday = editMember.Birthday;
                oldMember.Sex = editMember.Sex;
                oldMember.Province = editMember.Member_Province;
                oldMember.City = editMember.Member_City;
                oldMember.District = editMember.Member_District;
                oldMember.Street = editMember.Member_Street;
                oldMember.Address = editMember.Address;

                var identityResult = _memberService.UpdateMember(oldMember, editMember.Password, editMember.Password2);
                if (!identityResult.Succeeded)
                {
                    result.ErrorMessage = identityResult.Errors.FirstOrDefault();
                }
                else
                {
                    _storageFileService.ReplaceFile(oldMember.Id.ToGuid(), MemberBaseModule.Key, MemberBaseModule.DisplayName, editMember.Avatar.ToGuid(), "Avatar");
                }
            }

            return Json(result);
        }

        [AdminAuthorize(PermissionsArray = new[] { Permissions.ViewMemberKey })]
        public ActionResult List()
        {
            return View();
        }

        [AdminAuthorize(PermissionsArray = new[] { Permissions.ViewMemberKey })]
        public ActionResult ListOnPage()
        {
            var result = new DataTableJsonResult();

            //取参数值
            int draw, pageIndex, pageSize, totalCount;
            string sortColumn;
            bool isDesc;
            Request.GetDatatableParameters(out draw, out pageIndex, out pageSize, out sortColumn, out isDesc);
            result.draw = draw;

            //取查询条件
            var userName = Request.Get("extra_search[UserName]");
            var checkUserName = string.IsNullOrWhiteSpace(userName);
            
            var nickName = Request.Get("extra_search[NickName]");
            var checkNickName = string.IsNullOrWhiteSpace(nickName);

            var sex = Request.Get("extra_search[Sex]");
            var checkSex = string.IsNullOrWhiteSpace(sex);
            var sexInt = sex.To<int>();

             var createTimeBegin = Request.Get("extra_search[CreateTimeBegin]");
            var checkCreateTimeBegin = string.IsNullOrWhiteSpace(createTimeBegin);
            var createTimeBeginTime = createTimeBegin.To<DateTime>();

            var createTimeEnd = Request.Get("extra_search[CreateTimeEnd]");
            var checkCreateTimeEnd = string.IsNullOrWhiteSpace(createTimeEnd);
            var createTimeEndTime = createTimeEnd.To<DateTime>();
            
            var invitationCode = Request.Get("extra_search[InvitationCode]");
            var checkInvitationCode = string.IsNullOrWhiteSpace(invitationCode);

            var phoneNumber = Request.Get("extra_search[PhoneNumber]");
            var checkPhoneNumber = string.IsNullOrWhiteSpace(phoneNumber);

            var province = Request.Get("extra_search[Order_Province]");
            var checkProvince = string.IsNullOrWhiteSpace(province);

            var city = Request.Get("extra_search[Order_City]");
            var checkCity = string.IsNullOrWhiteSpace(city);

            var buyMoneyMin = Request.Get("extra_search[BuyMoneyMin]");
            var checkBuyMoneyMin = string.IsNullOrWhiteSpace(buyMoneyMin) || decimal.Parse(buyMoneyMin) <= 0;
            var buyMoneyMinDec = 0M;
            if (!checkBuyMoneyMin)
                buyMoneyMinDec = decimal.Parse(buyMoneyMin);


            var buyMoneyMax = Request.Get("extra_search[BuyMoneyMax]");
            var checkBuyMoneyMax = string.IsNullOrWhiteSpace(buyMoneyMax) || decimal.Parse(buyMoneyMax) <= 0;
            var buyMoneyMaxDec = 0M;
            if (!checkBuyMoneyMax)
                buyMoneyMaxDec = decimal.Parse(buyMoneyMax);

            var integralMin = Request.Get("extra_search[IntegralMin]");
            var checkIntegralMin = string.IsNullOrWhiteSpace(integralMin) || decimal.Parse(integralMin) <= 0;
            var integralMinDec = 0M;
            if (!checkIntegralMin)
                integralMinDec = decimal.Parse(integralMin);


            var integralMax = Request.Get("extra_search[IntegralMax]");
            var checkIntegralMax = string.IsNullOrWhiteSpace(integralMax) || decimal.Parse(integralMax) <= 0;
            var integralMaxDec = 0M;
            if (!checkIntegralMax)
                integralMaxDec = decimal.Parse(integralMax);

            Expression<Func<MemberFull, bool>> expression =
                l => (checkUserName || l.UserName.Contains(userName)) &&
                     (checkNickName || l.NickName.Contains(nickName)) &&
                     (checkPhoneNumber || l.PhoneNumber.Contains(phoneNumber)) &&
                     (checkInvitationCode || l.InvitationCode.Contains(invitationCode)) &&
                     (checkSex || (int)l.Sex == sexInt) &&
                     (checkCreateTimeBegin || l.CreateTime >= createTimeBeginTime) &&
                     (checkCreateTimeEnd || l.CreateTime <= createTimeEndTime) &&
                     (checkProvince || l.Province == province) &&
                     (checkCity || l.City == city) &&
                     (checkBuyMoneyMin || l.BuyMoney >= buyMoneyMinDec) &&
                     (checkBuyMoneyMax || l.BuyMoney <= buyMoneyMaxDec) &&
                     (checkIntegralMin || l.Integral >= integralMinDec) &&
                     (checkIntegralMax || l.Integral <= integralMaxDec) &&
                     l.UserType == UserType.Member;

            Expression<Func<MemberFull, object>> orderByExpression;
            //设置排序
            switch (sortColumn)
            {
                case "Birthday":
                    orderByExpression = u => new { u.Birthday };
                    break;
                case "Sex":
                    orderByExpression = u => new { u.Sex };
                    break;
                case "NickName":
                    orderByExpression = u => new { u.NickName };
                    break;
                case "CreateTime":
                    orderByExpression = u => new { u.CreateTime };
                    break;
                case "PhoneNumber":
                    orderByExpression = u => new { u.PhoneNumber };
                    break;
                case "Email":
                    orderByExpression = u => new { u.Email };
                    break;
                case "LockoutEnabled":
                    orderByExpression = u => new { u.LockoutEnabled };
                    break;
                case "BuyMoney":
                    orderByExpression = u => new { u.BuyMoney };
                    break;
                case "OrderCount":
                    orderByExpression = u => new { u.OrderCount };
                    break;
                case "Integral":
                    orderByExpression = u => new { u.Integral };
                    break;
                default:
                    orderByExpression = u => new { u.UserName };
                    break;
            }

            //分页查询
            var members = _memberService.GetMemberFullListPaged(pageIndex, pageSize, expression, orderByExpression, isDesc, out totalCount);

            result.data = members;
            result.recordsTotal = totalCount;
            result.recordsFiltered = totalCount;
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [AdminAuthorize(PermissionsArray = new[] { Permissions.EditMemberKey })]
        public ActionResult Switch(SwitchMemberViewModel switchUser)
        {
            var result = new DataJsonResult();
            Member member = _memberService.FindMemberById(switchUser.MemberId);

            if (member != null)
            {
                if (member.UserName.Equals("bocadmin", StringComparison.OrdinalIgnoreCase))
                {
                    result.ErrorMessage = "内置账号不可以禁用！";
                }
                else
                {
                    var identityResult = _memberService.SetLockoutEnabled(member.Id, switchUser.Enabled);

                    if (!identityResult.Succeeded)
                    {
                        result.ErrorMessage = identityResult.Errors.FirstOrDefault();
                    }
                }
            }

            else
            {
                result.ErrorMessage = "此用户名的账号不存在！";
            }

            return Json(result);
        }

        [HttpPost]
        [AdminAuthorize(PermissionsArray = new[] { Permissions.DeleteMemberKey })]
        public async Task<ActionResult> Delete(string memberId)
        {
            var result = new DataJsonResult();
            Member member = _memberService.FindMemberById(memberId);

            if (member != null)
            {
                if (member.UserName.Equals("bocadmin", StringComparison.OrdinalIgnoreCase))
                {
                    result.ErrorMessage = "内置账号不可以删除！";
                }
                else
                {
                    var identityResult = await _memberService.Delete(member);

                    if (!identityResult.Succeeded)
                    {
                        result.ErrorMessage = identityResult.Errors.FirstOrDefault();
                    }
                }
            }
            else
            {
                result.ErrorMessage = "此用户名的账号不存在！";
            }

            return Json(result);
        }

        /// <summary>
        /// 导出
        /// </summary>
        /// <returns></returns>
        [AdminAuthorize(PermissionsArray = new[] { Permissions.ViewMemberKey })]
        public FileResult MemberToExecl()
        {
            //取查询条件
            var userName = Request.Get("UserName");
            var checkUserName = string.IsNullOrWhiteSpace(userName);

            var nickName = Request.Get("NickName");
            var checkNickName = string.IsNullOrWhiteSpace(nickName);

            var sex = Request.Get("Sex");
            var checkSex = string.IsNullOrWhiteSpace(sex);
            var sexInt = sex.To<int>();

            var createTimeBegin = Request.Get("CreateTimeBegin");
            var checkCreateTimeBegin = string.IsNullOrWhiteSpace(createTimeBegin);
            var createTimeBeginTime = createTimeBegin.To<DateTime>();

            var createTimeEnd = Request.Get("CreateTimeEnd");
            var checkCreateTimeEnd = string.IsNullOrWhiteSpace(createTimeEnd);
            var createTimeEndTime = createTimeEnd.To<DateTime>();

            var invitationCode = Request.Get("InvitationCode");
            var checkInvitationCode = string.IsNullOrWhiteSpace(invitationCode);

            var phoneNumber = Request.Get("PhoneNumber");
            var checkPhoneNumber = string.IsNullOrWhiteSpace(phoneNumber);

            var province = Request.Get("Order_Province");
            var checkProvince = string.IsNullOrWhiteSpace(province);

            var city = Request.Get("Order_City");
            var checkCity = string.IsNullOrWhiteSpace(city);

            var buyMoneyMin = Request.Get("BuyMoneyMin");
            var checkBuyMoneyMin = string.IsNullOrWhiteSpace(buyMoneyMin) || decimal.Parse(buyMoneyMin) <= 0;
            var buyMoneyMinDec = 0M;
            if (!checkBuyMoneyMin)
                buyMoneyMinDec = decimal.Parse(buyMoneyMin);


            var buyMoneyMax = Request.Get("BuyMoneyMax");
            var checkBuyMoneyMax = string.IsNullOrWhiteSpace(buyMoneyMax) || decimal.Parse(buyMoneyMax) <= 0;
            var buyMoneyMaxDec = 0M;
            if (!checkBuyMoneyMax)
                buyMoneyMaxDec = decimal.Parse(buyMoneyMax);

            var integralMin = Request.Get("IntegralMin");
            var checkIntegralMin = string.IsNullOrWhiteSpace(integralMin) || decimal.Parse(integralMin) <= 0;
            var integralMinDec = 0M;
            if (!checkIntegralMin)
                integralMinDec = decimal.Parse(integralMin);


            var integralMax = Request.Get("IntegralMax");
            var checkIntegralMax = string.IsNullOrWhiteSpace(integralMax) || decimal.Parse(integralMax) <= 0;
            var integralMaxDec = 0M;
            if (!checkIntegralMax)
                integralMaxDec = decimal.Parse(integralMax);

            Expression<Func<MemberFull, bool>> expression =
                l => (checkUserName || l.UserName.Contains(userName)) &&
                     (checkNickName || l.NickName.Equals(nickName, StringComparison.OrdinalIgnoreCase)) &&
                     (checkPhoneNumber || l.PhoneNumber.Contains(phoneNumber)) &&
                     (checkInvitationCode || l.InvitationCode.Equals(invitationCode, StringComparison.OrdinalIgnoreCase)) &&
                     (checkSex || (int)l.Sex == sexInt) &&
                     (checkCreateTimeBegin || l.CreateTime >= createTimeBeginTime) &&
                     (checkCreateTimeEnd || l.CreateTime <= createTimeEndTime) &&
                     (checkProvince || l.Province == province) &&
                     (checkCity || l.City == city) &&
                     (checkBuyMoneyMin || l.BuyMoney >= buyMoneyMinDec) &&
                     (checkBuyMoneyMax || l.BuyMoney <= buyMoneyMaxDec) &&
                     (checkIntegralMin || l.Integral >= integralMinDec) &&
                     (checkIntegralMax || l.Integral <= integralMaxDec) &&
                     l.UserType == UserType.Member;

            Expression<Func<MemberFull, object>> orderByExpression = u => new { u.CreateTime };
           

            //分页查询
            int totalCount = 0;
            var list = _memberService.GetMemberFullListPaged(1, 999999, expression, orderByExpression, true, out totalCount);


            //创建Excel文件的对象  
            HSSFWorkbook book = new HSSFWorkbook();
            //添加一个sheet  
            ISheet sheet1 = book.CreateSheet("Sheet1");
            if (list != null && list.Count > 0)
            {
                //给sheet1添加第一行的头部标题  
                IRow row1 = sheet1.CreateRow(0);
                row1.CreateCell(0).SetCellValue("用户名");
                row1.CreateCell(1).SetCellValue("手机号");
                row1.CreateCell(2).SetCellValue("昵称");
                row1.CreateCell(3).SetCellValue("性别");
                row1.CreateCell(4).SetCellValue("邀请码");
                row1.CreateCell(5).SetCellValue("总消费额");
                row1.CreateCell(6).SetCellValue("订单总数");
                row1.CreateCell(7).SetCellValue("积分");
                row1.CreateCell(8).SetCellValue("注册时间");
                row1.CreateCell(9).SetCellValue("状态");
                var i = 0;
                foreach (var member in list)
                {
                    IRow rowtemp = sheet1.CreateRow(i + 1);
                    rowtemp.CreateCell(0).SetCellValue(member.UserName);
                    rowtemp.CreateCell(1).SetCellValue(member.PhoneNumber);
                    rowtemp.CreateCell(2).SetCellValue(member.NickName);
                    rowtemp.CreateCell(3).SetCellValue(member.Sex.Description());
                    rowtemp.CreateCell(4).SetCellValue(member.InvitationCode);
                    rowtemp.CreateCell(5).SetCellValue(member.BuyMoney.ToString("#0.00"));
                    rowtemp.CreateCell(6).SetCellValue(member.OrderCount);
                    rowtemp.CreateCell(7).SetCellValue(member.Integral.ToString());
                    rowtemp.CreateCell(8).SetCellValue(string.Format("{0:yyyy-MM-dd HH:mm:ss}", member.CreateTime));

                    var statusName = "";
                    if (member.LockoutEnabled)
                        statusName = "已禁用";
                    else
                        statusName = "已启用";

                    rowtemp.CreateCell(9).SetCellValue(statusName);
                    i++;
                }
            }
            // 写入到客户端   
            var ms = new MemoryStream();
            book.Write(ms);
            ms.Seek(0, SeekOrigin.Begin);
            var dt = DateTime.Now;
            var dateTime = dt.ToString("yyMMddHHmmssfff");
            var fileName = "会员列表" + dateTime + ".xls";
            return File(ms, "application/vnd.ms-excel", fileName);
        }
    }

}