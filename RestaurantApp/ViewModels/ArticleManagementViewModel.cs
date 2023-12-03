using EntityFramework.Enums;
using EntityFramework.Models;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using Prism.Services.Dialogs;
using RestaurantApp.Services.Interface;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Timers;
using System.Windows;
using System.Windows.Input;

namespace RestaurantApp.ViewModels
{
    public class ArticleManagementViewModel : BindableBase
    {
        private Timer _timer;
        private IDatabaseService _databaseService;
        private IDialogService _dialogService;
        private IRegionManager _regionManager;
        private DelegateCommand<Article> _showEditArticleDialogCommand;
        private DelegateCommand _showAddArticleDialogCommand;
        private DelegateCommand _getAllArticlesCommand;
        private DelegateCommand _showAddArticleByDataEntryCommand;
        private DelegateCommand<Article> _showArticleDetailsCommand;
        private DelegateCommand<Article> _deleteArticleCommand;
        private DelegateCommand _clearFiltersCommand;
        private DelegateCommand _filterArticlesCommand;
        private ObservableCollection<Article> _articles;
        private string _articleName;

        
        public ArticleManagementViewModel(IDatabaseService databaseService, IDialogService dialogService, IRegionManager regionManager)
        {
            _dialogService = dialogService;
            _databaseService = databaseService;
            _regionManager = regionManager;
            InitializeMouseInactivityTimer();
            InitializeMouseEvents();
        }

        public string ArticleName
        {
            get
            {
                return _articleName;
            }

            set
            {
                _articleName = value;
                RaisePropertyChanged();
            }
        }

        public User User { get;set; }

        public ObservableCollection<Article> Articles
        {
            get
            {
                return _articles;
            }

            set
            {
                _articles = value;
                RaisePropertyChanged();
            }
        }

        private void InitializeMouseInactivityTimer()
        {
            _timer = new Timer(5000);
            _timer.Elapsed += MouseInactivityTimerElapsed;
            _timer.Start();
        }

        private void MouseInactivityTimerElapsed(object sender, ElapsedEventArgs e)
        {
            Debug.WriteLine("Logoff!");
        }


        private void InitializeMouseEvents()
        {
            Mouse.AddMouseMoveHandler(Application.Current.MainWindow, MouseMoveHandler);
        }

        private void MouseMoveHandler(object sender, MouseEventArgs e)
        {
            // Reset the timer on mouse move
            _timer.Stop();
            _timer.Start();
        }

        public List<ArticleDetails> ArticleDetailsList { get; set; }

        public DelegateCommand ShowAddArticleDialogCommand
        {
            get
            {
                _showAddArticleDialogCommand = new DelegateCommand(ShowAddArticleDialog);
                return _showAddArticleDialogCommand;
            }
        }

        public DelegateCommand FilterArticlesCommand
        {
            get
            {
                _filterArticlesCommand = new DelegateCommand(FilterArticles);
                return _filterArticlesCommand;
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

        public DelegateCommand ClearFiltersCommand
        {
            get
            {
                _clearFiltersCommand = new DelegateCommand(ClearFilters);
                return _clearFiltersCommand;
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

        private bool UserLogin()
        {
            bool isResultGood = false;

            _dialogService.ShowDialog("userLoginDialog", r =>
            {
                if (r.Result == ButtonResult.OK)
                {
                    isResultGood = true;
                    User = r.Parameters.GetValue<User>("user");
                }
                else
                {
                    isResultGood = false;
                }
            });

            return isResultGood;
        }

        private async void GetAllArticles()
        {
            //Temporarily
            User = null;

            if (Articles is not null)
            {
                Articles.Clear();
            }

            bool result = true;

            if (User is null)
            {
                result = UserLogin();
            }

            if (!result)
            {
                _regionManager.RequestNavigate("MainRegion", "Options");
                return;
            }

            if (User.UserRole is UserRole.Waiter)
            {
                MessageBox.Show("Waiter can't access to article management!", "Access forbidden", MessageBoxButton.OK, MessageBoxImage.Error);
                _regionManager.RequestNavigate("MainRegion", "Options");
                return;
            }

            EFContext efContext = new EFContext();
            Articles = await _databaseService.GetAllArticles(efContext);
        }

        //Every time we press filter or clear filter the sql query will be created.
        //We do this because meanwhile other user could add new article.
        //Maybe it's not best approach because of app performance.

        private async void FilterArticles()
        {
            EFContext efContext = new EFContext();
            ObservableCollection<Article> originalArticles = await _databaseService.GetAllArticles(efContext);
            ObservableCollection<Article> filteredArticles = new ObservableCollection<Article>();

            if (ArticleName != string.Empty && ArticleName != null)
            {
                filteredArticles.AddRange(originalArticles.Where(x => x.Name.ToLower().Contains(ArticleName.ToLower())));
            }

            Articles = filteredArticles;
        }

        private async void ClearFilters()
        {
            ArticleName = string.Empty;
            EFContext efContext = new EFContext();
            Articles = await _databaseService.GetAllArticles(efContext);
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
            using EFContext efContext = new EFContext();

            article.IsDeleted = true;
            await _databaseService.EditArticle(article, efContext);
            Articles.Remove(article);
        }

        private void ShowArticleDetails(Article article)
        {
            NavigationParameters navigationParameters = new NavigationParameters
            {
                { "article", article }
            };

            _regionManager.RequestNavigate("MainRegion", "ArticleDetailsUserControl", navigationParameters);
        }
    }
}
