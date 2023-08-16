using EntityFramework.Models;
using Prism.Commands;
using Prism.Mvvm;
using RestaurantApp.Services.Interface;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace RestaurantApp.ViewModels
{
    public class AddArticleByDataEntryViewModel : BindableBase
    {
        private IDatabaseService _databaseService;
        private DelegateCommand _loadAllArticlesCommand;
        private DelegateCommand<string> _addArticleByBarcodeCommand;
        private DelegateCommand<string> _addArticleByNameCommand;
        private DelegateCommand _saveCommand;
        private DelegateCommand<ArticleDetails> _deleteArticleDetailsFromDataEntryCommand;
        private ObservableCollection<Article> _articles;
        private List<ArticleDetails> _dataEntryArticles = new List<ArticleDetails>();
        private List<string> _articleNames = new List<string>();
        private string _articleName;
        private string _dataEntryNumber;
        private string _barcode;

        public AddArticleByDataEntryViewModel(IDatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

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

        public List<string> ArticleNames
        {
            get
            {
                return _articleNames;
            }

            set
            {
                _articleNames = value;
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
            Articles = await _databaseService.GetAllArticles();

            foreach (Article article in Articles)
            {
                ArticleNames.Add(article.Name);
            }
        }

        private void GetArticleByBarcode(string barcode)
        {
            long barcodeLong = long.Parse(barcode);
            Article article = Articles.FirstOrDefault(x => x.Barcode == barcodeLong);
            ArticleDetails articleDetails = new ArticleDetails();

            if (article != null)
            {
                articleDetails.Article = article;
                DataEntryArticles.Add(articleDetails);
            }

            Barcode = string.Empty;
            RaisePropertyChanged(nameof(DataEntryArticles));
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
            if (DataEntryNumber is null || DataEntryNumber == string.Empty)
            {
                MessageBox.Show("Data entry number can't be empty!", "Data entry", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (DataEntryArticles.Count == 0)
            {
                MessageBox.Show("Please add articles!", "Data entry", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            DataEntry dataEntryCheck = await _databaseService.GetDataEntryByNumber(int.Parse(DataEntryNumber));

            if (dataEntryCheck is not null)
            {
                MessageBox.Show("Data entry with that number already exists!", "Data entry", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            DataEntry dataEntry = new DataEntry();
            decimal totalAmount = 0;

            foreach (ArticleDetails articleDetails in DataEntryArticles)
            {
                if (articleDetails.EntryPrice <= 0 || articleDetails.Quantity <= 0)
                {
                    MessageBox.Show("One or more article details properties are not valid!", "Data entry", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                await _databaseService.AddArticleDetails(articleDetails);
            }

            List<Article> articles = CreateArticleListFromArticleDetails(DataEntryArticles);
            totalAmount = CalculateTotalAmount(DataEntryArticles);

            dataEntry.DataEntryNumber = int.Parse(DataEntryNumber);
            dataEntry.TotalAmount = totalAmount;
            dataEntry.Articles = articles;
            await _databaseService.AddDataEntry(dataEntry);

            MessageBox.Show("Data entry is saved!", "Data entry", MessageBoxButton.OK, MessageBoxImage.Information);
            DataEntryNumber = string.Empty;
            DataEntryArticles.Clear();
            RaisePropertyChanged(nameof(DataEntryArticles));
        }

        private List<Article> CreateArticleListFromArticleDetails(List<ArticleDetails> articleDetails)
        {
            List<Article> articles = new List<Article>();

            foreach (ArticleDetails articleDetail in articleDetails)
            {
                articles.Add(articleDetail.Article);
            }

            return articles;
        }

        private decimal CalculateTotalAmount(List<ArticleDetails> articleDetails)
        {
            decimal totalAmount = 0;

            foreach (ArticleDetails articleDetail in articleDetails)
            {
                totalAmount += articleDetail.EntryPrice * articleDetail.Quantity;
            }

            return totalAmount;
        }
    }
}
