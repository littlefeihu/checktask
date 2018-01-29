using LexisNexis.Red.Common.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LexisNexis.Red.Common.HelpClass;

namespace LexisNexis.Red.Common.Entity
{
    public class TOCNode
    {
        public TOCNode()
        {

        }
        public int ID { set; get; }
        public int ParentId { set; get; }
        public string Title { set; get; }
        public string Role { set; get; }

        public string GuideCardTitle { set; get; }
        public long SortOrder { set; get; }

        public List<TOCNode> ChildNodes { set; get; }

        private TOCNode _parentNode;

        public TOCNode ParentNode
        {
            get
            {
                return _parentNode;
            }
            internal set
            {
                _parentNode = value;

                if (_parentNode != null)
                {
                    _nodeLevel = _parentNode._nodeLevel + 1;
                }
            }
        }

        private int _nodeLevel = 0;

        public int NodeLevel
        {
            get { return _nodeLevel; }
            set
            {
                _nodeLevel = value;
            }
        }
        public bool IsLeafNode()
        {
            return (this.Role != Constants.ANCESTOR);
        }

        public bool IsTopLevel()
        {
            return (this.NodeLevel == 1);
        }
        public bool HasParentNode()
        {
            return (this.ParentNode != null);
        }
        public bool HasChildNodes()
        {
            return (this.ChildNodes != null && this.ChildNodes.Count > 0);
        }

        public bool HasSibling(bool isNext, out TOCNode sibling)
        {
            sibling = isNext ? this.NextSibling() : this.PreviousSibling();

            return (sibling != null);
        }

        public TOCNode GetRootTOCNode()
        {
            var node = this;
            if (node.NodeLevel != 0)
            {
                while (node.NodeLevel != 0)
                {
                    if (node.ParentNode != null)
                        node = node.ParentNode;
                }
            }
            return node;
        }
    }
}
