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
    public partial class SettingsFormn : Form
    {
        public SettingsFormn()
        {
            InitializeComponent();
            textBox1.Text = Properties.Settings.Default.PasukonName;
            textBox2.Text = Properties.Settings.Default.UserName;
            textBox3.Text = Properties.Settings.Default.PassPhrase;
            textBox4.Text = Properties.Settings.Default.DefaultPlaylist;
            string Preset = Properties.Settings.Default.Skin;
            if (Preset == "Skin 1")
            {
                radioButton1.Checked = true;
            }
            else if (Preset == "Skin 2")
            {
                radioButton2.Checked = true;
            }
            else if (Preset == "Skin 3")
            {
                radioButton3.Checked = true;
            }
        }

        private void AcceptChanges_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == "")
            {
                label1.ForeColor = Color.Red;
                textBox1.Text = "Enter a name";
            }
            if (textBox2.Text == "")
            {
                label2.ForeColor = Color.Red;
                textBox2.Text = "Enter a name";
            }
            if (textBox3.Text == "")
            {
                label3.ForeColor = Color.Red;
                textBox3.Text = "Enter a passphrase";
            }
            if (textBox1.Text == "Enter a name"||textBox2.Text == "Enter a name")
            {
                textBox1.Text = "Don't get smart with me";
                textBox2.Text = "Don't get smart with me";
            }
            if (textBox1.Text == "Don't get smart with me" || textBox2.Text == "Don't get smart with me")
            {
                textBox1.Text = "Enter a name";
                textBox2.Text = "Enter a name";
            }
            string picture = null;
            if (radioButton1.Checked == true)
            {
                picture = "Skin 1"; 
            } else if (radioButton2.Checked == true)
            {
                picture = "Skin 2";
            } else if (radioButton3.Checked == true)
            {
                picture = "Skin 3";
            }
            (System.Windows.Forms.Application.OpenForms["Form1"] as Anima.Form1).SetImage(picture);
            Properties.Settings.Default.PasukonName = textBox1.Text;
            Properties.Settings.Default.UserName = textBox2.Text;
            Properties.Settings.Default.Save();
            Properties.Settings.Default.PassPhrase = textBox3.Text;
            Properties.Settings.Default.DefaultPlaylist = textBox4.Text;
            Properties.Settings.Default.Skin = picture;
            Properties.Settings.Default.Save();
            (System.Windows.Forms.Application.OpenForms["SysTrayApp"] as Anima.SysTrayApp).RebuildSpeech();
            Properties.Settings.Default.SetupRunOnce = true;
            Properties.Settings.Default.Save();
            this.Hide();
            
        }

        private void ResetALL_Click(object sender, EventArgs e)
        {
            this.Close();
            (System.Windows.Forms.Application.OpenForms["SysTrayApp"] as Anima.SysTrayApp).ResetToDefaults();
        }

        public void button1_Click(object sender, EventArgs e)
        { 
            openFileDialog1.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);
            openFileDialog1.Filter = "Playlist files (*.wpl)|*.wpl|All files|*.*";
            DialogResult result = openFileDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                textBox4.Text = openFileDialog1.FileName;
            }
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton1.Checked)
            {
                (System.Windows.Forms.Application.OpenForms["Form1"] as Anima.Form1).SetImage("Skin 1");
            }
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton2.Checked)
            {
                (System.Windows.Forms.Application.OpenForms["Form1"] as Anima.Form1).SetImage("Skin 2");
            }
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton3.Checked)
            {
                (System.Windows.Forms.Application.OpenForms["Form1"] as Anima.Form1).SetImage("Skin 3");
            }
        }

    }
}
