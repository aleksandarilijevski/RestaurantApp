﻿using EntityFramework.Models;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using RestaurantApp.Services.Interface;
using System.Collections.ObjectModel;

namespace RestaurantApp.ViewModels
{
    public class ArticleManagementViewModel : BindableBase
    {
        private IDatabaseService _databaseService;
        private IDialogService _dialogService;
        private DelegateCommand<Article> _showEditArticleDialogCommand;
        private DelegateCommand _showAddArticleDialogCommand;
        private DelegateCommand _getAllArticlesCommand;
        private DelegateCommand<Article> _deleteArticleCommand;
        private ObservableCollection<Article> _articles;

        public ArticleManagementViewModel(IDatabaseService databaseService, IDialogService dialogService)
        {
            _dialogService = dialogService;
            _databaseService = databaseService;
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

        public ObservableCollection<Article> Articles
        {
            get => _articles;
            set
            {
                _articles = value;
                RaisePropertyChanged();
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
