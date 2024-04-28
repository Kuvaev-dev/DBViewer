using System.Data.SqlClient;

namespace CloudERPDBViewer
{
    public partial class DatabaseConfigForm : Form
    {
        // Определение события для передачи строки подключения
        public event EventHandler<string> ConnectionStringSaved;

        public DatabaseConfigForm()
        {
            InitializeComponent();
        }

        private void DatabaseConfigForm_Load(object sender, EventArgs e)
        {
            textBoxDataSource.Text = Settings.Default.DataSource;
            textBoxInitialCatalog.Text = Settings.Default.InitialCatalog;
            checkBoxIntegratedSecurity.Checked = Settings.Default.IntegratedSecurity;
            textBoxUsername.Text = Settings.Default.Username;
            textBoxPassword.Text = Settings.Default.Password;
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            Settings.Default.DataSource = textBoxDataSource.Text;
            Settings.Default.InitialCatalog = textBoxInitialCatalog.Text;
            Settings.Default.IntegratedSecurity = checkBoxIntegratedSecurity.Checked;
            Settings.Default.Username = textBoxUsername.Text;
            Settings.Default.Password = textBoxPassword.Text;
            Settings.Default.Save();

            string connectionString = GetConnectionString();

            ConnectionStringSaved?.Invoke(this, connectionString);

            MessageBox.Show("Конфигурация сохранена.");
        }

        private void buttonTestConnection_Click(object sender, EventArgs e)
        {
            string connectionString = GetConnectionString();

            try
            {
                using SqlConnection connection = new(connectionString);
                connection.Open();

                string query = "SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE'";
                SqlCommand command = new SqlCommand(query, connection);
                SqlDataReader reader = command.ExecuteReader();

                if (reader.HasRows)
                {
                    MessageBox.Show("Подключение успешно. Запрос выполнен.");
                }
                else
                {
                    MessageBox.Show("Подключение успешно. Нет доступных таблиц.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при подключении к базе данных: " + ex.Message);
            }
        }

        private string GetConnectionString()
        {
            string dataSource = textBoxDataSource.Text;
            string initialCatalog = textBoxInitialCatalog.Text;
            bool integratedSecurity = checkBoxIntegratedSecurity.Checked;
            string username = textBoxUsername.Text;
            string password = textBoxPassword.Text;

            string connectionString = "Data Source=" + dataSource + ";Initial Catalog=" + initialCatalog + ";";

            if (integratedSecurity)
            {
                connectionString += "Integrated Security=True;";
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(username) && !string.IsNullOrWhiteSpace(password))
                {
                    connectionString += "User ID=" + username + ";Password=" + password + ";";
                }
            }

            return connectionString;
        }
    }
}
