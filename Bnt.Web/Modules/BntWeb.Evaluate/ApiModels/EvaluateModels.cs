using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BntWeb.Environment;
using BntWeb.Evaluate.Services;
using BntWeb.FileSystems.Media;
using BntWeb.MemberBase.Services;
using Autofac;
using BntWeb.Evaluate.Models;

namespace BntWeb.Evaluate.ApiModels
{

    public class CreateEvaluateModel
    {
        /// <summary>
        /// 内容
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// 评分
        /// </summary>
        public int Score { get; set; }


        public List<Base64Image> Images { set; get; }
    }

    public class ListEvaluateModel
    {
       

        /// <summary>
        /// ID
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// 内容
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// 评分
        /// </summary>
        public int Score { get; set; }

        /// <summary>
        /// 评价人Id
        /// </summary>
        public string MemberId { get; set; }

        /// <summary>
        /// 评价人名称
        /// </summary>
        public string MemberName { get; set; }


        /// <summary>
        /// 评价时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        public List<SimplifiedStorageFile> Images { set; get; }

        public SimplifiedStorageFile Avatar { set; get; }

        public ListEvaluateModel(Models.Evaluate model)
        {
            Id = model.Id;
            Content = model.Content;
            Score = model.Score;
            MemberId = model.MemberId;
            MemberName = model.MemberName;
            CreateTime = model.CreateTime;

            var storageFileService = HostConstObject.Container.Resolve<IStorageFileService>();
            
            Images = storageFileService.GetFiles(model.Id, EvaluateModule.Key).Select(me => me.Simplified()).ToList();

            var memberService = HostConstObject.Container.Resolve<IMemberService>();

            var file = memberService.GetAvatarFile(model.MemberId);
            Avatar = file?.Simplified();
        }
    }

    public class EvaluateDetail
    {
        /// <summary>
        /// ID
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// 内容
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// 评分
        /// </summary>
        public int Score { get; set; }

        /// <summary>
        /// 评价人Id
        /// </summary>
        public string MemberId { get; set; }

        /// <summary>
        /// 评价人名称
        /// </summary>
        public string MemberName { get; set; }

        /// <summary>
        /// 评价时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        public List<SimplifiedStorageFile> Images { set; get; }

        public SimplifiedStorageFile Avatar { set; get; }

        public EvaluateDetail(Models.Evaluate model, List<StorageFile> imgList)
        {
            Id = model.Id;
            Content = model.Content;
            Score = model.Score;
            MemberId = model.MemberId;
            MemberName = model.MemberName;
            CreateTime = model.CreateTime;

            Images = imgList.Select(me => me.Simplified()).ToList();
        }
    }

}