using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Xaml.Navigation;
using LexisNexis.Red.Common.Business;
using LexisNexis.Red.Common.Common;
using LexisNexis.Red.Common.Entity;
using Microsoft.Practices.Prism.Mvvm;
using LexisNexis.Red.Common.BusinessModel;
using Microsoft.Practices.Prism.Commands;
using LexisNexis.Red.WindowsStore.Html;
using System;
using Windows.ApplicationModel.Resources;
using LexisNexis.Red.WindowsStore.Tools;

namespace LexisNexis.Red.WindowsStore.ViewModels
{
    public class ContentPageViewModel : BaseViewModel
    {
        private const string COLLAPSED_ICON = "\uE013";
        private const string EXPAND_ICON = "\uE015";
        private const string LEAF_NODE_ICON = "\uE160";

        #region Commands
        public DelegateCommand ShrinkCommand
        {
            get;
            private set;
        }
        public DelegateCommand ForWardCommand
        {
            get;
            private set;
        }
        public DelegateCommand BackWardCommand
        {
            get;
            private set;
        }

        #endregion
        #region Field

        private PublicationViewModel publication;
        [RestorableState]
        public PublicationViewModel Publication
        {
            get { return publication; }
            set { SetProperty(ref publication, value); }
        }

        private bool leftSidePanelVisible = true;
        public bool LeftSidePanelVisible
        {
            get { return leftSidePanelVisible; }
            set { SetProperty(ref leftSidePanelVisible, value); }
        }

        private bool isLoadingTop = false;
        public bool IsLoadingTop
        {
            get { return isLoadingTop; }
            set { SetProperty(ref isLoadingTop, value); }
        }

        private bool isLoadingBottom = false;
        public bool IsLoadingBottom
        {
            get { return isLoadingBottom; }
            set { SetProperty(ref isLoadingBottom, value); }
        }

        private string loadingTitle;
        public string LoadingTitle
        {
            get { return loadingTitle; }
            set { SetProperty(ref loadingTitle, value); }
        }

        private bool isPBO;
        [RestorableState]
        public bool IsPBO
        {
            get { return isPBO; }
            set { SetProperty(ref isPBO, value); }
        }

        private string pageNum = "Page ";
        [RestorableState]
        public string PageNum
        {
            get { return pageNum; }
            set { SetProperty(ref pageNum, value); }
        }

        private bool ispageResultShow;
        [RestorableState]
        public bool IspageResultShow
        {
            get { return ispageResultShow; }
            set { SetProperty(ref ispageResultShow, value); }
        }
        #endregion

        public ContentPageViewModel()
        {
            ShrinkCommand = new DelegateCommand(() =>
            {
                LeftSidePanelVisible = !LeftSidePanelVisible;
            });
            ForWardCommand = DelegateCommand.FromAsyncHandler(GoForWardRecord);
            BackWardCommand = DelegateCommand.FromAsyncHandler(GoBackWardRecord);
        }

        public override async void OnNavigatedTo(object navigationParameter, NavigationMode navigationMode, Dictionary<string, object> viewModelState)
        {
            base.OnNavigatedTo(navigationParameter, navigationMode, viewModelState);
            if (navigationParameter != null)
            {
                string param = navigationParameter.ToString();
                string[] paramList = param.Split('|');

                var bookId = int.Parse(paramList[0]);
                if (SessionService.SessionState.ContainsKey(ALL_PUBLICATIONS_SESSION_KEY))
                {
                    var publications =
                          SessionService.SessionState[ALL_PUBLICATIONS_SESSION_KEY] as List<PublicationViewModel>;

                    Publication = publications.FirstOrDefault(x => x.BookId == bookId);
                }
                if (navigationMode == NavigationMode.New)
                {
                    HistoryNavigator = new NavigationRecordManager();
                    if (paramList.Length > 1)
                    {
                        CurrentTocNode = new TocCurrentNode
                        {
                            ID = int.Parse(paramList[1]),
                            Role = NodeExpandStatue.Leaf
                        };
                    }
                }
                await InitialContent(paramList, navigationMode);
            }
        }

