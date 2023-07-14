using EntityFramework.Models;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using RestaurantApp.Services.Interface;
using System;
using System.Threading.Tasks;

namespace RestaurantApp.ViewModels
{
    public class AddArticleViewModel : BindableBase, IDialogAware
    {
        private IDatabaseService _databaseService;
        private string _title = "Add new article";
        private DelegateCommand<Article> _addArticleCommand;
        private Article _article;
        private ArticleQuantity _articleQuantity = new ArticleQuantity();

        public event Action<IDialogResult> RequestClose;

        public Article Article
        {
            get { return _article; }
            set { SetProperty(ref _article, value); }
        }

        public ArticleQuantity ArticleQuantity
        {
            get
            {
                return _articleQuantity;
            }

            set
            {
                SetProperty(ref _articleQuantity, value);
            }
        }

        public AddArticleViewModel(IDatabaseService databaseService)
        {
            _databaseService = databaseService;
            _article = new Article();
        }

        public DelegateCommand<Article> AddArticleCommand
        {
            get
            {
                _addArticleCommand = new DelegateCommand<Article>(AddArticle);
                return _addArticleCommand;
            }
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

        }

        private async Task ApplyArticleQuantity(int articleId)
        {
            ArticleQuantity.ArticleID = _article.ID;
        }

        private async Task AddArticleQuantity(ArticleQuantity articleQuantity)
        {
            await _databaseService.AddArticleQuantity(articleQuantity);
        }

        private async void AddArticle(Article article)
        {
            int articleId = await _databaseService.AddArticle(article);
            await ApplyArticleQuantity(articleId);
            await AddArticleQuantity(_articleQuantity);

            CloseDialog("true");
        }
    }
}
