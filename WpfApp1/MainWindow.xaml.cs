using System.Net.Http;
using System.Windows;

namespace WpfApp1
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        // Event handler برای کلیک روی دکمه
        private async void FetchDataButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // فراخوانی تابع FetchStudentsAsync برای دریافت داده‌ها از API
                var result = await FetchStudentsAsync();
                ResultTextBox.Text = result; // نمایش نتیجه در TextBox
            }
            catch (Exception ex)
            {
                ResultTextBox.Text = $"Error: {ex.Message}";
            }
        }

        // تابعی برای فراخوانی API و دریافت داده‌ها
        private async Task<string> FetchStudentsAsync()
        {
            using (HttpClient client = new HttpClient())
            {
                // ارسال درخواست GET به API
                HttpResponseMessage response = await client.GetAsync("http://localhost:1234/api/students");

                // بررسی اینکه آیا درخواست موفقیت‌آمیز بوده است
                if (response.IsSuccessStatusCode)
                {
                    // خواندن محتوای پاسخ به عنوان یک رشته
                    return await response.Content.ReadAsStringAsync();
                }
                else
                {
                    throw new Exception("Failed to fetch students data");
                }
            }
        }
    }
}