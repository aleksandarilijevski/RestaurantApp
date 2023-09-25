using EntityFramework.Enums;
using EntityFramework.Models;
using RestaurantApp.Enums;
using RestaurantApp.Services;
using RestaurantApp.Services.Interface;
using Table = EntityFramework.Models.Table;

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

            int articleDetailsId = await _databaseService.AddArticleDetails(articleDetail, _efContext);

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
        public async Task EditBill()
        {
            //Arrange
            string registrationNumber = "unitTest";

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
                RegistrationNumber = registrationNumber,
                PaymentType = PaymentType.Cash
            };

            //Act
            int userId = await _databaseService.AddUser(user, _efContext);
            bill.UserID = userId;

            await _databaseService.CreateBill(bill, _efContext);
            bill.RegistrationNumber = "unitTestEdited";
            await _databaseService.EditBill(bill, _efContext);

            //Assert
            Assert.That(bill.RegistrationNumber, Is.Not.EqualTo(registrationNumber));
            await _databaseService.DeleteBill(bill, _efContext);
            await _databaseService.DeleteUser(user, _efContext);
        }

        [Test]
        public async Task EditDataEntry()
        {
            //Arrange
            int dataEntryNumber = 1;

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
                DataEntryNumber = dataEntryNumber,
                TotalAmount = 100
            };

            //Act
            int articleId = await _databaseService.AddArticle(article, _efContext);
            articleDetail.ArticleID = articleId;
            await _databaseService.AddArticleDetails(articleDetail, _efContext);

            dataEntry.ArticleDetails = articleDetails;
            await _databaseService.AddDataEntry(dataEntry, _efContext);
            dataEntry.DataEntryNumber = 5;
            await _databaseService.EditDataEntry(dataEntry, _efContext);

            //Assert
            Assert.That(dataEntry.DataEntryNumber, Is.Not.EqualTo(dataEntryNumber));
            await _databaseService.DeleteDataEntry(dataEntry, _efContext);
        }

        [Test]
        public async Task EditOnlineOrder()
        {
            //Arrange
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
        public async Task EditSoldArticleDetails()
        {
            //Arrange
            decimal entryPrice = 5;

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
                EntryPrice = entryPrice,
                SoldQuantity = 5,
            };

            //Act
            int userId = await _databaseService.AddUser(user, _efContext);
            bill.UserID = userId;

            int billId = await _databaseService.CreateBill(bill, _efContext);
            soldArticleDetails.BillID = billId;

            await _databaseService.AddSoldArticleDetails(soldArticleDetails, _efContext);
            soldArticleDetails.EntryPrice = 10;
            await _databaseService.EditSoldArticleDetails(soldArticleDetails, _efContext);

            //Assert
            Assert.That(soldArticleDetails.EntryPrice, Is.Not.EqualTo(entryPrice));
            await _databaseService.DeleteUser(user, _efContext);
        }
    }
}