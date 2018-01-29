using LexisNexis.Red.Common.BusinessModel;
using LexisNexis.Red.Common.Entity;
using LexisNexis.Red.Common.HelpClass;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LexisNexis.Red.Common.Business
{
    public class AppServiceBase<T>
    {
        private static T instance = IoCContainer.Instance.Resolve<T>();

        public static T Instance
        {
            get
            {
                if (instance == null)
                    instance = IoCContainer.Instance.Resolve<T>();
                return instance;
            }
        }

    }
}
