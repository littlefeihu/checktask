using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;
using Windows.Storage.Streams;
using Windows.System.Profile;
using LexisNexis.Red.Common.BusinessModel;
using LexisNexis.Red.Common.Interface;

namespace LexisNexis.Red.WindowsStore.Implementation
{
    public class WindowsDevice:IDevice
    {
        public string GetDeviceID()
        {
            var token = HardwareIdentification.GetPackageSpecificToken(null);
            IBuffer buffer = token.Id;
            var md5Provider = HashAlgorithmProvider.OpenAlgorithm("MD5");
            var hashdata = md5Provider.HashData(buffer);


            return CryptographicBuffer.EncodeToHexString(hashdata);
        }

        public string GetDeviceOS()
        {
            return "Microsoft Windows 8 Enterprise";
        }

        public DeviceTypeEnum GetDeviceType()
        {
            return  DeviceTypeEnum.WindowsPC;
        }

        public string GetEreaderVersion()
        {
            return "3.0";
        }
    }
}