        private async Task InitialContent(string[] paramList, NavigationMode navigationMode)
        {
            #region PBO Search
            IsPBO = await PageSearchUtil.Instance.IsPBO(Publication.BookId);
            #endregion

            #region toc initial
            root = await PublicationUtil.Instance.GetDlBookTOC(Publication.BookId);
            listAll = root.ChildNodes;
            firstNode = null;
            GetFirstTocNode(listAll, ref firstNode);
            NavigationRecord record = new NavigationRecord
            {
                BookId = Publication.BookId,
                TOCId = firstNode.ID,
                Type = NavigationType.TOCDocument
            };
            var nodes = await InitialBreadcrumMenu(record, TOCAccessMode.All);
            BreadcrumbNavigator = new ObservableCollection<BreadcrumbNav>(nodes);
            #endregion
            if (navigationMode == NavigationMode.New)
            {
                #region index initial
                var indexs = await InitialIndexMenu(Publication.BookId);
                IndexMenuCollections = new ObservableCollection<IndexMenuItem>(indexs);
                HasIndex = IndexMenuCollections == null || IndexMenuCollections.Count == 0 ? false : true;
                #endregion

                #region tag
                var tags = AnnCategoryTagUtil.Instance.GetTags();
                var tagList = tags.Select(tag => new TagItem
                {
                    Tag = tag,
                    IsCheck = false
                });
                TagCollections = new ObservableCollection<TagItem>(tagList);
                #endregion

            }
        }

        #region TOC

        private ObservableCollection<BreadcrumbNav> breadcrumbNavigator;
        public ObservableCollection<BreadcrumbNav> BreadcrumbNavigator
        {
            get { return breadcrumbNavigator; }
            private set { SetProperty(ref breadcrumbNavigator, value); }
        }

