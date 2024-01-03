using EntityFramework.Models;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using RestaurantApp.Services.Interface;
using System;
using System.Windows;

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

        private async void EditArticleDetails(ArticleDetails articleDetails)
        {
            using EFContext efContext = new EFContext();

            if (articleDetails.OriginalQuantity == 0 || articleDetails.OriginalQuantity < 0)
            {
                MessageBox.Show("Article details quantity cannot be zero or less!", "Edit article details", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (articleDetails.EntryPrice == 0 || articleDetails.EntryPrice < 0)
            {
                MessageBox.Show("Article details entry price cannot be zero or less!", "Edit article details", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

         
            articleDetails.DataEntryQuantity = articleDetails.OriginalQuantity;
            await _databaseService.EditArticleDetails(articleDetails, efContext);
            CloseDialog("true");
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

        public virtual void OnDialogOpened(IDialogParameters parameters)
        {
            ArticleDetails articleDetails = parameters.GetValue<ArticleDetails>("articleDetail");
            ArticleDetails = (ArticleDetails)articleDetails.Clone();
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
    }
}
