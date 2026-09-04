using ChitaiGorod;
using System;
using System.Data.SqlClient;
using System.Windows;

namespace kakto
{
    public partial class OrderEditWindow : Window
    {
        private Order currentOrder;

        public OrderEditWindow()
        {
            InitializeComponent();

            LoadData();
        }

        public OrderEditWindow(Order order)
        {
            InitializeComponent();

            currentOrder = order;

            LoadData();

            ArticleBox.Text =
                order.ProductArticle;

            OrderDatePicker.SelectedDate =
                order.OrderDate;

            DeliveryDatePicker.SelectedDate =
                order.DeliveryDate;
        }

        private void LoadData()
        {
            using (SqlConnection connection =
                   DbHelper.GetConnection())
            {
                connection.Open();

                SqlCommand statusCommand =
                    new SqlCommand(
                    "SELECT StatusName FROM OrderStatuses",
                    connection);

                SqlDataReader reader =
                    statusCommand.ExecuteReader();

                while (reader.Read())
                {
                    StatusBox.Items.Add(
                        reader["StatusName"].ToString());
                }

                reader.Close();

                SqlCommand pickupCommand =
                    new SqlCommand(
                    "SELECT Address FROM PickupPoints",
                    connection);

                reader =
                    pickupCommand.ExecuteReader();

                while (reader.Read())
                {
                    PickupBox.Items.Add(
                        reader["Address"].ToString());
                }
            }
        }
        private void OrderDatePicker_SelectedDateChanged(
    object sender,
    System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (OrderDatePicker.SelectedDate != null)
            {
                DeliveryDatePicker.DisplayDateStart =
                    OrderDatePicker.SelectedDate;
            }
        }
        private void Save_Click(
            object sender,
            RoutedEventArgs e)
        {
            try
            {
                using (SqlConnection connection =
                       DbHelper.GetConnection())
                {
                    connection.Open();

                    int statusId = 1;
                    int pickupId = 1;

                    SqlCommand statusCommand =
                        new SqlCommand(
                        @"SELECT StatusID
                          FROM OrderStatuses
                          WHERE StatusName=@name",
                        connection);

                    statusCommand.Parameters.AddWithValue(
                        "@name",
                        StatusBox.Text);

                    statusId =
                        Convert.ToInt32(
                            statusCommand.ExecuteScalar());

                    SqlCommand pickupCommand =
                        new SqlCommand(
                        @"SELECT PickupPointID
                          FROM PickupPoints
                          WHERE Address=@address",
                        connection);

                    pickupCommand.Parameters.AddWithValue(
                        "@address",
                        PickupBox.Text);

                    pickupId =
                        Convert.ToInt32(
                            pickupCommand.ExecuteScalar());

                    if (currentOrder == null)
                    {
                        SqlCommand insertOrder =
                            new SqlCommand(
                            @"INSERT INTO Orders
                              (
                                OrderDate,
                                DeliveryDate,
                                PickupPointID,
                                UserID,
                                ReceiveCode,
                                StatusID
                              )
                              VALUES
                              (
                                @OrderDate,
                                @DeliveryDate,
                                @PickupPointID,
                                1,
                                999,
                                @StatusID
                              )",
                            connection);

                        insertOrder.Parameters.AddWithValue(
                            "@OrderDate",
                            OrderDatePicker.SelectedDate);

                        insertOrder.Parameters.AddWithValue(
                            "@DeliveryDate",
                            DeliveryDatePicker.SelectedDate);

                        insertOrder.Parameters.AddWithValue(
                            "@PickupPointID",
                            pickupId);

                        insertOrder.Parameters.AddWithValue(
                            "@StatusID",
                            statusId);

                        insertOrder.ExecuteNonQuery();

                        SqlCommand getOrder =
                            new SqlCommand(
                            "SELECT MAX(OrderID) FROM Orders",
                            connection);

                        int orderId =
                            Convert.ToInt32(
                                getOrder.ExecuteScalar());

                        SqlCommand insertProduct =
                            new SqlCommand(
                            @"INSERT INTO OrderProducts
                              (
                                OrderID,
                                ProductArticle,
                                Quantity
                              )
                              VALUES
                              (
                                @OrderID,
                                @Article,
                                1
                              )",
                            connection);

                        insertProduct.Parameters.AddWithValue(
                            "@OrderID",
                            orderId);

                        insertProduct.Parameters.AddWithValue(
                            "@Article",
                            ArticleBox.Text);

                        insertProduct.ExecuteNonQuery();
                    }
                    else
                    {
                        SqlCommand updateOrder =
                            new SqlCommand(
                            @"UPDATE Orders
                              SET
                              OrderDate=@OrderDate,
                              DeliveryDate=@DeliveryDate,
                              PickupPointID=@PickupPointID,
                              StatusID=@StatusID
                              WHERE OrderID=@OrderID",
                            connection);

                        updateOrder.Parameters.AddWithValue(
                            "@OrderDate",
                            OrderDatePicker.SelectedDate);

                        updateOrder.Parameters.AddWithValue(
                            "@DeliveryDate",
                            DeliveryDatePicker.SelectedDate);

                        updateOrder.Parameters.AddWithValue(
                            "@PickupPointID",
                            pickupId);

                        updateOrder.Parameters.AddWithValue(
                            "@StatusID",
                            statusId);

                        updateOrder.Parameters.AddWithValue(
                            "@OrderID",
                            currentOrder.OrderID);

                        updateOrder.ExecuteNonQuery();
                    }
                }

                MessageBox.Show(
                    "Заказ успешно сохранён.",
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