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
