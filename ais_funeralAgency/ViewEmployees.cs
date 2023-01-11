using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace ais_funeralAgency
{
    public partial class ViewEmployees : Form
    {
        public ViewEmployees()
        {
            InitializeComponent();
        }

        //Переменная соединения
        MySqlConnection conn;
        //DataAdapter представляет собой объект Command , получающий данные из источника данных.
        private MySqlDataAdapter MyDA = new MySqlDataAdapter();
        //Объявление BindingSource, основная его задача, это обеспечить унифицированный доступ к источнику данных.
        private BindingSource bSource = new BindingSource();
        //DataSet - расположенное в оперативной памяти представление данных, обеспечивающее согласованную реляционную программную 
        //модель независимо от источника данных.DataSet представляет полный набор данных, включая таблицы, содержащие, упорядочивающие 
        //и ограничивающие данные, а также связи между таблицами.
        private DataSet ds = new DataSet();
        //Представляет одну таблицу данных в памяти.
        private DataTable table = new DataTable();
        //Переменная для ID записи в БД, выбранной в гриде. Пока она не содердит значения, лучше его инициализировать с 0
        //что бы в БД не отправлялся null
        string id_selected_rows = "0";

        public void ManagerRole(int role)
        {
            switch (role)
            {
                //И в зависимости от того, какая роль (цифра) хранится в поле класса и передана в метод, показываются те или иные кнопки.
                //Вы можете скрыть их и не отображать вообще, здесь они просто выключены
                case 0:
                    dataGridView1.AllowUserToAddRows = true;
                    break;
            }
        }

        //Метод получения ID выделенной строки, для последующего вызова его в нужных методах
        public void GetSelectedIDString()
        {
            //Переменная для индекс выбранной строки в гриде
            string index_selected_rows;
            //Индекс выбранной строки
            index_selected_rows = dataGridView1.SelectedCells[0].RowIndex.ToString();
            //ID конкретной записи в Базе данных, на основании индекса строки
            id_selected_rows = dataGridView1.Rows[Convert.ToInt32(index_selected_rows)].Cells[0].Value.ToString();
            //Указываем ID выделенной строки в метке
            label3.Text = id_selected_rows;
        }

        public void UpdateEmployees()
        {
            //Получаем ID изменяемого студента
            string redact_id = id_selected_rows;
            //Переменная для индекс выбранной строки в гриде
            string index_selected_rows;
            //Индекс выбранной строки
            index_selected_rows = dataGridView1.SelectedCells[0].RowIndex.ToString();
            // устанавливаем соединение с БД
            conn.Open();
            // запрос обновления данных
            string query2 = $"UPDATE Employees SET Employees.name_employees='{dataGridView1.Rows[Convert.ToInt32(index_selected_rows)].Cells[1].Value.ToString()}', Employees.phone_employees='{dataGridView1.Rows[Convert.ToInt32(index_selected_rows)].Cells[2].Value.ToString()}', Employees.status_employees='{dataGridView1.Rows[Convert.ToInt32(index_selected_rows)].Cells[6].Value}' WHERE (Employees.id_employees='{redact_id}')";
            // объект для выполнения SQL-запроса
            MySqlCommand command = new MySqlCommand(query2, conn);
            //Выполняем запрос и проверяе мего на ошибки
            try
            {
                command.ExecuteNonQuery();
            }
            catch (MySql.Data.MySqlClient.MySqlException ex) when (ex.Message == "Data too long for column 'phone_employees' at row 1")
            {
                MessageBox.Show($"Слишком длиное значение телефона! \nОшибка: {ex.Message}", "Ошибка обновления данных", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (MySql.Data.MySqlClient.MySqlException ex) when (ex.Message.EndsWith("for key 'Employees.phone_employees'"))
            {
                MessageBox.Show($"Работник с таким телефоном уже существует! \nОшибка: {ex.Message}", "Ошибка обновления данных", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка обновления данных", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            
            // закрываем подключение к БД
            conn.Close();
            //Обновляем DataGrid
            reload_list();
        }

        public void reload_list()
        {
            //Обнуляем id выбраной записи
            id_selected_rows = "0";
            label3.Text = "";
            //Зануляем штуки для фильтров
            textBox1.Text = null;
            textBox2.Text = null;
            comboBox1.Text = null;
            //Чистим виртуальную таблицу
            table.Clear();
            //Вызываем метод получения записей, который вновь заполнит таблицу
            GetListUsers();
        }

        public void GetListUsers()
        {
            //Запрос для вывода строк в БД
            string commandStr = "SELECT Employees.id_employees AS 'Номер работника', Employees.name_employees AS 'ФИО работника', Employees.phone_employees AS 'Телефон работника', Employees.position_employees AS 'Номер должности', Positions.title_positions AS 'Название должности', Positions.amount_positions AS 'Зарплата', Employees.status_employees AS 'Номер статуса', StatusEmployees.title_statusEmployees AS 'Статус' FROM Employees LEFT JOIN Accounts ON Employees.account_employees = Accounts.id_accounts LEFT JOIN Positions ON Employees.position_employees = Positions.id_positions LEFT JOIN StatusEmployees ON Employees.status_employees = StatusEmployees.id_statusEmployees";
            //Открываем соединение
            conn.Open();
            //Объявляем команду, которая выполнить запрос в соединении conn
            MyDA.SelectCommand = new MySqlCommand(commandStr, conn);
            //Заполняем таблицу записями из БД
            MyDA.Fill(table);
            //Указываем, что источником данных в bindingsource является заполненная выше таблица
            bSource.DataSource = table;
            //Указываем, что источником данных ДатаГрида является bindingsource
            dataGridView1.DataSource = bSource;
            //Закрываем соединение
            conn.Close();
            //Отражаем количество записей в ДатаГриде
            int count_rows = dataGridView1.RowCount - 1;
            label4.Text = (count_rows).ToString();
        }

        private void ViewEmployees_Load(object sender, EventArgs e)
        {
            // строка подключения к БД
            string connStr = "server=chuc.caseum.ru;port=33333;user=st_3_20_11;database=is_3_20_st11_KURS;password=67959087";
            // создаём объект для подключения к БД
            conn = new MySqlConnection(connStr);
            //Вызываем метод для заполнение дата Грида
            GetListUsers();
            //Растягивание полей грида
            dataGridView1.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dataGridView1.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dataGridView1.Columns[4].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dataGridView1.Columns[5].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dataGridView1.Columns[7].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            //Ширина полей
            dataGridView1.Columns[0].Width = 110;
            dataGridView1.Columns[3].Width = 115;
            //Режим для полей "Только для чтения"
            dataGridView1.Columns[0].ReadOnly = true;
            dataGridView1.Columns[3].ReadOnly = true;
            dataGridView1.Columns[4].ReadOnly = true;
            dataGridView1.Columns[5].ReadOnly = true;
            dataGridView1.Columns[7].ReadOnly = true;
            //Убираем заголовки строк
            dataGridView1.RowHeadersVisible = true;
            //Показываем заголовки столбцов
            dataGridView1.ColumnHeadersVisible = true;
            //Передаём номер роли в метод
            ManagerRole(Auth.auth_role);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Hide();
            Main main = new Main();
            main.Show();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            reload_list();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show($"Изменить данные в заказе № {id_selected_rows}?", "Изменеие данных", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                UpdateEmployees();
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            //Магические строки
            try
            {
                dataGridView1.CurrentCell = dataGridView1[e.ColumnIndex, e.RowIndex];
            }
            catch
            {

            }
            dataGridView1.CurrentRow.Selected = true;
            //Метод получения ID выделенной строки в глобальную переменную
            GetSelectedIDString();
        }

        private void dataGridView1_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            e.Control.KeyPress -= new KeyPressEventHandler(ColumnKeyPress);
            if (dataGridView1.CurrentCell.ColumnIndex == 6)
            {
                TextBox textBox = e.Control as TextBox;
                if (textBox != null)
                {
                    textBox.KeyPress += new KeyPressEventHandler(ColumnKeyPress);
                }
            }
        }

        public void ColumnKeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            table.Clear();
            string commandStr;
            if (comboBox1.SelectedIndex <= 0)
            {
                commandStr = $"SELECT Employees.id_employees AS 'Номер работника', Employees.name_employees AS 'ФИО работника', Employees.phone_employees AS 'Телефон работника', Employees.position_employees AS 'Номер должности', Positions.title_positions AS 'Название должности', Positions.amount_positions AS 'Зарплата', Employees.status_employees AS 'Номер статуса', StatusEmployees.title_statusEmployees AS 'Статус' FROM Employees LEFT JOIN Accounts ON Employees.account_employees = Accounts.id_accounts LEFT JOIN Positions ON Employees.position_employees = Positions.id_positions LEFT JOIN StatusEmployees ON Employees.status_employees = StatusEmployees.id_statusEmployees WHERE Employees.name_employees LIKE '%{textBox1.Text}%' AND Employees.phone_employees LIKE '%{textBox2.Text}%'";
            }
            else
            {
                commandStr = $"SELECT Employees.id_employees AS 'Номер работника', Employees.name_employees AS 'ФИО работника', Employees.phone_employees AS 'Телефон работника', Employees.position_employees AS 'Номер должности', Positions.title_positions AS 'Название должности', Positions.amount_positions AS 'Зарплата', Employees.status_employees AS 'Номер статуса', StatusEmployees.title_statusEmployees AS 'Статус' FROM Employees LEFT JOIN Accounts ON Employees.account_employees = Accounts.id_accounts LEFT JOIN Positions ON Employees.position_employees = Positions.id_positions LEFT JOIN StatusEmployees ON Employees.status_employees = StatusEmployees.id_statusEmployees WHERE Employees.name_employees LIKE '%{textBox1.Text}%' AND Employees.phone_employees LIKE '%{textBox2.Text}%' AND Employees.status_employees = {comboBox1.SelectedIndex};";
            }
            //Открываем соединение
            conn.Open();
            //Объявляем команду, которая выполнить запрос в соединении conn
            MyDA.SelectCommand = new MySqlCommand(commandStr, conn);
            //Заполняем таблицу записями из БД
            MyDA.Fill(table);
            conn.Close();
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            table.Clear();
            string commandStr;
            if (comboBox1.SelectedIndex <= 0)
            {
                commandStr = $"SELECT Employees.id_employees AS 'Номер работника', Employees.name_employees AS 'ФИО работника', Employees.phone_employees AS 'Телефон работника', Employees.position_employees AS 'Номер должности', Positions.title_positions AS 'Название должности', Positions.amount_positions AS 'Зарплата', Employees.status_employees AS 'Номер статуса', StatusEmployees.title_statusEmployees AS 'Статус' FROM Employees LEFT JOIN Accounts ON Employees.account_employees = Accounts.id_accounts LEFT JOIN Positions ON Employees.position_employees = Positions.id_positions LEFT JOIN StatusEmployees ON Employees.status_employees = StatusEmployees.id_statusEmployees WHERE Employees.name_employees LIKE '%{textBox1.Text}%' AND Employees.phone_employees LIKE '%{textBox2.Text}%'";
            }
            else
            {
                commandStr = $"SELECT Employees.id_employees AS 'Номер работника', Employees.name_employees AS 'ФИО работника', Employees.phone_employees AS 'Телефон работника', Employees.position_employees AS 'Номер должности', Positions.title_positions AS 'Название должности', Positions.amount_positions AS 'Зарплата', Employees.status_employees AS 'Номер статуса', StatusEmployees.title_statusEmployees AS 'Статус' FROM Employees LEFT JOIN Accounts ON Employees.account_employees = Accounts.id_accounts LEFT JOIN Positions ON Employees.position_employees = Positions.id_positions LEFT JOIN StatusEmployees ON Employees.status_employees = StatusEmployees.id_statusEmployees WHERE Employees.name_employees LIKE '%{textBox1.Text}%' AND Employees.phone_employees LIKE '%{textBox2.Text}%' AND Employees.status_employees = {comboBox1.SelectedIndex};";
            }
            //Открываем соединение
            conn.Open();
            //Объявляем команду, которая выполнить запрос в соединении conn
            MyDA.SelectCommand = new MySqlCommand(commandStr, conn);
            //Заполняем таблицу записями из БД
            MyDA.Fill(table);
            conn.Close();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (comboBox1.SelectedIndex)
            {
                case 0:
                    table.Clear();
                    //Запрос для вывода строк в БД
                    string commandStr = $"SELECT Employees.id_employees AS 'Номер работника', Employees.name_employees AS 'ФИО работника', Employees.phone_employees AS 'Телефон работника', Employees.position_employees AS 'Номер должности', Positions.title_positions AS 'Название должности', Positions.amount_positions AS 'Зарплата', Employees.status_employees AS 'Номер статуса', StatusEmployees.title_statusEmployees AS 'Статус' FROM Employees LEFT JOIN Accounts ON Employees.account_employees = Accounts.id_accounts LEFT JOIN Positions ON Employees.position_employees = Positions.id_positions LEFT JOIN StatusEmployees ON Employees.status_employees = StatusEmployees.id_statusEmployees WHERE Employees.name_employees LIKE '%{textBox1.Text}%' AND Employees.phone_employees LIKE '%{textBox2.Text}%'";
                    //Открываем соединение
                    conn.Open();
                    //Объявляем команду, которая выполнить запрос в соединении conn
                    MyDA.SelectCommand = new MySqlCommand(commandStr, conn);
                    //Заполняем таблицу записями из БД
                    MyDA.Fill(table);
                    conn.Close();
                    break;
                case 1:
                    table.Clear();
                    //Запрос для вывода строк в БД
                    string commandStr1 = $"SELECT Employees.id_employees AS 'Номер работника', Employees.name_employees AS 'ФИО работника', Employees.phone_employees AS 'Телефон работника', Employees.position_employees AS 'Номер должности', Positions.title_positions AS 'Название должности', Positions.amount_positions AS 'Зарплата', Employees.status_employees AS 'Номер статуса', StatusEmployees.title_statusEmployees AS 'Статус' FROM Employees LEFT JOIN Accounts ON Employees.account_employees = Accounts.id_accounts LEFT JOIN Positions ON Employees.position_employees = Positions.id_positions LEFT JOIN StatusEmployees ON Employees.status_employees = StatusEmployees.id_statusEmployees WHERE Employees.name_employees LIKE '%{textBox1.Text}%' AND Employees.phone_employees LIKE '%{textBox2.Text}%' AND Employees.status_employees = {comboBox1.SelectedIndex};";
                    //Открываем соединение
                    conn.Open();
                    //Объявляем команду, которая выполнить запрос в соединении conn
                    MyDA.SelectCommand = new MySqlCommand(commandStr1, conn);
                    //Заполняем таблицу записями из БД
                    MyDA.Fill(table);
                    conn.Close();
                    break;
                case 2:
                    table.Clear();
                    //Запрос для вывода строк в БД
                    string commandStr2 = $"SELECT Employees.id_employees AS 'Номер работника', Employees.name_employees AS 'ФИО работника', Employees.phone_employees AS 'Телефон работника', Employees.position_employees AS 'Номер должности', Positions.title_positions AS 'Название должности', Positions.amount_positions AS 'Зарплата', Employees.status_employees AS 'Номер статуса', StatusEmployees.title_statusEmployees AS 'Статус' FROM Employees LEFT JOIN Accounts ON Employees.account_employees = Accounts.id_accounts LEFT JOIN Positions ON Employees.position_employees = Positions.id_positions LEFT JOIN StatusEmployees ON Employees.status_employees = StatusEmployees.id_statusEmployees WHERE Employees.name_employees LIKE '%{textBox1.Text}%' AND Employees.phone_employees LIKE '%{textBox2.Text}%' AND Employees.status_employees = {comboBox1.SelectedIndex};";
                    //Открываем соединение
                    conn.Open();
                    //Объявляем команду, которая выполнить запрос в соединении conn
                    MyDA.SelectCommand = new MySqlCommand(commandStr2, conn);
                    //Заполняем таблицу записями из БД
                    MyDA.Fill(table);
                    conn.Close();
                    break;
                case 3:
                    table.Clear();
                    //Запрос для вывода строк в БД
                    string commandStr3 = $"SELECT Employees.id_employees AS 'Номер работника', Employees.name_employees AS 'ФИО работника', Employees.phone_employees AS 'Телефон работника', Employees.position_employees AS 'Номер должности', Positions.title_positions AS 'Название должности', Positions.amount_positions AS 'Зарплата', Employees.status_employees AS 'Номер статуса', StatusEmployees.title_statusEmployees AS 'Статус' FROM Employees LEFT JOIN Accounts ON Employees.account_employees = Accounts.id_accounts LEFT JOIN Positions ON Employees.position_employees = Positions.id_positions LEFT JOIN StatusEmployees ON Employees.status_employees = StatusEmployees.id_statusEmployees WHERE Employees.name_employees LIKE '%{textBox1.Text}%' AND Employees.phone_employees LIKE '%{textBox2.Text}%' AND Employees.status_employees = {comboBox1.SelectedIndex};";
                    //Открываем соединение
                    conn.Open();
                    //Объявляем команду, которая выполнить запрос в соединении conn
                    MyDA.SelectCommand = new MySqlCommand(commandStr3, conn);
                    //Заполняем таблицу записями из БД
                    MyDA.Fill(table);
                    conn.Close();
                    break;
                case 4:
                    table.Clear();
                    //Запрос для вывода строк в БД
                    string commandStr4 = $"SELECT Employees.id_employees AS 'Номер работника', Employees.name_employees AS 'ФИО работника', Employees.phone_employees AS 'Телефон работника', Employees.position_employees AS 'Номер должности', Positions.title_positions AS 'Название должности', Positions.amount_positions AS 'Зарплата', Employees.status_employees AS 'Номер статуса', StatusEmployees.title_statusEmployees AS 'Статус' FROM Employees LEFT JOIN Accounts ON Employees.account_employees = Accounts.id_accounts LEFT JOIN Positions ON Employees.position_employees = Positions.id_positions LEFT JOIN StatusEmployees ON Employees.status_employees = StatusEmployees.id_statusEmployees WHERE Employees.name_employees LIKE '%{textBox1.Text}%' AND Employees.phone_employees LIKE '%{textBox2.Text}%' AND Employees.status_employees = {comboBox1.SelectedIndex};";
                    //Открываем соединение
                    conn.Open();
                    //Объявляем команду, которая выполнить запрос в соединении conn
                    MyDA.SelectCommand = new MySqlCommand(commandStr4, conn);
                    //Заполняем таблицу записями из БД
                    MyDA.Fill(table);
                    conn.Close();
                    break;
            }
        }
    }
}
