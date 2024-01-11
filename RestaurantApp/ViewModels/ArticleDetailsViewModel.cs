using EntityFramework.Models;
using Prism.Mvvm;
using Prism.Regions;

namespace RestaurantApp.ViewModels
{
    public class ArticleDetailsViewModel : BindableBase, INavigationAware
    {
        private Article _article;

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
