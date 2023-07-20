using EntityFramework.Models;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using Prism.Services.Dialogs;
using RestaurantApp.Services.Interface;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace RestaurantApp.ViewModels
{
    public class ArticleManagementViewModel : BindableBase
    {
        private IDatabaseService _databaseService;
        private IDialogService _dialogService;
        private IRegionManager _regionManager;
        private DelegateCommand<Article> _showEditArticleDialogCommand;
        private DelegateCommand _showAddArticleDialogCommand;
        private DelegateCommand _getAllArticlesCommand;
        private DelegateCommand _showAddArticleByDispathNoteCommand;
        private DelegateCommand<Article> _deleteArticleCommand;
        private ObservableCollection<Article> _articles;
        private List<ArticleDetails> _articleDetailsList;

        public ArticleManagementViewModel(IDatabaseService databaseService, IDialogService dialogService, IRegionManager regionManager)
        {
            _dialogService = dialogService;
            _databaseService = databaseService;
            _regionManager = regionManager;
        }

        public DelegateCommand ShowAddArticleDialogCommand
        {
            get
            {
                _showAddArticleDialogCommand = new DelegateCommand(ShowAddArticleDialog);
                return _showAddArticleDialogCommand;
            }
        }

        public DelegateCommand<Article> DeleteArticleCommand
        {
            get
            {
                _deleteArticleCommand = new DelegateCommand<Article>(DeleteArticle);
                return _deleteArticleCommand;
            }
        }

        public DelegateCommand<Article> ShowEditArticleDialogCommand
        {
            get
            {
                _showEditArticleDialogCommand = new DelegateCommand<Article>(ShowEditArticleDialog);
                return _showEditArticleDialogCommand;
            }
        }

        public DelegateCommand GetAllArticlesCommand
        {
            get
            {
                _getAllArticlesCommand = new DelegateCommand(GetAllArticles);
                return _getAllArticlesCommand;
            }
        }

        public DelegateCommand ShowAddArticleByDispathNoteCommand
        {
            get
            {
                _showAddArticleByDispathNoteCommand = new DelegateCommand(ShowAddArticleByDispathNote);
                return _showAddArticleByDispathNoteCommand;
            }
        }

        public ObservableCollection<Article> Articles
        {
            get => _articles;
            set
            {
                _articles = value;
                RaisePropertyChanged();
            }
        }

        public List<ArticleDetails> ArticleDetailsList
        {
            get
            {
                return _articleDetailsList;
            }

            set
            {
                _articleDetailsList = value;
            }
        }

        private void ShowEditArticleDialog(Article article)
        {
            DialogParameters dialogParametars = new DialogParameters
            {
                { "article", article }
            };

            _dialogService.ShowDialog("editArticleDialog", dialogParametars, r =>
            {
                Article resultData = r.Parameters.GetValue<Article>("article");
            });
        }

        private async void ShowAddArticleDialog()
        {
            _dialogService.ShowDialog("addArticleDialog");
            GetAllArticles();
        }

        private async void ShowAddArticleByDispathNote()
        {
            _regionManager.RequestNavigate("MainRegion","AddArticleByDispatchNote");
        }

        private async void GetAllArticles()
        {
            Articles = await _databaseService.GetAllArticles();
        }

        private async void DeleteArticle(Article article)
        {
            await _databaseService.DeleteArticle(article);
            Articles.Remove(article);
        }
    }
}
