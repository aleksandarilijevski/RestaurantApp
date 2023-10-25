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
            IDatabaseService? _databaseService = _databaseServiceFactory.Create();
            EFContext? _efContext = _efContextFactory.Create();

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
            await _databaseService.AddArticle(article1, _efContext);
            await _databaseService.AddArticle(article2, _efContext);

            ObservableCollection<Article> articles = await _databaseService.GetAllArticles();


            //Assert
            Assert.That(articles, Is.Not.Empty);
        }

        [Test]
        public async Task GetAllUsers()
        {
            //Arrange
            IDatabaseService? _databaseService = _databaseServiceFactory.Create();
            EFContext? _efContext = _efContextFactory.Create();

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
            await _databaseService.AddUser(user1, _efContext);
            await _databaseService.AddUser(user2, _efContext);

            ObservableCollection<User> users = await _databaseService.GetAllUsers();

            //Assert
            Assert.That(users, Is.Not.Empty);
        }

        [Test]
        public async Task GetAllTables()
        {
            //Arrange
            IDatabaseService? _databaseService = _databaseServiceFactory.Create();
            EFContext? _efContext = _efContextFactory.Create();

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
            await _databaseService.AddTable(table1, _efContext);
            await _databaseService.AddTable(table2, _efContext);

            List<Table> tables = await _databaseService.GetAllTables();

            //Assert
            Assert.That(tables, Is.Not.Empty);

        }

        [Test]
        public async Task AddArticle()
        {
            //Arrange
            IDatabaseService? _databaseService = _databaseServiceFactory.Create();
            EFContext? _efContext = _efContextFactory.Create();

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
        }

        [Test]
        public async Task AddUser()
        {
            //Arrange
            IDatabaseService? _databaseService = _databaseServiceFactory.Create();
            EFContext? _efContext = _efContextFactory.Create();

            User user = new User
            {
                FirstAndLastName = "UnitTest",
                Barcode = 123,
                DateOfBirth = DateTime.UtcNow,
                JMBG = 1,
                UserRole = UserRole.Waiter
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
            IDatabaseService? _databaseService = _databaseServiceFactory.Create();
            EFContext? _efContext = _efContextFactory.Create();

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

        [Test]
        public async Task AddTable()
        {
            //Arrange
            IDatabaseService? _databaseService = _databaseServiceFactory.Create();
            EFContext? _efContext = _efContextFactory.Create();

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
            await _databaseService.AddTable(table, _efContext);
            Table tableFind = await _databaseService.GetTableByID(tableId, _efContext);

            //Assert
            Assert.That(tableFind, Is.Not.Null);
            await _databaseService.DeleteTable(table, _efContext);
        }

        [Test]
        public async Task AddTableArticleQuantity()
        {
            //Arrange
            IDatabaseService? _databaseService = _databaseServiceFactory.Create();
            EFContext? _efContext = _efContextFactory.Create();

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
            int articleId = await _databaseService.AddArticle(article, _efContext);
            articleDetail.ArticleID = articleId;

            await _databaseService.AddArticleDetails(articleDetail, _efContext);

            tableArticleQuantity.ArticleID = articleId;
            tableArticleQuantity.ArticleDetails = articleDetails;

            int tableArticleQuantityId = await _databaseService.AddTableArticleQuantity(tableArticleQuantity, _efContext);
            TableArticleQuantity tableArticleQuantityFind = await _databaseService.GetTableArticleQuantityByID(tableArticleQuantityId, _efContext);

            //Assert
            Assert.That(tableArticleQuantityFind, Is.Not.Null);
            await _databaseService.DeleteArticle(article, _efContext);
        }

        [Test]
        public async Task AddBill()
        {
            //Arrange
            IDatabaseService? _databaseService = _databaseServiceFactory.Create();
            EFContext? _efContext = _efContextFactory.Create();

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
            int userId = await _databaseService.AddUser(user, _efContext);
            bill.UserID = userId;

            int billId = await _databaseService.CreateBill(bill, _efContext);
            Bill billFind = await _databaseService.GetBillByID(billId, _efContext);

            //Assert
            Assert.That(billFind, Is.Not.Null);
            await _databaseService.DeleteBill(bill, _efContext);
            await _databaseService.DeleteUser(user, _efContext);
        }

        [Test]
        public async Task AddDataEntry()
        {
            //Arrange
            IDatabaseService? _databaseService = _databaseServiceFactory.Create();
            EFContext? _efContext = _efContextFactory.Create();

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
            int articleId = await _databaseService.AddArticle(article, _efContext);
            articleDetail.ArticleID = articleId;
            await _databaseService.AddArticleDetails(articleDetail, _efContext);

            dataEntry.ArticleDetails = articleDetails;
            int dataEntryId = await _databaseService.AddDataEntry(dataEntry, _efContext);

            DataEntry dataEntryFind = await _databaseService.GetDataEntryByID(dataEntryId, _efContext);

            //Assert
            Assert.That(dataEntryFind, Is.Not.Null);
            await _databaseService.DeleteDataEntry(dataEntryFind, _efContext);
        }

        [Test]
        public async Task AddOnlineOrder()
        {
            //Arrange
            IDatabaseService? _databaseService = _databaseServiceFactory.Create();
            EFContext? _efContext = _efContextFactory.Create();

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
            int userId = await _databaseService.AddUser(user, _efContext);
            onlineOrder.UserID = userId;

            int onlineOrderId = await _databaseService.AddOnlineOrder(onlineOrder, _efContext);

            OnlineOrder onlineOrderFind = await _databaseService.GetOnlineOrderByID(onlineOrderId, _efContext);

            //Assert
            Assert.That(onlineOrderFind, Is.Not.Null);
            await _databaseService.DeleteOnlineOrder(onlineOrderFind, _efContext);
            await _databaseService.DeleteUser(user, _efContext);
        }

        [Test]
        public async Task AddSoldArticleDetails()
        {
            //Arrange
            IDatabaseService? _databaseService = _databaseServiceFactory.Create();
            EFContext? _efContext = _efContextFactory.Create();

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
            int userId = await _databaseService.AddUser(user, _efContext);
            bill.UserID = userId;

            int billId = await _databaseService.CreateBill(bill, _efContext);
            soldArticleDetails.BillID = billId;
            int soldArticleDetailsId = await _databaseService.AddSoldArticleDetails(soldArticleDetails, _efContext);

            SoldArticleDetails soldArticleDetailsFind = await _databaseService.GetSoldArticleDetailsByID(soldArticleDetailsId, _efContext);

            //Assert
            Assert.That(soldArticleDetailsFind, Is.Not.Null);
            await _databaseService.DeleteUser(user, _efContext);
        }

        [Test]
        public async Task DeleteArticle()
        {
            //Arrange
            IDatabaseService? _databaseService = _databaseServiceFactory.Create();
            EFContext? _efContext = _efContextFactory.Create();

            Article article = new Article
            {
                Barcode = 1,
                Name = "Test",
                IsDeleted = false,
                Price = 10
            };

            //Act
            int articleId = await _databaseService.AddArticle(article, _efContext);
            await _databaseService.DeleteArticle(article, _efContext);
            Article articleFind = await _databaseService.GetArticleByID(articleId, _efContext);

            //Assert
            Assert.That(articleFind, Is.Null);
        }

        [Test]
        public async Task DeleteArticleDetails()
        {
            //Arrange
            IDatabaseService? _databaseService = _databaseServiceFactory.Create();
            EFContext? _efContext = _efContextFactory.Create();

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
            await _databaseService.DeleteArticleDetails(articleDetails, _efContext);
            ArticleDetails articleDetailsFind = await _databaseService.GetArticleDetailsByID(articleDetailsId, _efContext);

            //Assert
            Assert.That(articleDetailsFind, Is.Null);
            await _databaseService.DeleteArticle(article, _efContext);
        }

        [Test]
        public async Task DeleteBill()
        {
            //Arrange
            IDatabaseService? _databaseService = _databaseServiceFactory.Create();
            EFContext? _efContext = _efContextFactory.Create();

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
            int userId = await _databaseService.AddUser(user, _efContext);
            bill.UserID = userId;

            int billId = await _databaseService.CreateBill(bill, _efContext);
            await _databaseService.DeleteBill(bill, _efContext);
            Bill billFind = await _databaseService.GetBillByID(billId, _efContext);

            //Assert
            Assert.That(billFind, Is.Null);
            await _databaseService.DeleteUser(user, _efContext);
        }

        [Test]
        public async Task DeleteDataEntry()
        {
            //Arrange
            IDatabaseService? _databaseService = _databaseServiceFactory.Create();
            EFContext? _efContext = _efContextFactory.Create();

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
            int articleId = await _databaseService.AddArticle(article, _efContext);
            articleDetail.ArticleID = articleId;
            await _databaseService.AddArticleDetails(articleDetail, _efContext);

            dataEntry.ArticleDetails = articleDetails;
            int dataEntryId = await _databaseService.AddDataEntry(dataEntry, _efContext);

            await _databaseService.DeleteDataEntry(dataEntry, _efContext);
            DataEntry dataEntryFind = await _databaseService.GetDataEntryByID(dataEntryId, _efContext);

            //Assert
            Assert.That(dataEntryFind, Is.Null);
        }

        [Test]
        public async Task DeleteOnlineOrder()
        {
            //Arrange
            IDatabaseService? _databaseService = _databaseServiceFactory.Create();
            EFContext? _efContext = _efContextFactory.Create();

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
            int userId = await _databaseService.AddUser(user, _efContext);
            onlineOrder.UserID = userId;

            int onlineOrderId = await _databaseService.AddOnlineOrder(onlineOrder, _efContext);
            await _databaseService.DeleteOnlineOrder(onlineOrder, _efContext);
            OnlineOrder onlineOrderFind = await _databaseService.GetOnlineOrderByID(onlineOrderId, _efContext);

            //Assert
            Assert.That(onlineOrderFind, Is.Null);
            await _databaseService.DeleteUser(user, _efContext);
        }

        [Test]
        public async Task DeleteSoldArticleDetails()
        {
            //Arrange
            IDatabaseService? _databaseService = _databaseServiceFactory.Create();
            EFContext? _efContext = _efContextFactory.Create();

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
            int userId = await _databaseService.AddUser(user, _efContext);
            bill.UserID = userId;

            int billId = await _databaseService.CreateBill(bill, _efContext);
            soldArticleDetails.BillID = billId;
            int soldArticleDetailsId = await _databaseService.AddSoldArticleDetails(soldArticleDetails, _efContext);

            await _databaseService.DeleteSoldArticleDetails(soldArticleDetails, _efContext);
            SoldArticleDetails soldArticleDetailsFind = await _databaseService.GetSoldArticleDetailsByID(soldArticleDetailsId, _efContext);

            //Assert
            Assert.That(soldArticleDetailsFind, Is.Null);
            await _databaseService.DeleteUser(user, _efContext);
        }

        [Test]
        public async Task DeleteTable()
        {
            //Arrange
            IDatabaseService? _databaseService = _databaseServiceFactory.Create();
            EFContext? _efContext = _efContextFactory.Create();

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
            await _databaseService.AddTable(table, _efContext);
            await _databaseService.DeleteTable(table, _efContext);
            Table tableFind = await _databaseService.GetTableByID(tableId, _efContext);

            //Assert
            Assert.That(tableFind, Is.Null);
        }

        [Test]
        public async Task DeleteTableArticleQuantity()
        {
            //Arrange
            IDatabaseService? _databaseService = _databaseServiceFactory.Create();
            EFContext? _efContext = _efContextFactory.Create();

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
            int articleId = await _databaseService.AddArticle(article, _efContext);
            articleDetail.ArticleID = articleId;

            await _databaseService.AddArticleDetails(articleDetail, _efContext);

            tableArticleQuantity.ArticleID = articleId;
            tableArticleQuantity.ArticleDetails = articleDetails;

            int tableArticleQuantityId = await _databaseService.AddTableArticleQuantity(tableArticleQuantity, _efContext);
            await _databaseService.DeleteTableArticleQuantity(tableArticleQuantity, _efContext);
            TableArticleQuantity tableArticleQuantityFind = await _databaseService.GetTableArticleQuantityByID(tableArticleQuantityId, _efContext);

            //Assert
            Assert.That(tableArticleQuantityFind, Is.Null);
            await _databaseService.DeleteArticle(article, _efContext);
        }


        [Test]
        public async Task DeleteUser()
        {
            //Arrange
            IDatabaseService? _databaseService = _databaseServiceFactory.Create();
            EFContext? _efContext = _efContextFactory.Create();

            User user = new User
            {
                FirstAndLastName = "UnitTest",
                Barcode = 123,
                DateOfBirth = DateTime.UtcNow,
                JMBG = 1,
                UserRole = UserRole.Waiter
            };

            //Act
            int userId = await _databaseService.AddUser(user, _efContext);
            await _databaseService.DeleteUser(user, _efContext);
            User userFind = await _databaseService.GetUserByID(userId, _efContext);

            //Assert
            Assert.That(userFind, Is.Null);
        }

        [Test]
        public async Task EditArticle()
        {
            //Arrange
            IDatabaseService? _databaseService = _databaseServiceFactory.Create();
            EFContext? _efContext = _efContextFactory.Create();

            string articleName = "UnitTestArticle";

            Article article = new Article
            {
                Barcode = 123,
                IsDeleted = false,
                Name = articleName,
                Price = 10
            };

            //Act
            await _databaseService.AddArticle(article, _efContext);
            article.Name = "UnitTestArticleEdited";
            await _databaseService.EditArticle(article, _efContext);

            //Assert
            Assert.That(article.Name, Is.Not.EqualTo(articleName));
            await _databaseService.DeleteArticle(article, _efContext);
        }

        [Test]
        public async Task EditArticleDetails()
        {
            //Arrange
            IDatabaseService? _databaseService = _databaseServiceFactory.Create();
            EFContext? _efContext = _efContextFactory.Create();

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
            int articleId = await _databaseService.AddArticle(article, _efContext);

            articleDetails.ArticleID = articleId;
            await _databaseService.AddArticleDetails(articleDetails, _efContext);

            articleDetails.OriginalQuantity = 15;
            await _databaseService.EditArticleDetails(articleDetails, _efContext);

            //Assert
            Assert.That(articleDetails.OriginalQuantity, Is.Not.EqualTo(originalQuantity));
            await _databaseService.DeleteArticle(article, _efContext);
        }

        [Test]
        public async Task EditOnlineOrder()
        {
            //Arrange
            IDatabaseService? _databaseService = _databaseServiceFactory.Create();
            EFContext? _efContext = _efContextFactory.Create();

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
            int userId = await _databaseService.AddUser(user, _efContext);
            onlineOrder.UserID = userId;

            await _databaseService.AddOnlineOrder(onlineOrder, _efContext);
            onlineOrder.Firstname = "unitTestEdited";
            await _databaseService.EditOnlineOrder(onlineOrder, _efContext);

            //Assert
            Assert.That(onlineOrder.Firstname, Is.Not.EqualTo(firstName));
            await _databaseService.DeleteOnlineOrder(onlineOrder, _efContext);
            await _databaseService.DeleteUser(user, _efContext);
        }

        [Test]
        public async Task EditTable()
        {
            //Arrange
            IDatabaseService? _databaseService = _databaseServiceFactory.Create();
            EFContext? _efContext = _efContextFactory.Create();

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
            await _databaseService.AddTable(table, _efContext);

            table.InUse = true;
            await _databaseService.EditTable(table, _efContext);

            //Assert
            Assert.That(table.InUse, Is.Not.False);
            await _databaseService.DeleteTable(table, _efContext);
        }

        [Test]
        public async Task EditTableArticleQuantity()
        {
            //Arrange
            IDatabaseService? _databaseService = _databaseServiceFactory.Create();
            EFContext? _efContext = _efContextFactory.Create();

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
            int articleId = await _databaseService.AddArticle(article, _efContext);
            articleDetail.ArticleID = articleId;

            int articleDetailsId = await _databaseService.AddArticleDetails(articleDetail, _efContext);

            tableArticleQuantity.ArticleID = articleId;
            tableArticleQuantity.ArticleDetails = articleDetails;

            await _databaseService.AddTableArticleQuantity(tableArticleQuantity, _efContext);
            tableArticleQuantity.Quantity = 50;
            await _databaseService.EditTableArticleQuantity(tableArticleQuantity, _efContext);

            //Assert
            Assert.That(tableArticleQuantity.Quantity, Is.Not.EqualTo(quantity));
            await _databaseService.DeleteArticle(article, _efContext);
        }

        [Test]
        public async Task EditUser()
        {
            //Arrange
            IDatabaseService? _databaseService = _databaseServiceFactory.Create();
            EFContext? _efContext = _efContextFactory.Create();

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
            await _databaseService.AddUser(user, _efContext);
            user.FirstAndLastName = "UnitTestEdited";
            await _databaseService.EditUser(user, _efContext);

            //Assert
            Assert.That(user.FirstAndLastName, Is.Not.EqualTo(firstAndLastname));
            await _databaseService.DeleteUser(user, _efContext);
        }
    }
}