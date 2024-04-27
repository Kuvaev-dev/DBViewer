using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CloudERPDBViewer
{
    public partial class LoginForm : Form
    {
        private readonly string connectionString = @"Data Source=localhost\sqlexpress;Initial Catalog=master;Integrated Security=True;";

        public LoginForm()
        {
            InitializeComponent();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text;
            string password = txtPassword.Text;

            if (AuthenticateUser(username, password))
            {
                // Пользователь успешно аутентифицирован
                MessageBox.Show("Вы успешно вошли в систему.");
                // Открываем основную форму приложения или выполняем другие действия после аутентификации
                Form1 mainForm = new Form1();
                mainForm.Show();
                this.Hide();
            }
            else
            {
                // Пользователь не аутентифицирован
                MessageBox.Show("Ошибка аутентификации. Пожалуйста, проверьте свой логин и пароль.");
            }
        }

        private bool AuthenticateUser(string username, string password)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    // Проверяем существует ли такой пользователь и правильный ли пароль
                    string query = "SELECT COUNT(*) FROM sys.sql_logins WHERE name = @Username AND PWDCOMPARE(@Password, password_hash) = 1";
                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Username", username);
                        command.Parameters.AddWithValue("@Password", password);
                        int count = (int)command.ExecuteScalar();
                        return count > 0;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при аутентификации: " + ex.Message);
                    return false;
                }
            }
        }

        private void LoginForm_Load(object sender, EventArgs e)
        {

        }
    }
}