        private string selectedContent;
        [RestorableState]
        public string SelectedContent
        {
            get { return selectedContent; }
            private set { SetProperty(ref selectedContent, value); }
        }
        [RestorableState]
        public TocCurrentNode CurrentTocNode { get; set; }
        [RestorableState]
        public int CurrentLevel { get; set; }
        public async Task BreadcrumItemExpandStatueChange(BreadcrumbNav selectedNode)
        {
            NodeExpandStatue selectedNodeStatue = NodeExpandStatue.Leaf;
            if (selectedNode.NodeLevel == 0)
            {
                if (CurrentLevel > 0)
                {
                    selectedNodeStatue = NodeExpandStatue.Collapse;

                    List<BreadcrumbNav> bindingBreadcrumbNavigator = new List<BreadcrumbNav>();
                    bindingBreadcrumbNavigator.Add(BreadcrumbNavigator[0]);
                    int tocID = BreadcrumbNavigator[1].ID;
                    bindingBreadcrumbNavigator.AddRange(selectedNode.ChildNodes.Select(node => new BreadcrumbNav
                        {
                            ID = node.ID,
                            Title = node.Title,
                            GuideCardTitle = node.GuideCardTitle,
                            Role = node.Role,
                            NodeLevel = node.NodeLevel,
                            ChildNodes = node.ChildNodes,
                            ParentId = node.ParentId,
                            ParentNode = node.ParentNode,
                            Icon = node.Role == Constants.ANCESTOR ? COLLAPSED_ICON : LEAF_NODE_ICON,
                            IsHighLight = tocID == node.ID
                        }));
                    //reset selected node after tapped toc header
                    selectedNode = bindingBreadcrumbNavigator.First(node => node.ID == tocID);
                    BreadcrumbNavigator = new ObservableCollection<BreadcrumbNav>(bindingBreadcrumbNavigator);
                    CurrentLevel = 0;
                }
            }
            else if (selectedNode.Role == Constants.ANCESTOR)
            {
                List<BreadcrumbNav> bindingBreadcrumbNavigator = BreadcrumbNavigator.ToList();
                // expand  
                if (selectedNode.NodeLevel > CurrentLevel)
                {
                    selectedNodeStatue = NodeExpandStatue.Expand;

                    bindingBreadcrumbNavigator.RemoveAll(n => n.NodeLevel == selectedNode.NodeLevel && n.ID != selectedNode.ID);
                    selectedNode.Icon = EXPAND_ICON;
                    selectedNode.IsHighLight = true;
                    if (selectedNode.ChildNodes != null)
                    {
                        bindingBreadcrumbNavigator.AddRange(
                        selectedNode.ChildNodes.Select(node => new BreadcrumbNav
                        {
                            ID = node.ID,
                            Title = node.Title,
                            GuideCardTitle = node.GuideCardTitle,
                            Role = node.Role,
                            NodeLevel = node.NodeLevel,
                            ChildNodes = node.ChildNodes,
                            ParentId = node.ParentId,
                            ParentNode = node.ParentNode,
                            Icon = node.Role == Constants.ANCESTOR ? COLLAPSED_ICON : LEAF_NODE_ICON
                        }));
                    }
                    foreach (var breadcrumb in bindingBreadcrumbNavigator)
                    {
                        breadcrumb.IsHighLight = breadcrumb.ID == selectedNode.ID;
                    }
                    BreadcrumbNavigator = new ObservableCollection<BreadcrumbNav>(bindingBreadcrumbNavigator);
                    CurrentLevel = selectedNode.NodeLevel;
                }
                //collapse
                else if (selectedNode.NodeLevel <= CurrentLevel)
                {
                    selectedNodeStatue = NodeExpandStatue.Collapse;

                    bindingBreadcrumbNavigator.RemoveAll(n => n.NodeLevel >= selectedNode.NodeLevel);
                    bindingBreadcrumbNavigator.AddRange(
                     selectedNode.ParentNode.ChildNodes.Select(node => new BreadcrumbNav
                     {
                         ID = node.ID,
                         Title = node.Title,
                         GuideCardTitle = node.GuideCardTitle,
                         Role = node.Role,
                         NodeLevel = node.NodeLevel,
                         ChildNodes = node.ChildNodes,
                         ParentId = node.ParentId,
                         ParentNode = node.ParentNode,
                         Icon = node.Role == Constants.ANCESTOR ? COLLAPSED_ICON : LEAF_NODE_ICON
                     }));
                    foreach (var breadcrumb in bindingBreadcrumbNavigator)
                    {
                        breadcrumb.IsHighLight = breadcrumb.ID == selectedNode.ID;
                    }
                    BreadcrumbNavigator = new ObservableCollection<BreadcrumbNav>(bindingBreadcrumbNavigator);
                    CurrentLevel = selectedNode.NodeLevel - 1;
                }
            }
            else//open doc content
            {
                foreach (var breadcrumb in BreadcrumbNavigator)
                {
                    breadcrumb.IsHighLight = breadcrumb.ID == selectedNode.ID;
                }
                if (selectedNode.ID == CurrentTocNode.ID)
                    return;
                var content = await PublicationContentUtil.Instance.GetContentFromTOC(Publication.BookId, new TOCNode
                     {
                         ID = selectedNode.ID,
                         Title = selectedNode.Title,
                         ParentId = selectedNode.ParentId,
                         NodeLevel = selectedNode.NodeLevel,
                         GuideCardTitle = selectedNode.GuideCardTitle,
                         Role = selectedNode.Role,
                         ChildNodes = selectedNode.ChildNodes
                     });
                var record = new NavigationRecord
                {
                    TOCId = selectedNode.ID,
                    Type = NavigationType.TOCDocument
                };

                ChangeTocContent(await HtmlHelper.PageSplit(content, record), record.TOCId);
                CurrentLevel = selectedNode.NodeLevel - 1;
                HistoryNavigator.Record(Publication.BookId, selectedNode.ID);
            }

            CurrentTocNode = new TocCurrentNode
            {
                ID = selectedNode.ID,
                Role = selectedNodeStatue
            };
        }

        TOCNode root = null;
        TOCNode firstNode = null;
        List<TOCNode> listAll = null;

