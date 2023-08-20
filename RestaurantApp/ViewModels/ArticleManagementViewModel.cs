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
        private DelegateCommand _showAddArticleByDataEntryCommand;
        private DelegateCommand<Article> _showArticleDetailsCommand;
        private DelegateCommand<Article> _deleteArticleCommand;
        private ObservableCollection<Article> _articles;
        private List<ArticleDetails> _articleDetailsList;

        public ArticleManagementViewModel(IDatabaseService databaseService, IDialogService dialogService, IRegionManager regionManager)
        {
            _dialogService = dialogService;
            _databaseService = databaseService;
            _regionManager = regionManager;
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

        public DelegateCommand ShowAddArticleByDataEntryCommand
        {
            get
            {
                _showAddArticleByDataEntryCommand = new DelegateCommand(ShowAddArticleByDataEntry);
                return _showAddArticleByDataEntryCommand;
            }
        }

        public DelegateCommand<Article> ShowArticleDetailsCommand
        {
            get
            {
                _showArticleDetailsCommand = new DelegateCommand<Article>(ShowArticleDetails);
                return _showArticleDetailsCommand;
            }
        }

        private void ShowEditArticleDialog(Article article)
        {
            DialogParameters dialogParameters = new DialogParameters
            {
                { "article", article }
            };

            _dialogService.ShowDialog("editArticleDialog", dialogParameters, r =>
            {
                Article resultData = r.Parameters.GetValue<Article>("article");
            });

            RaisePropertyChanged(nameof(Articles));
        }

        private async void GetAllArticles()
        {
            Articles = await _databaseService.GetAllArticles();
        }

        private void ShowAddArticleDialog()
        {
            _dialogService.ShowDialog("addArticleDialog");
            GetAllArticles();
        }

        private void ShowAddArticleByDataEntry()
        {
            _regionManager.RequestNavigate("MainRegion", "AddArticleByDataEntry");
        }

        private async void DeleteArticle(Article article)
        {
            article.IsDeleted = true;
            await _databaseService.EditArticle(article);
            Articles.Remove(article);
        }

        private void ShowArticleDetails(Article article)
        {
            NavigationParameters navigationParameters = new NavigationParameters
            {
                { "article", article }
            };

            _regionManager.RequestNavigate("MainRegion", "ArticleDetails", navigationParameters);
        }
    }
}
