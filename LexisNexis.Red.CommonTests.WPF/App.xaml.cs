using LexisNexis.Red.Common;
using LexisNexis.Red.Common.HelpClass;
using LexisNexis.Red.Common.Interface;
using LexisNexis.Red.CommonTests.Impl;
using LexisNexis.Red.CommonTests.Interface;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;

namespace LexisNexis.Red.CommonTests.WPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            try
            {
                IoCContainer.Instance.RegisterInterface<IDevice, Device>();
                IoCContainer.Instance.RegisterInterface<IDirectory, MyDirectory>();
                IoCContainer.Instance.RegisterInterface<ICryptogram, Cryptogram>();
                IoCContainer.Instance.RegisterInterface<INetwork, Network>();
                IoCContainer.Instance.RegisterInterface<IPackageFile, PackageFile>();
                LexisNexis.Red.Common.HelpClass.GlobalAccess.Instance.Init().Wait();
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
