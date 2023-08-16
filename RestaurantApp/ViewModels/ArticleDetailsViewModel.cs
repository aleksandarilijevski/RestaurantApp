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
        private DelegateCommand<ArticleDetails> _deleteArticleDetailsCommand;
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
                RaisePropertyChanged();
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

        public DelegateCommand<ArticleDetails> DeleteArticleDetailsCommand
        {
            get
            {
                _deleteArticleDetailsCommand = new DelegateCommand<ArticleDetails>(DeleteArticleDetail);
                return _deleteArticleDetailsCommand;
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
            Article = (Article)navigationContext.Parameters["article"];
        }

        private async void DeleteArticleDetail(ArticleDetails articleDetails)
        {
            await _databaseService.DeleteArticleDetails(articleDetails);
            RaisePropertyChanged(nameof(Article));
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
