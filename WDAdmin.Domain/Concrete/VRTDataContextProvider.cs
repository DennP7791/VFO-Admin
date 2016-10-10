using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Linq;
using WDAdmin.Domain.Abstract;

namespace WDAdmin.Domain.Concrete
{
    //Implementation of DBContextProvider for VRT
    public class VRTDataContextProvider : IVRTDataContextProvider
    {
        private DataContext dataContext;
       
        public VRTDataContextProvider(string connectionString)
        {
            dataContext = new DataContext(connectionString);
        }

        public DataContext dc
        {
            get { return dataContext; }
        }
    }
}
