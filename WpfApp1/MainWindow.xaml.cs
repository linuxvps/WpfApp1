using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using System.IO;

namespace WpfApp1
{
    public partial class MainWindow : Window
    {
        private static readonly HttpClient client = new HttpClient();

        public MainWindow()
        {
            InitializeComponent();
        }

        private async void LoadDataButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // ارسال درخواست GET به API
                string url = "http://localhost:6666/api/predictions";
                HttpResponseMessage response = await client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    string jsonString = await response.Content.ReadAsStringAsync();

                    // چاپ پاسخ JSON در کنسول برای بررسی
                    Console.WriteLine(jsonString);
                    var options = new JsonSerializerOptions
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase // یا هر تنظیم دیگری که مناسب است
                    };

                    var predictionResponse = JsonSerializer.Deserialize<PredictionAndPlotResponse>(jsonString, options);

                    // افزودن داده‌های پیش‌بینی به ListView
                    PredictionsListView.ItemsSource = predictionResponse.Predictions;

                    // تبدیل تصویر Base64 به Bitmap و نمایش آن در UI
                    if (!string.IsNullOrEmpty(predictionResponse.PlotImage))
                    {
                        DisplayPlot(predictionResponse.PlotImage);
                    }
                    else
                    {
                        MessageBox.Show("Plot image is empty.");
                    }
                }
                else
                {
                    MessageBox.Show($"Failed to load predictions. Status Code: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        private void DisplayPlot(string base64Image)
        {
            try
            {
                // حذف prefix از Base64 string (data:image/png;base64,)
                string base64Data = base64Image.Split(',')[1];
                byte[] binaryData = Convert.FromBase64String(base64Data);

                using (var ms = new MemoryStream(binaryData))
                {
                    var image = new BitmapImage();
                    image.BeginInit();
                    image.StreamSource = ms;
                    image.CacheOption = BitmapCacheOption.OnLoad;
                    image.EndInit();

                    // نمایش تصویر در کنترل Image
                    ChartImage.Source = image;
                }
            }
            catch (FormatException ex)
            {
                MessageBox.Show($"Error in image format: {ex.Message}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error displaying plot: {ex.Message}");
            }
        }
    }
}