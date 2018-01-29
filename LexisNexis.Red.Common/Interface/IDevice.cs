using LexisNexis.Red.Common.BusinessModel;
namespace LexisNexis.Red.Common.Interface
{
    public interface IDevice
    {
        /// <summary>
        /// getdeviceid 
        /// </summary>
        /// <returns></returns>
        string GetDeviceID();

        /// <summary>
        /// GetDevice OS
        /// </summary>
        /// <returns></returns>
        string GetDeviceOS();

        DeviceTypeEnum GetDeviceType();

        string GetEreaderVersion();

    }
}