        private async Task<List<BreadcrumbNav>> InitialBreadcrumMenu(NavigationRecord record, TOCAccessMode mode)
        {
            List<BreadcrumbNav> selectedBreadcrumbNavs = null;
            //restore toc and get content
            if (CurrentTocNode != null && CurrentTocNode.ID != 0)
            {
                List<TOCNode> selectedNodes = new List<TOCNode>();
                TOCNode tocNode = null;
                GetTocNodeByID(listAll, ref tocNode, CurrentTocNode.ID);

                if (CurrentTocNode.Role == NodeExpandStatue.Leaf)
                {
                    switch (mode)
                    {
                        case TOCAccessMode.All:
                            var content1 = await PublicationContentUtil.Instance.GetContentFromTOC(record.BookId, tocNode);

                            ChangeTocContent(await HtmlHelper.PageSplit(content1, record), record.TOCId);
                            HistoryNavigator.Record(Publication.BookId, tocNode.ID, record.Type, record.PageNum, record.Tag);
                            break;
                        case TOCAccessMode.IgnoreHistory:
                            var content2 = await PublicationContentUtil.Instance.GetContentFromTOC(record.BookId, tocNode, false);
                            ChangeTocContent(await HtmlHelper.PageSplit(content2, record), record.TOCId);
                            break;
                        case TOCAccessMode.NoReload:
                            break;
                    }
                    CurrentLevel = tocNode.NodeLevel - 1;
                }
                else if (CurrentTocNode.Role == NodeExpandStatue.Expand)
                {
                    CurrentLevel = tocNode.NodeLevel;
                }
                else if (CurrentTocNode.Role == NodeExpandStatue.Collapse)
                {
                    CurrentLevel = tocNode.NodeLevel - 1;
                }
                //add parent
                TOCNode tempNode = tocNode.ParentNode;
                while (tempNode != null && tempNode.NodeLevel > 0)
                {
                    selectedNodes.Insert(0, tempNode);
                    tempNode = tempNode.ParentNode;
                }

                //add me and children
                if (CurrentTocNode.Role == NodeExpandStatue.Expand)
                {
                    selectedNodes.Add(tocNode);
                    selectedNodes.AddRange(tocNode.ChildNodes);
                }
                else
                {
                    selectedNodes.AddRange(tocNode.ParentNode.ChildNodes);
                }

                selectedBreadcrumbNavs = selectedNodes.Select(node => new BreadcrumbNav
                    {
                        ID = node.ID,
                        Title = node.Title,
                        GuideCardTitle = node.GuideCardTitle,
                        Role = node.Role,
                        NodeLevel = node.NodeLevel,
                        ChildNodes = node.ChildNodes,
                        ParentId = node.ParentId,
                        ParentNode = node.ParentNode,
                        Icon = node.Role == Constants.ANCESTOR ? (node.NodeLevel > CurrentLevel ? COLLAPSED_ICON : EXPAND_ICON) : LEAF_NODE_ICON,
                        IsHighLight = node.ID == CurrentTocNode.ID
                    }).ToList<BreadcrumbNav>();
            }
            else
            {
                CurrentTocNode = new TocCurrentNode
                {
                    ID = record.TOCId,
                    Role = NodeExpandStatue.Leaf
                };
                CurrentLevel = 0;
                var content = await PublicationContentUtil.Instance.GetContentFromTOC(record.BookId, firstNode);
                ChangeTocContent(await HtmlHelper.PageSplit(content, record), record.TOCId);
                HistoryNavigator.Record(Publication.BookId, record.TOCId, record.Type, record.PageNum, record.Tag);
                selectedBreadcrumbNavs = listAll.Select(node => new BreadcrumbNav
                {
                    ID = node.ID,
                    Title = node.Title,
                    GuideCardTitle = node.GuideCardTitle,
                    Role = node.Role,
                    NodeLevel = node.NodeLevel,
                    ChildNodes = node.ChildNodes,
                    ParentId = node.ParentId,
                    ParentNode = root,
                    Icon = node.Role == Constants.ANCESTOR ? COLLAPSED_ICON : LEAF_NODE_ICON,
                    IsHighLight = false
                }).ToList();
                selectedBreadcrumbNavs[0].IsHighLight = true;
            }
            selectedBreadcrumbNavs.Insert(0, new BreadcrumbNav
                {
                    ID = 0,
                    Title = "Table of Contents",
                    NodeLevel = 0,
                    ChildNodes = root.ChildNodes
                });
            return selectedBreadcrumbNavs;
        }

        private void GetTocNodeByID(List<TOCNode> nodes, ref TOCNode selectedNode, int id)
        {
            foreach (TOCNode node in nodes)
            {
                if (selectedNode != null)
                    return;
                if (node.ID == id)
                {
                    selectedNode = node;
                    return;
                }
                if (node.ChildNodes != null)
                {
                    GetTocNodeByID(node.ChildNodes, ref selectedNode, id);
                }
            }
        }

