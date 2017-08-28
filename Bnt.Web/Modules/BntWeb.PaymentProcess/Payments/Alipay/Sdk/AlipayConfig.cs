using System.Web;
using System.Text;
using System.IO;
using System.Net;
using System;
using System.Collections.Generic;

namespace Com.Alipay
{
    /// <summary>
    /// 类名：Config
    /// 功能：基础配置类
    /// 详细：设置帐户有关信息及返回路径
    /// 版本：3.4
    /// 修改日期：2016-03-08
    /// 说明：
    /// 以下代码只是为了方便商户测试而提供的样例代码，商户可以根据自己网站的需要，按照技术文档编写,并非一定要使用该代码。
    /// 该代码仅供学习和研究支付宝接口使用，只是提供一个参考。
    /// </summary>
    public class AlipayConfig2
    {

        //↓↓↓↓↓↓↓↓↓↓请在这里配置您的基本信息↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓

        // 合作身份者ID，以2088开头由16位纯数字组成的字符串，查看地址：https://b.alipay.com/order/pidAndKey.htm
        public string Partner { get; set; }

        // 收款支付宝账号
        public string SellerId { get; set; }

        //商户的私钥,原始格式，RSA公私钥生成：https://doc.open.alipay.com/doc2/detail.htm?spm=a219a.7629140.0.0.nBDxfy&treeId=58&articleId=103242&docType=1
        public string PrivateKey { get; set; }

        //支付宝的公钥，查看地址：https://b.alipay.com/order/pidAndKey.htm 
        public string AlipayPublicKey { get; set; }

        // MD5密钥，安全检验码，由数字和字母组成的32位字符串，查看地址：https://b.alipay.com/order/pidAndKey.htm
        public string Md5Key { get; set; } = "";

        // 签名方式
        public SignType SignType { get; set; } = SignType.RSA;

        // 字符编码格式 目前支持 gbk 或 utf-8
        public readonly string InputCharset = "utf-8";

        // 支付类型 ，无需修改
        public readonly string PaymentType = "1";

        // 调用的接口名，无需修改
        public readonly string Service = "create_direct_pay_by_user";

        //↑↑↑↑↑↑↑↑↑↑请在这里配置您的基本信息↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑


        //↓↓↓↓↓↓↓↓↓↓请在这里配置防钓鱼信息，如果没开通防钓鱼功能，请忽视不要填写 ↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓

        //防钓鱼时间戳  若要使用请调用类文件submit中的Query_timestamp函数
        public readonly string AntiPhishingKey = "";

        //客户端的IP地址 非局域网的外网IP地址，如：221.0.0.1
        public readonly string ExterInvokeIP = "";

        //↑↑↑↑↑↑↑↑↑↑请在这里配置防钓鱼信息，如果没开通防钓鱼功能，请忽视不要填写 ↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑

    }

    public enum SignType
    {
        RSA,
        MD5
    }
}