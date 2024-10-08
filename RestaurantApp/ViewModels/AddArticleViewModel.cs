﻿using EntityFramework.Models;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using RestauranApp.Utilities.Constants;
using RestaurantApp.Services.Interface;
using System;
using System.Collections.ObjectModel;
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

        public string Title { get; set; } = ViewConstants.AddArticleTitle;

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

        private async void AddArticle(Article article)
        {
            using EFContext efContext = new EFContext();

            if (article.Name is null)
            {
                MessageBox.Show(MessageBoxConstants.ArticleNameCanNotBeEmpty, MessageBoxConstants.AddArticleTitle, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (article.Price <= 0)
            {
                MessageBox.Show(MessageBoxConstants.ArticlePriceCanNotBeZeroOrLess, MessageBoxConstants.AddArticleTitle, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (ArticleDetails.EntryPrice <= 0)
            {
                MessageBox.Show(MessageBoxConstants.ArticleDetailsEntryPriceCanNotBeZeroOrLess, MessageBoxConstants.AddArticleTitle, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            ObservableCollection<Article> articles = await _databaseService.GetAllArticles(efContext);

            foreach (Article articleMatch in articles)
            {
                if (articleMatch.Barcode == article.Barcode)
                {
                    MessageBox.Show(MessageBoxConstants.ArticleWithThatBarcodeAlreadyExists, MessageBoxConstants.AddArticleTitle, MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }

            int articleId = await _databaseService.AddArticle(article, efContext);
            ArticleDetails.ArticleID = articleId;
            ArticleDetails.DataEntryQuantity = ArticleDetails.OriginalQuantity;
            await _databaseService.AddArticleDetails(ArticleDetails, efContext);

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

        public virtual void OnDialogOpened(IDialogParameters parameters)
        {

        }
    }
}
