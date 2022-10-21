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
                    label4.Text = "Максимальный";
                    label4.ForeColor = Color.Green;
                    button1.Enabled = true;
                    button2.Enabled = true;
                    button3.Enabled = true;
                    button1.Visible = true;
                    button2.Visible = true;
                    button3.Visible = true;
                    break;
                case 1:
                    label4.Text = "Умеренный";
                    label4.ForeColor = Color.YellowGreen;
                    button1.Enabled = true;
                    button2.Enabled = true;
                    button3.Enabled = false;
                    button1.Visible = true;
                    button2.Visible = true;
                    button3.Visible = false;
                    break;
                //Если по какой то причине в классе ничего не содержится, то всё отключается вообще
                default:
                    label4.Text = "Неопределённый";
                    label4.ForeColor = Color.Red;
                    button1.Enabled = false;
                    button2.Enabled = false;
                    button3.Enabled = false;
                    button1.Visible = false;
                    button2.Visible = false;
                    button3.Visible = false;
                    break;
            }
        }

        private void Main_Load(object sender, EventArgs e)
        {
            this.Hide();
            Auto auto = new Auto();
            auto.ShowDialog();
            
            if (Auth.auth)
            {
                //Отображаем рабочую форму
                this.Show();
                //Вытаскиваем из класса поля в label
                label3.Text = $"Здравствуйте {Auth.auth_fio}";
                //Красим текст в label в зелёный цвет
                label3.ForeColor = Color.Green;
                ManagerRole(Auth.auth_role);
            }
            //иначе
            else
            {
                //Закрываем форму
                this.Close();
            }
        }
    }
}
