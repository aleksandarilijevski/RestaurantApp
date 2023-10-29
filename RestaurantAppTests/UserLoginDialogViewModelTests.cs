using EntityFramework.Enums;
using EntityFramework.Models;
using Microsoft.EntityFrameworkCore;
using RestaurantApp.Factories;
using RestaurantApp.Factories.Interfaces;
using RestaurantApp.Services;
using RestaurantApp.Services.Interface;
using RestaurantApp.ViewModels;

namespace RestaurantAppTests
{
    public class UserLoginDialogViewModelTests
    {
        private IAbstractFactory<IDatabaseService> _databaseServiceFactory;
        private IAbstractFactory<UserLoginDialogViewModel> _userLoginDialogViewModelFactory;
        private IAbstractFactory<EFContext> _efContextFactory;
        private DbContextOptions options = new DbContextOptionsBuilder<EFContext>().UseInMemoryDatabase("TestingDatabase").Options;

        public UserLoginDialogViewModelTests()
        {
            _databaseServiceFactory = new AbstractFactory<IDatabaseService>(() => new DatabaseService());
            _efContextFactory = new AbstractFactory<EFContext>(() => new EFContext(options));
            _userLoginDialogViewModelFactory = new AbstractFactory<UserLoginDialogViewModel>(() => new UserLoginDialogViewModel(_databaseServiceFactory.Create()));
        }

        [Test]
        public async Task Login()
        {
            //Arrange
            IDatabaseService? _databaseService = _databaseServiceFactory.Create();
            EFContext? _efContext = _efContextFactory.Create();
            UserLoginDialogViewModel _userLoginDialogViewModel = _userLoginDialogViewModelFactory.Create();

            User user = new User
            {
                FirstAndLastName = "UnitTest1",
                Barcode = 123,
                DateOfBirth = DateTime.UtcNow,
                JMBG = 1,
                UserRole = UserRole.Waiter
            };

            //Act
            await _databaseService.AddUser(user, _efContext);

            //Assert

        }
    }
}
