using EntityFramework.Models;
using Prism.Commands;
using Prism.Mvvm;
using RestaurantApp.Services.Interface;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Threading;

namespace RestaurantApp.ViewModels
{
    public class AddArticleByDispatchNoteViewModel : BindableBase
    {
        private IDatabaseService _databaseService;
        private DelegateCommand _loadAllArticlesCommand;
        private DelegateCommand<string> _addArticleByBarcodeCommand;
        private DelegateCommand<string> _addArticleByNameCommand;
        private DelegateCommand _saveCommand;
        private ObservableCollection<Article> _articles;
        private List<string> _articleNames = new List<string>();
        private string _articleName;
        private string _dispatchNoteNumber;
        private string _barcode;

        public AddArticleByDispatchNoteViewModel(IDatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        public List<ArticleDetails> DispatchNoteArticles { get; set; } = new List<ArticleDetails>();

        public ObservableCollection<Article> Articles
        {
            get
            {
                return _articles;
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

        public string DispatchNoteNumber
        {
            get
            {
                return _dispatchNoteNumber;
            }

            set
            {
                _dispatchNoteNumber = value;
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
            _articles = await _databaseService.GetAllArticles();

            foreach (Article article in _articles)
            {
                _articleNames.Add(article.Name);
            }
        }

        private void GetArticleByBarcode(string barcode)
        {
            long barcodeLong = long.Parse(barcode);
            Article article = Articles.FirstOrDefault(x => x.Barcode == barcodeLong);
            ArticleDetails articleDetails = new ArticleDetails();

            if (article != null)
            {
                article.Quantity = 1;

                articleDetails.Article = article;
                DispatchNoteArticles.Add(articleDetails);
            }

            Barcode = string.Empty;
            RaisePropertyChanged(nameof(DispatchNoteArticles));
        }

        private void GetArticleByName(string articleName)
        {
            ArticleDetails articleDetails = new ArticleDetails();
            Article article = Articles.FirstOrDefault(x => x.Name.ToLower() == articleName.ToLower());

            if (article != null)
            {
                articleDetails.Article = article;
                DispatchNoteArticles.Add(articleDetails);
            }

            ArticleName = string.Empty;
            RaisePropertyChanged(nameof(DispatchNoteArticles));
        }

        private decimal GetTotalAmount(List<ArticleDetails> articleDetails)
        {
            decimal totalAmount = 0;

            foreach (ArticleDetails articleDetail in articleDetails)
            {
                totalAmount += articleDetail.EntryPrice;
            }

            return totalAmount;
        }

        private async void Save()
        {
            DispatchNote dispatchNote = new DispatchNote();
            decimal totalAmount = 0;

            foreach (ArticleDetails articleDetails in DispatchNoteArticles)
            {
                await _databaseService.AddArticleDetails(articleDetails);
            }

            List<Article> articles = CreateArticleListFromArticleDetails(DispatchNoteArticles);
            totalAmount = CalculateTotalAmount(DispatchNoteArticles);

            dispatchNote.DispatchNoteNumber = int.Parse(DispatchNoteNumber);
            dispatchNote.TotalAmount = totalAmount;
            dispatchNote.Articles = articles;
            await _databaseService.AddDispatchNote(dispatchNote);
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

        private async Task<ArticleDetails> GetArticleDetailsByArticleID(int articleId)
        {
            ArticleDetails articleDetails = await _databaseService.GetArticleDetailsByArticleID(articleId);
            return articleDetails;
        }


        private async Task EditArticle(Article article)
        {
            await _databaseService.EditArticle(article);
        }

        private async Task AddArticleDetails(ArticleDetails articleDetails)
        {
            await _databaseService.AddArticleDetails(articleDetails);
        }

        private async Task AddDispatchNote(DispatchNote dispatchNote)
        {
            await _databaseService.AddDispatchNote(dispatchNote);
        }
    }
}
