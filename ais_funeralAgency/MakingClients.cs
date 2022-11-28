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
        //Переменная соединения
        MySqlConnection conn;

        private void Auto_Load(object sender, EventArgs e)
        {
            string connStr = "server=chuc.caseum.ru;port=33333;user=st_3_20_11;database=is_3_20_st11_KURS;password=67959087";
            conn = new MySqlConnection(connStr);
        }

        public bool InsertPrepods(string name, string phone, string adress)
        {
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
            string c_name = textBox1.Text;
            string c_phone = textBox2.Text;
            string c_adress = textBox3.Text;
            if (InsertPrepods(c_name, c_phone, c_adress)) 
            {
                label6.Visible = true;            
            }
            else
            {
                MessageBox.Show("Произошла ошибка.", "Ошибка");
            }
        }
    }
}
