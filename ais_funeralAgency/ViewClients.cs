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
    public partial class ViewClients : Form
    {
        public ViewClients()
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

        public void UpdateClients()
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
            string query2 = $"UPDATE Clients SET Clients.name_clients='{dataGridView1.Rows[Convert.ToInt32(index_selected_rows)].Cells[1].Value.ToString()}', Clients.phone_clients='{dataGridView1.Rows[Convert.ToInt32(index_selected_rows)].Cells[2].Value.ToString()}', Clients.address_clients='{dataGridView1.Rows[Convert.ToInt32(index_selected_rows)].Cells[3].Value.ToString()}' WHERE (Clients.id_clients='{redact_id}')";
            // объект для выполнения SQL-запроса
            MySqlCommand command = new MySqlCommand(query2, conn);
            // выполняем запрос
            try
            {
                command.ExecuteNonQuery();
            }
            catch (MySql.Data.MySqlClient.MySqlException ex) when (ex.Message == "Data too long for column 'phone_clients' at row 1")
            {
                MessageBox.Show($"Слишком длиное значение телефона! \nОшибка: {ex.Message}", "Ошибка обновления данных", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (MySql.Data.MySqlClient.MySqlException ex) when (ex.Message.EndsWith("for key 'Clients.phone_clients'"))
            {
                MessageBox.Show($"Клиент с таким телефоном уже существует! \nОшибка: {ex.Message}", "Ошибка обновления данных", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            //Чистим виртуальную таблицу
            table.Clear();
            //Вызываем метод получения записей, который вновь заполнит таблицу
            GetListUsers();
        }

        public void GetListUsers()
        {
            //Запрос для вывода строк в БД
            string commandStr = "SELECT Clients.id_clients AS 'Номер клиента', Clients.name_clients AS 'ФИО клиента', Clients.phone_clients AS 'Телефон клиента', Clients.address_clients AS 'Адрес клиента' FROM Clients";
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
            int count_rows = dataGridView1.RowCount;
            label4.Text = (count_rows).ToString();

        }

        private void ViewClients_Load(object sender, EventArgs e)
        {
            // строка подключения к БД
            string connStr = "server=chuc.caseum.ru;port=33333;user=st_3_20_11;database=is_3_20_st11_KURS;password=67959087";
            // создаём объект для подключения к БД
            conn = new MySqlConnection(connStr);
            //Вызываем метод для заполнение дата Грида
            GetListUsers();
            //Растягивание полей грида
            dataGridView1.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dataGridView1.Columns[3].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            //Ширина полей
            dataGridView1.Columns[0].Width = 80;
            dataGridView1.Columns[2].Width = 175;
            //Режим для полей "Только для чтения"
            dataGridView1.Columns[0].ReadOnly = true;
            //Убираем заголовки строк
            dataGridView1.RowHeadersVisible = true;
            //Показываем заголовки столбцов
            dataGridView1.ColumnHeadersVisible = true;
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
                UpdateClients();
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

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            table.Clear();
            string commandStr = $"SELECT Clients.id_clients AS 'Номер клиента', Clients.name_clients AS 'ФИО клиента', Clients.phone_clients AS 'Телефон клиента', Clients.address_clients AS 'Адрес клиента' FROM Clients WHERE Clients.name_clients LIKE '%{textBox1.Text}%' AND Clients.phone_clients LIKE '%{textBox2.Text}%'";
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
            string commandStr = $"SELECT Clients.id_clients AS 'Номер клиента', Clients.name_clients AS 'ФИО клиента', Clients.phone_clients AS 'Телефон клиента', Clients.address_clients AS 'Адрес клиента' FROM Clients WHERE Clients.name_clients LIKE '%{textBox1.Text}%' AND Clients.phone_clients LIKE '%{textBox2.Text}%'";
            //Открываем соединение
            conn.Open();
            //Объявляем команду, которая выполнить запрос в соединении conn
            MyDA.SelectCommand = new MySqlCommand(commandStr, conn);
            //Заполняем таблицу записями из БД
            MyDA.Fill(table);
            conn.Close();
        }
    }
}
