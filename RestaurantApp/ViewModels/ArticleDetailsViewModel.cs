using EntityFramework.Models;
using Prism.Mvvm;
using Prism.Regions;

namespace RestaurantApp.ViewModels
{
    public class ArticleDetailsViewModel : BindableBase,INavigationAware
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
    }
}
