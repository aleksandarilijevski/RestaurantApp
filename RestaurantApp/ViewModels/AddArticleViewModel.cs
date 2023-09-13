using EntityFramework.Models;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using RestaurantApp.Services.Interface;
using System;
using System.Windows;

namespace RestaurantApp.ViewModels
{
    public class AddArticleViewModel : BindableBase, IDialogAware
    {
        private IDatabaseService _databaseService;
        private DelegateCommand<Article> _addArticleCommand;

        public AddArticleViewModel(IDatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        public event Action<IDialogResult> RequestClose;

        public string Title { get; set; } = "Add new article";

        public Article Article { get; set; } = new Article();

        public ArticleDetails ArticleDetails { get; set; } = new ArticleDetails();

        public DelegateCommand<Article> AddArticleCommand
        {
            get
            {
                _addArticleCommand = new DelegateCommand<Article>(AddArticle);
                return _addArticleCommand;
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

        }

        private async void AddArticle(Article article)
        {
            using EFContext efContext = new EFContext();

            if (article.Name is null)
            {
                MessageBox.Show("Article name can not be empty!", "Add article", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (article.Price <= 0)
            {
                MessageBox.Show("Article price can not be zero or less!", "Add article", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (ArticleDetails.EntryPrice <= 0)
            {
                MessageBox.Show("Article detail entry price can not be zero or less!", "Add article", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            int articleId = await _databaseService.AddArticle(article, efContext);
            ArticleDetails.ArticleID = articleId;
            await _databaseService.AddArticleDetails(ArticleDetails, efContext);

            CloseDialog("true");
        }
    }
}
