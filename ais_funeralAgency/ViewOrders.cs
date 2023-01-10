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
    public partial class ViewOrders : Form
    {
        public ViewOrders()
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
        
        public void UpdateOrders()
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
            string query2 = $"UPDATE Orders SET Orders.price_orders='{dataGridView1.Rows[Convert.ToInt32(index_selected_rows)].Cells[3].Value}', Orders.nameDeceased_orders='{dataGridView1.Rows[Convert.ToInt32(index_selected_rows)].Cells[4].Value.ToString()}', Orders.lifeYears_orders='{dataGridView1.Rows[Convert.ToInt32(index_selected_rows)].Cells[5].Value.ToString()}', Orders.comments_orders='{dataGridView1.Rows[Convert.ToInt32(index_selected_rows)].Cells[6].Value.ToString()}', Orders.status_orders='{dataGridView1.Rows[Convert.ToInt32(index_selected_rows)].Cells[12].Value}', Orders.nameEmpl_orders='{dataGridView1.Rows[Convert.ToInt32(index_selected_rows)].Cells[14].Value}', Orders.nameClient_orders='{dataGridView1.Rows[Convert.ToInt32(index_selected_rows)].Cells[19].Value}' WHERE (Orders.id_orders='{redact_id}')";
            // объект для выполнения SQL-запроса
            MySqlCommand command = new MySqlCommand(query2, conn);
            // выполняем запрос
            command.ExecuteNonQuery();
            // закрываем подключение к БД
            conn.Close();
            //Обновляем DataGrid
            reload_list();
        }

        //Метод обновления DataGreed
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
            string commandStr = "SELECT Orders.id_orders AS 'Номер заказа', Orders.dataStart_orders AS 'Дата создания заказа', Orders.dataEnd_orders AS 'Дата завершения заказа', Orders.price_orders AS 'Итоговая сумма', Orders.nameDeceased_orders AS 'Имя умршего', Orders.lifeYears_orders AS 'Годы жизни умершего', Orders.comments_orders AS 'Комментарий к закзау', Type.id_type AS 'Номер услуги', Type.title_type AS 'Название услуги', Type.price_type AS 'Стоимость услуги', Type.amount_type AS 'Количество товара', TypeCategories.title__typeCategories AS 'Категория', Orders.status_orders AS 'Номер статуса', Status.title_status AS 'Статус заказа', Orders.nameEmpl_orders AS 'Номер сотрудника', Employees.name_employees AS 'Имя сотрудника', Employees.phone_employees AS 'Телефон сотрудника', Positions.title_positions AS 'Должность', StatusEmployees.title_statusEmployees AS 'Статус сотрудника', Orders.nameClient_orders AS 'Номер клиента', Clients.name_clients AS 'Имя клиента', Clients.phone_clients AS 'Телефон клиента', Clients.address_clients AS 'Адрес клиента' FROM Orders LEFT JOIN Orders_Type ON Orders.id_orders = Orders_Type.Orders_Type_orderID LEFT JOIN Type ON Orders_Type.Orders_Type_typeID = Type.id_type LEFT JOIN TypeCategories ON Type.categories_type = TypeCategories.id_typeCategories LEFT JOIN Status ON Orders.status_orders = Status.id_status LEFT JOIN Employees ON Orders.nameEmpl_orders = Employees.id_employees LEFT JOIN Positions ON Employees.position_employees = Positions.id_positions LEFT JOIN StatusEmployees ON Employees.status_employees = StatusEmployees.id_statusEmployees LEFT JOIN Clients ON Orders.nameClient_orders = Clients.id_clients;";
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

        private void ViewOrders_Load(object sender, EventArgs e)
        {
            // строка подключения к БД
            string connStr = "server=chuc.caseum.ru;port=33333;user=st_3_20_11;database=is_3_20_st11_KURS;password=67959087";
            // создаём объект для подключения к БД
            conn = new MySqlConnection(connStr);
            //Вызываем метод для заполнение дата Грида
            GetListUsers();
            //Режим для полей "Только для чтения"
            dataGridView1.Columns[0].ReadOnly = true;
            dataGridView1.Columns[1].ReadOnly = true;
            dataGridView1.Columns[2].ReadOnly = true;
            dataGridView1.Columns[7].ReadOnly = true;
            dataGridView1.Columns[8].ReadOnly = true;
            dataGridView1.Columns[9].ReadOnly = true;
            dataGridView1.Columns[10].ReadOnly = true;
            //Ширина полей
            dataGridView1.Columns[0].Width = 80;
            dataGridView1.Columns[1].Width = 110;
            dataGridView1.Columns[2].Width = 110;
            dataGridView1.Columns[4].Width = 300;
            dataGridView1.Columns[5].Width = 210;
            dataGridView1.Columns[6].Width = 150;
            dataGridView1.Columns[7].Width = 80;
            dataGridView1.Columns[8].Width = 180;
            dataGridView1.Columns[9].Width = 110;
            dataGridView1.Columns[10].Width = 116;
            dataGridView1.Columns[11].Width = 110;
            dataGridView1.Columns[12].Width = 80;
            dataGridView1.Columns[13].Width = 110;
            dataGridView1.Columns[14].Width = 115;
            dataGridView1.Columns[15].Width = 300;
            dataGridView1.Columns[16].Width = 140;
            dataGridView1.Columns[17].Width = 150;
            dataGridView1.Columns[18].Width = 130;
            dataGridView1.Columns[19].Width = 90;
            dataGridView1.Columns[20].Width = 300;
            dataGridView1.Columns[21].Width = 140;
            dataGridView1.Columns[22].Width = 300;
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
                UpdateOrders();
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

        public void dataGridView1_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            e.Control.KeyPress -= new KeyPressEventHandler(ColumnKeyPress);
            if (dataGridView1.CurrentCell.ColumnIndex == 3 || dataGridView1.CurrentCell.ColumnIndex == 12)
            {
                TextBox textBox = e.Control as TextBox;
                if (textBox != null)
                {
                    textBox.KeyPress += new KeyPressEventHandler(ColumnKeyPress);
                }
            }
        }

        public void ColumnKeyPress (object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            //(dataGridView1.DataSource as DataTable).DefaultView.RowFilter = $"Clients.phone_clients LIKE '%{textBox2.Text}%'";
            table.Clear();
            string commandStr;
            if (comboBox1.SelectedIndex <= 0)
            {
                commandStr = $"SELECT Orders.id_orders AS 'Номер заказа', Orders.dataStart_orders AS 'Дата создания заказа', Orders.dataEnd_orders AS 'Дата завершения заказа', Orders.price_orders AS 'Итоговая сумма', Orders.nameDeceased_orders AS 'Имя умршего', Orders.lifeYears_orders AS 'Годы жизни умершего', Orders.comments_orders AS 'Комментарий к закзау', Type.id_type AS 'Номер услуги', Type.title_type AS 'Название услуги', Type.price_type AS 'Стоимость услуги', Type.amount_type AS 'Количество товара', TypeCategories.title__typeCategories AS 'Категория', Orders.status_orders AS 'Номер статуса', Status.title_status AS 'Статус заказа', Orders.nameEmpl_orders AS 'Номер сотрудника', Employees.name_employees AS 'Имя сотрудника', Employees.phone_employees AS 'Телефон сотрудника', Positions.title_positions AS 'Должность', StatusEmployees.title_statusEmployees AS 'Статус сотрудника', Orders.nameClient_orders AS 'Номер клиента', Clients.name_clients AS 'Имя клиента', Clients.phone_clients AS 'Телефон клиента', Clients.address_clients AS 'Адрес клиента' FROM Orders LEFT JOIN Orders_Type ON Orders.id_orders = Orders_Type.Orders_Type_orderID LEFT JOIN Type ON Orders_Type.Orders_Type_typeID = Type.id_type LEFT JOIN TypeCategories ON Type.categories_type = TypeCategories.id_typeCategories LEFT JOIN Status ON Orders.status_orders = Status.id_status LEFT JOIN Employees ON Orders.nameEmpl_orders = Employees.id_employees LEFT JOIN Positions ON Employees.position_employees = Positions.id_positions LEFT JOIN StatusEmployees ON Employees.status_employees = StatusEmployees.id_statusEmployees LEFT JOIN Clients ON Orders.nameClient_orders = Clients.id_clients WHERE Orders.id_orders LIKE '%{textBox1.Text}%' AND Clients.phone_clients LIKE '%{textBox2.Text}%';";
            }
            else
            {
                commandStr = $"SELECT Orders.id_orders AS 'Номер заказа', Orders.dataStart_orders AS 'Дата создания заказа', Orders.dataEnd_orders AS 'Дата завершения заказа', Orders.price_orders AS 'Итоговая сумма', Orders.nameDeceased_orders AS 'Имя умршего', Orders.lifeYears_orders AS 'Годы жизни умершего', Orders.comments_orders AS 'Комментарий к закзау', Type.id_type AS 'Номер услуги', Type.title_type AS 'Название услуги', Type.price_type AS 'Стоимость услуги', Type.amount_type AS 'Количество товара', TypeCategories.title__typeCategories AS 'Категория', Orders.status_orders AS 'Номер статуса', Status.title_status AS 'Статус заказа', Orders.nameEmpl_orders AS 'Номер сотрудника', Employees.name_employees AS 'Имя сотрудника', Employees.phone_employees AS 'Телефон сотрудника', Positions.title_positions AS 'Должность', StatusEmployees.title_statusEmployees AS 'Статус сотрудника', Orders.nameClient_orders AS 'Номер клиента', Clients.name_clients AS 'Имя клиента', Clients.phone_clients AS 'Телефон клиента', Clients.address_clients AS 'Адрес клиента' FROM Orders LEFT JOIN Orders_Type ON Orders.id_orders = Orders_Type.Orders_Type_orderID LEFT JOIN Type ON Orders_Type.Orders_Type_typeID = Type.id_type LEFT JOIN TypeCategories ON Type.categories_type = TypeCategories.id_typeCategories LEFT JOIN Status ON Orders.status_orders = Status.id_status LEFT JOIN Employees ON Orders.nameEmpl_orders = Employees.id_employees LEFT JOIN Positions ON Employees.position_employees = Positions.id_positions LEFT JOIN StatusEmployees ON Employees.status_employees = StatusEmployees.id_statusEmployees LEFT JOIN Clients ON Orders.nameClient_orders = Clients.id_clients WHERE Orders.id_orders LIKE '%{textBox1.Text}%' AND Clients.phone_clients LIKE '%{textBox2.Text}%' AND Orders.status_orders = {comboBox1.SelectedIndex};";
            }
            //Открываем соединение
            conn.Open();
            //Объявляем команду, которая выполнить запрос в соединении conn
            MyDA.SelectCommand = new MySqlCommand(commandStr, conn);
            //Заполняем таблицу записями из БД
            MyDA.Fill(table);
            conn.Close();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            //(dataGridView1.DataSource as DataTable).DefaultView.RowFilter = $"Clients.phone_clients LIKE '%{textBox2.Text}%'";
            table.Clear();
            string commandStr;
            if (comboBox1.SelectedIndex <= 0)
            {
                commandStr = $"SELECT Orders.id_orders AS 'Номер заказа', Orders.dataStart_orders AS 'Дата создания заказа', Orders.dataEnd_orders AS 'Дата завершения заказа', Orders.price_orders AS 'Итоговая сумма', Orders.nameDeceased_orders AS 'Имя умршего', Orders.lifeYears_orders AS 'Годы жизни умершего', Orders.comments_orders AS 'Комментарий к закзау', Type.id_type AS 'Номер услуги', Type.title_type AS 'Название услуги', Type.price_type AS 'Стоимость услуги', Type.amount_type AS 'Количество товара', TypeCategories.title__typeCategories AS 'Категория', Orders.status_orders AS 'Номер статуса', Status.title_status AS 'Статус заказа', Orders.nameEmpl_orders AS 'Номер сотрудника', Employees.name_employees AS 'Имя сотрудника', Employees.phone_employees AS 'Телефон сотрудника', Positions.title_positions AS 'Должность', StatusEmployees.title_statusEmployees AS 'Статус сотрудника', Orders.nameClient_orders AS 'Номер клиента', Clients.name_clients AS 'Имя клиента', Clients.phone_clients AS 'Телефон клиента', Clients.address_clients AS 'Адрес клиента' FROM Orders LEFT JOIN Orders_Type ON Orders.id_orders = Orders_Type.Orders_Type_orderID LEFT JOIN Type ON Orders_Type.Orders_Type_typeID = Type.id_type LEFT JOIN TypeCategories ON Type.categories_type = TypeCategories.id_typeCategories LEFT JOIN Status ON Orders.status_orders = Status.id_status LEFT JOIN Employees ON Orders.nameEmpl_orders = Employees.id_employees LEFT JOIN Positions ON Employees.position_employees = Positions.id_positions LEFT JOIN StatusEmployees ON Employees.status_employees = StatusEmployees.id_statusEmployees LEFT JOIN Clients ON Orders.nameClient_orders = Clients.id_clients WHERE Orders.id_orders LIKE '%{textBox1.Text}%' AND Clients.phone_clients LIKE '%{textBox2.Text}%';";
            }
            else
            {
                commandStr = $"SELECT Orders.id_orders AS 'Номер заказа', Orders.dataStart_orders AS 'Дата создания заказа', Orders.dataEnd_orders AS 'Дата завершения заказа', Orders.price_orders AS 'Итоговая сумма', Orders.nameDeceased_orders AS 'Имя умршего', Orders.lifeYears_orders AS 'Годы жизни умершего', Orders.comments_orders AS 'Комментарий к закзау', Type.id_type AS 'Номер услуги', Type.title_type AS 'Название услуги', Type.price_type AS 'Стоимость услуги', Type.amount_type AS 'Количество товара', TypeCategories.title__typeCategories AS 'Категория', Orders.status_orders AS 'Номер статуса', Status.title_status AS 'Статус заказа', Orders.nameEmpl_orders AS 'Номер сотрудника', Employees.name_employees AS 'Имя сотрудника', Employees.phone_employees AS 'Телефон сотрудника', Positions.title_positions AS 'Должность', StatusEmployees.title_statusEmployees AS 'Статус сотрудника', Orders.nameClient_orders AS 'Номер клиента', Clients.name_clients AS 'Имя клиента', Clients.phone_clients AS 'Телефон клиента', Clients.address_clients AS 'Адрес клиента' FROM Orders LEFT JOIN Orders_Type ON Orders.id_orders = Orders_Type.Orders_Type_orderID LEFT JOIN Type ON Orders_Type.Orders_Type_typeID = Type.id_type LEFT JOIN TypeCategories ON Type.categories_type = TypeCategories.id_typeCategories LEFT JOIN Status ON Orders.status_orders = Status.id_status LEFT JOIN Employees ON Orders.nameEmpl_orders = Employees.id_employees LEFT JOIN Positions ON Employees.position_employees = Positions.id_positions LEFT JOIN StatusEmployees ON Employees.status_employees = StatusEmployees.id_statusEmployees LEFT JOIN Clients ON Orders.nameClient_orders = Clients.id_clients WHERE Orders.id_orders LIKE '%{textBox1.Text}%' AND Clients.phone_clients LIKE '%{textBox2.Text}%' AND Orders.status_orders = {comboBox1.SelectedIndex};";
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
                    string commandStr = $"SELECT Orders.id_orders AS 'Номер заказа', Orders.dataStart_orders AS 'Дата создания заказа', Orders.dataEnd_orders AS 'Дата завершения заказа', Orders.price_orders AS 'Итоговая сумма', Orders.nameDeceased_orders AS 'Имя умршего', Orders.lifeYears_orders AS 'Годы жизни умершего', Orders.comments_orders AS 'Комментарий к закзау', Type.id_type AS 'Номер услуги', Type.title_type AS 'Название услуги', Type.price_type AS 'Стоимость услуги', Type.amount_type AS 'Количество товара', TypeCategories.title__typeCategories AS 'Категория', Orders.status_orders AS 'Номер статуса', Status.title_status AS 'Статус заказа', Orders.nameEmpl_orders AS 'Номер сотрудника', Employees.name_employees AS 'Имя сотрудника', Employees.phone_employees AS 'Телефон сотрудника', Positions.title_positions AS 'Должность', StatusEmployees.title_statusEmployees AS 'Статус сотрудника', Orders.nameClient_orders AS 'Номер клиента', Clients.name_clients AS 'Имя клиента', Clients.phone_clients AS 'Телефон клиента', Clients.address_clients AS 'Адрес клиента' FROM Orders LEFT JOIN Orders_Type ON Orders.id_orders = Orders_Type.Orders_Type_orderID LEFT JOIN Type ON Orders_Type.Orders_Type_typeID = Type.id_type LEFT JOIN TypeCategories ON Type.categories_type = TypeCategories.id_typeCategories LEFT JOIN Status ON Orders.status_orders = Status.id_status LEFT JOIN Employees ON Orders.nameEmpl_orders = Employees.id_employees LEFT JOIN Positions ON Employees.position_employees = Positions.id_positions LEFT JOIN StatusEmployees ON Employees.status_employees = StatusEmployees.id_statusEmployees LEFT JOIN Clients ON Orders.nameClient_orders = Clients.id_clients WHERE Orders.id_orders LIKE '%{textBox1.Text}%' AND Clients.phone_clients LIKE '%{textBox2.Text}%';";
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
                    string commandStr1 = $"SELECT Orders.id_orders AS 'Номер заказа', Orders.dataStart_orders AS 'Дата создания заказа', Orders.dataEnd_orders AS 'Дата завершения заказа', Orders.price_orders AS 'Итоговая сумма', Orders.nameDeceased_orders AS 'Имя умршего', Orders.lifeYears_orders AS 'Годы жизни умершего', Orders.comments_orders AS 'Комментарий к закзау', Type.id_type AS 'Номер услуги', Type.title_type AS 'Название услуги', Type.price_type AS 'Стоимость услуги', Type.amount_type AS 'Количество товара', TypeCategories.title__typeCategories AS 'Категория', Orders.status_orders AS 'Номер статуса', Status.title_status AS 'Статус заказа', Orders.nameEmpl_orders AS 'Номер сотрудника', Employees.name_employees AS 'Имя сотрудника', Employees.phone_employees AS 'Телефон сотрудника', Positions.title_positions AS 'Должность', StatusEmployees.title_statusEmployees AS 'Статус сотрудника', Orders.nameClient_orders AS 'Номер клиента', Clients.name_clients AS 'Имя клиента', Clients.phone_clients AS 'Телефон клиента', Clients.address_clients AS 'Адрес клиента' FROM Orders LEFT JOIN Orders_Type ON Orders.id_orders = Orders_Type.Orders_Type_orderID LEFT JOIN Type ON Orders_Type.Orders_Type_typeID = Type.id_type LEFT JOIN TypeCategories ON Type.categories_type = TypeCategories.id_typeCategories LEFT JOIN Status ON Orders.status_orders = Status.id_status LEFT JOIN Employees ON Orders.nameEmpl_orders = Employees.id_employees LEFT JOIN Positions ON Employees.position_employees = Positions.id_positions LEFT JOIN StatusEmployees ON Employees.status_employees = StatusEmployees.id_statusEmployees LEFT JOIN Clients ON Orders.nameClient_orders = Clients.id_clients WHERE Orders.id_orders LIKE '%{textBox1.Text}%' AND Clients.phone_clients LIKE '%{textBox2.Text}%' AND Orders.status_orders = {comboBox1.SelectedIndex};";
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
                    string commandStr2 = $"SELECT Orders.id_orders AS 'Номер заказа', Orders.dataStart_orders AS 'Дата создания заказа', Orders.dataEnd_orders AS 'Дата завершения заказа', Orders.price_orders AS 'Итоговая сумма', Orders.nameDeceased_orders AS 'Имя умршего', Orders.lifeYears_orders AS 'Годы жизни умершего', Orders.comments_orders AS 'Комментарий к закзау', Type.id_type AS 'Номер услуги', Type.title_type AS 'Название услуги', Type.price_type AS 'Стоимость услуги', Type.amount_type AS 'Количество товара', TypeCategories.title__typeCategories AS 'Категория', Orders.status_orders AS 'Номер статуса', Status.title_status AS 'Статус заказа', Orders.nameEmpl_orders AS 'Номер сотрудника', Employees.name_employees AS 'Имя сотрудника', Employees.phone_employees AS 'Телефон сотрудника', Positions.title_positions AS 'Должность', StatusEmployees.title_statusEmployees AS 'Статус сотрудника', Orders.nameClient_orders AS 'Номер клиента', Clients.name_clients AS 'Имя клиента', Clients.phone_clients AS 'Телефон клиента', Clients.address_clients AS 'Адрес клиента' FROM Orders LEFT JOIN Orders_Type ON Orders.id_orders = Orders_Type.Orders_Type_orderID LEFT JOIN Type ON Orders_Type.Orders_Type_typeID = Type.id_type LEFT JOIN TypeCategories ON Type.categories_type = TypeCategories.id_typeCategories LEFT JOIN Status ON Orders.status_orders = Status.id_status LEFT JOIN Employees ON Orders.nameEmpl_orders = Employees.id_employees LEFT JOIN Positions ON Employees.position_employees = Positions.id_positions LEFT JOIN StatusEmployees ON Employees.status_employees = StatusEmployees.id_statusEmployees LEFT JOIN Clients ON Orders.nameClient_orders = Clients.id_clients WHERE Orders.id_orders LIKE '%{textBox1.Text}%' AND Clients.phone_clients LIKE '%{textBox2.Text}%' AND Orders.status_orders = {comboBox1.SelectedIndex};";
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
                    string commandStr3 = $"SELECT Orders.id_orders AS 'Номер заказа', Orders.dataStart_orders AS 'Дата создания заказа', Orders.dataEnd_orders AS 'Дата завершения заказа', Orders.price_orders AS 'Итоговая сумма', Orders.nameDeceased_orders AS 'Имя умршего', Orders.lifeYears_orders AS 'Годы жизни умершего', Orders.comments_orders AS 'Комментарий к закзау', Type.id_type AS 'Номер услуги', Type.title_type AS 'Название услуги', Type.price_type AS 'Стоимость услуги', Type.amount_type AS 'Количество товара', TypeCategories.title__typeCategories AS 'Категория', Orders.status_orders AS 'Номер статуса', Status.title_status AS 'Статус заказа', Orders.nameEmpl_orders AS 'Номер сотрудника', Employees.name_employees AS 'Имя сотрудника', Employees.phone_employees AS 'Телефон сотрудника', Positions.title_positions AS 'Должность', StatusEmployees.title_statusEmployees AS 'Статус сотрудника', Orders.nameClient_orders AS 'Номер клиента', Clients.name_clients AS 'Имя клиента', Clients.phone_clients AS 'Телефон клиента', Clients.address_clients AS 'Адрес клиента' FROM Orders LEFT JOIN Orders_Type ON Orders.id_orders = Orders_Type.Orders_Type_orderID LEFT JOIN Type ON Orders_Type.Orders_Type_typeID = Type.id_type LEFT JOIN TypeCategories ON Type.categories_type = TypeCategories.id_typeCategories LEFT JOIN Status ON Orders.status_orders = Status.id_status LEFT JOIN Employees ON Orders.nameEmpl_orders = Employees.id_employees LEFT JOIN Positions ON Employees.position_employees = Positions.id_positions LEFT JOIN StatusEmployees ON Employees.status_employees = StatusEmployees.id_statusEmployees LEFT JOIN Clients ON Orders.nameClient_orders = Clients.id_clients WHERE Orders.id_orders LIKE '%{textBox1.Text}%' AND Clients.phone_clients LIKE '%{textBox2.Text}%' AND Orders.status_orders = {comboBox1.SelectedIndex};";
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
                    string commandStr4 = $"SELECT Orders.id_orders AS 'Номер заказа', Orders.dataStart_orders AS 'Дата создания заказа', Orders.dataEnd_orders AS 'Дата завершения заказа', Orders.price_orders AS 'Итоговая сумма', Orders.nameDeceased_orders AS 'Имя умршего', Orders.lifeYears_orders AS 'Годы жизни умершего', Orders.comments_orders AS 'Комментарий к закзау', Type.id_type AS 'Номер услуги', Type.title_type AS 'Название услуги', Type.price_type AS 'Стоимость услуги', Type.amount_type AS 'Количество товара', TypeCategories.title__typeCategories AS 'Категория', Orders.status_orders AS 'Номер статуса', Status.title_status AS 'Статус заказа', Orders.nameEmpl_orders AS 'Номер сотрудника', Employees.name_employees AS 'Имя сотрудника', Employees.phone_employees AS 'Телефон сотрудника', Positions.title_positions AS 'Должность', StatusEmployees.title_statusEmployees AS 'Статус сотрудника', Orders.nameClient_orders AS 'Номер клиента', Clients.name_clients AS 'Имя клиента', Clients.phone_clients AS 'Телефон клиента', Clients.address_clients AS 'Адрес клиента' FROM Orders LEFT JOIN Orders_Type ON Orders.id_orders = Orders_Type.Orders_Type_orderID LEFT JOIN Type ON Orders_Type.Orders_Type_typeID = Type.id_type LEFT JOIN TypeCategories ON Type.categories_type = TypeCategories.id_typeCategories LEFT JOIN Status ON Orders.status_orders = Status.id_status LEFT JOIN Employees ON Orders.nameEmpl_orders = Employees.id_employees LEFT JOIN Positions ON Employees.position_employees = Positions.id_positions LEFT JOIN StatusEmployees ON Employees.status_employees = StatusEmployees.id_statusEmployees LEFT JOIN Clients ON Orders.nameClient_orders = Clients.id_clients WHERE Orders.id_orders LIKE '%{textBox1.Text}%' AND Clients.phone_clients LIKE '%{textBox2.Text}%' AND Orders.status_orders = {comboBox1.SelectedIndex};";
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
