using PdfSharp.Drawing;
using PdfSharp.Pdf;
using Prism.Mvvm;

namespace RestaurantApp.ViewModels
{
    public class BillViewModelTest : BindableBase
    {

        private void CreateReceipt()
        {
            PdfDocument document = new PdfDocument();
            PdfPage page = document.AddPage();
            XGraphics gfx = XGraphics.FromPdfPage(page);
            XFont font = new XFont("Arial", 15, XFontStyle.Bold);

            gfx.DrawString("Test!", font, XBrushes.Black, new XRect(0, 0, page.Width, page.Height), XStringFormats.Center);

            document.Save("C:\\Users\\XANDRO\\Desktop\\invoice.pdf");
        }
    }
}
