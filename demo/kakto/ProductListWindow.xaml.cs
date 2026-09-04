using ChitaiGorod;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace kakto
{
    public partial class ProductListWindow : Window
    {
        private List<Product> allProducts = new List<Product>();


        private ProductEditWindow editWindow;

        public ProductListWindow()
        {
            InitializeComponent();
            if (CurrentUser.Role == "Гость" ||
    CurrentUser.Role == "Авторизированный клиент")
            {
                SearchBox.Visibility =
                    Visibility.Collapsed;

                DiscountFilter.Visibility =
                    Visibility.Collapsed;

                SortBox.Visibility =
                    Visibility.Collapsed;

                OrdersButton.Visibility =
                    Visibility.Collapsed;
            }
            if (CurrentUser.Role != "Администратор")
            {
                AddButton.Visibility =
                    Visibility.Collapsed;

                DeleteButton.Visibility =
                    Visibility.Collapsed;
            }

            UserInfoText.Text =
                $"{CurrentUser.FullName} ({CurrentUser.Role})";

            if (CurrentUser.Role != "Администратор")
            {
                AddButton.Visibility =
                    Visibility.Collapsed;

                DeleteButton.Visibility =
                    Visibility.Collapsed;
            }

            LoadProductsFromDb();
            ApplyFilters();
        }

        private void OrdersButton_Click(
    object sender,
    RoutedEventArgs e)
        {
            OrderListWindow window =
                new OrderListWindow();

            window.Show();

            Close();
        }
        private void LoadProductsFromDb()
        {
            allProducts.Clear();

            using (SqlConnection connection = DbHelper.GetConnection())
            {
                connection.Open();

                string query =
                @"SELECT
                    p.Article,
                    p.ProductName,
                    p.Description,
                    p.Supplier,
                    p.Price,
                    p.Discount,
                    p.StockQuantity,
                    p.PhotoPath,
                    p.CategoryID,
                    p.ManufacturerID,
                    c.CategoryName,
                    m.ManufacturerName
                FROM Products p
                INNER JOIN Categories c
                    ON p.CategoryID = c.CategoryID
                INNER JOIN Manufacturers m
                    ON p.ManufacturerID = m.ManufacturerID";

                SqlCommand command =
                    new SqlCommand(query, connection);

                SqlDataReader reader =
                    command.ExecuteReader();

                while (reader.Read())
                {
                    string photo = reader["PhotoPath"].ToString();

                    if (string.IsNullOrWhiteSpace(photo))
                        photo = "picture.png";

                    allProducts.Add(new Product
                    {
                        Article = reader["Article"].ToString(),
                        ProductName = reader["ProductName"].ToString(),
                        Description = reader["Description"].ToString(),
                        Supplier = reader["Supplier"].ToString(),
                        CategoryName = reader["CategoryName"].ToString(),
                        ManufacturerName = reader["ManufacturerName"].ToString(),
                        Price = (decimal)reader["Price"],
                        Discount = (int)reader["Discount"],
                        StockQuantity = (int)reader["StockQuantity"],
                        PhotoPath = $"Resources/{photo}",
                        CategoryID = (int)reader["CategoryID"],
                        ManufacturerID = (int)reader["ManufacturerID"],
                    });
                }
            }
        }
        private void ExitButton_Click(object sender,
                              RoutedEventArgs e)
        {
            if (MessageBox.Show(
                "Выйти из учетной записи?",
                "Подтверждение",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question)
                == MessageBoxResult.Yes)
            {
                CurrentUser.UserID = 0;
                CurrentUser.FullName = null;
                CurrentUser.Role = null;

                LoginWindow loginWindow =
                    new LoginWindow();

                loginWindow.Show();

                Close();
            }
        }
        private void ProductsList_MouseDoubleClick(
    object sender,
    System.Windows.Input.MouseButtonEventArgs e)
        {
            if (CurrentUser.Role != "Администратор")
                return;

            Product product =
                ProductsList.SelectedItem as Product;

            if (product == null)
                return;

           
            if (editWindow != null)
            {
                editWindow.Activate();
                return;
            }

            editWindow =
                new ProductEditWindow(product);

            editWindow.Closed +=
                (s, args) =>
                {
                    editWindow = null;

                    LoadProductsFromDb();

                    ApplyFilters();
                };

            editWindow.Show();
        }

        private void AddButton_Click(object sender,
                             RoutedEventArgs e)
        {
            if (editWindow != null)
            {
                editWindow.Activate();
                return;
            }
            

            editWindow =
                new ProductEditWindow();

            editWindow.Closed +=
                (s, args) =>
                {
                    editWindow = null;

                    LoadProductsFromDb();

                    ApplyFilters();
                };

            editWindow.Show();
        }

        private void DeleteButton_Click(object sender,
                                RoutedEventArgs e)
        {
            Product product =
                ProductsList.SelectedItem as Product;

            if (product == null)
            {
                MessageBox.Show(
                    "Выберите товар для удаления.",
                    "Предупреждение",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            using (SqlConnection connection =
                   DbHelper.GetConnection())
            {
                connection.Open();

                SqlCommand checkCommand =
                    new SqlCommand(
                    @"SELECT COUNT(*)
              FROM OrderProducts
              WHERE ProductArticle=@article",
                    connection);

                checkCommand.Parameters.AddWithValue(
                    "@article",
                    product.Article);

                int count =
                    (int)checkCommand.ExecuteScalar();

                if (count > 0)
                {
                    MessageBox.Show(
                        "Товар присутствует в заказах и не может быть удалён.",
                        "Ошибка",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);

                    return;
                }

                if (MessageBox.Show(
                    "Удалить выбранный товар?",
                    "Подтверждение",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning)
                    != MessageBoxResult.Yes)
                {
                    return;
                }

                SqlCommand deleteCommand =
                    new SqlCommand(
                    @"DELETE FROM Products
              WHERE Article=@article",
                    connection);

                deleteCommand.Parameters.AddWithValue(
                    "@article",
                    product.Article);

                deleteCommand.ExecuteNonQuery();
            }

            LoadProductsFromDb();

            ApplyFilters();
        }
        private void ApplyFilters()
        {
            if (SearchBox == null ||
    DiscountFilter == null ||
    SortBox == null ||
    ProductsList == null ||
    CountText == null)
                return;
            IEnumerable<Product> products = allProducts;

            string search =
                SearchBox.Text?.ToLower() ?? "";

            if (!string.IsNullOrWhiteSpace(search))
            {
                products = products.Where(p =>
                    (p.ProductName ?? "").ToLower().Contains(search) ||
                    (p.Description ?? "").ToLower().Contains(search) ||
                    (p.Supplier ?? "").ToLower().Contains(search) ||
                    (p.CategoryName ?? "").ToLower().Contains(search) ||
                    (p.ManufacturerName ?? "").ToLower().Contains(search));
            }

            if (DiscountFilter.SelectedIndex == 1)
                products = products.Where(p => p.Discount >= 0 && p.Discount < 13);

            if (DiscountFilter.SelectedIndex == 2)
                products = products.Where(p => p.Discount >= 13 && p.Discount < 17);

            if (DiscountFilter.SelectedIndex == 3)
                products = products.Where(p => p.Discount >= 17);

            switch (SortBox.SelectedIndex)
            {
                case 1:
                    products = products.OrderBy(p => p.Price);
                    break;

                case 2:
                    products = products.OrderByDescending(p => p.Price);
                    break;

                case 3:
                    products = products.OrderBy(p => p.StockQuantity);
                    break;

                case 4:
                    products = products.OrderByDescending(p => p.StockQuantity);
                    break;
            }

            List<Product> result = products.ToList();

            ProductsList.ItemsSource = result;

            CountText.Text =
                $"Найдено {result.Count} из {allProducts.Count}";
        }

        private void SearchBox_TextChanged(object sender,
                                           TextChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void FilterChanged(object sender,
                                   SelectionChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void Refresh_Click(object sender,
                                   RoutedEventArgs e)
        {
            SearchBox.Clear();

            DiscountFilter.SelectedIndex = 0;

            SortBox.SelectedIndex = 0;

            LoadProductsFromDb();

            ApplyFilters();
        }
    }
}