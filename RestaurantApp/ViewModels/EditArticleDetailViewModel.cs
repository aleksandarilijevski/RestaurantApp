using EntityFramework.Models;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using RestaurantApp.Services.Interface;
using System;

namespace RestaurantApp.ViewModels
{
    public class EditArticleDetailViewModel : BindableBase, IDialogAware
    {
        private IDatabaseService _databaseService;
        private DelegateCommand<ArticleDetails> _editArticleDetailsCommand;
        private ArticleDetails _articleDetails;

        public EditArticleDetailViewModel(IDatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        public event Action<IDialogResult> RequestClose;

        public string Title { get; set; } = "Edit article detail";

        public ArticleDetails ArticleDetails
        {
            get
            {
                return _articleDetails;
            }

            set
            {
                _articleDetails = value;
                RaisePropertyChanged();
            }
        }

        public DelegateCommand<ArticleDetails> EditArticleDetailsCommand
        {
            get
            {
                _editArticleDetailsCommand = new DelegateCommand<ArticleDetails>(EditArticleDetails);
                return _editArticleDetailsCommand;
            }
        }

        protected virtual void CloseDialog(string parameter)
        {
            ButtonResult result = ButtonResult.None;

            if (parameter?.ToLower() == "true")
                result = ButtonResult.OK;
            else if (parameter?.ToLower() == "false")
                result = ButtonResult.Cancel;

            RaiseRequestClose(new DialogResult(result));
        }

        public virtual void RaiseRequestClose(IDialogResult dialogResult)
        {
            RequestClose?.Invoke(dialogResult);
        }

        public virtual bool CanCloseDialog()
        {
            return true;
        }

        public virtual void OnDialogClosed()
        {

        }

        public virtual void OnDialogOpened(IDialogParameters parameters)
        {
            ArticleDetails = parameters.GetValue<ArticleDetails>("articleDetail");
        }

        private async void EditArticleDetails(ArticleDetails articleDetails)
        {
            using EFContext efContext = new EFContext();
            articleDetails.DataEntryQuantity = articleDetails.OriginalQuantity;
            await _databaseService.EditArticleDetails(articleDetails, efContext);
            CloseDialog("true");
        }
    }
}
