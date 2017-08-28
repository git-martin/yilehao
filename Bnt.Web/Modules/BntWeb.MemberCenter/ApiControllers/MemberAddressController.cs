using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using BntWeb.Data;
using BntWeb.Data.Services;
using BntWeb.Logistics.Services;
using BntWeb.MemberBase.Models;
using BntWeb.MemberBase.Services;
using BntWeb.MemberCenter.ApiModels;
using BntWeb.Validation;
using BntWeb.WebApi.Filters;
using BntWeb.WebApi.Models;

namespace BntWeb.MemberCenter.ApiControllers
{
    public class MemberAddressController : BaseApiController
    {

        private readonly ICurrencyService _currencyService;
        private readonly IMemberService _memberService;
        private readonly IShippingAreaService _shippingAreaService;

        public MemberAddressController(ICurrencyService currencyService, IMemberService memberService, IShippingAreaService shippingAreaService)
        {
            _currencyService = currencyService;
            _memberService = memberService;
            _shippingAreaService = shippingAreaService;
        }

        [HttpPost]
        [BasicAuthentication]
        public ApiResult AddAddress([FromBody]CreateMemberAddressModels editModel)
        {

            Argument.ThrowIfNullOrEmpty(editModel.Address, "详细地址");
            Argument.ThrowIfNullOrEmpty(editModel.Contacts, "收货人");
            Argument.ThrowIfNullOrEmpty(editModel.Phone, "联系电话");
            Argument.ThrowIfNullOrEmpty(editModel.Province, "省");
            Argument.ThrowIfNullOrEmpty(editModel.City, "市");
            //Argument.ThrowIfNullOrEmpty(editModel.District, "区");
            //Argument.ThrowIfNullOrEmpty(editModel.Street, "街道");
            Argument.ThrowIfNullOrEmpty(editModel.RegionName, "省市区街道");
            //Argument.ThrowIfNullOrEmpty(editModel.Postcode, "邮政编码");

            var notShipping = _shippingAreaService.NotShippingArea(editModel.City) ??
                              _shippingAreaService.NotShippingArea(editModel.Province);
            if (notShipping != null)
            {
                throw new WebApiInnerException("3002", "您添加的地址不在我们的配送范围内！");
            }

            if (!string.IsNullOrWhiteSpace(editModel.Postcode))
            {
                editModel.Postcode = Utility.Extensions.StringExtensions.NumericOnly(editModel.Postcode);
                if (editModel.Postcode.Length < 6)
                    throw new WebApiInnerException("0002", "邮编必须为6位纯数字");
            }
            
            var myAddress = _currencyService.GetList<MemberAddress>(me => me.MemberId == AuthorizedUser.Id);
            var model = new MemberAddress
            {
                Id = KeyGenerator.GetGuidKey(),
                MemberId = AuthorizedUser.Id,
                Address = editModel.Address,
                Contacts = editModel.Contacts,
                Phone = editModel.Phone,
                Province = editModel.Province,
                City = editModel.City,
                District = editModel.District,
                Street = editModel.Street,
                RegionName = editModel.RegionName,
                Postcode = editModel.Postcode
            };

            if (myAddress == null || myAddress.Count == 0)
                model.IsDefault = true;
            if (!_currencyService.Create(model))
                throw new WebApiInnerException("3001", "添加失败,内部执出错");

            var member = _currencyService.GetSingleById<MemberExtension>(AuthorizedUser.Id);
            if (string.IsNullOrWhiteSpace(member.Address) && (myAddress == null || myAddress.Count == 0))
            {
                member.Address = model.Address;
                _currencyService.Update(member);
            }

            var result = new ApiResult();
            result.SetData(new ListAddressModel(model));
            return result;
        }

        [HttpDelete]
        [BasicAuthentication]
        public ApiResult DeleteAddress(Guid addressId)
        {

            var address = _currencyService.GetSingleById<MemberAddress>(addressId);
            if (address == null)
                throw new WebApiInnerException("3002", "地址不存在");

            if (_currencyService.DeleteByConditon<MemberAddress>(me => me.Id == addressId) < 1)
                throw new WebApiInnerException("3003", "删除失败内部执出错");

            var result = new ApiResult();
            return result;
        }

        [HttpPatch]
        [BasicAuthentication]
        public ApiResult EditAddress([FromBody]EditMemberAddressModels editModel)
        {
            Argument.ThrowIfNullOrEmpty(editModel.Address, "地址");
            Argument.ThrowIfNullOrEmpty(editModel.Contacts, "收件人");
            Argument.ThrowIfNullOrEmpty(editModel.Phone, "联系电话");
            Argument.ThrowIfNullOrEmpty(editModel.Province, "省");
            Argument.ThrowIfNullOrEmpty(editModel.City, "市");
            //Argument.ThrowIfNullOrEmpty(editModel.District, "区");
            //Argument.ThrowIfNullOrEmpty(editModel.Street, "街道");
            Argument.ThrowIfNullOrEmpty(editModel.RegionName, "省市区街道");
            //Argument.ThrowIfNullOrEmpty(editModel.Postcode, "邮政编码");

            var notShipping = _shippingAreaService.NotShippingArea(editModel.City) ??
                              _shippingAreaService.NotShippingArea(editModel.Province);
            if (notShipping != null)
            {
                throw new WebApiInnerException("3002", "您添加的地址不在我们的配送范围内！");
            }

            if (!string.IsNullOrWhiteSpace(editModel.Postcode))
            {
                editModel.Postcode = Utility.Extensions.StringExtensions.NumericOnly(editModel.Postcode);
                if (editModel.Postcode.Length < 6)
                    throw new WebApiInnerException("0002", "邮编必须为6位纯数字");
            }

            var address = _currencyService.GetSingleById<MemberAddress>(editModel.AddressId);
            
            if (address == null)
                throw new WebApiInnerException("3004", "地址不存在");
            
            address.Address = editModel.Address;
            address.Contacts = editModel.Contacts;
            address.Phone = editModel.Phone;
            address.Province = editModel.Province;
            address.City = editModel.City;
            address.District = editModel.District;
            address.Street = editModel.Street;
            address.RegionName = editModel.RegionName;
            address.Postcode = editModel.Postcode;

            if (!_currencyService.Update(address))
            {
                throw new WebApiInnerException("3005", "编辑失败，内部执行错误");
            }
            var result = new ApiResult();
            return result;
        }

        [HttpPatch]
        [BasicAuthentication]
        public ApiResult SetDefault(Guid addressId)
        {
            var address = _currencyService.GetSingleById<MemberAddress>(addressId);
            if (address == null)
                throw new WebApiInnerException("3004", "地址不存在");

            address.IsDefault = true;

            _memberService.SetDefaultAddress(AuthorizedUser.Id, addressId);

            var result = new ApiResult();
            return result;
        }

        [HttpGet]
        [BasicAuthentication]
        public ApiResult MyAddressList()
        {

            var addresses = _currencyService.GetList<MemberAddress>(me => me.MemberId == AuthorizedUser.Id);
            var result = new ApiResult();
            var data = new
            {
                Addresses = addresses.Select(me => new ListAddressModel(me)).ToList()
            };

            result.SetData(data);
            return result;
        }

        [HttpGet]
        [BasicAuthentication]
        public ApiResult MyAddressList(Guid addressId)
        {
            var address = _currencyService.GetSingleByConditon<MemberAddress>(me => me.MemberId == AuthorizedUser.Id && me.Id.Equals(addressId));
            var result = new ApiResult();
            result.SetData(new ListAddressModel(address));
            return result;
        }

    }
}
