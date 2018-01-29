using System;
using System.Collections.Generic;
using System.Text;

namespace LexisNexis.Red.Common.Segment.Setting
{
    class SettingLoader
    {
        private void Load(string fileName)
        {
            PanGuSettings.Load(fileName);
        }

        public SettingLoader(string fileName)
        {
            Load(fileName);
        }

        public SettingLoader()
        {
            //string fileName = Framework.Path.GetAssemblyPath() + "xml";
            //Load(fileName);
        }
    }
}
