using kakto;
using System;
using System.Data.SqlClient;
using System.Windows;
using System.Collections.Generic;

namespace ChitaiGorod
{
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (SqlConnection connection = DbHelper.GetConnection())
                {
                    connection.Open();

                    string query =
                    @"SELECT
                        u.UserID,
                        u.FullName,
                        r.RoleName
                    FROM Users u
                    INNER JOIN Roles r
                        ON u.RoleID = r.RoleID
                    WHERE u.Login = @login
                    AND u.Password = @password";

                    SqlCommand command =
                        new SqlCommand(query, connection);

                    command.Parameters.AddWithValue(
                        "@login",
                        LoginBox.Text);

                    command.Parameters.AddWithValue(
                        "@password",
                        PasswordBox.Password);

                    SqlDataReader reader =
                        command.ExecuteReader();

                    if (reader.Read())
                    {
                        CurrentUser.UserID =
                            (int)reader["UserID"];

                        CurrentUser.FullName =
                            reader["FullName"].ToString();

                        CurrentUser.Role =
                            reader["RoleName"].ToString();

                        ProductListWindow window =
                            new ProductListWindow();

                        window.Show();

                        Close();
                    }
                    else
                    {
                        MessageBox.Show(
                            "Неверный логин или пароль",
                            "Ошибка",
                            MessageBoxButton.OK,
                            MessageBoxImage.Error);
                    }
                }
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

        private void GuestButton_Click(object sender, RoutedEventArgs e)
        {
            CurrentUser.UserID = 0;
            CurrentUser.FullName = "Гость";
            CurrentUser.Role = "Гость";

            ProductListWindow window =
                new ProductListWindow();

            window.Show();

            Close();
        }
    }
}