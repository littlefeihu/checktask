using LexisNexis.Red.Common.Business;
using LexisNexis.Red.Common.BusinessModel;
using LexisNexis.Red.Common.HelpClass;
using LexisNexis.Red.WindowsStore.Tools;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Mvvm;
using Microsoft.Practices.Prism.StoreApps.Interfaces;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml;
using Windows.Web.Http;

namespace LexisNexis.Red.WindowsStore.ViewModels
{
    public class AnnotationViewModel : BaseBindableViewModel
    {
        #region Attribute
        private string annotationTitle;
        [RestorableState]
        public string AnnotationTitle
        {
            get { return annotationTitle; }
            set
            {
                SetProperty(ref annotationTitle, value);
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

        private ObservableCollection<TagItem> tagCollections;
        [RestorableState]
        public ObservableCollection<TagItem> TagCollections
        {
            get { return tagCollections; }
            private set { SetProperty(ref tagCollections, value); }
        }

        private Visibility newNoteBtnVisible;
        public Visibility NewNoteBtnVisible
        {
            get { return newNoteBtnVisible; }
            private set { SetProperty(ref newNoteBtnVisible, value); }
        }

        private Visibility notePanelVisibility;
        public Visibility NotePanelVisibility
        {
            get { return notePanelVisibility; }
            private set { SetProperty(ref notePanelVisibility, value); }
        }

        private Visibility highlightPanelVisibility;
        public Visibility HighlightPanelVisibility
        {
            get { return highlightPanelVisibility; }
            private set { SetProperty(ref highlightPanelVisibility, value); }
        }

        private Visibility editTagPanelVisibility;
        public Visibility EditTagPanelVisibility
        {
            get { return editTagPanelVisibility; }
            private set { SetProperty(ref editTagPanelVisibility, value); }
        }

        private Visibility annotationBoardVisible = Visibility.Visible;
        public Visibility AnnotationBoardVisible
        {
            get { return annotationBoardVisible; }
            private set { SetProperty(ref annotationBoardVisible, value); }
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

        private AnnotationStatue AnnoStatue { set; get; }

        #endregion

        #region Command
        public DelegateCommand NewTagBoardOpenedCommand
        {
            get;
            private set;
        }

        public DelegateCommand NewNoteBoardOpenedCommand
        {
            get;
            private set;
        }

        public DelegateCommand EditorBoardOpenedCommand
        {
            get;
            private set;
        }

        public DelegateCommand EditorBoardClosedCommand
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

        public delegate void UpdateTagCollectionHandle(TagItem tagItem, TagEditEnum tagEditEnum);
        public event UpdateTagCollectionHandle UpdateTagCollection;
        public delegate void AddAnnotationHandle(AnnotationStatue annotationType);
        public event AddAnnotationHandle AddAnnotation;
        public AnnotationViewModel()
        {
            NewTagCommand = new DelegateCommand(NewTag);
            CancelAddTagCommand = new DelegateCommand(CancelAddTag);
            NewTagBoardOpenedCommand = new DelegateCommand(NewTagBoardOpened);
            NewNoteBoardOpenedCommand = new DelegateCommand(NewNoteBoardOpened);
            EditorBoardOpenedCommand = new DelegateCommand(EditorBoardOpened);
            EditorBoardClosedCommand = new DelegateCommand(EditorBoardClosed);

            var tags = AnnCategoryTagUtil.Instance.GetTags();
            var tagList = tags.Select(tag => new TagItem
            {
                Tag = tag,
                IsCheck = false
            });
            TagCollections = new ObservableCollection<TagItem>(tagList);
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
                    UpdateTagCollection.Invoke(tagItem, TagEditEnum.New);
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
                        UpdateTagCollection.Invoke(TagCollections[editTagIndex], TagEditEnum.Edit);
                    }
                }
                SwitchAnnoAndTag(true);
            }
        }

        private void EditorBoardClosed()
        {
            Switch(AnnoStatue);
        }

        private void EditorBoardOpened()
        {
            AnnotationTitle = ResourceLoader.GetString("EditTagsTitle");
            NewNoteBtnVisible = Visibility.Visible;
            NotePanelVisibility = Visibility.Collapsed;
            HighlightPanelVisibility = Visibility.Collapsed;
            EditTagPanelVisibility = Visibility.Visible;

            SwitchAnnoAndTag(true);
        }

        private void NewNoteBoardOpened()
        {
            Switch(AnnotationStatue.Note);
        }

        private void NewTagBoardOpened()
        {
            SwitchAnnoAndTag(false);
            SelectedTag = -1;
            EditTagID = Guid.Empty;
            NewTagTitle = ResourceLoader.GetString("NewTagTitle");
            TagNameText = string.Empty;
        }

        public void Switch(AnnotationStatue statue)
        {
            AnnoStatue = statue;
            AnnotationTitle = AnnoStatue == AnnotationStatue.Highlight ? ResourceLoader.GetString("Highlight") : ResourceLoader.GetString("AnnotationTitle");
            NewNoteBtnVisible = AnnoStatue == AnnotationStatue.Highlight ? Visibility.Visible : Visibility.Collapsed;
            NotePanelVisibility = AnnoStatue == AnnotationStatue.Highlight ? Visibility.Collapsed : Visibility.Visible;
            HighlightPanelVisibility = Visibility.Visible;
            EditTagPanelVisibility = Visibility.Collapsed;
            SwitchAnnoAndTag(true);
        }

        private void SwitchAnnoAndTag(bool isAnnotation)
        {
            NewTagBoardVisible = isAnnotation ? Visibility.Collapsed : Visibility.Visible;
            AnnotationBoardVisible = isAnnotation ? Visibility.Visible : Visibility.Collapsed;
        }

        public void DeleteTag(Guid guid)
        {
            AnnCategoryTagUtil.Instance.DeleteTag(guid);
            var deleteTag = TagCollections.FirstOrDefault(x => x.Tag.TagId == guid);
            if (deleteTag != null)
            {
                TagCollections.Remove(deleteTag);
                UpdateTagCollection.Invoke(deleteTag, TagEditEnum.Delete);
            }
        }

        public void EditTagDialogOpen(Guid guid)
        {
            SwitchAnnoAndTag(false);
            var tag = TagCollections.FirstOrDefault(x => x.Tag.TagId == guid);
            SelectedTag = AnnotationTools.TagColors.FindIndex(x => x.ColorValue == tag.Tag.Color);
            EditTagID = guid;
            NewTagTitle = ResourceLoader.GetString("EditTagTitle");
            TagNameText = tag.Tag.Title;
        }

        public void AddAnnotationNotify()
        {
            AddAnnotation.Invoke(AnnoStatue);
        }
    }
}
