using System.Collections.Generic;
using LexisNexis.Red.Common.Entity;
using Microsoft.Practices.Prism.Mvvm;

namespace LexisNexis.Red.WindowsStore.ViewModels
{
    
    public class BreadcrumbNav:BindableBase
    {
        public int ID { set; get; }
        public int ParentId { set; get; }
        public string Title { set; get; }
        public string Role { set; get; }
        public string GuideCardTitle { set; get; }
        public List<TOCNode> ChildNodes { set; get; }
        public TOCNode ParentNode { set; get; }
        public int NodeLevel { set; get; } 

        private bool isHighLight;
        public bool IsHighLight
        {
            set { SetProperty(ref isHighLight, value); }
            get { return isHighLight; }
        }

        private string icon;
        public string Icon
        {
            set { SetProperty(ref icon, value); }
            get { return icon; }
        }

    }
}
