using ChitaiGorod;
using Microsoft.Win32;
using System;
using System.Data.SqlClient;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace kakto
{
    public partial class ProductEditWindow : Window
    {
        private Product currentProduct;

        private string selectedPhoto = "";

        private bool isEditMode = false;

        public ProductEditWindow()
        {
            InitializeComponent();

            LoadCategories();
            LoadManufacturers();
            LoadUnits();
            LoadSuppliers();

            ArticleBox.Text =
                GenerateArticle();

            PhotoImage.Source =
                new BitmapImage(
                    new Uri("Resources/picture.png",
                    UriKind.Relative));
        }

        public ProductEditWindow(Product product)
        {
            InitializeComponent();

            LoadCategories();
            LoadManufacturers();

            currentProduct = product;

            isEditMode = true;

            FillData();
        }
        private void LoadSuppliers()
        {
            SupplierBox.Items.Add("Виктор Астафьев");
            SupplierBox.Items.Add("Гилберт Кит Честертон");
            SupplierBox.Items.Add("Кирилл Каланджи");
            SupplierBox.Items.Add("Людмила Улицкая");
            SupplierBox.Items.Add("Аркадий Гайдар");
            SupplierBox.Items.Add("Юрий Родичев");
            SupplierBox.Items.Add("Шон Кэрролл");
            SupplierBox.Items.Add("Яков Гордин");
            SupplierBox.Items.Add("Иосиф Бродский");
            SupplierBox.Items.Add("Дмитрий Мережковский");
            SupplierBox.Items.Add("Дмитрий Щербаков");
        }
        private void LoadUnits()
        {
            using (SqlConnection connection =
                   DbHelper.GetConnection())
            {
                connection.Open();

                SqlCommand command =
                    new SqlCommand(
                        @"SELECT UnitID,
                         UnitName
                  FROM Units",
                        connection);

                SqlDataReader reader =
                    command.ExecuteReader();

                while (reader.Read())
                {
                    UnitBox.Items.Add(
                        new ComboBoxItem
                        {
                            Content =
                                reader["UnitName"].ToString(),

                            Tag =
                                reader["UnitID"]
                        });
                }
            }
        }
        private void FillData()
        {
            ArticleBox.Text =
                currentProduct.Article;

            NameBox.Text =
                currentProduct.ProductName;

            SupplierBox.Text =
                currentProduct.Supplier;

            PriceBox.Text =
                currentProduct.Price.ToString();

            QuantityBox.Text =
                currentProduct.StockQuantity.ToString();

            DiscountBox.Text =
                currentProduct.Discount.ToString();

            DescriptionBox.Text =
                currentProduct.Description;

            CategoryBox.Text =
                currentProduct.CategoryName;

            ManufacturerBox.Text =
                currentProduct.ManufacturerName;

            if (!string.IsNullOrWhiteSpace(currentProduct.PhotoPath))
            {
                try
                {
                    PhotoImage.Source =
                        new BitmapImage(
                            new Uri(
                                currentProduct.PhotoPath,
                                UriKind.RelativeOrAbsolute));
                }
                catch { }
            }
        }

        private void LoadCategories()
        {
            using (SqlConnection connection =
                   DbHelper.GetConnection())
            {
                connection.Open();

                SqlCommand command =
                    new SqlCommand(
                        @"SELECT CategoryID,
                         CategoryName
                  FROM Categories",
                        connection);

                SqlDataReader reader =
                    command.ExecuteReader();

                while (reader.Read())
                {
                    CategoryBox.Items.Add(
                        new ComboBoxItem
                        {
                            Content =
                                reader["CategoryName"].ToString(),

                            Tag =
                                reader["CategoryID"]
                        });
                }
            }
        }

        private void LoadManufacturers()
        {
            using (SqlConnection connection =
                   DbHelper.GetConnection())
            {
                connection.Open();

                SqlCommand command =
                    new SqlCommand(
                        @"SELECT ManufacturerID,
                         ManufacturerName
                  FROM Manufacturers",
                        connection);

                SqlDataReader reader =
                    command.ExecuteReader();

                while (reader.Read())
                {
                    ManufacturerBox.Items.Add(
                        new ComboBoxItem
                        {
                            Content =
                                reader["ManufacturerName"].ToString(),

                            Tag =
                                reader["ManufacturerID"]
                        });
                }
            }
        }

        
        private string GenerateArticle()
        {
            return Guid.NewGuid()
                .ToString("N")
                .Substring(0, 6)
                .ToUpper();
        }
        private string savedPhotoName =
    "picture.png";
        private void ChoosePhoto_Click(
    object sender,
    RoutedEventArgs e)
        {
            OpenFileDialog dialog =
                new OpenFileDialog();

            dialog.Filter =
                "Изображения|*.jpg;*.jpeg;*.png";

            if (dialog.ShowDialog() == true)
            {
                BitmapImage image =
                    new BitmapImage(
                        new Uri(dialog.FileName));

                if (image.PixelWidth > 300 ||
                    image.PixelHeight > 200)
                {
                    MessageBox.Show(
                        "Размер изображения не должен превышать 300x200 пикселей.",
                        "Ошибка",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);

                    return;
                }

                string fileName =
                    Guid.NewGuid().ToString() +
                    System.IO.Path.GetExtension(
                        dialog.FileName);

                string destination =
                    System.IO.Path.Combine(
                        "Resources",
                        fileName);

                File.Copy(
                    dialog.FileName,
                    destination,
                    true);

                savedPhotoName =
                    fileName;

                PhotoImage.Source =
                    new BitmapImage(
                        new Uri(destination,
                        UriKind.Relative));
            }
        }
        private void Back_Click(
    object sender,
    RoutedEventArgs e)
        {
            Close();
        }
        private void Save_Click(
     object sender,
     RoutedEventArgs e)
        {
            try
            {
                decimal price =
                    decimal.Parse(PriceBox.Text);

                int quantity =
                    int.Parse(QuantityBox.Text);

                int discount =
                    int.Parse(DiscountBox.Text);

                if (string.IsNullOrWhiteSpace(NameBox.Text))
                {
                    MessageBox.Show(
                        "Введите название товара.",
                        "Ошибка",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);

                    return;
                }

                if (price < 0)
                {
                    MessageBox.Show(
                        "Цена не может быть отрицательной.");

                    return;
                }

                if (quantity < 0)
                {
                    MessageBox.Show(
                        "Количество не может быть отрицательным.");

                    return;
                }

                if (discount < 0 || discount > 100)
                {
                    MessageBox.Show(
                        "Скидка должна быть от 0 до 100%.");

                    return;
                }

                ComboBoxItem category =
                    (ComboBoxItem)CategoryBox.SelectedItem;

                ComboBoxItem manufacturer =
                    (ComboBoxItem)ManufacturerBox.SelectedItem;

                if (category == null ||
                    manufacturer == null)
                {
                    MessageBox.Show(
                        "Выберите категорию и производителя.");

                    return;
                }

                using (SqlConnection connection =
                       DbHelper.GetConnection())
                {
                    connection.Open();

                    if (isEditMode)
                    {
                        SqlCommand command =
                            new SqlCommand(
                            @"UPDATE Products
                      SET ProductName=@name,
                          CategoryID=@category,
                          ManufacturerID=@manufacturer,
                          Supplier=@supplier,
                          Price=@price,
                          PhotoPath=@photo,
                          Discount=@discount,
                          StockQuantity=@quantity,
                          Description=@description
                      WHERE Article=@article",
                            connection);
                        command.Parameters.AddWithValue(
                        "@photo",
                        savedPhotoName);
                        command.Parameters.AddWithValue(
                            "@article",
                            currentProduct.Article);

                        command.Parameters.AddWithValue(
                            "@name",
                            NameBox.Text);

                        command.Parameters.AddWithValue(
                            "@category",
                            category.Tag);

                        command.Parameters.AddWithValue(
                            "@manufacturer",
                            manufacturer.Tag);

                        command.Parameters.AddWithValue(
                            "@supplier",
                            SupplierBox.Text);

                        command.Parameters.AddWithValue(
                            "@price",
                            price);

                        command.Parameters.AddWithValue(
                            "@discount",
                            discount);

                        command.Parameters.AddWithValue(
                            "@quantity",
                            quantity);

                        command.Parameters.AddWithValue(
                            "@description",
                            DescriptionBox.Text);

                        command.ExecuteNonQuery();
                    }
                    else
                    {
                        SqlCommand command =
                            new SqlCommand(
                            @"INSERT INTO Products
        (
            Article,
            ProductName,
            UnitID,
            ManufacturerID,
            CategoryID,
            Supplier,
            Price,
            Discount,
            StockQuantity,
            Description,
            PhotoPath
        )
        VALUES
        (
            @article,
            @name,
            1,
            @manufacturer,
            @category,
            @supplier,
            @price,
            @discount,
            @quantity,
            @description,
            @photo,
savedPhotoName
        )",
                            connection);
                        command.Parameters.AddWithValue(
                        "@photo",
                        savedPhotoName);

                        command.Parameters.AddWithValue(
                            "@article",
                            ArticleBox.Text);

                        command.Parameters.AddWithValue(
                            "@name",
                            NameBox.Text);

                        command.Parameters.AddWithValue(
                            "@manufacturer",
                            manufacturer.Tag);

                        command.Parameters.AddWithValue(
                            "@category",
                            category.Tag);

                        command.Parameters.AddWithValue(
                            "@supplier",
                            SupplierBox.Text);

                        command.Parameters.AddWithValue(
                            "@price",
                            price);

                        command.Parameters.AddWithValue(
                            "@discount",
                            discount);

                        command.Parameters.AddWithValue(
                            "@quantity",
                            quantity);

                        command.Parameters.AddWithValue(
                            "@description",
                            DescriptionBox.Text);

                        command.Parameters.AddWithValue(
                            "@photo",
                            "picture.png");

                        command.ExecuteNonQuery();
                    }
                }

                MessageBox.Show(
                    "Товар успешно сохранён.",
                    "Информация",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);

                Close();
            }

            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }
    }
}