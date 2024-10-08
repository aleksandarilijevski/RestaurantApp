﻿using EntityFramework.Models;
using Microsoft.EntityFrameworkCore;
using RestaurantApp.Services.Interface;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace RestaurantApp.Services
{
    public class DatabaseService : IDatabaseService
    {
        public async Task<ObservableCollection<Article>> GetAllArticles(EFContext efContext)
        {
            List<Article> articles = await efContext.Articles.Include(x => x.ArticleDetails).Where(x => x.IsDeleted == false).ToListAsync();
            return new ObservableCollection<Article>(articles);
        }

        public async Task<List<TableArticleQuantity>> GetTableArticleQuantityByArticleID(int articleId, EFContext efContext)
        {
            List<TableArticleQuantity> tableArticleQuantities = await efContext.TableArticleQuantities.Select(x => x).Include(x => x.ArticleDetails).Where(x => x.ArticleID == articleId).ToListAsync();
            return tableArticleQuantities;
        }

        public async Task<Article> GetArticleByID(int id, EFContext efContext)
        {
            Article article = await efContext.Articles.Include(x => x.ArticleDetails).FirstOrDefaultAsync(x => x.ID == id);
            return article;
        }

        public async Task<int> AddOnlineOrder(OnlineOrder onlineOrder, EFContext efContext)
        {
            efContext.OnlineOrders.Add(onlineOrder);
            await efContext.SaveChangesAsync();
            return onlineOrder.ID;
        }

        public async Task<TableArticleQuantity> GetTableArticleQuantityByID(int id, EFContext efContext)
        {
            TableArticleQuantity tableArticleQuantity = await efContext.TableArticleQuantities.FirstOrDefaultAsync(x => x.ID == id);
            return tableArticleQuantity;
        }

        public async Task<Article> GetArticleByBarcode(long barcode, EFContext efContext)
        {
            Article article = await efContext.Articles.FirstOrDefaultAsync(x => x.Barcode == barcode && x.IsDeleted == false);
            return article;
        }

        public async Task<int> AddArticle(Article article, EFContext efContext)
        {
            efContext.Articles.Add(article);
            await efContext.SaveChangesAsync();
            return article.ID;
        }

        public async Task EditArticle(Article article, EFContext efContext)
        {
            efContext.Entry(article).State = EntityState.Modified;
            await efContext.SaveChangesAsync();
        }

        public async Task<ObservableCollection<User>> GetAllUsers(EFContext efContext)
        {
            List<User> users = await efContext.Users.Select(x => x).Where(x => x.IsDeleted == false).ToListAsync();
            return new ObservableCollection<User>(users);
        }

        public async Task<ObservableCollection<OnlineOrder>> GetAllOnlineOrders(EFContext efContext)
        {
            List<OnlineOrder> onlineOrders = await efContext.OnlineOrders.Include(x => x.TableArticleQuantities).ThenInclude(x => x.ArticleDetails).ThenInclude(x => x.Article).Include(x => x.User).ToListAsync();
            return new ObservableCollection<OnlineOrder>(onlineOrders);
        }

        public async Task<int> AddUser(User user, EFContext efContext)
        {
            efContext.Users.Add(user);
            await efContext.SaveChangesAsync();
            return user.ID;
        }

        public async Task EditUser(User user, EFContext efContext)
        {
            efContext.Entry(user).State = EntityState.Modified;
            await efContext.SaveChangesAsync();
        }

        public async Task DeleteUser(User user, EFContext efContext)
        {
            efContext.Users.Remove(user);
            await efContext.SaveChangesAsync();
        }

        public async Task<Table> GetTableByID(int id, EFContext efContext)
        {
            Table table = await efContext.Tables.Include(x => x.TableArticleQuantities).ThenInclude(x => x.ArticleDetails).ThenInclude(x => x.Article).Include(x => x.User).FirstOrDefaultAsync(x => x.ID == id);
            return table;
        }

        public async Task<int> AddTable(Table table, EFContext efContext)
        {
            efContext.Tables.Add(table);
            await efContext.SaveChangesAsync();
            return table.ID;
        }

        public async Task EditTable(Table table, EFContext efContext)
        {
            efContext.Entry(table).State = EntityState.Modified;
            await efContext.SaveChangesAsync();
        }

        public async Task EditOnlineOrder(OnlineOrder onlineOrder, EFContext efContext)
        {
            efContext.Entry(onlineOrder).State = EntityState.Modified;
            await efContext.SaveChangesAsync();
        }

        public async Task<List<Table>> GetAllTables(EFContext efContext)
        {
            List<Table> tables = await efContext.Tables.ToListAsync();
            return tables;
        }

        public async Task<List<ArticleDetails>> GetArticleDetailsByArticleID(int articleId, EFContext efContext)
        {
            List<ArticleDetails> articleDetails = await efContext.ArticleDetails.Where(x => x.ArticleID == articleId).ToListAsync();
            return articleDetails;
        }

        public async Task<int> AddArticleDetails(ArticleDetails articleDetails, EFContext efContext)
        {
            efContext.Attach(articleDetails);
            efContext.ArticleDetails.Add(articleDetails);
            await efContext.SaveChangesAsync();
            return articleDetails.ID;
        }

        public async Task EditArticleDetails(ArticleDetails articleDetails, EFContext efContext)
        {
            efContext.Entry(articleDetails).State = EntityState.Modified;
            await efContext.SaveChangesAsync();
        }

        public async Task<int> AddDataEntry(DataEntry dataEntry, EFContext efContext)
        {
            efContext.DataEntries.Add(dataEntry);
            await efContext.SaveChangesAsync();
            return dataEntry.ID;
        }

        public async Task<DataEntry> GetDataEntryByNumber(string dataEntryNumber, EFContext efContext)
        {
            DataEntry dataEntry = await efContext.DataEntries.FirstOrDefaultAsync(x => x.DataEntryNumber == dataEntryNumber);
            return dataEntry;
        }

        public async Task DeleteArticleDetails(ArticleDetails articleDetails, EFContext efContext)
        {
            efContext.ArticleDetails.Remove(articleDetails);
            await efContext.SaveChangesAsync();
        }

        public async Task<int> AddTableArticleQuantity(TableArticleQuantity tableArticleQuantity, EFContext efContext)
        {
            efContext.TableArticleQuantities.Add(tableArticleQuantity);
            await efContext.SaveChangesAsync();
            return tableArticleQuantity.ID;
        }

        public async Task EditTableArticleQuantity(TableArticleQuantity tableArticleQuantity, EFContext efContext)
        {
            efContext.Entry(tableArticleQuantity).State = EntityState.Modified;
            await efContext.SaveChangesAsync();
        }

        public async Task DeleteTableArticleQuantity(TableArticleQuantity tableArticleQuantity, EFContext efContext)
        {
            efContext.TableArticleQuantities.Remove(tableArticleQuantity);
            await efContext.SaveChangesAsync();
        }

        public async Task<int> CreateBill(Bill bill, EFContext efContext)
        {
            efContext.Bills.Add(bill);
            await efContext.SaveChangesAsync();
            return bill.ID;
        }

        public async Task<int> CreateConfiguration(Configuration configuration, EFContext efContext)
        {
            efContext.Configurations.Add(configuration);
            await efContext.SaveChangesAsync();
            return configuration.ID;
        }

        public async Task<Configuration> GetConfiguration()
        {
            using EFContext efContext = new EFContext();
            Configuration configuration = await efContext.Configurations.FirstOrDefaultAsync();
            return configuration;
        }

        public async Task EditConfiguration(Configuration configuration, EFContext efContext)
        {
            efContext.Entry(configuration).State = EntityState.Modified;
            await efContext.SaveChangesAsync();
        }

        public async Task<List<Bill>> GetAllBills()
        {
            using EFContext efContext = new EFContext();

            List<Bill> bills = await efContext.Bills.Select(x => x)
                .Include(x => x.Table)
                .ThenInclude(x => x.TableArticleQuantities)
                .ThenInclude(x => x.Article)

                .Include(x => x.Table)
                .ThenInclude(x => x.TableArticleQuantities)
                .ThenInclude(x => x.Bill)

                .Include(x => x.Table)
                .ThenInclude(x => x.TableArticleQuantities)
                .ThenInclude(x => x.ArticleDetails)
                .ThenInclude(x => x.Article)

                .Include(x => x.OnlineOrder)
                .ThenInclude(x => x.TableArticleQuantities)
                .ThenInclude(x => x.ArticleDetails)
                .ThenInclude(x => x.Article)
                .ToListAsync();
            return bills;
        }

        public async Task<ObservableCollection<DataEntry>> GetAllDataEntries(EFContext efContext)
        {
            List<DataEntry> dataEntries = await efContext.DataEntries.Include(x => x.ArticleDetails).ThenInclude(x => x.Article).ToListAsync();
            return new ObservableCollection<DataEntry>(dataEntries);
        }

        public async Task<int> AddSoldArticleDetails(SoldArticleDetails soldArticleDetails, EFContext efContext)
        {
            efContext.SoldArticleDetails.Add(soldArticleDetails);
            await efContext.SaveChangesAsync();
            return soldArticleDetails.ID;
        }

        public async Task<List<SoldArticleDetails>> GetSoldArticleDetailsByBillID(int billID, EFContext efContext)
        {
            List<SoldArticleDetails> soldArticleDetails = await efContext.SoldArticleDetails.Select(x => x).Where(x => x.BillID == billID).ToListAsync();
            return soldArticleDetails;
        }

        public async Task<List<SoldArticleDetails>> GetAllSoldArticleDetails()
        {
            using EFContext efContext = new EFContext();
            List<SoldArticleDetails> soldArticleDetails = await efContext.SoldArticleDetails.ToListAsync();
            return soldArticleDetails;
        }

        public async Task<User> GetUserByBarcode(long barcode, EFContext efContext)
        {
            User user = await efContext.Users.FirstOrDefaultAsync(x => x.Barcode == barcode);
            return user;
        }

        public async Task<User> GetUserByID(int id, EFContext efContext)
        {
            User user = await efContext.Users.FirstOrDefaultAsync(x => x.ID == id);
            return user;
        }

        public async Task DeleteArticle(Article article, EFContext efContext)
        {
            efContext.Articles.Remove(article);
            await efContext.SaveChangesAsync();
        }

        public async Task<ArticleDetails> GetArticleDetailsByID(int id, EFContext efContext)
        {
            ArticleDetails articleDetails = await efContext.ArticleDetails.FirstOrDefaultAsync(x => x.ID == id);
            return articleDetails;
        }

        public async Task DeleteTable(Table table, EFContext efContext)
        {
            efContext.Tables.Remove(table);
            await efContext.SaveChangesAsync();
        }

        public async Task<Bill> GetBillByID(int id, EFContext efContext)
        {
            Bill bill = await efContext.Bills.FirstOrDefaultAsync(x => x.ID == id);
            return bill;
        }

        public async Task DeleteBill(Bill bill, EFContext efContext)
        {
            efContext.Bills.Remove(bill);
            await efContext.SaveChangesAsync();
        }

        public async Task<DataEntry> GetDataEntryByID(int id, EFContext efContext)
        {
            DataEntry dataEntry = await efContext.DataEntries.FirstOrDefaultAsync(x => x.ID == id);
            return dataEntry;
        }

        public async Task DeleteDataEntry(DataEntry dataEntry, EFContext efContext)
        {
            efContext.DataEntries.Remove(dataEntry);
            await efContext.SaveChangesAsync();
        }

        public async Task<OnlineOrder> GetOnlineOrderByID(int id, EFContext efContext)
        {
            OnlineOrder onlineOrder = await efContext.OnlineOrders.FirstOrDefaultAsync(x => x.ID == id);
            return onlineOrder;
        }

        public async Task DeleteOnlineOrder(OnlineOrder onlineOrder, EFContext efContext)
        {
            efContext.OnlineOrders.Remove(onlineOrder);
            await efContext.SaveChangesAsync();
        }

        public async Task<SoldArticleDetails> GetSoldArticleDetailsByID(int id, EFContext efContext)
        {
            SoldArticleDetails soldArticleDetails = await efContext.SoldArticleDetails.FirstOrDefaultAsync(x => x.ID == id);
            return soldArticleDetails;
        }

        public async Task DeleteSoldArticleDetails(SoldArticleDetails soldArticleDetails, EFContext efContext)
        {
            efContext.SoldArticleDetails.Remove(soldArticleDetails);
            await efContext.SaveChangesAsync();
        }

        public async Task<User> GetUserByJMBG(long jmbg, EFContext efContext)
        {
            User user = await efContext.Users.FirstOrDefaultAsync(x => x.JMBG == jmbg && x.IsDeleted == false);
            return user;
        }
    }
}