        #endregion
        #region Index

        private ObservableCollection<IndexMenuItem> indexMenuCollections;
        [RestorableState]
        public ObservableCollection<IndexMenuItem> IndexMenuCollections
        {
            get { return indexMenuCollections; }
            private set { SetProperty(ref indexMenuCollections, value); }
        }

        private bool hasIndex;
        [RestorableState]
        public bool HasIndex
        {
            get { return hasIndex; }
            private set { SetProperty(ref hasIndex, value); }
        }

        private string indexContent = string.Empty;
        [RestorableState]
        public string IndexContent
        {
            get { return indexContent; }
            private set { SetProperty(ref indexContent, value); }
        }

        private string currentIndex;
        [RestorableState]
        public string CurrentIndex
        {
            get { return currentIndex; }
            private set { SetProperty(ref currentIndex, value); }
        }

        public string SelectIndexTitle { set; get; }
        private async Task<List<IndexMenuItem>> InitialIndexMenu(int bookId)
        {
            List<IndexMenuItem> allIndex = new List<IndexMenuItem>();
            var list = await PublicationUtil.Instance.GetIndexsByBookId(Publication.BookId);
            if (list == null)
            {
                IndexContent = string.Empty;
                return new List<IndexMenuItem>();
            }

            foreach (string index in list.Keys)
            {
                allIndex.Add(new IndexMenuItem
                    {
                        Index = index,
                        IsHeader = true,
                        Title = index
                    });
                allIndex.AddRange(list[index].Select(
                    i => new IndexMenuItem
                    {
                        Index = index,
                        IsHeader = false,
                        FileName = i.FileName,
                        Title = i.Title
                    }));
            }
            if (allIndex.Count > 0)
            {
                CurrentIndex = allIndex[1].Index;
                SelectIndexTitle = allIndex[1].Title;
                IndexContent = await PublicationContentUtil.Instance.GetContentFromIndex(Publication.BookId, new Index
                {
                    Title = allIndex[1].Title,
                    FileName = allIndex[1].FileName
                });
            }

            return allIndex;
        }

        public async Task CurrentSectedIndexChanged(IndexMenuItem selectedIndex)
        {
            SelectIndexTitle = selectedIndex.Title;
            CurrentIndex = selectedIndex.Index;
            IndexContent = string.Empty;
            IndexContent = await PublicationContentUtil.Instance.GetContentFromIndex(Publication.BookId, new Index
                {
                    Title = selectedIndex.Title,
                    FileName = selectedIndex.FileName
                });

        }
        #endregion
        #region Recently History
        private ObservableCollection<RecentHistoryItem> historyPublications;
        public ObservableCollection<RecentHistoryItem> HistoryPublications
        {
            get { return historyPublications; }
            set { SetProperty(ref historyPublications, value); }
        }

        public async Task RecentHistoryToTOC(RecentHistoryItem recentHistoryItem)
        {
            int TOCId = await PublicationContentUtil.Instance.GetTOCIDByDocId(recentHistoryItem.BookId, recentHistoryItem.DOCID);
            var record = new NavigationRecord
            {
                BookId = recentHistoryItem.BookId,
                TOCId = TOCId,
                Type = NavigationType.History
            };
            await RestoreToc(record, TOCAccessMode.All);
        }
        [RestorableState]
        public NavigationRecordManager HistoryNavigator { get; set; }

        private async Task GoForWardRecord()
        {
            await RestoreToc(HistoryNavigator.GoForward(), TOCAccessMode.IgnoreHistory);
        }
        private async Task GoBackWardRecord()
        {
            await RestoreToc(HistoryNavigator.GoBack(), TOCAccessMode.IgnoreHistory);
        }

