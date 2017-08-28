using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using BntWeb.Feedback.Models;

namespace BntWeb.Feedback.ViewModels
{
    public class ProcesseFeedbackViewModel
    {
        /// <summary>
        /// 活动Id
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// 处理状态
        /// </summary>
        [Required]
        [Display(Name = "处理状态")]
        public ProcesseStatus ProcesseStatus { get; set; }

        /// <summary>
        /// 处理意见
        /// </summary>
        [Display(Name = "处理意见")]
        public string ProcesseRemark { get; set; }

    }
}