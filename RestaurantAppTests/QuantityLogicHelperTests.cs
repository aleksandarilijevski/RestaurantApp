using EntityFramework.Models;
using Microsoft.EntityFrameworkCore;
using RestaurantApp.Factories;
using RestaurantApp.Factories.Interfaces;
using RestaurantApp.Services;
using RestaurantApp.Services.Interface;
using System.Collections.ObjectModel;

namespace RestaurantAppTests
{
    public class QuantityLogicHelperTests
    {
        private IAbstractFactory<EFContext> _efContextFactory;
        private IAbstractFactory<IDatabaseService> _databaseServiceFactory;

        private DbContextOptions options = new DbContextOptionsBuilder<EFContext>().UseInMemoryDatabase("TestingDatabase").Options;

        public QuantityLogicHelperTests()
        {
            _efContextFactory = new AbstractFactory<EFContext>(() => new EFContext(options));
            _databaseServiceFactory = new AbstractFactory<IDatabaseService>(() => new DatabaseService());
        }

        [Test]
        public async Task GetAvailableQuantityTest()
        {
            //Arrange
            IDatabaseService? databaseService = _databaseServiceFactory.Create();
            EFContext? efContext = _efContextFactory.Create();

            Article article1 = new Article
            {
                Barcode = 123,
                IsDeleted = false,
                Name = "UnitTestArticle1",
                Price = 10
            };

            Article article2 = new Article
            {
                Barcode = 123,
                IsDeleted = false,
                Name = "UnitTestArticle1",
                Price = 10
            };

            ArticleDetails articleDetails = new ArticleDetails
            {
                OriginalQuantity = 20
            };

            ArticleDetails articleDetails2 = new ArticleDetails
            {
                OriginalQuantity = 40
            };

            //Act
            article1.ArticleDetails = new List<ArticleDetails> { articleDetails };
            article2.ArticleDetails = new List<ArticleDetails> { articleDetails2 };

            int articleId1 = await databaseService.AddArticle(article1, efContext);
            int articleId2 = await databaseService.AddArticle(article2, efContext);

            articleDetails.ArticleID = articleId1;
            articleDetails2.ArticleID = articleId2;

            ObservableCollection<Article> articles = new ObservableCollection<Article>(await databaseService.GetAllArticles(efContext));

            int quantity = 0;

            foreach (Article article in articles)
            {
                quantity += article.ArticleDetails.Sum(x => x.OriginalQuantity);
            }

            //Assert
            Assert.That(quantity, Is.Not.Zero);
        }

        [Test]
        public async Task IncreaseReservedQuantity()
        {
            IDatabaseService? databaseService = _databaseServiceFactory.Create();
            EFContext? efContext = _efContextFactory.Create();

            //Arrange
            int quantity = 20;
            int quantityToAdd = 7;

            Article article = new Article
            {
                Barcode = 123,
                IsDeleted = false,
                Name = "UnitTestArticle1",
                Price = 10
            };

            ArticleDetails articleDetails = new ArticleDetails
            {
                ReservedQuantity = quantity
            };

            //Act
            article.ArticleDetails = new List<ArticleDetails> { articleDetails };

            int articleId1 = await databaseService.AddArticle(article, efContext);
            articleDetails.ArticleID = articleId1;

            articleDetails.ReservedQuantity += quantityToAdd;
            ArticleDetails articleDetailsCheck = await databaseService.GetArticleDetailsByID(articleDetails.ID, efContext);

            //Assert
            Assert.That(articleDetailsCheck.ReservedQuantity, Is.Not.EqualTo(quantity));
        }


        [Test]
        public async Task DecreaseReservedQuantityTest()
        {
            IDatabaseService? databaseService = _databaseServiceFactory.Create();
            EFContext? efContext = _efContextFactory.Create();

            //Arrange
            int quantity = 20;
            int quantityToDecrease = 7;

            Article article = new Article
            {
                Barcode = 123,
                IsDeleted = false,
                Name = "UnitTestArticle1",
                Price = 10
            };

            ArticleDetails articleDetails = new ArticleDetails
            {
                ReservedQuantity = quantity
            };

            //Act
            article.ArticleDetails = new List<ArticleDetails> { articleDetails };

            int articleId1 = await databaseService.AddArticle(article, efContext);
            articleDetails.ArticleID = articleId1;

            articleDetails.ReservedQuantity -= quantityToDecrease;
            ArticleDetails articleDetailsCheck = await databaseService.GetArticleDetailsByID(articleDetails.ID, efContext);

            //Assert
            Assert.That(articleDetailsCheck.ReservedQuantity, Is.LessThan(quantity));
        }

        [Test]
        public async Task DecreaseOriginalQuantity()
        {
            IDatabaseService? databaseService = _databaseServiceFactory.Create();
            EFContext? efContext = _efContextFactory.Create();

            //Arrange
            int quantity = 20;
            int quantityToDecrease = 7;

            Article article = new Article
            {
                Barcode = 123,
                IsDeleted = false,
                Name = "UnitTestArticle1",
                Price = 10
            };

            ArticleDetails articleDetails = new ArticleDetails
            {
                OriginalQuantity = quantity
            };

            //Act
            article.ArticleDetails = new List<ArticleDetails> { articleDetails };

            int articleId1 = await databaseService.AddArticle(article, efContext);
            articleDetails.ArticleID = articleId1;

            articleDetails.OriginalQuantity -= quantityToDecrease;
            ArticleDetails articleDetailsCheck = await databaseService.GetArticleDetailsByID(articleDetails.ID, efContext);

            //Assert
            Assert.That(articleDetailsCheck.OriginalQuantity, Is.LessThan(quantity));
        }
    }
}
