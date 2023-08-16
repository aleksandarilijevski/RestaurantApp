using EntityFramework.Models;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using RestaurantApp.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace RestaurantApp.Utilities.Helpers
{
    public static class DrawningHelper
    {
        public static void DrawFakeBill(decimal totalPrice, decimal cash, decimal change, List<TableArticleQuantity> tableArticleQuantities)
        {
            PdfDocument pdfDocument = new PdfDocument();
            PdfPage pdfPage = pdfDocument.AddPage();

            pdfPage.Width = 500;
            pdfPage.Height = 0;

            XGraphics gfx = XGraphics.FromPdfPage(pdfPage);

            XPen pen = new XPen(XColors.Black, 1);
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            XFont font = new XFont("Verdana", 15, XFontStyle.Regular);

            int offset = 20;

            gfx.DrawString("PORUDZBINA", font, XBrushes.Black, new XRect(15, offset, pdfPage.Width, 0));
            offset += 20;

            gfx.DrawString(DateTime.Now.ToString("dd/MM/yyyy"), font, XBrushes.Black, new XRect(15, offset, pdfPage.Width, 0));
            gfx.DrawString("-".PadLeft(45) + DateTime.Now.ToString("hh:mm").PadLeft(35), font, XBrushes.Black, new XRect(15, offset, pdfPage.Width, 0));
            offset += 20;

            gfx.DrawString("-----------------------------------------------------------------------", font, XBrushes.Black, new XRect(15, offset, pdfPage.Width, 0));
            offset += 20;

            foreach (TableArticleQuantity tableArticleQuantity in tableArticleQuantities)
            {
                if (tableArticleQuantity.Article.Name.Length > 15)
                {
                    gfx.DrawString(tableArticleQuantity.Article.Name, font, XBrushes.Black, new XRect(15, offset, pdfPage.Width, 0));
                    offset += 20;
                }
                else
                {
                    gfx.DrawString(tableArticleQuantity.Article.Name, font, XBrushes.Black, new XRect(15, offset, pdfPage.Width, 0));
                }

                gfx.DrawString($"{tableArticleQuantity.Article.Price}".PadLeft(28) + $"{tableArticleQuantity.Quantity}".PadLeft(18) + $"{tableArticleQuantity.Article.Price * tableArticleQuantity.Quantity}".PadLeft(28), font, XBrushes.Black, new XRect(15, offset, pdfPage.Width, 0));
                offset += 20;
            }

            gfx.DrawString("-----------------------------------------------------------------------", font, XBrushes.Black, new XRect(15, offset, pdfPage.Width, 0));
            offset += 20;

            gfx.DrawString("ZA UPLATU:" + totalPrice.ToString().PadLeft(62), font, XBrushes.Black, new XRect(15, offset, pdfPage.Width, 0));
            offset += 20;

            gfx.DrawString("-----------------------------------------------------------------------", font, XBrushes.Black, new XRect(15, offset, pdfPage.Width, 0));
            offset += 20;

            gfx.DrawString("OVO JE PORUDZBINA", font, XBrushes.Black, new XRect(15, offset, pdfPage.Width, 0));
            offset += 40;

            gfx.DrawString("SACEKAJTE VAS", font, XBrushes.Black, new XRect(15, offset, pdfPage.Width, 0));
            offset += 20;

            gfx.DrawString("FISKALNI RACUN", font, XBrushes.Black, new XRect(15, offset, pdfPage.Width, 0));
            offset += 20;

            pdfPage.Height -= offset;

            string path = "C:\\Users\\" + Environment.UserName + "\\Desktop\\fakeInvoice.pdf";

            pdfDocument.Save(path);
            pdfDocument.Close();
        }

        public static void DrawBill(decimal totalPrice, decimal cash, decimal change, List<TableArticleQuantity> tableArticleQuantities, XImage xImage, PaymentType paymentType)
        {
            PdfDocument document = new PdfDocument();
            PdfPage page = document.AddPage();

            page.Width = 500;
            page.Height = 0;

            XGraphics gfx = XGraphics.FromPdfPage(page);

            XPen pen = new XPen(XColors.Black, 1);
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            XFont font = new XFont("Verdana", 15, XFontStyle.Regular);

            int offset = 0;

            gfx.DrawString("===============FISKALNI RACUN===============", font, XBrushes.Black, new XRect(0, offset, page.Width, page.Height), XStringFormats.TopCenter);
            offset += 20;
            gfx.DrawString("123456789", font, XBrushes.Black, new XRect(0, offset, page.Width, 0), XStringFormats.TopCenter);
            offset += 20;
            gfx.DrawString("VP DIMA TOPOLA", font, XBrushes.Black, new XRect(0, offset, page.Width, 0), XStringFormats.TopCenter);
            offset += 20;
            gfx.DrawString("Mije Todorovica 76", font, XBrushes.Black, new XRect(0, offset, page.Width, 0), XStringFormats.TopCenter);
            offset += 50;
            gfx.DrawString("Kasir :", font, XBrushes.Black, new XRect(15, offset, page.Width, 0));
            offset += 20;
            gfx.DrawString("ESIR Broj :", font, XBrushes.Black, new XRect(15, offset, page.Width, 0));
            offset += 20;
            gfx.DrawString("==============PROMET PRODAJA==============", font, XBrushes.Black, new XRect(10, offset, page.Width, 0));
            offset += 20;
            gfx.DrawString("Artikli", font, XBrushes.Black, new XRect(15, offset, page.Width, 0));
            offset += 20;
            gfx.DrawString("----------------------------------------------------------------------", font, XBrushes.Black, new XRect(15, offset, page.Width, 0));
            offset += 5;
            gfx.DrawString("----------------------------------------------------------------------", font, XBrushes.Black, new XRect(15, offset, page.Width, 0));
            offset += 20;
            gfx.DrawString("Naziv                Cena                 Kol                   Ukupno", font, XBrushes.Black, new XRect(15, offset, page.Width, 0));
            offset += 20;

            foreach (TableArticleQuantity tableArticleQuantity in tableArticleQuantities)
            {
                if (tableArticleQuantity.Article.Name.Length > 15)
                {
                    gfx.DrawString(tableArticleQuantity.Article.Name, font, XBrushes.Black, new XRect(15, offset, page.Width, 0));
                    offset += 20;
                }
                else
                {
                    gfx.DrawString(tableArticleQuantity.Article.Name, font, XBrushes.Black, new XRect(15, offset, page.Width, 0));
                }

                gfx.DrawString($"{tableArticleQuantity.Article.Price}".PadLeft(28) + $"{tableArticleQuantity.Quantity}".PadLeft(18) + $"{tableArticleQuantity.Article.Price * tableArticleQuantity.Quantity}".PadLeft(28), font, XBrushes.Black, new XRect(15, offset, page.Width, 0));
                offset += 20;
            }

            offset += 5;
            gfx.DrawString("----------------------------------------------------------------------", font, XBrushes.Black, new XRect(15, offset, page.Width, 0));

            offset += 15;
            gfx.DrawString($"Ukupan iznos :", font, XBrushes.Black, new XRect(15, offset, page.Width, 0));
            gfx.DrawString($"{totalPrice}".PadLeft(78), font, XBrushes.Black, new XRect(15, offset, page.Width, 0));

            if (paymentType == PaymentType.Cash)
            {
                offset += 20;
                gfx.DrawString($"Gotovina :                                                         {cash.ToString("0.00")}", font, XBrushes.Black, new XRect(15, offset, page.Width, 0));

                offset += 20;
                gfx.DrawString($"Povracaj :                                                          {change}", font, XBrushes.Black, new XRect(15, offset, page.Width, 0));
            }

            offset += 20;
            gfx.DrawString("----------------------------------------------------------------------", font, XBrushes.Black, new XRect(15, offset, page.Width, 0));

            offset += 5;
            gfx.DrawString("----------------------------------------------------------------------", font, XBrushes.Black, new XRect(15, offset, page.Width, 0));

            offset += 15;
            gfx.DrawString("Oznaka              Ime         Stopa                            Porez", font, XBrushes.Black, new XRect(15, offset, page.Width, 0));

            double pdv = (double)totalPrice * 0.20;
            offset += 15;
            gfx.DrawString($"DJ                  0-PDV        20.00%                           {pdv.ToString("0.00")}", font, XBrushes.Black, new XRect(15, offset, page.Width, 0));

            offset += 15;
            gfx.DrawString("----------------------------------------------------------------------", font, XBrushes.Black, new XRect(15, offset, page.Width, 0));

            offset += 15;
            gfx.DrawString($"Ukupan iznos poreza:                                              {pdv.ToString("0.00")}", font, XBrushes.Black, new XRect(15, offset, page.Width, 0));

            offset += 15;
            gfx.DrawString("----------------------------------------------------------------------", font, XBrushes.Black, new XRect(15, offset, page.Width, 0));

            offset += 5;
            gfx.DrawString("----------------------------------------------------------------------", font, XBrushes.Black, new XRect(15, offset, page.Width, 0));

            offset += 15;
            gfx.DrawString("PFR Vreme:", font, XBrushes.Black, new XRect(15, offset, page.Width, 0));
            gfx.DrawString(DateTime.Now.ToString("dd/MM/yyyy  HH:mm:ss").PadLeft(70), font, XBrushes.Black, new XRect(15, offset, page.Width, 0));

            offset += 20;
            gfx.DrawString("PFR br.rac:", font, XBrushes.Black, new XRect(15, offset, page.Width, 0));

            offset += 20;
            gfx.DrawString("Brojac racuna:", font, XBrushes.Black, new XRect(15, offset, page.Width, 0));

            offset += 15;
            gfx.DrawString("----------------------------------------------------------------------", font, XBrushes.Black, new XRect(15, offset, page.Width, 0));

            offset += 5;
            gfx.DrawString("----------------------------------------------------------------------", font, XBrushes.Black, new XRect(15, offset, page.Width, 0));

            offset += 10;
            gfx.DrawImage(xImage, 150, offset, 200, 200);

            offset += 230;
            gfx.DrawString("============KRAJ FISKALNOG RACUNA============", font, XBrushes.Black, new XRect(10, offset, page.Width, 0));

            offset += 20;
            gfx.DrawString("HVALA NA POSETI".PadLeft(48), font, XBrushes.Black, new XRect(10, offset, page.Width, 0)); ;

            offset += 15;
            gfx.DrawString("----------------------------------------------------------------------", font, XBrushes.Black, new XRect(15, offset, page.Width, 0));

            page.Height -= offset;

            string path = "C:\\Users\\" + Environment.UserName + "\\Desktop\\invoice.pdf";

            document.Save(path);
            document.Close();
        }
    }
}
