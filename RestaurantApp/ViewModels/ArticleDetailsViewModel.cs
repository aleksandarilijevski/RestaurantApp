using EntityFramework.Models;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using Prism.Services.Dialogs;
using RestaurantApp.Services.Interface;
using System.Threading.Tasks;

namespace RestaurantApp.ViewModels
{
    public class ArticleDetailsViewModel : BindableBase, INavigationAware
    {
        private IDatabaseService _databaseService;
        private IDialogService _dialogService;
        private DelegateCommand<ArticleDetails> _showEditArticleDetailCommand;
        private Article _article;

        public ArticleDetailsViewModel(IDatabaseService databaseService, IDialogService dialogService)
        {
            _databaseService = databaseService;
            _dialogService = dialogService;
        }

        public Article Article
        {
            get
            {
                return _article;
            }

            set
            {
                _article = value;
            }
        }

        public DelegateCommand<ArticleDetails> ShowEditArticleDetailCommand
        {
            get
            {
                _showEditArticleDetailCommand = new DelegateCommand<ArticleDetails>(ShowEditArticleDetail);
                return _showEditArticleDetailCommand;
            }
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return false;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {

        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            _article = (Article)navigationContext.Parameters["article"];
        }

        private async Task<ArticleDetails> GetArticleDetailsByID(int id)
        {
            ArticleDetails articleDetails = await _databaseService.GetArticleDetailsByID(id);
            return articleDetails;
        }

        public void ShowEditArticleDetail(ArticleDetails articleDetails)
        {
            DialogParameters dialogParameters = new DialogParameters
            {
                {"articleDetail", articleDetails}
            };

            _dialogService.ShowDialog("editArticleDetailDialog", dialogParameters, r => { });
        }
    }
}
