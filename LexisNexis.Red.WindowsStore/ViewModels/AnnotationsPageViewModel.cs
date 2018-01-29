using LexisNexis.Red.Common.Business;
using LexisNexis.Red.Common.BusinessModel;
using LexisNexis.Red.WindowsStore.Tools;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Navigation;

namespace LexisNexis.Red.WindowsStore.ViewModels
{
    public class AnnotationsPageViewModel : BaseViewModel
    {
        private const string FILTER_VALUE_ALL = "All";
        private const string FILTER_VALUE_NOTES = "Notes";
        private const string FILTER_VALUE_HIGHLIGHTS = "Highlights";


        #region Commands
        public DelegateCommand ShrinkCommand
        {
            get;
            private set;
        }
        public DelegateCommand NewTagBoardOpenedCommand
        {
            get;
            private set;
        }
        public DelegateCommand TagMenuCommand
        {
            get;
            private set;
        }

        public DelegateCommand NewTagCommand
        {
            get;
            private set;
        }

        public DelegateCommand CancelAddTagCommand
        {
            get;
            private set;
        }
        #endregion

        #region Field

        private ObservableCollection<TagItem> tagCollections;
        [RestorableState]
        public ObservableCollection<TagItem> TagCollections
        {
            get { return tagCollections; }
            private set { SetProperty(ref tagCollections, value); }
        }

        private string selectedFilter = FILTER_VALUE_ALL;
        [RestorableState]
        public string SelectedFilter
        {
            get { return selectedFilter; }
            set
            {
                SetProperty(ref selectedFilter, value);
                FilterData();
            }
        }

        private int selectedTag = -1;
        [RestorableState]
        public int SelectedTag
        {
            get { return selectedTag; }
            set
            {
                SetProperty(ref selectedTag, value);
            }
        }

        private bool isOrphans = false;
        [RestorableState]
        public bool IsOrphans
        {
            get { return isOrphans; }
            set
            {
                SetProperty(ref isOrphans, value);
                FilterData();
            }
        }

        private bool hasAnnotations;
        [RestorableState]
        public bool HasAnnotations
        {
            get { return hasAnnotations; }
            private set { SetProperty(ref hasAnnotations, value); }
        }


        private bool leftSidePanelVisible = true;
        public bool LeftSidePanelVisible
        {
            get { return leftSidePanelVisible; }
            private set { SetProperty(ref leftSidePanelVisible, value); }
        }

        private Visibility tagBoardVisible = Visibility.Visible;
        public Visibility TagBoardVisible
        {
            get { return tagBoardVisible; }
            private set { SetProperty(ref tagBoardVisible, value); }
        }


        private Visibility newTagBoardVisible = Visibility.Collapsed;
        public Visibility NewTagBoardVisible
        {
            get { return newTagBoardVisible; }
            private set { SetProperty(ref newTagBoardVisible, value); }
        }

        private string newTagTitle;
        [RestorableState]
        public string NewTagTitle
        {
            get { return newTagTitle; }
            set
            {
                SetProperty(ref newTagTitle, value);
            }
        }

        private string tagNameText;
        [RestorableState]
        public string TagNameText
        {
            get { return tagNameText; }
            set
            {
                SetProperty(ref tagNameText, value);
            }
        }

        private Guid EditTagID { set; get; }
        #endregion

        public AnnotationsPageViewModel()
        {
            ShrinkCommand = new DelegateCommand(() => { LeftSidePanelVisible = !LeftSidePanelVisible; });
            NewTagCommand = new DelegateCommand(NewTag);
            CancelAddTagCommand = new DelegateCommand(CancelAddTag);
            NewTagBoardOpenedCommand = new DelegateCommand(NewTagBoardOpened);
            TagMenuCommand = new DelegateCommand(TagMenuBoardOpened);
        }

        private void CancelAddTag()
        {
            SwitchAnnoAndTag(true);
        }

        private void NewTag()
        {
            if (!string.IsNullOrEmpty(TagNameText) && SelectedTag > -1)
            {
                if (EditTagID == Guid.Empty)//New Tag
                {
                    var tag = AnnCategoryTagUtil.Instance.AddAndReturnTag(TagNameText, AnnotationTools.TagColors[SelectedTag].ColorValue);
                    var tagItem = new TagItem
                    {
                        Tag = tag,
                        IsCheck = false
                    };
                    TagCollections.Add(tagItem);
                }
                else// Edit Tag
                {
                    var tag = AnnCategoryTagUtil.Instance.UpdateAndReturnTag(EditTagID, TagNameText, AnnotationTools.TagColors[SelectedTag].ColorValue);
                    var editTagIndex = TagCollections.ToList<TagItem>().FindIndex(x => x.Tag.TagId == EditTagID);
                    if (editTagIndex > -1)
                    {
                        TagCollections[editTagIndex] = new TagItem
                        {
                            Tag = tag,
                            IsCheck = TagCollections[editTagIndex].IsCheck
                        };
                    }
                }
                SwitchAnnoAndTag(true);
            }
        }

        private void NewTagBoardOpened()
        {
            SwitchAnnoAndTag(false);
            SelectedTag = -1;
            EditTagID = Guid.Empty;
            NewTagTitle = ResourceLoader.GetString("NewTagTitle");
            TagNameText = string.Empty;
        }

        private void TagMenuBoardOpened()
        {
            SwitchAnnoAndTag(true);
        }
        public override void OnNavigatedTo(object navigationParameter, NavigationMode navigationMode, Dictionary<string, object> viewModelState)
        {
            base.OnNavigatedTo(navigationParameter, navigationMode, viewModelState);
            var tags = AnnCategoryTagUtil.Instance.GetTags();
            var tagList = tags.Select(tag => new TagItem
                {
                    Tag = tag,
                    IsCheck = false
                });

            TagCollections = new ObservableCollection<TagItem>(tagList);
            TagCollections.CollectionChanged += TagOrderChanged;
        }

        private void TagOrderChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    AnnCategoryTagUtil.Instance.Sort(TagCollections.Select(x => x.Tag).ToList<AnnotationTag>());
                    break;
            }
        }
        private void FilterData()
        {

            //throw new NotImplementedException();
        }

        public void DeleteTag(Guid guid)
        {
            AnnCategoryTagUtil.Instance.DeleteTag(guid);
            var deleteTag = TagCollections.FirstOrDefault(x => x.Tag.TagId == guid);
            if (deleteTag != null)
            {
                TagCollections.Remove(deleteTag);
            }
        }

        public void EditDialogOpen(Guid guid)
        {
            SwitchAnnoAndTag(false);
            var tag = TagCollections.FirstOrDefault(x => x.Tag.TagId == guid);
            SelectedTag = AnnotationTools.TagColors.FindIndex(x => x.ColorValue == tag.Tag.Color);
            EditTagID = guid;
            NewTagTitle = ResourceLoader.GetString("EditTagTitle");
            TagNameText = tag.Tag.Title;
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

        private void SwitchAnnoAndTag(bool isAnnotation)
        {
            NewTagBoardVisible = isAnnotation ? Visibility.Collapsed : Visibility.Visible;
            TagBoardVisible = isAnnotation ? Visibility.Visible : Visibility.Collapsed;
        }
    }

    public class TagItem : BaseBindableViewModel
    {
        private AnnotationTag tag;
        public AnnotationTag Tag
        {
            get { return tag; }
            set { SetProperty(ref tag, value); }
        }

        private bool isCheck;
        public bool IsCheck
        {
            get { return isCheck; }
            set { SetProperty(ref isCheck, value); }
        }
    }
}
