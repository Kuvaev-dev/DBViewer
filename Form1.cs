using Dapper;
using System.Data;
using System.Data.SqlClient;

namespace CloudERPDBViewer
{
    public partial class Form1 : Form
    {
        private readonly string connectionString;
        private string selectedColumn = "";
        private string selectedSortOrder = "";
        private string selectedTableName = "";

        public Form1(string connstr, string username)
        {
            connectionString = connstr;

            InitializeComponent();
            LoadDatabaseObjects(username);
            LoadSortColumns();
            LoadSortOrders();
            LoadSortOptions();

            dataGridViewData.CellDoubleClick += DataGridViewData_CellDoubleClick;
            dataGridViewData.KeyDown += DataGridViewData_KeyDown;
            dataGridViewData.RowsAdded += DataGridViewData_RowsAdded;
        }

        private void LoadSortOptions()
        {
            comboBoxSortData.Items.AddRange(new string[] {
                "Alphabetical",
                "Reverse Alphabetical",
                "First 5 Rows",
                "First 10 Rows"
            });
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void LoadDatabaseObjects(string username)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                if (IsAdmin(username))
                {
                    LoadObjectsForUser(connection, username, "Tables");
                    LoadObjectsForUser(connection, username, "Triggers");
                    LoadObjectsForUser(connection, username, "Procedures");
                    LoadObjectsForUser(connection, username, "Views");
                }
                else if (IsEmployee(username))
                {
                    LoadObjectsForUser(connection, username, "Tables");
                    LoadObjectsForUser(connection, username, "Procedures");
                    LoadObjectsForUser(connection, username, "Views");
                }
                else if (IsCustomer(username))
                {
                    LoadObjectsForUser(connection, username, "Tables");
                    LoadObjectsForUser(connection, username, "Views");
                }
                else
                {
                    throw new ArgumentException("Invalid username");
                }
            }
        }


        private void LoadObjectsForUser(SqlConnection connection, string username, string objectType)
        {
            string sqlQuery = "";

            switch (objectType)
            {
                case "Tables":
                    sqlQuery = @"
                SELECT DISTINCT t.name
                FROM sys.tables t
                INNER JOIN sys.schemas s ON t.schema_id = s.schema_id
                WHERE s.name = 'dbo' AND t.name NOT LIKE 'sys%' AND EXISTS (
                    SELECT 1
                    FROM sys.database_permissions dp
                    WHERE dp.major_id = t.object_id
                    AND dp.grantee_principal_id = USER_ID(@Username)
                )";
                    break;
                case "Triggers":
                    sqlQuery = @"
                SELECT DISTINCT tr.name
                FROM sys.triggers tr
                INNER JOIN sys.objects o ON tr.parent_id = o.object_id
                INNER JOIN sys.schemas s ON o.schema_id = s.schema_id
                WHERE s.name = 'dbo' AND EXISTS (
                    SELECT 1
                    FROM sys.database_permissions dp
                    WHERE dp.major_id = tr.object_id
                    AND dp.grantee_principal_id = USER_ID(@Username)
                )";
                    break;
                case "Procedures":
                    sqlQuery = @"
                SELECT DISTINCT p.name
                FROM sys.procedures p
                INNER JOIN sys.schemas s ON p.schema_id = s.schema_id
                WHERE s.name = 'dbo' AND EXISTS (
                    SELECT 1
                    FROM sys.database_permissions dp
                    WHERE dp.major_id = p.object_id
                    AND dp.grantee_principal_id = USER_ID(@Username)
                )";
                    break;
                case "Views":
                    sqlQuery = @"
                SELECT DISTINCT v.name
                FROM sys.views v
                INNER JOIN sys.schemas s ON v.schema_id = s.schema_id
                WHERE s.name = 'dbo' AND EXISTS (
                    SELECT 1
                    FROM sys.database_permissions dp
                    WHERE dp.major_id = v.object_id
                    AND dp.grantee_principal_id = USER_ID(@Username)
                )";
                    break;
                default:
                    throw new ArgumentException("Invalid objectType");
            }

            var objectsData = connection.Query(sqlQuery, new { Username = username });

            if (objectsData != null && objectsData.Any())
            {
                TreeNode objectsNode = new TreeNode(objectType);

                foreach (var obj in objectsData)
                {
                    TreeNode node = new TreeNode(obj.name.ToString())
                    {
                        Tag = objectType
                    };
                    objectsNode.Nodes.Add(node);
                }

                treeView.Nodes.Add(objectsNode);
            }
        }

