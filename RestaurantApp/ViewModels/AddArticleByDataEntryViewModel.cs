﻿using EntityFramework.Models;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using RestauranApp.Utilities.Constants;
using RestaurantApp.Services.Interface;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace RestaurantApp.ViewModels
{
    public class AddArticleByDataEntryViewModel : BindableBase
    {
        private string _articleName;

        private string _dataEntryNumber;

        private string _barcode;

        private IDatabaseService _databaseService;

        private IRegionManager _regionManager;

        private DelegateCommand _loadAllArticlesCommand;

        private DelegateCommand<string> _addArticleByBarcodeCommand;

        private DelegateCommand<string> _addArticleByNameCommand;

        private DelegateCommand _saveCommand;

        private DelegateCommand _navigateToDataEntryManagementCommand;

        private DelegateCommand<ArticleDetails> _deleteArticleDetailsFromDataEntryCommand;

        private ObservableCollection<Article> _articles;

        private List<ArticleDetails> _dataEntryArticles = new List<ArticleDetails>();
        private Uri ConstantHelper;

        public AddArticleByDataEntryViewModel(IDatabaseService databaseService, IRegionManager regionManager)
        {
            _databaseService = databaseService;
            _regionManager = regionManager;
        }

        public List<string> ArticleNames { get; set; } = new List<string>();

        public List<ArticleDetails> DataEntryArticles
        {
            get
            {
                return _dataEntryArticles;
            }

            set
            {
                _dataEntryArticles = value;
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

        public string DataEntryNumber
        {
            get
            {
                return _dataEntryNumber;
            }

            set
            {
                _dataEntryNumber = value;
                RaisePropertyChanged();
            }
        }

        public DelegateCommand LoadAllArticlesCommand
        {
            get
            {
                _loadAllArticlesCommand = new DelegateCommand(LoadAllArticles);
                return _loadAllArticlesCommand;
            }
        }

        public DelegateCommand<string> AddArticleByBarcodeCommand
        {
            get
            {
                _addArticleByBarcodeCommand = new DelegateCommand<string>(GetArticleByBarcode);
                return _addArticleByBarcodeCommand;
            }
        }

        public DelegateCommand<string> AddArticleByNameCommand
        {
            get
            {
                _addArticleByNameCommand = new DelegateCommand<string>(GetArticleByName);
                return _addArticleByNameCommand;
            }
        }

        public DelegateCommand SaveCommand
        {
            get
            {
                _saveCommand = new DelegateCommand(Save);
                return _saveCommand;
            }
        }

        public DelegateCommand<ArticleDetails> DeleteArticleDetailsFromDataEntryCommand
        {
            get
            {
                _deleteArticleDetailsFromDataEntryCommand = new DelegateCommand<ArticleDetails>(DeleteArticleDetailsFromDataEntry);
                return _deleteArticleDetailsFromDataEntryCommand;
            }
        }

        public DelegateCommand NavigateToDataEntryManagementCommand
        {
            get
            {
                _navigateToDataEntryManagementCommand = new DelegateCommand(NavigateToDataEntryManagement);
                return _navigateToDataEntryManagementCommand;
            }
        }

        public string Barcode
        {
            get
            {
                return _barcode;
            }

            set
            {
                _barcode = value;
                RaisePropertyChanged();
            }
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

        private async void LoadAllArticles()
        {
            EFContext efContext = new EFContext();

            Articles = await _databaseService.GetAllArticles(efContext);

            foreach (Article article in Articles)
            {
                ArticleNames.Add(article.Name);
            }
        }

        private void GetArticleByBarcode(string barcode)
        {
            if (long.TryParse(barcode, out long barcodeLong))
            {
                Article article = Articles.FirstOrDefault(x => x.Barcode == barcodeLong);
                ArticleDetails articleDetails = new ArticleDetails();

                if (article != null)
                {
                    articleDetails.Article = article;
                    DataEntryArticles.Add(articleDetails);
                }
            }
            else
            {
                MessageBox.Show(MessageBoxConstants.BarcodeIsInvalid, MessageBoxConstants.DataEntryTitle, MessageBoxButton.OK, MessageBoxImage.Error);
            }

            Barcode = string.Empty;
            RaisePropertyChanged(nameof(DataEntryArticles));
        }

        private void NavigateToDataEntryManagement()
        {
            _regionManager.RequestNavigate(RestauranApp.Utilities.Constants.ViewConstants.MainRegionViewName, RestauranApp.Utilities.Constants.ViewConstants.DataEntryManagement);
        }

        private void GetArticleByName(string articleName)
        {
            ArticleDetails articleDetails = new ArticleDetails();
            Article article = Articles.FirstOrDefault(x => x.Name.ToLower() == articleName.ToLower());

            if (article != null)
            {
                articleDetails.Article = article;
                DataEntryArticles.Add(articleDetails);
            }

            ArticleName = string.Empty;
            RaisePropertyChanged(nameof(DataEntryArticles));
        }

        private void DeleteArticleDetailsFromDataEntry(ArticleDetails articleDetails)
        {
            DataEntryArticles.Remove(articleDetails);
            RaisePropertyChanged(nameof(DataEntryArticles));
        }

        private async void Save()
        {
            using EFContext efContext = new EFContext();

            if (DataEntryArticles.Count == 0)
            {
                MessageBox.Show(MessageBoxConstants.PleaseAddArticles, MessageBoxConstants.DataEntryTitle, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            DataEntry dataEntryCheck = await _databaseService.GetDataEntryByNumber(DataEntryNumber, efContext);

            if (dataEntryCheck is not null)
            {
                MessageBox.Show(MessageBoxConstants.DataEntryWithThatNumberAlreadyExists, MessageBoxConstants.DataEntryTitle, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            DataEntry dataEntry = null;
            decimal totalAmount = 0;

            foreach (ArticleDetails articleDetails in DataEntryArticles)
            {
                if (articleDetails.EntryPrice == 0 || articleDetails.OriginalQuantity == 0)
                {
                    MessageBox.Show(MessageBoxConstants.OneOrMoreArticleDetailsPropertiesAreNotValid, MessageBoxConstants.DataEntryTitle, MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                int originalQuantity = 0;
                int isAvailable = 0;

                if (articleDetails.OriginalQuantity < 0)
                {
                    originalQuantity = articleDetails.Article.ArticleDetails.Sum(x => x.OriginalQuantity);
                    isAvailable = articleDetails.OriginalQuantity + originalQuantity;
                }

                if (isAvailable < 0)
                {
                    MessageBox.Show(MessageBoxConstants.QuantityDoesntExist, MessageBoxConstants.DataEntryTitle, MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                articleDetails.DataEntryQuantity = articleDetails.OriginalQuantity;

                await _databaseService.AddArticleDetails(articleDetails, efContext);
                await BindArticleDetailToExistingTableArticleQuantities(articleDetails, efContext);
            }

            List<Article> articles = await CreateArticleListFromArticleDetails(DataEntryArticles, efContext);
            totalAmount = CalculateTotalAmount(DataEntryArticles);

            dataEntry = new DataEntry
            {
                DataEntryNumber = DataEntryNumber,
                TotalAmount = totalAmount,
                ArticleDetails = DataEntryArticles
            };

            await _databaseService.AddDataEntry(dataEntry, efContext);

            MessageBox.Show(MessageBoxConstants.DataEntryIsSaved, MessageBoxConstants.DataEntryTitle, MessageBoxButton.OK, MessageBoxImage.Information);
            DataEntryNumber = string.Empty;
            DataEntryArticles.Clear();
            RaisePropertyChanged(nameof(DataEntryArticles));
        }

        private async Task BindArticleDetailToExistingTableArticleQuantities(ArticleDetails articleDetail, EFContext efContext)
        {
            List<TableArticleQuantity> tableArticleQuantities = await _databaseService.GetTableArticleQuantityByArticleID(articleDetail.ArticleID, efContext);

            foreach (TableArticleQuantity tableArticleQuantity in tableArticleQuantities)
            {
                tableArticleQuantity.ArticleDetails.Add(articleDetail);
            }
        }

        private async Task<List<Article>> CreateArticleListFromArticleDetails(List<ArticleDetails> articleDetails, EFContext efContext)
        {
            List<Article> articles = new List<Article>();

            foreach (ArticleDetails articleDetail in articleDetails)
            {
                Article article = await _databaseService.GetArticleByID(articleDetail.ArticleID, efContext);
                articles.Add(article);
            }

            return articles;
        }

        private decimal CalculateTotalAmount(List<ArticleDetails> articleDetails)
        {
            decimal totalAmount = 0;
            totalAmount += articleDetails.Sum(x => x.EntryPrice * x.OriginalQuantity);

            return totalAmount;
        }
    }
}