        private async Task RestoreToc(NavigationRecord record, TOCAccessMode mode)
        {
            if (record.BookId != Publication.BookId)
            {
                var publications = SessionService.SessionState[ALL_PUBLICATIONS_SESSION_KEY] as List<PublicationViewModel>;
                Publication = publications.FirstOrDefault(x => x.BookId == record.BookId);
                root = await PublicationUtil.Instance.GetDlBookTOC(Publication.BookId);
                listAll = root.ChildNodes;
                firstNode = null;
                GetFirstTocNode(listAll, ref firstNode);
                var indexs = await InitialIndexMenu(Publication.BookId);
                IndexMenuCollections = new ObservableCollection<IndexMenuItem>(indexs);
                HasIndex = IndexMenuCollections == null || IndexMenuCollections.Count == 0 ? false : true;
                Keywords = string.Empty;
                IsPBO = await PageSearchUtil.Instance.IsPBO(Publication.BookId);
            }
            CurrentTocNode = new TocCurrentNode
            {
                ID = record.TOCId,
                Role = NodeExpandStatue.Leaf
            };
            var nodes = await InitialBreadcrumMenu(record, mode);
            BreadcrumbNavigator = new ObservableCollection<BreadcrumbNav>(nodes);
        }
        #endregion

        #region Tag
        private ObservableCollection<TagItem> tagCollections;
        [RestorableState]
        public ObservableCollection<TagItem> TagCollections
        {
            get { return tagCollections; }
            private set { SetProperty(ref tagCollections, value); }
        }

        public void UpdateTagCollections(TagItem tagItem, TagEditEnum tagEditEnum)
        {
            if (tagItem != null)
            {
                switch (tagEditEnum)
                {
                    case TagEditEnum.New:
                        TagCollections.Add(tagItem);
                        break;
                    case TagEditEnum.Edit:
                        var editTagIndex = TagCollections.ToList<TagItem>().FindIndex(x => x.Tag.TagId == tagItem.Tag.TagId);
                        if (editTagIndex > -1)
                        {
                            TagCollections[editTagIndex] = new TagItem
                            {
                                Tag = tagItem.Tag,
                                IsCheck = TagCollections[editTagIndex].IsCheck
                            };
                        }
                        break;
                    case TagEditEnum.Delete:
                        var deleteTagIndex = TagCollections.ToList<TagItem>().FindIndex(x => x.Tag.TagId == tagItem.Tag.TagId);
                        if (deleteTagIndex > -1)
                        {
                            TagCollections.RemoveAt(deleteTagIndex);
                        }
                        break;
                }
            }
        }

        public void CheckAllTags()
        {
            foreach (var tag in TagCollections)
            {
                if (!tag.IsCheck)
                {
                    tag.IsCheck = true;
                }
            }
        }



        #endregion
        #region toc content
        public void LoadingContent(string title, RollingDirection direct)
        {
            LoadingTitle = title;
            if (direct == RollingDirection.Up)
            {
                IsLoadingTop = true;
                IsLoadingBottom = false;
            }
            else if (direct == RollingDirection.Down)
            {
                IsLoadingTop = false;
                IsLoadingBottom = true;
            }
        }

        public void LoadingCompleted()
        {
            IsLoadingTop = false;
            IsLoadingBottom = false;
        }

        private void GetFirstTocNode(List<TOCNode> nodes, ref TOCNode firstNode)
        {
            foreach (TOCNode node in nodes)
            {
                if (firstNode != null)
                    return;
                if (node.Role == "me")
                {
                    firstNode = node;
                    return;
                }
                if (node.ChildNodes != null)
                {
                    GetFirstTocNode(node.ChildNodes, ref firstNode);
                }
            }
        }

        public async Task<string> GoBackWardContent(TOCNode last)
        {
            var lastContent = await PublicationContentUtil.Instance.GetContentFromTOC(Publication.BookId, new TOCNode
            {
                ID = last.ID,
                Title = last.Title,
                ParentId = last.ParentId,
                NodeLevel = last.NodeLevel,
                GuideCardTitle = last.GuideCardTitle,
                Role = last.Role,
                ChildNodes = last.ChildNodes
            }, false);
            return lastContent;
        }

        public async Task<string> GoForWardContent(TOCNode next)
        {
            var nextContent = await PublicationContentUtil.Instance.GetContentFromTOC(Publication.BookId, new TOCNode
            {
                ID = next.ID,
                Title = next.Title,
                ParentId = next.ParentId,
                NodeLevel = next.NodeLevel,
                GuideCardTitle = next.GuideCardTitle,
                Role = next.Role,
                ChildNodes = next.ChildNodes
            }, false);
            return nextContent;
        }

