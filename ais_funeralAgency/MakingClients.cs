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
    public partial class MakingClients : Form
    {
        public MakingClients()
        {
            InitializeComponent();
        }
        string connStr = "server=chuc.caseum.ru;port=33333;user=st_3_20_11;database=is_3_20_st11_KURS;password=67959087";
        //Переменная соединения
        MySqlConnection conn;

        private void Auto_Load(object sender, EventArgs e)
        {

        }

        public bool InsertPrepods(string name, string phone, string adress)
        {
            conn = new MySqlConnection(connStr);
            //определяем переменную, хранящую количество вставленных строк
            int InsertCount = 0;
            //Объявляем переменную храняющую результат операции
            bool result = false;
            // открываем соединение
            conn.Open();
            // запросы
            // запрос вставки данных
            string query = $"INSERT INTO Clients (name_clients, phone_clients, address_clients) VALUES ('{name}', '{phone}', '{adress}')";
            try
            {
                // объект для выполнения SQL-запроса
                MySqlCommand command = new MySqlCommand(query, conn);
                // выполняем запрос
                InsertCount = command.ExecuteNonQuery();
                // закрываем подключение к БД
            }
            catch
            {
                //Если возникла ошибка, то запрос не вставит ни одной строки
                InsertCount = 0;
            }
            finally
            {
                //Но в любом случае, нужно закрыть соединение
                conn.Close();
                //Ессли количество вставленных строк было не 0, то есть вставлена хотя бы 1 строка
                if (InsertCount != 0)
                {
                    //то результат операции - истина
                    result = true;
                }
            }
            //Вернём результат операции, где его обработает алгоритм
            return result;
        }



        private void button1_Click(object sender, EventArgs e)
        {
            conn = new MySqlConnection(connStr);
            //Запрос в БД на предмет того, если ли строка с подходящим логином и паролем
            //string sql = "SELECT * FROM Accounts WHERE login_accounts = @un and  pass_accounts= @up";
            string sql = "SELECT * FROM Clients WHERE phone_clients= @up";
            //Открытие соединения
            conn.Open();
            //Объявляем таблицу
            DataTable table = new DataTable();
            //Объявляем адаптер
            MySqlDataAdapter adapter = new MySqlDataAdapter();
            //Объявляем команду
            MySqlCommand command = new MySqlCommand(sql, conn);
            //Определяем параметры
            command.Parameters.Add("@up", MySqlDbType.VarChar, 25);
            //Присваиваем параметрам значение
            command.Parameters["@up"].Value = textBox2.Text;
            //Заносим команду в адаптер
            adapter.SelectCommand = command;
            //Заполняем таблицу
            adapter.Fill(table);
            //Закрываем соединение
            conn.Close();

            if (textBox1.Text.Length < 2)
            {
                label7.Visible = true;
            }
            else if (textBox2.Text.Length < 8)
            {
                label10.Visible = false;
                label8.Visible = true;
            }
            else if (textBox2.Text.Length > 20)
            {
                label8.Visible = false;
                label10.Visible = true;
            }
            else if (textBox3.Text.Length < 5)
            {
                label9.Visible = true;
            }
            else
            {
                label7.Visible = false;
                label8.Visible = false;
                label9.Visible = false;
                label10.Visible = false;
                if (InsertPrepods(textBox1.Text, textBox2.Text, textBox3.Text))
                {
                    label6.Visible = true;
                }
                else if (table.Rows.Count > 0)
                {
                    MessageBox.Show("Пользователь с таким номером телефона уже существует!", "Ошибка добавления данных", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void textBox1_Enter(object sender, EventArgs e)
        {
            label6.Visible = false;
        }

        private void textBox2_Enter(object sender, EventArgs e)
        {
            label6.Visible = false;
        }

        private void textBox3_Enter(object sender, EventArgs e)
        {
            label6.Visible = false;
        }
    }
}
