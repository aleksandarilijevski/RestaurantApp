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
        private string _title = "Edit article detail";
        private ArticleDetails _articleDetails;

        public EditArticleDetailViewModel(IDatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        public string Title
        {
            get
            {
                return _title;
            }

            set
            {
                _title = value;
            }
        }

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

        public event Action<IDialogResult> RequestClose;

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
            await _databaseService.EditArticleDetails(articleDetails);
            CloseDialog("true");
        }
    }
}
