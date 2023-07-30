using EntityFramework.Models;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantApp.ViewModels
{
    public class PaymentViewModel : BindableBase, INavigationAware
    {
        private decimal _totalPrice;
        private Table _table;
        private DelegateCommand _issueBillCommand;
        private DelegateCommand _getTotalPriceCommand;

        public PaymentViewModel()
        {

        }

        public decimal TotalPrice
        {
            get
            {
                return _totalPrice;
            }

            set
            {
                _totalPrice = value;
                RaisePropertyChanged();
            }
        }

        public DelegateCommand GetTotalPriceCommand
        {
            get
            {
               _getTotalPriceCommand = new DelegateCommand(GetTotalPrice);
                return _getTotalPriceCommand;
            }
        }

        public DelegateCommand IssueBillCommand
        {
            get
            {
                _issueBillCommand = new DelegateCommand(IssueBill);
                return _issueBillCommand;
            }
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            _table = (Table)navigationContext.Parameters["table"];
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return false;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
        }

        private async void IssueBill()
        {
            decimal totalPrice = CalculateTotalPrice();

            PdfDocument document = new PdfDocument();
            PdfPage page = document.AddPage();
            page.Width = 500;
            //page.Height = 760;
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

            foreach (Article article in _table.Articles)
            {
                if (article.Name.Length > 15)
                {
                    gfx.DrawString(article.Name, font, XBrushes.Black, new XRect(15, offset, page.Width, 0));
                    offset += 20;
                }
                else
                {
                    gfx.DrawString(article.Name, font, XBrushes.Black, new XRect(15, offset, page.Width, 0));
                }

                //gfx.DrawString($"{article.Price}".PadLeft(28) + $"{article.Quantity}".PadLeft(18) + $"{article.Price * article.Quantity}".PadLeft(28), font, XBrushes.Black, new XRect(15, offset, page.Width, 0));
                offset += 20;
            }

            offset += 5;
            gfx.DrawString("----------------------------------------------------------------------", font, XBrushes.Black, new XRect(15, offset, page.Width, 0));

            offset += 15;
            gfx.DrawString($"Ukupan iznos :", font, XBrushes.Black, new XRect(15, offset, page.Width, 0));
            gfx.DrawString($"{totalPrice}".PadLeft(78), font, XBrushes.Black, new XRect(15, offset, page.Width, 0));

            offset += 20;
            gfx.DrawString("Gotovina :", font, XBrushes.Black, new XRect(15, offset, page.Width, 0));

            offset += 20;
            gfx.DrawString("Povracaj :", font, XBrushes.Black, new XRect(15, offset, page.Width, 0));

            offset += 20;
            gfx.DrawString("----------------------------------------------------------------------", font, XBrushes.Black, new XRect(15, offset, page.Width, 0));

            offset += 5;
            gfx.DrawString("----------------------------------------------------------------------", font, XBrushes.Black, new XRect(15, offset, page.Width, 0));

            offset += 15;
            gfx.DrawString("Oznaka              Ime         Stopa                            Porez", font, XBrushes.Black, new XRect(15, offset, page.Width, 0));
            
            offset += 15;
            gfx.DrawString("----------------------------------------------------------------------", font, XBrushes.Black, new XRect(15, offset, page.Width, 0));
            
            offset += 15;
            gfx.DrawString("Ukupan iznos poreza:", font, XBrushes.Black, new XRect(15, offset, page.Width, 0));

            offset += 15;
            gfx.DrawString("----------------------------------------------------------------------", font, XBrushes.Black, new XRect(15, offset, page.Width, 0));

            offset += 5;
            gfx.DrawString("----------------------------------------------------------------------", font, XBrushes.Black, new XRect(15, offset, page.Width, 0));
            
            offset += 15;
            gfx.DrawString("PFR Vreme:", font, XBrushes.Black, new XRect(15, offset, page.Width, 0));

            offset += 20;
            gfx.DrawString("PFR br.rac:", font, XBrushes.Black, new XRect(15, offset, page.Width, 0));

            offset += 20;
            gfx.DrawString("Brojac racuna:", font, XBrushes.Black, new XRect(15, offset, page.Width, 0));

            offset += 15;
            gfx.DrawString("----------------------------------------------------------------------", font, XBrushes.Black, new XRect(15, offset, page.Width, 0));

            offset += 5;
            gfx.DrawString("----------------------------------------------------------------------", font, XBrushes.Black, new XRect(15, offset, page.Width, 0));

            XImage image = await GetCustomQRCode("http://google.com/");

            offset += 10;
            gfx.DrawImage(image, 150, offset, 200, 200);

            offset += 230;
            gfx.DrawString("============KRAJ FISKALNOG RACUNA============", font, XBrushes.Black, new XRect(10, offset, page.Width, 0));

            offset += 20;
            gfx.DrawString("HVALA NA POSETI".PadLeft(48), font, XBrushes.Black, new XRect(10, offset, page.Width, 0)); ;

            offset += 15;
            gfx.DrawString("----------------------------------------------------------------------", font, XBrushes.Black, new XRect(15, offset, page.Width, 0));

            document.Save("C:\\Users\\ilije\\OneDrive\\Desktop\\invoice.pdf");
            document.Close();
        }

        private void GetTotalPrice()
        {
            _totalPrice = CalculateTotalPrice();
            RaisePropertyChanged(nameof(TotalPrice));
        }

        private decimal CalculateTotalPrice()
        {
            decimal totalPrice = 0;

            foreach (Article article in _table.Articles)
            {
               // totalPrice += article.Price * article.Quantity;
            }

            return totalPrice;
        }

        private async Task<XImage> GetCustomQRCode(string text)
        {
            string url = "https://api.qrserver.com/v1/create-qr-code/?size=150x150&data=" + text;

            HttpClient client = new HttpClient();
            Stream stream = await client.GetStreamAsync(url);

            XImage image = XImage.FromStream(stream);
            return image;
        }
    }
}
