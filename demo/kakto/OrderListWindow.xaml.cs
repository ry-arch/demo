using ChitaiGorod;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Windows;

namespace kakto
{
    public partial class OrderListWindow : Window
    {
        public OrderListWindow()
        {
            InitializeComponent();

            LoadOrders();

            if (CurrentUser.Role != "Администратор")
            {
                AddButton.Visibility =
                    Visibility.Collapsed;

                DeleteButton.Visibility =
                    Visibility.Collapsed;
            }
        }

        private void LoadOrders()
        {
            List<Order> orders =
                new List<Order>();

            using (SqlConnection connection =
                   DbHelper.GetConnection())
            {
                connection.Open();

                string query =
                @"SELECT
                    o.OrderID,
                    op.ProductArticle,
                    s.StatusName,
                    p.Address,
                    o.OrderDate,
                    o.DeliveryDate
                  FROM Orders o
                  INNER JOIN OrderProducts op
                    ON o.OrderID = op.OrderID
                  INNER JOIN PickupPoints p
                    ON o.PickupPointID = p.PickupPointID
                  INNER JOIN OrderStatuses s
                    ON o.StatusID = s.StatusID";

                SqlCommand command =
                    new SqlCommand(query, connection);

                SqlDataReader reader =
                    command.ExecuteReader();

                while (reader.Read())
                {
                    orders.Add(new Order
                    {
                        OrderID =
                            (int)reader["OrderID"],

                        ProductArticle =
                            reader["ProductArticle"].ToString(),

                        StatusName =
                            reader["StatusName"].ToString(),

                        PickupAddress =
                            reader["Address"].ToString(),

                        OrderDate =
                            (System.DateTime)reader["OrderDate"],

                        DeliveryDate =
                            (System.DateTime)reader["DeliveryDate"]
                    });
                }
            }

            OrdersList.ItemsSource =
                orders;
        }

        private void Back_Click(
            object sender,
            RoutedEventArgs e)
        {
            new ProductListWindow().Show();

            Close();
        }

        private void AddButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            new OrderEditWindow().ShowDialog();

            LoadOrders();
        }

        private void OrdersList_MouseDoubleClick(
            object sender,
            System.Windows.Input.MouseButtonEventArgs e)
        {
            if (CurrentUser.Role != "Администратор")
                return;

            Order order =
                OrdersList.SelectedItem as Order;

            if (order == null)
                return;

            new OrderEditWindow(order)
                .ShowDialog();

            LoadOrders();
        }


        private void DeleteButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            Order order =
                OrdersList.SelectedItem as Order;

            if (order == null)
                return;

            using (SqlConnection connection =
       DbHelper.GetConnection())
            {
                connection.Open();

                SqlCommand deleteProducts =
                    new SqlCommand(
                    @"DELETE FROM OrderProducts
          WHERE OrderID=@id",
                    connection);

                deleteProducts.Parameters.AddWithValue(
                    "@id",
                    order.OrderID);

                deleteProducts.ExecuteNonQuery();

                SqlCommand deleteOrder =
                    new SqlCommand(
                    @"DELETE FROM Orders
          WHERE OrderID=@id",
                    connection);

                deleteOrder.Parameters.AddWithValue(
                    "@id",
                    order.OrderID);

                deleteOrder.ExecuteNonQuery();
            }

            LoadOrders();
        }
    }
}