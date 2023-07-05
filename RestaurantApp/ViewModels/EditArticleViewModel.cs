using EntityFramework.Models;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using RestaurantApp.Services.Interface;
using System;

namespace RestaurantApp.ViewModels
{
    public class EditArticleViewModel : BindableBase, IDialogAware
    {
        private IDatabaseService _databaseService;
        private DelegateCommand<string> _closeDialogCommand;
        private DelegateCommand<Article> _editArticleCommand;
        private Article _article;
        private string _title = "Edit article";

        public DelegateCommand<string> CloseDialogCommand => _closeDialogCommand ?? (_closeDialogCommand = new DelegateCommand<string>(CloseDialog));
        public event Action<IDialogResult> RequestClose;

        public EditArticleViewModel(IDatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        public DelegateCommand<Article> EditArticleCommand
        {
            get
            {
                _editArticleCommand = new DelegateCommand<Article>(EditArticle);
                return _editArticleCommand;
            }
        }

        public Article Article
        {
            get { return _article; }
            set { SetProperty(ref _article, value); }
        }

        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
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
            Article = parameters.GetValue<Article>("article");
        }

        private async void EditArticle(Article article)
        {
            await _databaseService.EditArticle(article);
            CloseDialogCommand.Execute("true");
        }
    }
}
