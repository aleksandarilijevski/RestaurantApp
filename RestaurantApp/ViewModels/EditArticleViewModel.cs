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

        public EditArticleViewModel(IDatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        public string Title { get; set; } = "Edit article";

        public event Action<IDialogResult> RequestClose;

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

        public virtual void OnDialogOpened(IDialogParameters parameters)
        {
            Article article = parameters.GetValue<Article>("article");
            Article = (Article)article.Clone();
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

            if (Article.Name == string.Empty)
            {
                MessageBox.Show("Article name cannot be empty!", "Edit article", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (Article.Price == 0 || Article.Price < 0)
            {
                MessageBox.Show("Article price cannot be zero or less!", "Edit article", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            await _databaseService.EditArticle(Article, efContext);
            CloseDialog("true");
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