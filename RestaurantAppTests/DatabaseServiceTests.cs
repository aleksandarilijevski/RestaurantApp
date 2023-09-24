
using EntityFramework.Models;
using RestaurantApp.Services;
using RestaurantApp.Services.Interface;

namespace RestaurantAppTests
{
    public class Tests
    {
        private IDatabaseService _databaseService;
        private EFContext _efContext;

        public Tests()
        {
            _databaseService = new DatabaseService();
            _efContext = new EFContext();
        }

        [Test]
        public async Task AddArticle()
        {
            //Arrange
            Article article = new Article
            {
                Barcode = 123,
                IsDeleted = false,
                Name = "UnitTestArticle",
                Price = 10
            };

            //Act
            int articleId = await _databaseService.AddArticle(article, _efContext);
            Article articleFind = await _databaseService.GetArticleByID(articleId, _efContext);

            //Assert
            Assert.That(articleFind, Is.Not.Null);
            await _databaseService.DeleteArticle(articleFind, _efContext);
        }

        [Test]
        public async Task AddUser()
        {
            //Arrange
            User user = new User
            {
                FirstAndLastName = "UnitTest",
                Barcode = 123,
                DateOfBirth = DateTime.UtcNow,
            };

            //Act
            int userId = await _databaseService.AddUser(user, _efContext);
            User userFind = await _databaseService.GetUserByID(userId, _efContext);

            //Assert
            Assert.That(userFind, Is.Not.Null);
            await _databaseService.DeleteUser(user, _efContext);
        }

        [Test]
        public async Task AddArticleDetails()
        {
            //Arrange
            Article article = new Article
            {
                Barcode = 123,
                IsDeleted = false,
                Name = "UnitTestArticle",
                Price = 10
            };

            ArticleDetails articleDetails = new ArticleDetails
            {
                OriginalQuantity = 10,
                ReservedQuantity = 0,
            };

            //Act
            int articleId = await _databaseService.AddArticle(article, _efContext);
            articleDetails.ArticleID = articleId;
            int articleDetailsId = await _databaseService.AddArticleDetails(articleDetails, _efContext);
            ArticleDetails articleDetailsFind = await _databaseService.GetArticleDetailsByID(articleDetailsId, _efContext);

            //Assert
            Assert.That(articleDetailsFind, Is.Not.Null);
            await _databaseService.DeleteArticle(article, _efContext);
        }

    }
}