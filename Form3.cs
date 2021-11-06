using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace Anima
{
    public partial class Form3 : Form
    {
        public Form3()
        {
            InitializeComponent();
            string path = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);
            string[] Songs = Directory.GetFiles(path, "*.mp3", SearchOption.TopDirectoryOnly);
            List<string> SongList = new List<string>(); 
            int i = 0;
            foreach (string Asong in Songs)
            {
                SongList.Add(Path.GetFileNameWithoutExtension(Asong));
                i = i + 1;
            }
            SongList.Add("");
            SongList.Add("There are " + i + " songs");
            SongBox.DataSource = SongList;
        }
      
    }
}