        private void LoadParameters(string procedureName)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                var sql = @"
                    SELECT 
                        name,
                        TYPE_NAME(user_type_id) AS type 
                    FROM 
                        sys.parameters 
                    WHERE 
                        object_id = OBJECT_ID(@ProcedureName)";

                var parameters = connection.Query(sql, new { ProcedureName = procedureName });

                flowLayoutPanelParameters.Controls.Clear();

                foreach (var param in parameters)
                {
                    Label label = new()
                    {
                        Text = param.name.ToString() + ":"
                    };
                    TextBox textBox = new()
                    {
                        Name = param.name.ToString()
                    };
                    flowLayoutPanelParameters.Controls.Add(label);
                    flowLayoutPanelParameters.Controls.Add(textBox);
                }

                Button executeButton = new()
                {
                    Text = "Вызвать процедуру"
                };
                executeButton.Click += (sender, e) =>
                {
                    ExecuteProcedure(procedureName);
                };
                flowLayoutPanelParameters.Controls.Add(executeButton);
            }
        }

        private void ExecuteProcedure(string procedureName)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                var parameters = new DynamicParameters();

                foreach (Control control in flowLayoutPanelParameters.Controls)
                {
                    if (control is TextBox textBox)
                    {
                        parameters.Add(textBox.Name, textBox.Text);
                    }
                }

                var dataTable = new DataTable();
                using (var command = new SqlCommand(procedureName, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    foreach (var parameter in parameters.ParameterNames)
                    {
                        command.Parameters.AddWithValue(parameter, parameters.Get<object>(parameter));
                    }

                    using var adapter = new SqlDataAdapter(command);
                    adapter.Fill(dataTable);
                }

                if (dataTable.Rows.Count > 0)
                {
                    dataGridViewData.DataSource = dataTable;
                }
                else
                {
                    MessageBox.Show("Procedure did not return any data.");
                }
            }
        }

        private void LoadTriggerFields(string triggerName)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                var sql = "SELECT OBJECT_NAME(parent_id) AS parent_table FROM sys.triggers WHERE name = @TriggerName";
                var tableName = connection.QueryFirstOrDefault<string>(sql, new { TriggerName = triggerName });

                if (!string.IsNullOrEmpty(tableName))
                {
                    // Load trigger fields UI

                    Button executeTriggerButton = new()
                    {
                        Text = "Execute Trigger"
                    };
                    executeTriggerButton.Click += (sender, e) =>
                    {
                        ExecuteTrigger(triggerName, tableName);
                    };
                    flowLayoutPanelParameters.Controls.Add(executeTriggerButton);
                }
                else
                {
                    MessageBox.Show("Failed to determine the table associated with the trigger.");
                }
            }
        }

        private bool IsAdmin(string username)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                var sql = "SELECT COUNT(*) FROM sys.database_principals WHERE name = @Username AND IS_ROLEMEMBER('db_owner', name) = 1";
                var count = connection.ExecuteScalar<int>(sql, new { Username = username });

                return count > 0;
            }
        }

        private bool IsEmployee(string username)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                var sql = "SELECT COUNT(*) FROM sys.database_principals WHERE name = @Username AND IS_ROLEMEMBER('employee', name) = 1";
                var count = connection.ExecuteScalar<int>(sql, new { Username = username });

                return count > 0;
            }
        }

        private bool IsCustomer(string username)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                var sql = "SELECT COUNT(*) FROM sys.database_principals WHERE name = @Username AND IS_ROLEMEMBER('customer', name) = 1";
                var count = connection.ExecuteScalar<int>(sql, new { Username = username });

                return count > 0;
            }
        }


        private void ExecuteTrigger(string triggerName, string tableName)
        {
            // Implement trigger execution logic
            MessageBox.Show("Trigger executed successfully.");
        }

        private void LoadData(string objectName)
        {
            selectedTableName = objectName;

            if (IsTrigger(objectName))
            {
                // Не выполняем никаких действий для триггера
                return;
            }

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var sql = $"SELECT * FROM {objectName}";

                if (!string.IsNullOrEmpty(selectedColumn))
                {
                    sql += $" ORDER BY {selectedColumn} {selectedSortOrder}";
                }

                var dataAdapter = new SqlDataAdapter(sql, connection);
                var dataTable = new DataTable();
                dataAdapter.Fill(dataTable);

                dataGridViewData.DataSource = dataTable;
            }
        }

        private bool IsTrigger(string objectName)
        {
            // Проверяем, является ли объект триггером
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                var sql = "SELECT COUNT(*) FROM sys.triggers WHERE name = @TriggerName";
                var count = connection.ExecuteScalar<int>(sql, new { TriggerName = objectName });

                return count > 0;
            }
        }

        private void LoadInsertControls(string tableName)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                var sql = $"SELECT COLUMN_NAME, COLUMNPROPERTY(object_id(TABLE_NAME), COLUMN_NAME, 'IsIdentity') AS IsIdentity FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '{tableName}'";
                var columnsInfo = connection.Query(sql);

                foreach (var columnInfo in columnsInfo)
                {
                    if (columnInfo.IsIdentity == 1)
                        continue;

                    Label label = new()
                    {
                        Text = columnInfo.COLUMN_NAME + ":"
                    };
                    TextBox textBox = new()
                    {
                        Name = columnInfo.COLUMN_NAME
                    };
                    flowLayoutPanelParameters.Controls.Add(label);
                    flowLayoutPanelParameters.Controls.Add(textBox);
                }

                Button addButton = new()
                {
                    Text = "Добавить"
                };
                addButton.Click += (sender, e) =>
                {
                    InsertRecord(tableName);
                };
                flowLayoutPanelParameters.Controls.Add(addButton);
            }
        }

        private void InsertRecord(string tableName)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                var columns = new List<string>();
                var values = new List<string>();

                foreach (Control control in flowLayoutPanelParameters.Controls)
                {
                    if (control is TextBox textBox && textBox.Name != dataGridViewData.Columns[0].HeaderText)
                    {
                        columns.Add(textBox.Name);
                        values.Add(textBox.Text);
                    }
                }

                var sql = $"INSERT INTO {tableName} ({string.Join(", ", columns)}) VALUES ({string.Join(", ", values.Select(v => $"'{v}'"))})";

                try
                {
                    using (var command = new SqlCommand(sql, connection))
                    {
                        command.ExecuteNonQuery();
                    }

                    LoadData(selectedTableName);
                    MessageBox.Show("Запись успешно добавлена.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при добавлении записи: {ex.Message}");
                }
            }
        }

        private void DeleteRecord(string tableName, int rowIndex)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                var primaryKey = dataGridViewData.Rows[rowIndex].Cells[0].Value;

                var sql = $"DELETE FROM {tableName} WHERE {dataGridViewData.Columns[0].HeaderText} = '{primaryKey}'";

                try
                {
                    using (var command = new SqlCommand(sql, connection))
                    {
                        command.ExecuteNonQuery();
                    }

                    LoadData(selectedTableName);
                    MessageBox.Show("Запись успешно удалена.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при удалении записи: {ex.Message}");
                }
            }
        }

        private void UpdateRecord(string tableName, int rowIndex, string columnName, string newValue)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                var primaryKey = dataGridViewData.Rows[rowIndex].Cells[0].Value;

                var sql = $"UPDATE {tableName} SET {columnName} = '{newValue}' WHERE {dataGridViewData.Columns[0].HeaderText} = '{primaryKey}'";

                try
                {
                    using (var command = new SqlCommand(sql, connection))
                    {
                        command.ExecuteNonQuery();
                    }

                    LoadData(tableName);
                    MessageBox.Show("Запись успешно обновлена.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при обновлении записи: {ex.Message}");
                }
            }
        }

        private void LoadSortColumns()
        {
            foreach (DataGridViewColumn column in dataGridViewData.Columns)
            {
                comboBoxSort.Items.Add(column.HeaderText);
            }
        }

        private void LoadSortOrders()
        {
            comboBoxSortOrder.Items.AddRange(new string[] {
                "Ascending",
                "Descending"
            });
        }

        private void treeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            flowLayoutPanelParameters.Controls.Clear();

            if (e.Node.Tag != null)
            {
                string selectedType = e.Node.Tag.ToString();
                string selectedObjectName = e.Node.Text;

                if (selectedType == "Procedure")
                {
                    LoadParameters(selectedObjectName);
                }
                else if (selectedType == "Trigger")
                {
                    LoadTriggerFields(selectedObjectName);
                }
                else
                {
                    LoadData(selectedObjectName);
                    LoadInsertControls(selectedObjectName);
                    LoadSortColumns();
                }
            }
        }

        private void DataGridViewData_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow selectedRow = dataGridViewData.Rows[e.RowIndex];
                string columnName = dataGridViewData.Columns[e.ColumnIndex].HeaderText;
                string newValue = selectedRow.Cells[e.ColumnIndex].Value.ToString();
                UpdateRecord(selectedTableName, e.RowIndex, columnName, newValue);
            }
        }

        private void DataGridViewData_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            // Здесь можно обработать вставку записи
        }

        private void DataGridViewData_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete && dataGridViewData.SelectedRows.Count > 0)
            {
                int rowIndex = dataGridViewData.SelectedRows[0].Index;
                DeleteRecord(selectedTableName, rowIndex);
            }
        }

        private void comboBoxSort_SelectedIndexChanged(object sender, EventArgs e)
        {
            selectedColumn = comboBoxSort.SelectedItem.ToString();
            LoadData(selectedTableName);
        }

        private void comboBoxSortOrder_SelectedIndexChanged(object sender, EventArgs e)
        {
            selectedSortOrder = comboBoxSortOrder.SelectedItem.ToString() == "Ascending" ? "ASC" : "DESC";
            LoadData(selectedTableName);
        }

        private void comboBoxSortData_SelectedIndexChanged(object sender, EventArgs e)
        {
            var comboBox = sender as ComboBox;
            if (comboBox != null && comboBox.SelectedItem != null)
            {
                var selectedSortOption = comboBox.SelectedItem.ToString();

                switch (selectedSortOption)
                {
                    case "Alphabetical":
                        LoadData(selectedTableName);
                        break;
                    case "Reverse Alphabetical":
                        LoadData(selectedTableName);
                        break;
                    case "First 5 Rows":
                        LoadTopNData(selectedTableName, 5);
                        break;
                    case "First 10 Rows":
                        LoadTopNData(selectedTableName, 10);
                        break;
                    default:
                        break;
                }
            }
        }

        private void LoadTopNData(string tableName, int n)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var sql = $"SELECT TOP {n} * FROM {tableName}";

                if (!string.IsNullOrEmpty(selectedColumn))
                {
                    sql += $" ORDER BY {selectedColumn} {selectedSortOrder}";
                }

                var dataAdapter = new SqlDataAdapter(sql, connection);
                var dataTable = new DataTable();
                dataAdapter.Fill(dataTable);

                dataGridViewData.DataSource = dataTable;
            }
        }

        private void textBoxSearch_TextChanged(object sender, EventArgs e)
        {
            string searchText = textBoxSearch.Text.Trim();

            if (!string.IsNullOrEmpty(selectedTableName))
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    var sql = $"SELECT * FROM {selectedTableName} WHERE ";
                    bool isFirstCondition = true;

                    foreach (DataGridViewColumn column in dataGridViewData.Columns)
                    {
                        if (column.Index != 0)
                        {
                            if (!isFirstCondition)
                            {
                                sql += " OR ";
                            }
                            sql += $"{column.HeaderText} LIKE '%{searchText}%'";
                            isFirstCondition = false;
                        }
                    }

                    if (!string.IsNullOrEmpty(searchText))
                    {
                        var dataAdapter = new SqlDataAdapter(sql, connection);
                        var dataTable = new DataTable();
                        dataAdapter.Fill(dataTable);
                        dataGridViewData.DataSource = dataTable;
                    }
                    else
                    {
                        LoadData(selectedTableName);
                    }
                }
            }
        }

        private void ShowDatabaseConfigForm()
        {
            DatabaseConfigForm configForm = new();
            configForm.ConnectionStringSaved += ConfigForm_ConnectionStringSaved;
            configForm.ShowDialog();
        }

        private void ConfigForm_ConnectionStringSaved(object sender, string connectionString)
        {
            MessageBox.Show("Полученная строка подключения: " + connectionString);
            DataTable tableNames = GetTableNames(connectionString);
            dataGridViewData.DataSource = tableNames;
        }

        private static DataTable GetTableNames(string connectionString)
        {
            DataTable tableNames = new();

            using (SqlConnection connection = new(connectionString))
            {
                try
                {
                    connection.Open();
                    SqlCommand command = new("SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE'", connection);
                    SqlDataReader reader = command.ExecuteReader();
                    tableNames.Load(reader);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при получении списка таблиц: " + ex.Message);
                }
            }

            return tableNames;
        }

        private void buttonOpenConfigForm_Click(object sender, EventArgs e)
        {
            ShowDatabaseConfigForm();
        }

        private void logoutBtn_Click(object sender, EventArgs e)
        {
            Close();
            LoginForm loginForm = new(connectionString);
            loginForm.Show();
        }
    }
}