        public TOCNode GetBackwardNode()
        {
            TOCNode tocNode = null;
            GetTocNodeByID(listAll, ref tocNode, CurrentTocNode.ID);
            var last = PublicationContentUtil.Instance.GetPreviousPageByTreeNode(tocNode);
            return last;
        }
        public TOCNode GetForwardNode()
        {
            TOCNode tocNode = null;
            GetTocNodeByID(listAll, ref tocNode, CurrentTocNode.ID);
            var next = PublicationContentUtil.Instance.GetNextPageByTreeNode(tocNode);
            return next;
        }

        public async Task ResetTocByScroll(int tocId)
        {
            var record = new NavigationRecord
            {
                BookId = Publication.BookId,
                TOCId = tocId,
                Type = NavigationType.TOCDocument
            };
            if (CurrentTocNode.ID == tocId)
            {
                await RestoreToc(record, TOCAccessMode.NoReload);
                return;
            }

            if (IsSameParent(CurrentTocNode.ID, tocId))
            {
                foreach (var breadcrumb in BreadcrumbNavigator)
                {
                    breadcrumb.IsHighLight = breadcrumb.ID == tocId;
                }
                CurrentTocNode = new TocCurrentNode
                {
                    ID = tocId,
                    Role = NodeExpandStatue.Leaf
                };
            }
            else
            {
                await RestoreToc(record, TOCAccessMode.NoReload);
            }
        }

        public async void LinkAnalyze(string url)
        {
            var link = PublicationContentUtil.Instance.BuildHyperLink(Publication.BookId, url);
            if (link is IntraHyperlink)//current book
            {
                var intralink = link as IntraHyperlink;
                NavigationRecord record = new NavigationRecord
                {
                    BookId = Publication.BookId,
                    TOCId = intralink.TOCID,
                    Type = NavigationType.IntraLink,
                    Tag = intralink.Refpt
                };
                await RestoreToc(record, TOCAccessMode.All);
            }
            else if (link is InternalHyperlink)//other books
            {
                var internallink = link as InternalHyperlink;
                NavigationRecord record = new NavigationRecord
                {
                    BookId = internallink.BookID,
                    TOCId = internallink.TOCID,
                    Type = NavigationType.InternalLink,
                    Tag = internallink.Refpt
                };
                await RestoreToc(record, TOCAccessMode.All);
            }
            else if (link is ExternalHyperlink)
            {
                var exterlink = link as ExternalHyperlink;
                await Windows.System.Launcher.LaunchUriAsync(new Uri(exterlink.Url));
            }
            else if (link is AttachmentHyperlink)
            {
                var attachlink = link as AttachmentHyperlink;
                try
                {
                    var file = await Windows.Storage.ApplicationData.Current.LocalFolder.GetFileAsync(attachlink.TargetFileName);
                    if (file != null)
                    {
                        var options = new Windows.System.LauncherOptions();
                        options.PreferredApplicationDisplayName = file.DisplayType;
                        options.PreferredApplicationPackageFamilyName = file.ContentType;
                        await Windows.System.Launcher.LaunchFileAsync(file, options);
                    }
                }
                catch
                {
                    //TODO:log
                }
            }
        }
        private bool IsSameParent(int id1, int id2)
        {
            TOCNode node1 = null;
            TOCNode node2 = null;
            GetTocNodeByID(listAll, ref node1, id1);
            GetTocNodeByID(listAll, ref node2, id2);
            return node1.ParentId == node2.ParentId;
        }

        private async void ChangeTocContent(string content, int tocId)
        {
            LeftSidePanelVisible = true;
            if (IsPBO)
            {
                var page = await PageSearchUtil.Instance.GetFirstPageItem(Publication.BookId, tocId);
                PageNum = "Page " + (page == null ? string.Empty : page.Identifier.ToString());
            }
            var historys = PublicationContentUtil.Instance.GetRecentHistory();
            if (historys != null && historys.Count != 0)
            {
                HistoryPublications = new ObservableCollection<RecentHistoryItem>(historys);
            }
            SelectedContent = string.Empty;
            SelectedContent = content;
        }
        #endregion

