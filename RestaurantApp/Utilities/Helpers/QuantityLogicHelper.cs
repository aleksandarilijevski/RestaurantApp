using EntityFramework.Models;
using RestaurantApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestaurantApp.Utilities.Helpers
{
    public static class QuantityLogicHelper
    {
        public static async Task IncreaseReservedQuantity(ArticleHelperDetails articleHelperDetails)
        {
            List<TableArticleQuantity> tableArticleQuantities = await articleHelperDetails.DatabaseService.GetTableArticleQuantityByArticleID(articleHelperDetails.TableArticleQuantity.ArticleID, articleHelperDetails.EFContext);
            int usedQuantity = tableArticleQuantities.Sum(x => x.Quantity);

            foreach (ArticleDetails articleDetail in articleHelperDetails.ArticleDetails.OrderBy(x => x.CreatedDateTime))
            {
                if (articleHelperDetails.Quantity <= 0)
                {
                    break;
                }

                int availableQuantity = articleDetail.OriginalQuantity - articleDetail.ReservedQuantity;
                int quantityToReserve = Math.Min(availableQuantity, articleHelperDetails.Quantity);

                articleDetail.ReservedQuantity += quantityToReserve;
                articleHelperDetails.Quantity -= quantityToReserve;

                await articleHelperDetails.DatabaseService.EditArticleDetails(articleDetail, articleHelperDetails.EFContext);
            }
        }

        public static async Task DecreaseReservedQuantity(ArticleHelperDetails articleHelperDetails)
        {
            List<TableArticleQuantity> tableArticleQuantities = await articleHelperDetails.DatabaseService.GetTableArticleQuantityByArticleID(articleHelperDetails.TableArticleQuantity.ArticleID, articleHelperDetails.EFContext);
            int usedQuantity = tableArticleQuantities.Sum(x => x.Quantity);

            int quantityToBeRemoved = articleHelperDetails.TableArticleQuantity.Quantity;

            foreach (ArticleDetails articleDetail in articleHelperDetails.ArticleDetails.OrderBy(x => x.CreatedDateTime))
            {
                if (articleDetail.ReservedQuantity != 0)
                {
                    if (articleDetail.OriginalQuantity > articleDetail.ReservedQuantity)
                    {
                        int reservedToBeDeleted = Math.Min(articleDetail.ReservedQuantity, quantityToBeRemoved);
                        articleDetail.ReservedQuantity -= reservedToBeDeleted;
                        quantityToBeRemoved -= reservedToBeDeleted;

                        await articleHelperDetails.DatabaseService.EditArticleDetails(articleDetail, articleHelperDetails.EFContext);

                        if (quantityToBeRemoved != 0)
                        {
                            continue;
                        }
                    }
                    else if (articleDetail.ReservedQuantity == articleDetail.OriginalQuantity)
                    {
                        int reservedToBeDeleted = Math.Min(articleDetail.ReservedQuantity, quantityToBeRemoved);
                        articleDetail.ReservedQuantity -= reservedToBeDeleted;
                        quantityToBeRemoved -= reservedToBeDeleted;

                        await articleHelperDetails.DatabaseService.EditArticleDetails(articleDetail, articleHelperDetails.EFContext);

                        if (quantityToBeRemoved != 0)
                        {
                            continue;
                        }
                    }
                    else if (quantityToBeRemoved == 1)
                    {
                        articleDetail.ReservedQuantity--; ;
                        await articleHelperDetails.DatabaseService.EditArticleDetails(articleDetail, articleHelperDetails.EFContext);
                    }
                    else
                    {
                        continue;
                    }

                    break;
                }
            }
        }

        public static async Task DecreaseQuantityFromCell(ArticleHelperDetails articleHelperDetails)
        {
            List<TableArticleQuantity> tableArticleQuantities = await articleHelperDetails.DatabaseService.GetTableArticleQuantityByArticleID(articleHelperDetails.TableArticleQuantity.ArticleID, articleHelperDetails.EFContext);
            int usedQuantity = tableArticleQuantities.Sum(x => x.Quantity);

            foreach (ArticleDetails articleDetail in articleHelperDetails.ArticleDetails.OrderBy(x => x.CreatedDateTime))
            {
                if (articleDetail.ReservedQuantity != 0)
                {
                    if (articleDetail.OriginalQuantity > articleDetail.ReservedQuantity)
                    {
                        int reservedToBeDeleted = Math.Min(articleDetail.ReservedQuantity, articleHelperDetails.Quantity);
                        articleDetail.ReservedQuantity -= reservedToBeDeleted;
                        articleHelperDetails.Quantity -= reservedToBeDeleted;

                        await articleHelperDetails.DatabaseService.EditArticleDetails(articleDetail, articleHelperDetails.EFContext);

                        if (articleHelperDetails.Quantity != 0)
                        {
                            continue;
                        }
                    }
                    else if (articleDetail.ReservedQuantity == articleDetail.OriginalQuantity)
                    {
                        int reservedToBeDeleted = Math.Min(articleDetail.ReservedQuantity, articleHelperDetails.Quantity);
                        articleDetail.ReservedQuantity -= reservedToBeDeleted;
                        articleHelperDetails.Quantity -= reservedToBeDeleted;

                        await articleHelperDetails.DatabaseService.EditArticleDetails(articleDetail, articleHelperDetails.EFContext);

                        if (articleHelperDetails.Quantity != 0)
                        {
                            continue;
                        }
                    }
                    else if (articleHelperDetails.Quantity == 1)
                    {
                        articleDetail.ReservedQuantity--; ;
                        await articleHelperDetails.DatabaseService.EditArticleDetails(articleDetail, articleHelperDetails.EFContext);
                    }
                    else
                    {
                        continue;
                    }

                    break;
                }
            }
        }

        public static async Task<List<SoldArticleDetails>> DecreaseOriginalQuantity(ArticleHelperDetails articleHelperDetails)
        {
            List<SoldArticleDetails> soldArticleDetails = new List<SoldArticleDetails>();
            List<TableArticleQuantity> tableArticleQuantities = await articleHelperDetails.DatabaseService.GetTableArticleQuantityByArticleID(articleHelperDetails.TableArticleQuantity.ArticleID, articleHelperDetails.EFContext);
            int usedQuantity = tableArticleQuantities.Sum(x => x.Quantity);

            int quantityToBeRemoved = articleHelperDetails.TableArticleQuantity.Quantity;

            foreach (ArticleDetails articleDetail in articleHelperDetails.ArticleDetails.OrderBy(x => x.CreatedDateTime))
            {
                if (articleDetail.OriginalQuantity != 0)
                {
                    if (articleDetail.OriginalQuantity > articleDetail.ReservedQuantity)
                    {
                        int reservedToBeDeleted = Math.Min(articleDetail.OriginalQuantity, quantityToBeRemoved);
                        articleDetail.OriginalQuantity -= reservedToBeDeleted;

                        SoldArticleDetails soldArticleDetail = new SoldArticleDetails
                        {
                            SoldQuantity = reservedToBeDeleted,
                            EntryPrice = articleDetail.EntryPrice,
                            //BillID = articleHelperDetails.BillID
                        };

                        soldArticleDetails.Add(soldArticleDetail);

                        quantityToBeRemoved -= reservedToBeDeleted;

                        await articleHelperDetails.DatabaseService.EditArticleDetails(articleDetail, articleHelperDetails.EFContext);

                        if (quantityToBeRemoved != 0)
                        {
                            continue;
                        }
                    }
                    else if (articleDetail.ReservedQuantity == articleDetail.OriginalQuantity)
                    {
                        int reservedToBeDeleted = Math.Min(articleDetail.ReservedQuantity, quantityToBeRemoved);
                        articleDetail.OriginalQuantity -= reservedToBeDeleted;

                        SoldArticleDetails soldArticleDetail = new SoldArticleDetails
                        {
                            SoldQuantity = reservedToBeDeleted,
                            EntryPrice = articleDetail.EntryPrice,
                            //BillID = articleHelperDetails.BillID
                        };

                        soldArticleDetails.Add(soldArticleDetail);

                        quantityToBeRemoved -= reservedToBeDeleted;

                        await articleHelperDetails.DatabaseService.EditArticleDetails(articleDetail, articleHelperDetails.EFContext);

                        if (quantityToBeRemoved != 0)
                        {
                            continue;
                        }
                    }
                    else if (quantityToBeRemoved == 1)
                    {
                        articleDetail.OriginalQuantity--;

                        SoldArticleDetails soldArticleDetail = new SoldArticleDetails
                        {
                            SoldQuantity = articleDetail.OriginalQuantity,
                            EntryPrice = articleDetail.EntryPrice,
                            //BillID = articleHelperDetails.BillID
                        };

                        soldArticleDetails.Add(soldArticleDetail);
                        await articleHelperDetails.DatabaseService.EditArticleDetails(articleDetail, articleHelperDetails.EFContext);
                    }
                    else
                    {
                        continue;
                    }

                    break;
                }
            }
            return soldArticleDetails;
        }

        public static int GetAvailableQuantity(List<ArticleDetails> articleDetails)
        {
            int quantity = 0;

            if (articleDetails != null)
            {
                quantity = articleDetails.Sum(x => x.OriginalQuantity - x.ReservedQuantity);
            }

            return quantity;
        }
    }
}
