using EntityFramework.Models;
using Microsoft.EntityFrameworkCore;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using RestaurantApp.Services.Interface;
using System;
using System.Windows;

namespace RestaurantApp.ViewModels
{
    public class EditArticleViewModel : BindableBase, IDialogAware
    {
        private IDatabaseService _databaseService;
        private DelegateCommand _editArticleCommand;
        private Article _article;

        public event Action<IDialogResult> RequestClose;

        public EditArticleViewModel(IDatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        public Article Article
        {
            get
            {
                return _article;
            }

            set
            {
                _article = value;
                RaisePropertyChanged();
            }
        }

        public string Title { get; set; } = "Edit article";

        public DelegateCommand EditArticleCommand
        {
            get
            {
                _editArticleCommand = new DelegateCommand(EditArticle);
                return _editArticleCommand;
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
            Article = parameters.GetValue<Article>("article");
        }

        private async void EditArticle()
        {
            using EFContext efContext = new EFContext();

            Article articleFind = await _databaseService.GetArticleByBarcode(Article.Barcode, efContext);

            efContext.Entry(articleFind).State = EntityState.Detached;

            if (articleFind is not null && articleFind.ID != Article.ID)
            {
                MessageBox.Show("Article with that barcode already exists!", "Edit article", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            await _databaseService.EditArticle(Article, efContext);
            CloseDialog("true");
        }
    }
}