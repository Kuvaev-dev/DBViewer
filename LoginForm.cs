using System.Data.SqlClient;

namespace CloudERPDBViewer
{
    public partial class LoginForm : Form
    {
        private string connectionString;

        public LoginForm(string connectionString)
        {
            InitializeComponent();
            this.connectionString = connectionString;
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text;
            string password = txtPassword.Text;

            if (AuthenticateUser(username, password))
            {
                MessageBox.Show("Вы успешно вошли в систему.");
                Form1 mainForm = new(connectionString, username);
                mainForm.Show();
                Hide();
            }
            else
            {
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

        private void configDbBtn_Click(object sender, EventArgs e)
        {
            DatabaseConfigForm databaseConfigForm = new();
            databaseConfigForm.ConnectionStringSaved += DatabaseConfigForm_ConnectionStringSaved;
            databaseConfigForm.Show();
        }

        private void DatabaseConfigForm_ConnectionStringSaved(object sender, string e)
        {
            connectionString = e;
        }
    }
}
