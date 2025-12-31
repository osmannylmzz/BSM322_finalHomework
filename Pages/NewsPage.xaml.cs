    using System;
using System.Collections.ObjectModel;
using BusinessLayer.News;
using EntityLayer.Models;

namespace finalHomework.Pages
{
    public partial class NewsPage : ContentPage
    {
        private readonly ObservableCollection<NewsItem> _news = new();
        private readonly List<NewsCategory> _categories;

        public NewsPage()
        {
            InitializeComponent();

            // Kategorileri yükle
            _categories = NewsBL.GetCategories();

            // Picker'a kategorileri ekle
            foreach (var cat in _categories)
            {
                CategoryPicker.Items.Add(cat.Name);
            }

            NewsCollectionView.ItemsSource = _news;

            // Ýlk kategoriyi seç
            if (_categories.Count > 0)
            {
                CategoryPicker.SelectedIndex = 0;
            }
        }

        private async void OnCategoryChanged(object sender, EventArgs e)
        {
            if (CategoryPicker.SelectedIndex < 0)
                return;

            var selectedCategory = _categories[CategoryPicker.SelectedIndex];
            await LoadNewsAsync(selectedCategory);
        }

        private async Task LoadNewsAsync(NewsCategory category)
        {
            try
            {
                LoadingOverlay.IsVisible = true;
                _news.Clear();

                var items = await NewsBL.GetNewsAsync(category.RssUrl, category.Name);

                foreach (var item in items)
                {
                    _news.Add(item);
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Hata", $"Haberler yüklenirken hata oluþtu:\n{ex.Message}", "Tamam");
            }
            finally
            {
                LoadingOverlay.IsVisible = false;
            }
        }

        private async void OnNewsTapped(object sender, TappedEventArgs e)
        {
            try
            {
                if (sender is not Frame frame)
                    return;

                if (frame.BindingContext is not NewsItem newsItem)
                    return;

                // Detay sayfasýna git
                await Navigation.PushAsync(new NewsDetailPage(newsItem));
            }
            catch (Exception ex)
            {
                await DisplayAlert("Hata", ex.Message, "Tamam");
            }
        }
    }
}