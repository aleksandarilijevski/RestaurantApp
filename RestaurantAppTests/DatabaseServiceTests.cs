
using EntityFramework.Models;
using RestaurantApp.Services;
using RestaurantApp.Services.Interface;

namespace RestaurantAppTests
{
    public class Tests
    {
        private IDatabaseService _databaseService;

        public Tests()
        {
            _databaseService = new DatabaseService();
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
            int articleId = await _databaseService.AddArticle(article, new EFContext());
            Article articleFind = await _databaseService.GetArticleByID(articleId, new EFContext());

            //Assert
            Assert.That(articleFind, Is.Not.Null);
            await _databaseService.DeleteArticle(articleFind, new EFContext());
        }
    }
}