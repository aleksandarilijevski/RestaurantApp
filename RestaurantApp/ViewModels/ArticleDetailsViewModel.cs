using EntityFramework.Models;
using Microsoft.EntityFrameworkCore;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using Prism.Services.Dialogs;
using RestaurantApp.Services.Interface;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

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

        public async void ShowEditArticleDetail(ArticleDetails articleDetails)
        {
            DialogParameters dialogParameters = new DialogParameters
            {
                {"articleDetail", articleDetails}
            };

            _dialogService.ShowDialog("editArticleDetailDialog", dialogParameters, r => { });

            Article = await _databaseService.GetArticleByID(articleDetails.ArticleID, new EFContext());
        }

        private async void DeleteArticleDetail(ArticleDetails articleDetails)
        {
            using EFContext efContext = new EFContext();

            List<TableArticleQuantity> tableArticleQuantities = await _databaseService.GetTableArticleQuantityByArticleID(articleDetails.ArticleID, efContext);
            List<TableArticleQuantity> tableArticleQuantitiesFiltered = tableArticleQuantities.Where(x => x is not SoldTableArticleQuantity).ToList();

            if (tableArticleQuantitiesFiltered.Count != 0)
            {
                MessageBox.Show("You can't edit article details while it's used!", "Article details", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            efContext.ChangeTracker.Clear();

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
