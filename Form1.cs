using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SpeechLib;

namespace Anima
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            Screen rightmost = Screen.AllScreens[0];
            foreach (Screen screen in Screen.AllScreens)
            {
                if (screen.WorkingArea.Right > rightmost.WorkingArea.Right)
                    rightmost = screen;
            }
            this.Left = rightmost.WorkingArea.Right - this.Width;
            this.Top = rightmost.WorkingArea.Bottom - this.Height;
            this.TopLevel = true;
            this.Show();
        }
                
        protected override void OnLoad(EventArgs e)
        {
            PlaceLowerRight();
            base.OnLoad(e);
        }

        public void PlaceLowerRight()
        {
            //Determine "rightmost" screen
            Screen rightmost = Screen.AllScreens[0];
            foreach (Screen screen in Screen.AllScreens)
            {
                if (screen.WorkingArea.Right > rightmost.WorkingArea.Right)
                    rightmost = screen;
            }

            this.Left = rightmost.WorkingArea.Right - this.Width;
            this.Top = rightmost.WorkingArea.Bottom - this.Height;
            Pasukonchansay.Hide();
            Speechbubble.Hide();
            Pasukonchansay.BringToFront();
        }

        public void PlaceCenter()
        {
             Screen rightmost = Screen.AllScreens[0];
            foreach (Screen screen in Screen.AllScreens)
            {
                if (screen.WorkingArea.Right > rightmost.WorkingArea.Right)
                    rightmost = screen;
            }

            this.Left = (rightmost.WorkingArea.Right / 2) - (this.Width / 2);
            this.Top = rightmost.WorkingArea.Bottom - this.Height;
        }
        SpVoice voice = new SpVoice();
        public async void Pasukonsay(string message)
        {
            try
            {
                Pasukonchansay.Text = message;
                voice.Volume = 100;
                voice.Speak(message, SpeechVoiceSpeakFlags.SVSFlagsAsync);
                Pasukonchansay.Show();
                Speechbubble.Show();
                Pasukonchansay.BringToFront();
                
            } catch 
            {
                Pasukonchansay.Text = message;
                Pasukonchansay.Show();
                Speechbubble.Show();
                Pasukonchansay.BringToFront();
            }
            await Task.Delay(5000);
            Pasukonchansay.Hide();
            Speechbubble.Hide(); 

        }
        public async void Pasukonsay(string message, int length)
        {
            try
            {
                Pasukonchansay.Text = message;
                voice.Volume = 100;
                voice.Speak(message, SpeechVoiceSpeakFlags.SVSFlagsAsync);
                Pasukonchansay.Show();
                Speechbubble.Show();
                Pasukonchansay.BringToFront();
            } catch
            {
                Pasukonchansay.Text = message;
                Pasukonchansay.Show();
                Speechbubble.Show();
                Pasukonchansay.BringToFront();
                
            }
            await Task.Delay(length);
            Pasukonchansay.Hide();
            Speechbubble.Hide();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            string Skin = Properties.Settings.Default.Skin;
            SetImage(Skin);
        }

        public void SetImage(string skin)
        {
            string Choice = skin;
            switch (Choice)
            {
                case "Skin 1": pictureBox1.Image = Properties.Resources.Pasukon_chansmaller;
                    break;

                case "Skin 2": pictureBox1.Image = Properties.Resources.Irisu_smaller;
                    break;

                case "Skin 3": pictureBox1.Image = Properties.Resources.Chibi_smaller;
                    break;
            }
        }
    }
}
