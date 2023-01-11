using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ais_funeralAgency
{
    public partial class Main : Form
    {
        public Main()
        {
            InitializeComponent();
        }

        public void ManagerRole(int role)
        {
            switch (role)
            {
                //И в зависимости от того, какая роль (цифра) хранится в поле класса и передана в метод, показываются те или иные кнопки.
                //Вы можете скрыть их и не отображать вообще, здесь они просто выключены
                case 0:
                    button1.Enabled = true;
                    button2.Enabled = true;
                    button3.Enabled = true;
                    button5.Enabled = true;
                    button6.Enabled = true;

                    button1.Visible = true;
                    button2.Visible = true;
                    button3.Visible = true;
                    button5.Visible = true;
                    button6.Visible = true;
                    break;
                case 1:
                    button1.Enabled = true;
                    button2.Enabled = true;
                    button3.Enabled = false;
                    button5.Enabled = true;
                    button6.Enabled = false;

                    button1.Visible = true;
                    button2.Visible = true;
                    button3.Visible = false;
                    button5.Visible = true;
                    button6.Visible = false;
                    break;
                case 2:
                    button1.Enabled = false;
                    button2.Enabled = true;
                    button3.Enabled = false;
                    button5.Enabled = false;
                    button6.Enabled = true;

                    button1.Visible = false;
                    button2.Visible = true;
                    button3.Visible = false;
                    button5.Visible = false;
                    button6.Visible = true;
                    break;
                //Если по какой то причине в классе ничего не содержится, то всё отключается вообще
                default:
                    label3.Text = "Не определёный уровень доступа";
                    label3.ForeColor = Color.Red;
                    button1.Enabled = false;
                    button2.Enabled = false;
                    button3.Enabled = false;
                    button5.Enabled = false;
                    button6.Enabled = false;

                    button1.Visible = false;
                    button2.Visible = false;
                    button3.Visible = false;
                    button5.Visible = false;
                    button6.Visible = false;
                    break;
            }
        }

        private void Main_Load(object sender, EventArgs e)
        {   
            if (Auth.auth)
            {
                //Вытаскиваем из класса поля в label
                label3.Text = $"Здравствуйте {Auth.auth_fio}";
                ManagerRole(Auth.auth_role);
            }
            //иначе
            else
            {
                //Закрываем форму
                this.Close();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Hide();
            MakingOrders mOrders = new MakingOrders();
            mOrders.Show();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Hide();
            ViewOrders vOrders = new ViewOrders();
            vOrders.Show();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            this.Hide();
            ViewClients vClients = new ViewClients();
            vClients.Show();
        }
    }
}
