using EntityFramework.Models;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using RestaurantApp.Services.Interface;
using System.Collections.ObjectModel;

namespace RestaurantApp.ViewModels
{
    public class OrderingViewModel : BindableBase, INavigationAware
    {
        private IDatabaseService _databaseService;
        private DelegateCommand<string> _addArticleToTableCommand;
        private int _id;
        private ObservableCollection<Article> _articles = new ObservableCollection<Article>();

        public int ID
        {
            get { return _id; }
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
                RaisePropertyChanged(nameof(Articles));
            }
        }

        public OrderingViewModel(IDatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        public string Barcode
        {
            set
            {
                RaisePropertyChanged();
            }
        }

        public DelegateCommand<string> AddArticleToTableCommand
        {
            get
            {
                _addArticleToTableCommand = new DelegateCommand<string>(AddArticleToTable);
                return _addArticleToTableCommand;
            }
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            _id = int.Parse(navigationContext.Parameters["id"].ToString());
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return false;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
        }

        private async void AddArticleToTable(string barcode)
        {
            Article article = await _databaseService.GetArticleByBarcode(long.Parse(barcode));

            if (article is not null)
            {
                CheckIfArticleExists(article);
            }

            Barcode = string.Empty;
        }

        private async void CheckIfArticleExists(Article article)
        {
            if (Articles.Contains(article))
            {
                article.Quantity++;
            }
            else
            {
                article.Quantity = 1;
                _articles.Add(article);
            }
            RaisePropertyChanged(nameof(Articles));
        }
    }
}
    