        #region Searching
        private ObservableCollection<SearchResultModel> searchResultCollection;
        public ObservableCollection<SearchResultModel> SearchResultCollection
        {
            get { return searchResultCollection; }
            set { SetProperty(ref searchResultCollection, value); }
        }

        private ObservableCollection<SearchPageModel> searchPageCollection = new ObservableCollection<SearchPageModel>();
        public ObservableCollection<SearchPageModel> SearchPageCollection
        {
            get { return searchPageCollection; }
            set { SetProperty(ref searchPageCollection, value); }
        }

        private string keywords;
        [RestorableState]
        public string Keywords
        {
            get { return keywords; }
            set { SetProperty(ref keywords, value); }
        }

        private List<string> searchResultFilter = new List<string>
           {
                "All",
                "Legislation",
                "Commentary",
                "Forms & Precedents",
                "Case"
            };

        public List<string> SearchResultFilter
        {
            get { return searchResultFilter; }
        }

        private string searchFilter = "All";
        [RestorableState]
        public string SearchFilter
        {
            get { return searchFilter; }
            set
            {
                SetProperty(ref searchFilter, value);
                if (value == searchResultFilter[0])
                {
                    SearchResultCollection = SearchResultsAll == null || SearchResultsAll.Count == 0 ? null : new ObservableCollection<SearchResultModel>(SearchResultsAll);
                }
                else
                {
                    SearchResultCollection = SearchResultsAll == null || SearchResultsAll.Count == 0 ? null : new ObservableCollection<SearchResultModel>(SearchResultsAll.FindAll(r => r.ContentType == value));
                    if (SearchResultCollection != null && SearchResultCollection.Count == 0)
                        SearchResultCollection = null;
                }

            }
        }

        [RestorableState]
        public List<SearchResultModel> SearchResultsAll { set; get; }

        public void SearchDocument(string keys)
        {
            var collection = SearchUtil.Search(Publication.BookId, CurrentTocNode.ID, keys);
            if (collection != null)
            {
                var foundWordList = collection.FoundWordList;
                SearchResultsAll = collection.SearchDisplayResultList.Select(result => new SearchResultModel
                    {
                        TocId = result.TocId,
                        FirstLine = result.isDocument ? result.Head : result.TocTitle,
                        SecondLine = result.isDocument ? result.SnippetContent : result.GuideCardTitle,
                        ContentType = SearchResultFilter[(int)result.ContentType - 1],
                        Type = result.isDocument ? ResourceLoader.GetString("Document") : ResourceLoader.GetString("Publication"),
                        SnippetContent = result.SnippetContent,
                        Keywords = foundWordList,
                        HeadType = result.HeadType.ToString().ToLower(),
                        HeadIndex = result.HeadSequence
                    }).ToList<SearchResultModel>();
                SearchTools.KeepKeyword(foundWordList);
            }
            SearchFilter = searchResultFilter[0];
        }

        public async Task<bool> SearchPage(int pageNum)
        {
            IspageResultShow = true;
            var results = await PageSearchUtil.Instance.SeachByPageNum(publication.BookId, pageNum);
            SearchPageCollection = results == null || results.Count == 0 ? null : new ObservableCollection<SearchPageModel>(results.Select(b => new SearchPageModel
            {
                FileTitle = b.FileTitle,
                GuideCardTitle = b.GuideCardTitle,
                TOCID = b.TOCID,
                PageNum = pageNum
            }));
            return SearchPageCollection == null;
        }

        public async Task SearchResultToToc(SearchResultModel result)
        {
            var record = new NavigationRecord
            {
                BookId = Publication.BookId,
                TOCId = result.TocId,
                Type = NavigationType.Search,
                Tag = result
            };
            await RestoreToc(record, TOCAccessMode.All);
        }

        public async void GotoPage(SearchPageModel result)
        {
            var record = new NavigationRecord
            {
                BookId = Publication.BookId,
                TOCId = result.TOCID,
                Type = NavigationType.PBO,
                PageNum = result.PageNum
            };

            await RestoreToc(record, TOCAccessMode.All);
        }

        public void ClearKeywords()
        {
            SearchResultCollection = null;
            SearchResultsAll = null;
        }

        public void ClearPageNum()
        {
            SearchPageCollection = null;
            IspageResultShow = false;
        }
        #endregion
    }


}
