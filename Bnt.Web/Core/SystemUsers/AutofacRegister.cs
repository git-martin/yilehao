/* 
    ======================================================================== 
        File name：        IoCRegister
        Module:                
        Author：            罗嗣宝
        Create Time：    2016/5/16 15:47:46
        Modify By:        
        Modify Date:    
    ======================================================================== 
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Autofac;
using BntWeb.Environment.IoC;

namespace BntWeb.Core.SystemUsers
{
    public class AutofacRegister : IRegisterProvider
    {
        public void Register(ContainerBuilder builder)
        {
        }
    }
}