using EntityFramework.Models;
using Prism.Commands;
using Prism.Mvvm;
using RestaurantApp.Services.Interface;
using System.Collections.Generic;

namespace RestaurantApp.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private IDatabaseService _databaseService;
        private DelegateCommand _getAllArticalsCommand;

        public DelegateCommand GetAllArticalsCommand
        {
            get
            {
                _getAllArticalsCommand = new DelegateCommand(GetAllArticals);
                return _getAllArticalsCommand;
            }
        }

        public List<Artical> Articals { get; set; }

        public MainWindowViewModel(IDatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        private void GetAllArticals()
        {
            Articals = _databaseService.GetAllArticals();
        }
    }
}
