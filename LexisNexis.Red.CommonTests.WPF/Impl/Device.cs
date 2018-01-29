
using LexisNexis.Red.Common.BusinessModel;
using LexisNexis.Red.Common.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LexisNexis.Red.CommonTests.Interface
{
    public class Device : IDevice
    {
        public string GetDeviceID()
        {
            return "xx5pro3X36Y5yj7jnR1c7QQ==15";
            // return "2EAAEBF5E7761C322A7B610B319333B9DDD37A59";
        }
        public string GetDeviceOS()
        {
            return "Microsoft Windows 8 Enterprise";
        }


        public DeviceTypeEnum GetDeviceType()
        {
            return DeviceTypeEnum.WindowsPC;
        }

        public string GetEreaderVersion()
        {
            return "2.3.59";
        }
    }
}
