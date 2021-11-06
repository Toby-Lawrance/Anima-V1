using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Anima
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
            string[] commands = (System.Windows.Forms.Application.OpenForms["SysTrayApp"] as Anima.SysTrayApp).commands;
            List<string> items = new List<string>();
            int i = 0;
            foreach (string command in commands)
            {
                items.Add(command);
                i = i + 1;
            }
            items.Add("");
            items.Add("There are "+i+" commands! (This is not a command)");
            listBox1.DataSource = items;
        }

        private void ShowSongList_Click(object sender, EventArgs e)
        {
            Form SongList = new Anima.Form3();
            SongList.Show();
        }

        private void AlarmListShow_Click(object sender, EventArgs e)
        {
            Form AlarmList = new Anima.Form5();
            AlarmList.Show();
        }

    }
}
