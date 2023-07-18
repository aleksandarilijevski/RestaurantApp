﻿using EntityFramework.Models;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using RestaurantApp.Services.Interface;
using System;
using System.Threading.Tasks;

namespace RestaurantApp.ViewModels
{
    public class EditArticleViewModel : BindableBase, IDialogAware
    {
        private IDatabaseService _databaseService;
        private DelegateCommand<Article> _editArticleCommand;
        private Article _article;
        private ArticleDetails _articleDetails = new ArticleDetails();
        private string _title = "Edit article";

        public event Action<IDialogResult> RequestClose;

        public EditArticleViewModel(IDatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        public DelegateCommand<Article> EditArticleCommand
        {
            get
            {
                _editArticleCommand = new DelegateCommand<Article>(EditArticle);
                return _editArticleCommand;
            }
        }

        public Article Article
        {
            get { return _article; }
            set { SetProperty(ref _article, value); }
        }

        public ArticleDetails ArticleDetails
        {
            get
            {
                return _articleDetails;
            }

            set
            {
                SetProperty(ref _articleDetails, value);
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
            Article = parameters.GetValue<Article>("article");
        }

        private async Task EditArticleDetails(ArticleDetails articleDetails)
        {
            await _databaseService.EditArticleDetails(articleDetails);
        }

        private async void EditArticle(Article article)
        {
            ArticleDetails articleDetails = await _databaseService.GetArticleDetailsByArticleID(article.ID);

            articleDetails.Quantity = _articleDetails.Quantity;
            articleDetails.EntryPrice = _articleDetails.EntryPrice;

            await EditArticleDetails(articleDetails);
            await _databaseService.EditArticle(article);
            CloseDialog("true");
        }
    }
}
