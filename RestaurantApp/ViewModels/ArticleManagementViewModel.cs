﻿using EntityFramework.Enums;
using EntityFramework.Models;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using Prism.Services.Dialogs;
using RestauranApp.Utilities.Constants;
using RestaurantApp.Services.Interface;
using RestaurantApp.Utilities.Helpers;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace RestaurantApp.ViewModels
{
    public class ArticleManagementViewModel : BindableBase
    {
        private string _articleName;

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

        public ArticleManagementViewModel(IDatabaseService databaseService, IDialogService dialogService, IRegionManager regionManager)
        {
            _dialogService = dialogService;
            _databaseService = databaseService;
            _regionManager = regionManager;
        }

        public User User { get; set; }

        public List<ArticleDetails> ArticleDetailsList { get; set; }

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

        private async void ShowEditArticleDialog(Article article)
        {
            DialogParameters dialogParameters = new DialogParameters
            {
                { "article", article }
            };

            _dialogService.ShowDialog(ViewConstants.EditArticleDialogViewName, dialogParameters, r =>
            {
                Article resultData = r.Parameters.GetValue<Article>("article");
            });

            Articles = await _databaseService.GetAllArticles(new EFContext());
        }

        private bool UserLogin()
        {
            bool isResultGood = false;

            _dialogService.ShowDialog(ViewConstants.UserLoginDialogViewName, r =>
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
            if (Articles is not null)
            {
                Articles.Clear();
            }

            if (LoggedUserHelper.LoggedUser is null)
            {
                bool result = UserLogin();

                if (!result)
                {
                    _regionManager.RequestNavigate(ViewConstants.MainRegionViewName, ViewConstants.OptionsViewName);
                    return;
                }

                if (User.UserRole is UserRole.Waiter)
                {
                    MessageBox.Show(MessageBoxConstants.WaiterCantAccessToArticleManagement, MessageBoxConstants.AccessForbiddenTitle, MessageBoxButton.OK, MessageBoxImage.Error);
                    _regionManager.RequestNavigate(ViewConstants.MainRegionViewName, ViewConstants.OptionsViewName);
                    return;
                }

                LoggedUserHelper.LoggedUser = User;
            }

            EFContext efContext = new EFContext();
            Articles = await _databaseService.GetAllArticles(efContext);
        }

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
            _dialogService.ShowDialog(ViewConstants.AddArticleDialogViewName);
            GetAllArticles();
        }

        private void ShowAddArticleByDataEntry()
        {
            _regionManager.RequestNavigate(ViewConstants.MainRegionViewName, ViewConstants.AddArticleByDataEntryViewName);
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

            _regionManager.RequestNavigate(ViewConstants.MainRegionViewName, ViewConstants.ArticleDetailsUserControlViewName, navigationParameters);
        }
    }
}
