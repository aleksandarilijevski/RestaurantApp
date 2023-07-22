using EntityFramework.Models;
using Prism.Commands;
using Prism.Mvvm;
using RestaurantApp.Services.Interface;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

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
        private ArticleDetails _articleDetailsInput = new ArticleDetails();
        private string _articleName;
        private string _dispatchNoteNumber;
        private string _barcode;

        public AddArticleByDispatchNoteViewModel(IDatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        public List<Article> DispatchNoteArticles { get; set; } = new List<Article>();

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

        public ArticleDetails ArticleDetailsInput
        {
            get
            {
                return _articleDetailsInput;
            }

            set
            {
                _articleDetailsInput = value;
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

            if (article != null)
            {
                article.Quantity = 1;

                DispatchNoteArticles.Add(article);
            }

            Barcode = string.Empty;
            RaisePropertyChanged(nameof(DispatchNoteArticles));
        }

        private void GetArticleByName(string articleName)
        {
            Article article = Articles.FirstOrDefault(x => x.Name.ToLower() == articleName.ToLower());

            if (article != null)
            {
                DispatchNoteArticles.Add(article);
            }

            ArticleName = string.Empty;
            RaisePropertyChanged(nameof(DispatchNoteArticles));
        }

        private async void Save()
        {
            foreach (Article article in DispatchNoteArticles)
            {
                _articleDetailsInput.Article = article;

                await AddArticleDetails(_articleDetailsInput);
            }
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
