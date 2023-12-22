using EntityFramework.Models;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using Prism.Services.Dialogs;
using RestaurantApp.Services.Interface;

namespace RestaurantApp.ViewModels
{
    public class ArticleDetailsViewModel : BindableBase, INavigationAware
    {
        private Article _article;

        private IDatabaseService _databaseService;

        private IDialogService _dialogService;

        private DelegateCommand<ArticleDetails> _showEditArticleDetailCommand;

        private DelegateCommand<ArticleDetails> _deleteArticleDetailsCommand;

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

       public void ShowEditArticleDetail(ArticleDetails articleDetails)
        {
            DialogParameters dialogParameters = new DialogParameters
            {
                {"articleDetail", articleDetails}
            };

            _dialogService.ShowDialog("editArticleDetailDialog", dialogParameters, r => { });
        }

        private async void DeleteArticleDetail(ArticleDetails articleDetails)
        {
            using EFContext efContext = new EFContext();
            await _databaseService.DeleteArticleDetails(articleDetails, efContext);
            RaisePropertyChanged(nameof(Article));
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            Article = (Article)navigationContext.Parameters["article"];
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return false;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {

        }
    }
}
