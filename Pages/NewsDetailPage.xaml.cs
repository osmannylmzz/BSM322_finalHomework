using System;
using System.Threading.Tasks;
using EntityLayer.Models;

namespace finalHomework.Pages
{
    public partial class NewsDetailPage : ContentPage
    {
        private readonly NewsItem _newsItem;

        public NewsDetailPage(NewsItem newsItem)
        {
            InitializeComponent();
            _newsItem = newsItem;

            // Üst alan dolsun (boşluk + başlık)
            CategoryLabel.Text = _newsItem.Category ?? "";
            DateLabel.Text = _newsItem.PubDate ?? "";
            TitleLabel.Text = _newsItem.Title ?? "Haber";

            // WebView: haber sayfası
            ArticleWebView.Source = _newsItem.Link;
        }

        private void OnWebNavigating(object sender, WebNavigatingEventArgs e)
        {
            LoadingOverlay.IsVisible = true;
        }

        private async void OnWebNavigated(object sender, WebNavigatedEventArgs e)
        {
            LoadingOverlay.IsVisible = false;

            if (e.Result != WebNavigationResult.Success)
                return;

           
        }


        private async void OnShareClicked(object sender, EventArgs e)
        {
            try
            {
                await Share.Default.RequestAsync(new ShareTextRequest
                {
                    Title = _newsItem.Title,
                    Text = _newsItem.Title ?? "Haber",
                    Uri = _newsItem.Link
                });
            }
            catch (Exception ex)
            {
                await DisplayAlert("Hata", $"Paylaşım yapılamadı:\n{ex.Message}", "Tamam");
            }
        }

        private async void OnOpenLinkClicked(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(_newsItem.Link))
                    await Browser.Default.OpenAsync(_newsItem.Link, BrowserLaunchMode.SystemPreferred);
            }
            catch (Exception ex)
            {
                await DisplayAlert("Hata", $"Link açılamadı:\n{ex.Message}", "Tamam");
            }
        }
    }
}
