using EntityFramework.Enums;
using EntityFramework.Models;
using Microsoft.EntityFrameworkCore;
using RestaurantApp.Enums;
using RestaurantApp.Factories;
using RestaurantApp.Factories.Interfaces;
using RestaurantApp.Services;
using RestaurantApp.Services.Interface;
using System.Collections.ObjectModel;

namespace RestaurantAppTests
{
    public class Tests
    {
        private IAbstractFactory<IDatabaseService> _databaseServiceFactory;
        private IAbstractFactory<EFContext> _efContextFactory;
        private DbContextOptions options = new DbContextOptionsBuilder<EFContext>().UseInMemoryDatabase("TestingDatabase").Options;

        public Tests()
        {
            _databaseServiceFactory = new AbstractFactory<IDatabaseService>(() => new DatabaseService());
            _efContextFactory = new AbstractFactory<EFContext>(() => new EFContext(options));
        }

        [Test]
        public async Task GetAllArticles()
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
                Name = "UnitTestArticle2",
                Price = 10
            };

            //Act
            await databaseService.AddArticle(article1, efContext);
            await databaseService.AddArticle(article2, efContext);

            ObservableCollection<Article> articles = new ObservableCollection<Article>(await databaseService.GetAllArticles(efContext));

            //Assert
            Assert.That(articles, Is.Not.Empty);
        }

        [Test]
        public async Task GetAllUsers()
        {
            //Arrange
            IDatabaseService? databaseService = _databaseServiceFactory.Create();
            EFContext? efContext = _efContextFactory.Create();

            User user1 = new User
            {
                FirstAndLastName = "UnitTest1",
                Barcode = 123,
                DateOfBirth = DateTime.UtcNow,
                JMBG = 1,
                UserRole = UserRole.Waiter
            };

            User user2 = new User
            {
                FirstAndLastName = "UnitTest2",
                Barcode = 123,
                DateOfBirth = DateTime.UtcNow,
                JMBG = 1,
                UserRole = UserRole.Waiter
            };

            //Act
            await databaseService.AddUser(user1, efContext);
            await databaseService.AddUser(user2, efContext);

            ObservableCollection<User> users = new ObservableCollection<User>(await databaseService.GetAllUsers(efContext));

            //Assert
            Assert.That(users, Is.Not.Empty);
        }

        [Test]
        public async Task GetAllTables()
        {
            //Arrange
            IDatabaseService? databaseService = _databaseServiceFactory.Create();
            EFContext? efContext = _efContextFactory.Create();

            Table table1 = new Table
            {
                ID = 1,
                InUse = false
            };

            Table table2 = new Table
            {
                ID = 2,
                InUse = false
            };

            //Act
            await databaseService.AddTable(table1, efContext);
            await databaseService.AddTable(table2, efContext);

            List<Table> tables = await databaseService.GetAllTables();

            //Assert
            Assert.That(tables, Is.Not.Empty);

        }

        [Test]
        public async Task AddArticle()
        {
            //Arrange
            IDatabaseService? databaseService = _databaseServiceFactory.Create();
            EFContext? efContext = _efContextFactory.Create();

            Article article = new Article
            {
                Barcode = 123,
                IsDeleted = false,
                Name = "UnitTestArticle",
                Price = 10
            };

            //Act

            int articleId = await databaseService.AddArticle(article, efContext);
            Article articleFind = await databaseService.GetArticleByID(articleId, efContext);

            //Assert
            Assert.That(articleFind, Is.Not.Null);
        }

        [Test]
        public async Task AddUser()
        {
            //Arrange
            IDatabaseService? databaseService = _databaseServiceFactory.Create();
            EFContext? efContext = _efContextFactory.Create();

            User user = new User
            {
                FirstAndLastName = "UnitTest",
                Barcode = 123,
                DateOfBirth = DateTime.UtcNow,
                JMBG = 1,
                UserRole = UserRole.Waiter
            };

            //Act
            int userId = await databaseService.AddUser(user, efContext);
            User userFind = await databaseService.GetUserByID(userId, efContext);

            //Assert
            Assert.That(userFind, Is.Not.Null);
            await databaseService.DeleteUser(user, efContext);
        }

        [Test]
        public async Task AddArticleDetails()
        {
            //Arrange
            IDatabaseService? databaseService = _databaseServiceFactory.Create();
            EFContext? efContext = _efContextFactory.Create();

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
            int articleId = await databaseService.AddArticle(article, efContext);
            articleDetails.ArticleID = articleId;
            int articleDetailsId = await databaseService.AddArticleDetails(articleDetails, efContext);
            ArticleDetails articleDetailsFind = await databaseService.GetArticleDetailsByID(articleDetailsId, efContext);

            //Assert
            Assert.That(articleDetailsFind, Is.Not.Null);
            await databaseService.DeleteArticle(article, efContext);
        }

        [Test]
        public async Task AddTable()
        {
            //Arrange
            IDatabaseService? databaseService = _databaseServiceFactory.Create();
            EFContext? efContext = _efContextFactory.Create();

            List<Table> tables = new List<Table>();
            int tableId = 1;

            Table table = new Table
            {
                InUse = false
            };

            //Act
            if (tables.Count != 0)
            {
                tableId = tables.Max(x => x.ID + 1);
            }

            table.ID = tableId;
            await databaseService.AddTable(table, efContext);
            Table tableFind = await databaseService.GetTableByID(tableId, efContext);

            //Assert
            Assert.That(tableFind, Is.Not.Null);
            await databaseService.DeleteTable(table, efContext);
        }

        [Test]
        public async Task AddTableArticleQuantity()
        {
            //Arrange
            IDatabaseService? databaseService = _databaseServiceFactory.Create();
            EFContext? efContext = _efContextFactory.Create();

            Article article = new Article
            {
                Barcode = 123,
                IsDeleted = false,
                Name = "UnitTestArticle",
                Price = 10
            };

            ArticleDetails articleDetail = new ArticleDetails
            {
                OriginalQuantity = 10,
                ReservedQuantity = 0,
            };

            List<ArticleDetails> articleDetails = new List<ArticleDetails>
            {
                articleDetail
            };

            TableArticleQuantity tableArticleQuantity = new TableArticleQuantity
            {
                Quantity = 1
            };

            //Act
            int articleId = await databaseService.AddArticle(article, efContext);
            articleDetail.ArticleID = articleId;

            await databaseService.AddArticleDetails(articleDetail, efContext);

            tableArticleQuantity.ArticleID = articleId;
            tableArticleQuantity.ArticleDetails = articleDetails;

            int tableArticleQuantityId = await databaseService.AddTableArticleQuantity(tableArticleQuantity, efContext);
            TableArticleQuantity tableArticleQuantityFind = await databaseService.GetTableArticleQuantityByID(tableArticleQuantityId, efContext);

            //Assert
            Assert.That(tableArticleQuantityFind, Is.Not.Null);
            await databaseService.DeleteArticle(article, efContext);
        }

        [Test]
        public async Task AddBill()
        {
            //Arrange
            IDatabaseService? databaseService = _databaseServiceFactory.Create();
            EFContext? efContext = _efContextFactory.Create();

            User user = new User
            {
                FirstAndLastName = "Test",
                Barcode = 123,
                DateOfBirth = DateTime.Now,
                JMBG = 1,
                UserRole = UserRole.Waiter,
            };

            Bill bill = new Bill
            {
                TotalPrice = 100,
                Cash = 85,
                Change = 15,
                RegistrationNumber = "unitTest",
                PaymentType = PaymentType.Cash
            };

            //Act
            int userId = await databaseService.AddUser(user, efContext);
            bill.UserID = userId;

            int billId = await databaseService.CreateBill(bill, efContext);
            Bill billFind = await databaseService.GetBillByID(billId, efContext);

            //Assert
            Assert.That(billFind, Is.Not.Null);
            await databaseService.DeleteBill(bill, efContext);
            await databaseService.DeleteUser(user, efContext);
        }

        [Test]
        public async Task AddDataEntry()
        {
            //Arrange
            IDatabaseService? databaseService = _databaseServiceFactory.Create();
            EFContext? efContext = _efContextFactory.Create();

            Article article = new Article
            {
                Barcode = 123,
                IsDeleted = false,
                Name = "UnitTestArticle",
                Price = 10
            };

            ArticleDetails articleDetail = new ArticleDetails
            {
                OriginalQuantity = 10,
                ReservedQuantity = 0,
            };

            List<ArticleDetails> articleDetails = new List<ArticleDetails>
            {
                articleDetail
            };

            DataEntry dataEntry = new DataEntry
            {
                DataEntryNumber = 1,
                TotalAmount = 100
            };

            //Act
            int articleId = await databaseService.AddArticle(article, efContext);
            articleDetail.ArticleID = articleId;
            await databaseService.AddArticleDetails(articleDetail, efContext);

            dataEntry.ArticleDetails = articleDetails;
            int dataEntryId = await databaseService.AddDataEntry(dataEntry, efContext);

            DataEntry dataEntryFind = await databaseService.GetDataEntryByID(dataEntryId, efContext);

            //Assert
            Assert.That(dataEntryFind, Is.Not.Null);
            await databaseService.DeleteDataEntry(dataEntryFind, efContext);
        }

        [Test]
        public async Task AddOnlineOrder()
        {
            //Arrange
            IDatabaseService? databaseService = _databaseServiceFactory.Create();
            EFContext? efContext = _efContextFactory.Create();

            User user = new User
            {
                FirstAndLastName = "Test",
                Barcode = 123,
                DateOfBirth = DateTime.Now,
                JMBG = 1,
                UserRole = UserRole.Waiter,
            };

            OnlineOrder onlineOrder = new OnlineOrder
            {
                Firstname = "Test",
                IsPayed = false
            };

            //Act
            int userId = await databaseService.AddUser(user, efContext);
            onlineOrder.UserID = userId;

            int onlineOrderId = await databaseService.AddOnlineOrder(onlineOrder, efContext);

            OnlineOrder onlineOrderFind = await databaseService.GetOnlineOrderByID(onlineOrderId, efContext);

            //Assert
            Assert.That(onlineOrderFind, Is.Not.Null);
            await databaseService.DeleteOnlineOrder(onlineOrderFind, efContext);
            await databaseService.DeleteUser(user, efContext);
        }

        [Test]
        public async Task AddSoldArticleDetails()
        {
            //Arrange
            IDatabaseService? databaseService = _databaseServiceFactory.Create();
            EFContext? efContext = _efContextFactory.Create();

            User user = new User
            {
                FirstAndLastName = "Test",
                Barcode = 123,
                DateOfBirth = DateTime.Now,
                JMBG = 1,
                UserRole = UserRole.Waiter,
            };

            Bill bill = new Bill
            {
                TotalPrice = 100,
                Cash = 85,
                Change = 15,
                RegistrationNumber = "unitTest",
                PaymentType = PaymentType.Cash
            };

            SoldArticleDetails soldArticleDetails = new SoldArticleDetails
            {
                EntryPrice = 10,
                SoldQuantity = 5,
            };

            //Act
            int userId = await databaseService.AddUser(user, efContext);
            bill.UserID = userId;

            int billId = await databaseService.CreateBill(bill, efContext);
            soldArticleDetails.BillID = billId;
            int soldArticleDetailsId = await databaseService.AddSoldArticleDetails(soldArticleDetails, efContext);

            SoldArticleDetails soldArticleDetailsFind = await databaseService.GetSoldArticleDetailsByID(soldArticleDetailsId, efContext);

            //Assert
            Assert.That(soldArticleDetailsFind, Is.Not.Null);
            await databaseService.DeleteUser(user, efContext);
        }

        [Test]
        public async Task DeleteArticle()
        {
            //Arrange
            IDatabaseService? databaseService = _databaseServiceFactory.Create();
            EFContext? efContext = _efContextFactory.Create();

            Article article = new Article
            {
                Barcode = 1,
                Name = "Test",
                IsDeleted = false,
                Price = 10
            };

            //Act
            int articleId = await databaseService.AddArticle(article, efContext);
            await databaseService.DeleteArticle(article, efContext);
            Article articleFind = await databaseService.GetArticleByID(articleId, efContext);

            //Assert
            Assert.That(articleFind, Is.Null);
        }

        [Test]
        public async Task DeleteArticleDetails()
        {
            //Arrange
            IDatabaseService? databaseService = _databaseServiceFactory.Create();
            EFContext? efContext = _efContextFactory.Create();

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
            int articleId = await databaseService.AddArticle(article, efContext);
            articleDetails.ArticleID = articleId;
            int articleDetailsId = await databaseService.AddArticleDetails(articleDetails, efContext);
            await databaseService.DeleteArticleDetails(articleDetails, efContext);
            ArticleDetails articleDetailsFind = await databaseService.GetArticleDetailsByID(articleDetailsId, efContext);

            //Assert
            Assert.That(articleDetailsFind, Is.Null);
            await databaseService.DeleteArticle(article, efContext);
        }

        [Test]
        public async Task DeleteBill()
        {
            //Arrange
            IDatabaseService? databaseService = _databaseServiceFactory.Create();
            EFContext? efContext = _efContextFactory.Create();

            User user = new User
            {
                FirstAndLastName = "Test",
                Barcode = 123,
                DateOfBirth = DateTime.Now,
                JMBG = 1,
                UserRole = UserRole.Waiter,
            };

            Bill bill = new Bill
            {
                TotalPrice = 100,
                Cash = 85,
                Change = 15,
                RegistrationNumber = "unitTest",
                PaymentType = PaymentType.Cash
            };

            //Act
            int userId = await databaseService.AddUser(user, efContext);
            bill.UserID = userId;

            int billId = await databaseService.CreateBill(bill, efContext);
            await databaseService.DeleteBill(bill, efContext);
            Bill billFind = await databaseService.GetBillByID(billId, efContext);

            //Assert
            Assert.That(billFind, Is.Null);
            await databaseService.DeleteUser(user, efContext);
        }

        [Test]
        public async Task DeleteDataEntry()
        {
            //Arrange
            IDatabaseService? databaseService = _databaseServiceFactory.Create();
            EFContext? efContext = _efContextFactory.Create();

            Article article = new Article
            {
                Barcode = 123,
                IsDeleted = false,
                Name = "UnitTestArticle",
                Price = 10
            };

            ArticleDetails articleDetail = new ArticleDetails
            {
                OriginalQuantity = 10,
                ReservedQuantity = 0,
            };

            List<ArticleDetails> articleDetails = new List<ArticleDetails>
            {
                articleDetail
            };

            DataEntry dataEntry = new DataEntry
            {
                DataEntryNumber = 1,
                TotalAmount = 100
            };

            //Act
            int articleId = await databaseService.AddArticle(article, efContext);
            articleDetail.ArticleID = articleId;
            await databaseService.AddArticleDetails(articleDetail, efContext);

            dataEntry.ArticleDetails = articleDetails;
            int dataEntryId = await databaseService.AddDataEntry(dataEntry, efContext);

            await databaseService.DeleteDataEntry(dataEntry, efContext);
            DataEntry dataEntryFind = await databaseService.GetDataEntryByID(dataEntryId, efContext);

            //Assert
            Assert.That(dataEntryFind, Is.Null);
        }

        [Test]
        public async Task DeleteOnlineOrder()
        {
            //Arrange
            IDatabaseService? databaseService = _databaseServiceFactory.Create();
            EFContext? efContext = _efContextFactory.Create();

            User user = new User
            {
                FirstAndLastName = "Test",
                Barcode = 123,
                DateOfBirth = DateTime.Now,
                JMBG = 1,
                UserRole = UserRole.Waiter,
            };

            OnlineOrder onlineOrder = new OnlineOrder
            {
                Firstname = "Test",
                IsPayed = false
            };

            //Act
            int userId = await databaseService.AddUser(user, efContext);
            onlineOrder.UserID = userId;

            int onlineOrderId = await databaseService.AddOnlineOrder(onlineOrder, efContext);
            await databaseService.DeleteOnlineOrder(onlineOrder, efContext);
            OnlineOrder onlineOrderFind = await databaseService.GetOnlineOrderByID(onlineOrderId, efContext);

            //Assert
            Assert.That(onlineOrderFind, Is.Null);
            await databaseService.DeleteUser(user, efContext);
        }

        [Test]
        public async Task DeleteSoldArticleDetails()
        {
            //Arrange
            IDatabaseService? databaseService = _databaseServiceFactory.Create();
            EFContext? efContext = _efContextFactory.Create();

            User user = new User
            {
                FirstAndLastName = "Test",
                Barcode = 123,
                DateOfBirth = DateTime.Now,
                JMBG = 1,
                UserRole = UserRole.Waiter,
            };

            Bill bill = new Bill
            {
                TotalPrice = 100,
                Cash = 85,
                Change = 15,
                RegistrationNumber = "unitTest",
                PaymentType = PaymentType.Cash
            };

            SoldArticleDetails soldArticleDetails = new SoldArticleDetails
            {
                EntryPrice = 10,
                SoldQuantity = 5,
            };

            //Act
            int userId = await databaseService.AddUser(user, efContext);
            bill.UserID = userId;

            int billId = await databaseService.CreateBill(bill, efContext);
            soldArticleDetails.BillID = billId;
            int soldArticleDetailsId = await databaseService.AddSoldArticleDetails(soldArticleDetails, efContext);

            await databaseService.DeleteSoldArticleDetails(soldArticleDetails, efContext);
            SoldArticleDetails soldArticleDetailsFind = await databaseService.GetSoldArticleDetailsByID(soldArticleDetailsId, efContext);

            //Assert
            Assert.That(soldArticleDetailsFind, Is.Null);
            await databaseService.DeleteUser(user, efContext);
        }

        [Test]
        public async Task DeleteTable()
        {
            //Arrange
            IDatabaseService? databaseService = _databaseServiceFactory.Create();
            EFContext? efContext = _efContextFactory.Create();

            List<Table> tables = new List<Table>();
            int tableId = 1;

            Table table = new Table
            {
                InUse = false
            };

            //Act
            if (tables.Count != 0)
            {
                tableId = tables.Max(x => x.ID + 1);
            }

            table.ID = tableId;
            await databaseService.AddTable(table, efContext);
            await databaseService.DeleteTable(table, efContext);
            Table tableFind = await databaseService.GetTableByID(tableId, efContext);

            //Assert
            Assert.That(tableFind, Is.Null);
        }

        [Test]
        public async Task DeleteTableArticleQuantity()
        {
            //Arrange
            IDatabaseService? databaseService = _databaseServiceFactory.Create();
            EFContext? efContext = _efContextFactory.Create();

            Article article = new Article
            {
                Barcode = 123,
                IsDeleted = false,
                Name = "UnitTestArticle",
                Price = 10
            };

            ArticleDetails articleDetail = new ArticleDetails
            {
                OriginalQuantity = 10,
                ReservedQuantity = 0,
            };

            List<ArticleDetails> articleDetails = new List<ArticleDetails>
            {
                articleDetail
            };

            TableArticleQuantity tableArticleQuantity = new TableArticleQuantity
            {
                Quantity = 1
            };

            //Act
            int articleId = await databaseService.AddArticle(article, efContext);
            articleDetail.ArticleID = articleId;

            await databaseService.AddArticleDetails(articleDetail, efContext);

            tableArticleQuantity.ArticleID = articleId;
            tableArticleQuantity.ArticleDetails = articleDetails;

            int tableArticleQuantityId = await databaseService.AddTableArticleQuantity(tableArticleQuantity, efContext);
            await databaseService.DeleteTableArticleQuantity(tableArticleQuantity, efContext);
            TableArticleQuantity tableArticleQuantityFind = await databaseService.GetTableArticleQuantityByID(tableArticleQuantityId, efContext);

            //Assert
            Assert.That(tableArticleQuantityFind, Is.Null);
            await databaseService.DeleteArticle(article, efContext);
        }


        [Test]
        public async Task DeleteUser()
        {
            //Arrange
            IDatabaseService? databaseService = _databaseServiceFactory.Create();
            EFContext? efContext = _efContextFactory.Create();

            User user = new User
            {
                FirstAndLastName = "UnitTest",
                Barcode = 123,
                DateOfBirth = DateTime.UtcNow,
                JMBG = 1,
                UserRole = UserRole.Waiter
            };

            //Act
            int userId = await databaseService.AddUser(user, efContext);
            await databaseService.DeleteUser(user, efContext);
            User userFind = await databaseService.GetUserByID(userId, efContext);

            //Assert
            Assert.That(userFind, Is.Null);
        }

        [Test]
        public async Task EditArticle()
        {
            //Arrange
            IDatabaseService? databaseService = _databaseServiceFactory.Create();
            EFContext? efContext = _efContextFactory.Create();

            string articleName = "UnitTestArticle";

            Article article = new Article
            {
                Barcode = 123,
                IsDeleted = false,
                Name = articleName,
                Price = 10
            };

            //Act
            await databaseService.AddArticle(article, efContext);
            article.Name = "UnitTestArticleEdited";
            await databaseService.EditArticle(article, efContext);

            //Assert
            Assert.That(article.Name, Is.Not.EqualTo(articleName));
            await databaseService.DeleteArticle(article, efContext);
        }

        [Test]
        public async Task EditArticleDetails()
        {
            //Arrange
            IDatabaseService? databaseService = _databaseServiceFactory.Create();
            EFContext? efContext = _efContextFactory.Create();

            int originalQuantity = 10;

            Article article = new Article
            {
                Barcode = 123,
                IsDeleted = false,
                Name = "UnitTestArticle",
                Price = 10
            };

            ArticleDetails articleDetails = new ArticleDetails
            {
                OriginalQuantity = originalQuantity,
                ReservedQuantity = 0,
            };

            //Act
            int articleId = await databaseService.AddArticle(article, efContext);

            articleDetails.ArticleID = articleId;
            await databaseService.AddArticleDetails(articleDetails, efContext);

            articleDetails.OriginalQuantity = 15;
            await databaseService.EditArticleDetails(articleDetails, efContext);

            //Assert
            Assert.That(articleDetails.OriginalQuantity, Is.Not.EqualTo(originalQuantity));
            await databaseService.DeleteArticle(article, efContext);
        }

        [Test]
        public async Task EditOnlineOrder()
        {
            //Arrange
            IDatabaseService? databaseService = _databaseServiceFactory.Create();
            EFContext? efContext = _efContextFactory.Create();

            string firstName = "unitTest";

            User user = new User
            {
                FirstAndLastName = "Test",
                Barcode = 123,
                DateOfBirth = DateTime.Now,
                JMBG = 1,
                UserRole = UserRole.Waiter,
            };

            OnlineOrder onlineOrder = new OnlineOrder
            {
                Firstname = firstName,
                IsPayed = false
            };

            //Act
            int userId = await databaseService.AddUser(user, efContext);
            onlineOrder.UserID = userId;

            await databaseService.AddOnlineOrder(onlineOrder, efContext);
            onlineOrder.Firstname = "unitTestEdited";
            await databaseService.EditOnlineOrder(onlineOrder, efContext);

            //Assert
            Assert.That(onlineOrder.Firstname, Is.Not.EqualTo(firstName));
            await databaseService.DeleteOnlineOrder(onlineOrder, efContext);
            await databaseService.DeleteUser(user, efContext);
        }

        [Test]
        public async Task EditTable()
        {
            //Arrange
            IDatabaseService? databaseService = _databaseServiceFactory.Create();
            EFContext? efContext = _efContextFactory.Create();

            List<Table> tables = new List<Table>();
            int tableId = 1;

            Table table = new Table
            {
                InUse = false
            };

            //Act
            if (tables.Count != 0)
            {
                tableId = tables.Max(x => x.ID + 1);
            }

            table.ID = tableId;
            await databaseService.AddTable(table, efContext);

            table.InUse = true;
            await databaseService.EditTable(table, efContext);

            //Assert
            Assert.That(table.InUse, Is.Not.False);
            await databaseService.DeleteTable(table, efContext);
        }

        [Test]
        public async Task EditTableArticleQuantity()
        {
            //Arrange
            IDatabaseService? databaseService = _databaseServiceFactory.Create();
            EFContext? efContext = _efContextFactory.Create();

            int quantity = 1;

            Article article = new Article
            {
                Barcode = 123,
                IsDeleted = false,
                Name = "UnitTestArticle",
                Price = 10
            };

            ArticleDetails articleDetail = new ArticleDetails
            {
                OriginalQuantity = 10,
                ReservedQuantity = 0,
            };

            List<ArticleDetails> articleDetails = new List<ArticleDetails>
            {
                articleDetail
            };

            TableArticleQuantity tableArticleQuantity = new TableArticleQuantity
            {
                Quantity = quantity
            };

            //Act
            int articleId = await databaseService.AddArticle(article, efContext);
            articleDetail.ArticleID = articleId;

            int articleDetailsId = await databaseService.AddArticleDetails(articleDetail, efContext);

            tableArticleQuantity.ArticleID = articleId;
            tableArticleQuantity.ArticleDetails = articleDetails;

            await databaseService.AddTableArticleQuantity(tableArticleQuantity, efContext);
            tableArticleQuantity.Quantity = 50;
            await databaseService.EditTableArticleQuantity(tableArticleQuantity, efContext);

            //Assert
            Assert.That(tableArticleQuantity.Quantity, Is.Not.EqualTo(quantity));
            await databaseService.DeleteArticle(article, efContext);
        }

        [Test]
        public async Task EditUser()
        {
            //Arrange
            IDatabaseService? databaseService = _databaseServiceFactory.Create();
            EFContext? efContext = _efContextFactory.Create();

            string firstAndLastname = "UnitTest";

            User user = new User
            {
                FirstAndLastName = firstAndLastname,
                Barcode = 123,
                DateOfBirth = DateTime.UtcNow,
                JMBG = 1,
                UserRole = UserRole.Waiter
            };

            //Act
            await databaseService.AddUser(user, efContext);
            user.FirstAndLastName = "UnitTestEdited";
            await databaseService.EditUser(user, efContext);

            //Assert
            Assert.That(user.FirstAndLastName, Is.Not.EqualTo(firstAndLastname));
            await databaseService.DeleteUser(user, efContext);
        }
    }
}