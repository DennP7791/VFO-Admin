using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Linq;

namespace WDAdmin.Domain.Abstract
{
    //Interface implementation for DataContextProvider
    public interface IVRTDataContextProvider
    {
         DataContext dc { get; }
    }
}
