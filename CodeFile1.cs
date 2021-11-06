using System;
using System.Drawing;
using System.Windows.Forms;
using System.Threading.Tasks;
using SpeechLib;
using System.IO;
using System.Collections.Generic;
using System.Timers;
using System.Text;
using WMPLib;
using System.Runtime.InteropServices;
using System.Threading;
using System.Diagnostics;
using MediaPlayer;
using Microsoft.Win32;

namespace Anima
{
    public class SysTrayApp : Form
    {

        static void Main()
        {
            Application.EnableVisualStyles();
            System.Threading.Thread t = new System.Threading.Thread(new System.Threading.ThreadStart(FormI));
            t.SetApartmentState(System.Threading.ApartmentState.STA);
            t.Start();
        }
        private NotifyIcon m_notifyicon;
        private TextBox textBox1;
        private Button Submit;
        private Button Clear;
        private Button Commandlistbtn;
        private ContextMenu trayMenu;
        private string mode = "Default";
        private System.Timers.Timer alertTimer = new System.Timers.Timer();
        private System.Timers.Timer AlarmTimer = new System.Timers.Timer();
        private SpeechLib.SpSharedRecoContext objRecoContext = null;
        private SpeechLib.ISpeechRecoGrammar grammar = null;
        private SpeechLib.ISpeechGrammarRule menuRule = null;
        WindowsMediaPlayer Player = new WindowsMediaPlayer();
        [DllImport("winmm.dll")]
        static extern Int32 mciSendString(string command, string buffer, int bufferSize, IntPtr hwndCallback);
        // Use Ctrl+K, Ctrl+C to comment out a block
        public string[] commands = { "",// Yes it's the 0th value and no I did not realise earlier...if you want to remove the plus one on the numeration be my guest
                                Properties.Settings.Default.PasukonName+" Lock the computer",
                                Properties.Settings.Default.PasukonName+" turn off the computer",
                                Properties.Settings.Default.PasukonName+" Restart the computer",
                                "Reset your position",
                                "What is the time?", 
                                "What is the date?", 
                                "Could you run chrome?",
                                "Could you run stronghold kingdoms?",
                                "Could you run task manager?",
                                "Could you run Titanfall?",
                                "How are you?",
                                "Play some music",
                                "Pause the music",
                                "Carry on please",
                                "Next song",
                                "That last song again",
                                "It is too loud",
                                "It is too quiet",
                                "Hide please",
                                "Come out please",
                                "Open the cd drive",
                                "I would like to program",
                                "open Tumblr",
                                "open the attack on titan game",
                                "close chrome",
                                "close stronghold kingdoms",
                                "close task manager",
                                "Close Oblivion",
                                "Good Morning "+Properties.Settings.Default.PasukonName,
                                "Good Afternoon "+Properties.Settings.Default.PasukonName,
                                "Good Evening "+Properties.Settings.Default.PasukonName,
                                "Thank you",
                                "What is this song?",
                                "Don't do anything",
                                "Show manual entry box",
                                "Hide manual entry box",
                                Properties.Settings.Default.PasukonName+", be on guard",
                                "I would like to google something",
                                "Open a webpage",
                                Properties.Settings.Default.PasukonName+" disable the alarm",
                                Properties.Settings.Default.PasukonName+" enable the alarm",
                                "After this song  (To be implemented)",
                                "Show command list",
                                "Show song list",
                                "Show alarm list"};

        string[] ExtraCommands = { "Unmute",
                                   "Find it",
                                   "Goodbye "+Properties.Settings.Default.PasukonName,
                                   "Go to default mode"};

        public SysTrayApp()
        {
            
            if (Properties.Settings.Default.GuardMode == true)
            {
                mode = "Alert";
                (System.Windows.Forms.Application.OpenForms["Form1"] as Anima.Form1).PlaceCenter();
                alertTimer.Elapsed += new ElapsedEventHandler(AlertCheck);
                alertTimer.Interval = 7500;
                alertTimer.Enabled = true;
                GC.KeepAlive(alertTimer);
                Form Imageform = new Anima.Form1();
                Imageform.Show();
            }
            else
            {
                Form Imageform = new Anima.Form1();
                Screen rightmost = Screen.AllScreens[0];
                foreach (Screen screen in Screen.AllScreens)
                {
                    if (screen.WorkingArea.Right > rightmost.WorkingArea.Right)
                        rightmost = screen;
                }
                Imageform.Left = rightmost.WorkingArea.Right - Imageform.Width;
                Imageform.Top = rightmost.WorkingArea.Bottom - Imageform.Height;
                Imageform.Show();
            }

            if (Properties.Settings.Default.SetupRunOnce == false)
            {
                ResetToDefaults();
            }
            AlarmTimer.Elapsed += new ElapsedEventHandler(Alarmcheck);
            AlarmTimer.Interval = 5000;
            AlarmTimer.Enabled = true;
            GC.KeepAlive(AlarmTimer);

            InitializeComponent();          
          
            trayMenu = new ContextMenu();
            trayMenu.MenuItems.Add("Show entry box", OnShow);
            trayMenu.MenuItems.Add("Hide entry box", OnHide);
            trayMenu.MenuItems.Add("Exit " + Properties.Settings.Default.PasukonName, TimeSleep);
            
            this.WindowState = FormWindowState.Minimized;

            m_notifyicon = new NotifyIcon();
            m_notifyicon.Text = Properties.Settings.Default.PasukonName;
            m_notifyicon.Visible = true;
            m_notifyicon.Icon = new Icon(SystemIcons.Information, 40, 40);
            m_notifyicon.ContextMenu = trayMenu;
            m_notifyicon.Click += new System.EventHandler(TrayClick);


            EnableSpeech();
            Greeting();
        }

        public static void FormI()
        {
            Application.Run(new SysTrayApp());
            Application.Run(new Form1());
        }

        private void Alarmcheck(object sender, ElapsedEventArgs e)
        {
            Alarm();
        }
        bool shown = false;
        private void TrayClick(object sender, EventArgs e)
        {
            if (shown == true)
            {
                OnHide(null,null);
                shown = false;
            } else
            {
                OnShow(null,null);
                shown = true;
            }
        }

        public void ResetToDefaults()
        {
            try
            {
                Properties.Settings.Default.Upgrade();
            } catch
            {

            }
            string name = System.Security.Principal.WindowsIdentity.GetCurrent().Name.ToString();
            name = name.Remove(0, name.LastIndexOf(@"\") + 1);
            Properties.Settings.Default.UserName = name;
            Properties.Settings.Default.PasukonName = "Anima";
            Properties.Settings.Default.AlarmIO = false;
            RegistryKey rkApp = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
            rkApp.SetValue("Personal Assistant", Application.ExecutablePath.ToString());
            Properties.Settings.Default.DefaultPlaylist = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);
            Properties.Settings.Default.Skin = "Skin 1";
            Properties.Settings.Default.Save();
            System.Threading.Thread t = new System.Threading.Thread(new System.Threading.ThreadStart(Defaults));
            t.SetApartmentState(System.Threading.ApartmentState.STA);
            t.Start();
            
        }
        public static void Defaults()
        {
            Application.Run(new SettingsFormn());
        }
        protected override void OnLoad(EventArgs e)
        {
            ShowInTaskbar = false;
            base.OnLoad(e);
        }

        private void OnShow(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Normal;
            Show();
            shown = true;
        }

        private void OnHide(object sender, EventArgs e)
        {
            Hide();
            shown = false;
        }
        string path = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);
        public List<string> MusicSongs = new List<string>();
        public void EnableSpeech()
        {
            string[] songs = Directory.GetFiles(path, "*.mp3", SearchOption.TopDirectoryOnly);
            foreach (string song in songs)
            {
                MusicSongs.Add("Play " + Path.GetFileNameWithoutExtension(song));
            }
            try
            {
                objRecoContext = new SpeechLib.SpSharedRecoContext();

                //objRecoContext.Hypothesis += new _ISpeechRecoContextEvents_HypothesisEventHandler(Hypo_Event);

                objRecoContext.Recognition += new _ISpeechRecoContextEvents_RecognitionEventHandler(Reco_Event);

                grammar = objRecoContext.CreateGrammar(0);

                menuRule = grammar.Rules.Add("Commands", SpeechRuleAttributes.SRATopLevel | SpeechRuleAttributes.SRADynamic, 1);
                object PropValue = "";

                foreach (string command in commands)
                {
                    menuRule.InitialState.AddWordTransition(null, command, " ", SpeechGrammarWordType.SGLexical, command, 1, ref PropValue, 1.0F);
                }

                foreach (string command in ExtraCommands)
                {
                    menuRule.InitialState.AddWordTransition(null, command, " ", SpeechGrammarWordType.SGLexical, command, 1, ref PropValue, 1.0F);
                }

                foreach (string song in MusicSongs)
                {
                    menuRule.InitialState.AddWordTransition(null, song, " ", SpeechGrammarWordType.SGLexical, song, 1, ref PropValue, 1.0F);
                }
                menuRule.InitialState.AddWordTransition(null, Properties.Settings.Default.PassPhrase, " ", SpeechGrammarWordType.SGLexical, Properties.Settings.Default.PassPhrase, 1, ref PropValue, 1.0F);
                grammar.Rules.Commit();
                grammar.CmdSetRuleState("Commands", SpeechRuleState.SGDSActive);
            } catch
            {
                (System.Windows.Forms.Application.OpenForms["Form1"] as Anima.Form1).Pasukonsay("I apologise but there's an error with the voice recognition so it won't work.");
            }

        }

        public void RebuildSpeech()
        {
            menuRule.Clear();
            grammar.Reset();
            menuRule = grammar.Rules.Add("Commands", SpeechRuleAttributes.SRATopLevel | SpeechRuleAttributes.SRADynamic, 1);
            object PropValue = "";

            foreach (string command in commands)
            {
                menuRule.InitialState.AddWordTransition(null, command, " ", SpeechGrammarWordType.SGLexical, command, 1, ref PropValue, 1.0F);
            }

            foreach (string command in ExtraCommands)
            {
                menuRule.InitialState.AddWordTransition(null, command, " ", SpeechGrammarWordType.SGLexical, command, 1, ref PropValue, 1.0F);
            }

            foreach (string song in MusicSongs)
            {
                menuRule.InitialState.AddWordTransition(null, song, " ", SpeechGrammarWordType.SGLexical, song, 1, ref PropValue, 1.0F);
            }
            menuRule.InitialState.AddWordTransition(null, Properties.Settings.Default.PassPhrase, " ", SpeechGrammarWordType.SGLexical, Properties.Settings.Default.PassPhrase, 1, ref PropValue, 1.0F);
            grammar.Rules.Commit();
            grammar.CmdSetRuleState("Commands", SpeechRuleState.SGDSActive);
        }

        private async void Reco_Event(int StreamNumber, object StreamPosition, SpeechRecognitionType RecognitionType, ISpeechRecoResult Result)
        {
            await Task.Delay(250);
            textBox1.Text = Result.PhraseInfo.GetText(0, -1, true);
            Submit_Click(null, null);

        }

        //private async void Hypo_Event(int StreamNumber, object StreamPosition, ISpeechRecoResult Result)
        //{
        //    await Task.Delay(250);
        //    textBox1.Text = Result.PhraseInfo.GetText(0, -1, true);
        //    if (textBox1.Text == "Go to default mode")
        //    {
        //        (System.Windows.Forms.Application.OpenForms["Form1"] as PaskonV1.Form1).Pasukonsay("Back to normal mode");
        //        mode = "Default";
        //    }
        //    if (mode == "play")
        //    {
        //        PlaySong(textBox1.Text);
        //    }
        //    else
        //    {
        //        Submit_Click(null, null);
        //    }
        //}

        protected override void Dispose(bool isdisposing)
        {
            if (isdisposing)
            {
                m_notifyicon.Dispose();
            }
            base.Dispose(isdisposing);
        }

        // Gives a greeting based on time of day, fixed
        private async void Greeting()
        {
            if (Properties.Settings.Default.SetupRunOnce == false)
            {
                (System.Windows.Forms.Application.OpenForms["Form1"] as Anima.Form1).Pasukonsay("Hello, my name is Anima and I will be your personal assistant", 7500);
                await Task.Delay(10000);
                (System.Windows.Forms.Application.OpenForms["Form1"] as Anima.Form1).Pasukonsay("To see the main list of commands, you can say 'Show command list' at any time", 5000);
                await Task.Delay(5000);
                (System.Windows.Forms.Application.OpenForms["Form1"] as Anima.Form1).Pasukonsay("Like so - Show command list");
                await Task.Delay(2500);
                Form commandlist = new Form2();
                commandlist.Show();                
     
            }
            else
            {
                TimeSpan Morning = new TimeSpan(6, 0, 0);
                TimeSpan Noon = new TimeSpan(12, 0, 0);
                TimeSpan AfternoonEnd = new TimeSpan(19, 0, 0);
                TimeSpan Evening = new TimeSpan(22, 0, 0);
                TimeSpan now = DateTime.Now.TimeOfDay;
                string greeting;
                string name = Properties.Settings.Default.UserName;
                string Name = null;
                Name = name;
                if (name == "")
                {
                    Name = System.Security.Principal.WindowsIdentity.GetCurrent().Name.ToString();
                    Name = Name.Remove(0, name.LastIndexOf(@"\") + 1);
                    name = Name;
                }
                if ((now > Morning) && (now < Noon))
                {
                    greeting = ("Good Morning "+Name);
                }
                else
                    if ((now > Noon) && (now < AfternoonEnd))
                    {
                        greeting = ("Good Afternoon "+Name);
                    }
                    else
                        if ((now > AfternoonEnd) && (now < Evening))
                        {
                            greeting = ("Good Evening "+Name);
                        }
                        else
                        {
                            greeting = ("You're still up "+Name+"?");
                        }
                if (System.Windows.Forms.Application.OpenForms["Form1"] != null)
                {
                    (System.Windows.Forms.Application.OpenForms["Form1"] as Anima.Form1).Pasukonsay(greeting);
                }
            }
        }

        private void InitializeComponent()
        {
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.Submit = new System.Windows.Forms.Button();
            this.Clear = new System.Windows.Forms.Button();
            this.Commandlistbtn = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(4, 9);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(274, 20);
            this.textBox1.TabIndex = 0;
            // 
            // Submit
            // 
            this.Submit.BackColor = System.Drawing.SystemColors.MenuBar;
            this.Submit.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Submit.Location = new System.Drawing.Point(106, 35);
            this.Submit.Name = "Submit";
            this.Submit.Size = new System.Drawing.Size(80, 28);
            this.Submit.TabIndex = 1;
            this.Submit.Text = "Submit";
            this.Submit.UseVisualStyleBackColor = false;
            this.Submit.Click += new System.EventHandler(this.Submit_Click);
            // 
            // Clear
            // 
            this.Clear.BackColor = System.Drawing.SystemColors.MenuBar;
            this.Clear.Location = new System.Drawing.Point(192, 35);
            this.Clear.Name = "Clear";
            this.Clear.Size = new System.Drawing.Size(80, 28);
            this.Clear.TabIndex = 2;
            this.Clear.Text = "Clear";
            this.Clear.UseVisualStyleBackColor = false;
            this.Clear.Click += new System.EventHandler(this.Clear_Click);
            // 
            // Commandlistbtn
            // 
            this.Commandlistbtn.BackColor = System.Drawing.SystemColors.MenuBar;
            this.Commandlistbtn.Location = new System.Drawing.Point(11, 35);
            this.Commandlistbtn.Name = "Commandlistbtn";
            this.Commandlistbtn.Size = new System.Drawing.Size(28, 27);
            this.Commandlistbtn.TabIndex = 3;
            this.Commandlistbtn.Text = "?";
            this.Commandlistbtn.UseVisualStyleBackColor = false;
            this.Commandlistbtn.Click += new System.EventHandler(this.Commandlistbtn_Click);
            // 
            // SysTrayApp
            // 
            this.AcceptButton = this.Submit;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.ClientSize = new System.Drawing.Size(284, 67);
            this.Controls.Add(this.Commandlistbtn);
            this.Controls.Add(this.Clear);
            this.Controls.Add(this.Submit);
            this.Controls.Add(this.textBox1);
            this.Location = new System.Drawing.Point(750, 0);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(300, 105);
            this.MinimumSize = new System.Drawing.Size(300, 105);
            this.Name = "SysTrayApp";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = Properties.Settings.Default.PasukonName + " entry box";
            this.TopMost = true;
            this.WindowState = System.Windows.Forms.FormWindowState.Minimized;
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        private const int WS_SYSMENU = 0x80000;
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.Style &= ~WS_SYSMENU;
                return cp;
            }
        }


        private void Submit_Click(object sender, EventArgs e)
        {
            string Command = textBox1.Text;
            if (Command == "Go to default mode")
            {
                (System.Windows.Forms.Application.OpenForms["Form1"] as Anima.Form1).Pasukonsay("Back to normal mode");
                mode = "Default";
                Command = "";
            }
            if ((Command.StartsWith("Play") == true) && (mode != "Queue"))
            {
                PlaySong(Command);
                Command = "";
            }
            if (Command == "Reset to defaults")
            {
                ResetToDefaults();
            }
            textBox1.Text = "";
            switch (mode)
            {
                case "muted": if (Command == "Unmute")
                    {
                        (System.Windows.Forms.Application.OpenForms["Form1"] as Anima.Form1).Pasukonsay("I'm ready for commands");
                        mode = "Default";
                    }
                    break;

                case "Alert": GuardMode(Command);
                    break;

                case "Default": DefaultCommands(Command);
                    break;

                case "find": Google(Command);
                    break;

                case "page": Webpage(Command);
                    break;


                // case "Queue": AddSongToQueue(Command);
                //   break;

            }
        }

        private async void DefaultCommands(string Command)
        {
            if (Command == "")
            {
            }
            {
                int Choice = 0;
                for (int i = 1; i < commands.Length; i++)
                {
                    if (Command == commands[i])
                    {
                        Choice = i;
                    }
                }
                Choice = Choice + 1;
                switch (Choice)
                {
                    case 1:             //Just leave it in, a blank option seems to help it not just lock the computer a lot. Really glad I didn't have TurnOffComp() at the top t of the list.
                        break;

                    case 2: LockComp();
                        break;
                        
                    case 3: TurnOffComp();
                        break;
                                        
                    case 4: RestartComp();
                        break;

                    case 5: (System.Windows.Forms.Application.OpenForms["Form1"] as Anima.Form1).PlaceLowerRight();
                        break;

                    case 6: GiveTime();
                        break;

                    case 7: GiveDate();
                        break;

                    case 8: BootChrome();
                        break;

                    case 9: BootSK();
                        break;

                    case 10: BootTmnger();
                        break;

                    case 11: BootTitanfall();
                        break;

                    case 12: HowAreYou();
                        break;

                    case 13: PlaySong(Command);
                        break;

                    case 14: PauseMusic();
                        break;

                    case 15: ContinueMusic();
                        break;

                    case 16: NextSong();
                        break;

                    case 17: LastSong();
                        break;

                    case 18: VolumeDown();
                        break;

                    case 19: VolumeUP();
                        break;

                    case 21: (System.Windows.Forms.Application.OpenForms["Form1"] as Anima.Form1).Pasukonsay("I'm back again!");
                        (System.Windows.Forms.Application.OpenForms["Form1"] as Anima.Form1).Show();
                        break;

                    case 20: (System.Windows.Forms.Application.OpenForms["Form1"] as Anima.Form1).Pasukonsay("I'll just keep out of sight");
                        (System.Windows.Forms.Application.OpenForms["Form1"] as Anima.Form1).Hide();
                        break;

                    case 22: mciSendString("set CDAudio door open", "", 127, IntPtr.Zero);
                        break;

                    case 23: BootVS();
                        break;

                    case 24: OpenTumblr();
                        break;

                    case 25: OpenAoTgame();
                        break;

                    case 26: CloseProgram("Chrome");
                        break;

                    case 27: CloseProgram("StrongholdKingdoms");
                        break;

                    case 28: CloseProgram("Taskmgr");
                        break;

                    case 29: CloseProgram("Oblivion");
                        break;

                    case 30: (System.Windows.Forms.Application.OpenForms["Form1"] as Anima.Form1).Pasukonsay("I hope you have an enjoyable morning, I wish you the best of luck");
                        break;

                    case 31: (System.Windows.Forms.Application.OpenForms["Form1"] as Anima.Form1).Pasukonsay("It is reasonably nice I guess, enjoy the rest of it, have you woken up yet?");
                        break;

                    case 32: (System.Windows.Forms.Application.OpenForms["Form1"] as Anima.Form1).Pasukonsay("Evening " + Properties.Settings.Default.UserName + ", I wonder if the stars are out, I just love the night sky don't you?");
                        break;

                    case 33: (System.Windows.Forms.Application.OpenForms["Form1"] as Anima.Form1).Pasukonsay("You're welcome " + Properties.Settings.Default.UserName);
                        break;

                    case 34: NameSong();
                        break;

                    case 35: (System.Windows.Forms.Application.OpenForms["Form1"] as Anima.Form1).Pasukonsay("I'll wait till you need me");
                        mode = "muted";
                        break;

                    case 36: (System.Windows.Forms.Application.OpenForms["Form1"] as Anima.Form1).Pasukonsay("Displaying manual entry box");
                        OnShow(null, null);
                        break;

                    case 37: (System.Windows.Forms.Application.OpenForms["Form1"] as Anima.Form1).Pasukonsay("Hiding manual entry box");
                        Hide();
                        break;

                    case 38: (System.Windows.Forms.Application.OpenForms["Form1"] as Anima.Form1).Pasukonsay("I'm on guard, Sir!");
                        mode = "Alert";
                        Properties.Settings.Default.GuardMode = true;
                        (System.Windows.Forms.Application.OpenForms["Form1"] as Anima.Form1).PlaceCenter();
                        alertTimer.Elapsed += new ElapsedEventHandler(AlertCheck);
                        alertTimer.Interval = 7500;
                        alertTimer.Enabled = true;
                        GC.KeepAlive(alertTimer);
                        break;

                    case 39: OnShow(null, null);
                        (System.Windows.Forms.Application.OpenForms["Form1"] as Anima.Form1).Pasukonsay("What would you like to google? Please enter it into the box.");
                        await Task.Delay(1500);
                        mode = "find";
                        break;

                    case 40: objRecoContext.Pause();
                        OnShow(null, null);
                        (System.Windows.Forms.Application.OpenForms["Form1"] as Anima.Form1).Pasukonsay("What page shall I open? Please enter it into the box.");
                        await Task.Delay(1500);
                        mode = "page";
                        break;

                    case 41: (System.Windows.Forms.Application.OpenForms["Form1"] as Anima.Form1).Pasukonsay("Disabling the alarm");
                        Properties.Settings.Default.AlarmIO = false;
                        Properties.Settings.Default.Save();
                        break;

                    case 42: (System.Windows.Forms.Application.OpenForms["Form1"] as Anima.Form1).Pasukonsay("Enabling the alarm");
                        Properties.Settings.Default.AlarmIO = true;
                        Properties.Settings.Default.Save();
                        break;

                    //  case 43: (System.Windows.Forms.Application.OpenForms["Form1"] as PaskonV1.Form1).Pasukonsay("What shall I play next?");
                    //     mode = "Queue";
                    //     break;

                    case 44: (System.Windows.Forms.Application.OpenForms["Form1"] as Anima.Form1).Pasukonsay("Showing command list, this is everything you can say, almost");
                        try
                        {
                            Form CommandList = new Anima.Form2();
                            CommandList.Show();
                        } catch
                        {
                            (System.Windows.Forms.Application.OpenForms["Form1"] as Anima.Form1).Pasukonsay("Something went wrong");
                        }
                        break;

                    case 45: (System.Windows.Forms.Application.OpenForms["Form1"] as Anima.Form1).Pasukonsay("Showing the song list, just tell me to play any of these");
                        Form SongList = new Anima.Form3();
                        SongList.Show();
                        break;

                    case 46: try
                        {
                            Form AlarmList = new Anima.Form5();
                            AlarmList.Show();
                            (System.Windows.Forms.Application.OpenForms["Form1"] as Anima.Form1).Pasukonsay("Here's the alarm list");
                        } catch
                        {
                            (System.Windows.Forms.Application.OpenForms["Form1"] as Anima.Form1).Pasukonsay("Something went wrong");
                        }
                        break;
                }
            }
        }
       

        private void AlertCheck(object sender, ElapsedEventArgs e)
        {
            GuardMode(null);
        }

        private void GuardMode(string command)
        {
            Process[] processes;
            if (!(command == Properties.Settings.Default.PassPhrase))
            {
                processes = Process.GetProcessesByName("Taskmgr");
                try
                {
                    foreach (Process proc in processes)
                    {
                        (System.Windows.Forms.Application.OpenForms["Form1"] as Anima.Form1).Pasukonsay("Trying to use task manager to shut me down? Shame on you.");
                        proc.CloseMainWindow();
                    }
                }
                catch
                {
                }
                System.Diagnostics.Process.Start(@"C:\WINDOWS\system32\rundll32.exe", "user32.dll,LockWorkStation");
            }
            else
            {
                mode = "Default";
                alertTimer.Enabled = false;
                Properties.Settings.Default.GuardMode = false;
                (System.Windows.Forms.Application.OpenForms["Form1"] as Anima.Form1).Pasukonsay("You're back? I guarded carefully");
                (System.Windows.Forms.Application.OpenForms["Form1"] as Anima.Form1).PlaceLowerRight();
            }
        }

        string Search;
        private void Google(string Command)
        {
            string URL = @"http://google.com/search?q=";
            string SearchFix = Command.Replace(" ", "+");
            if (Command == "find it")
            {
                Process.Start(URL + Search);
                (System.Windows.Forms.Application.OpenForms["Form1"] as Anima.Form1).Pasukonsay("Googling " + Search.Replace("+", " "));
                Hide();
                mode = "Default";
            }
            else
            {
                Search += '+' + SearchFix;
            }
        }

        private void Webpage(string Command)
        {
            string URL = "www.";
            Process.Start(URL + Command);
            (System.Windows.Forms.Application.OpenForms["Form1"] as Anima.Form1).Pasukonsay("Opening " + Command);
            objRecoContext.Resume();
            Hide();
            mode = "Default";
        }
       // string SingleSongPlayed;
        public void PlaySong(string Song)
        {
            string CorrectSong = "";
            if (Song == "Play some music")
            {
                Player.URL = Properties.Settings.Default.DefaultPlaylist;
                CorrectSong = "some music";
                Player.settings.setMode("shuffle", true);
                Player.settings.setMode("autoRewind", true);
                Player.controls.play();
                (System.Windows.Forms.Application.OpenForms["Form1"] as Anima.Form1).Pasukonsay("Playing some music, enjoy the song");
            }
            else
            {
                CorrectSong = Song.Remove(0, 5);
                (System.Windows.Forms.Application.OpenForms["Form1"] as Anima.Form1).Pasukonsay("Playing " + CorrectSong + ", enjoy the song");
                Player.URL = path + @"\" + CorrectSong + ".mp3";
                Player.controls.play();
          //      SingleSongPlayed = "Just One";
               // Player.PlayStateChange += new WMPLib._WMPOCXEvents_PlayStateChangeEventHandler(wplayer_PlayStateChange);
            }
        }
        int QueueCount = 0;
       // int QueuePoint = 1;
        string[] Queue = new string[20];
         public void AddSongToQueue(string SongTOBAdded)
         {
             //TODO: Fix
             //SingleSongPlayed = "Queued";
             QueueCount = QueueCount + 1;
             Queue[QueueCount] = SongTOBAdded;
             mode = "Default";
             (System.Windows.Forms.Application.OpenForms["Form1"] as Anima.Form1).Pasukonsay("I will play " + SongTOBAdded.Remove(0, 5) + " next"); ;

         } 
       /* public void wplayer_PlayStateChange(int NewState)
        {
            if (NewState == (int)WMPLib.WMPPlayState.wmppsMediaEnded)
            {
                if (SingleSongPlayed == "Just One")
                {
               
                    PlaySong("Play some music");
                    SingleSongPlayed = "";
                }
                else if (SingleSongPlayed == "Queued")
                {
                    if (NewState == (int)WMPLib.WMPPlayState.wmppsMediaEnded)
                    {
                        if (QueuePoint > QueueCount)
                        {
                            for (int i = 0; i < Queue.Length; i++)
                            {
                                Queue[i] = "";
                            }
                            Queue[0] = "";
                            QueueCount = 0;
                            QueuePoint = 1;
                            SingleSongPlayed = "";
                            PlaySong("Play some music");
                        }
                        else
                        {
                            string CorrectSong = "";
                            CorrectSong = Queue[QueuePoint].Remove(0, 5);
                            //(System.Windows.Forms.Application.OpenForms["Form1"] as PaskonV1.Form1).Pasukonsay("Playing " + CorrectSong + ", enjoy the song");
                            Player.URL = path + @"\" + CorrectSong + ".mp3";
                            Player.controls.play();
                            QueuePoint = QueuePoint + 1;
                        }
                    }
                } 
            }
             
            //TODO Fix the song queueing
        } */

        private void PauseMusic()
        {
            (System.Windows.Forms.Application.OpenForms["Form1"] as Anima.Form1).Pasukonsay("Pausing music");
            Player.controls.pause();
        }

        private void ContinueMusic()
        {
            (System.Windows.Forms.Application.OpenForms["Form1"] as Anima.Form1).Pasukonsay("Playing from where you left off.");
            Player.controls.play();
        }
        private void NextSong()
        {
            (System.Windows.Forms.Application.OpenForms["Form1"] as Anima.Form1).Pasukonsay("Next song, coming up");
            Player.controls.next();
        }

        private void LastSong()
        {
            (System.Windows.Forms.Application.OpenForms["Form1"] as Anima.Form1).Pasukonsay("I did like that one, let's hear it again.");
            Player.controls.previous();
        }

        private void VolumeUP()
        {
            (System.Windows.Forms.Application.OpenForms["Form1"] as Anima.Form1).Pasukonsay("Raising volume");
            Player.settings.volume = Player.settings.volume + 10;
        }

        private void VolumeDown()
        {
            (System.Windows.Forms.Application.OpenForms["Form1"] as Anima.Form1).Pasukonsay("Lowering volume");
            Player.settings.volume = Player.settings.volume - 10;
        }

        private void NameSong()
        {
            if (Player.playState == WMPPlayState.wmppsPlaying)
            {
                string SongName = Player.currentMedia.name;
                (System.Windows.Forms.Application.OpenForms["Form1"] as Anima.Form1).Pasukonsay(SongName);
            }
            else
            {
                (System.Windows.Forms.Application.OpenForms["Form1"] as Anima.Form1).Pasukonsay("There isn't anything playing");
            }

        }

        private void HowAreYou()
        {
            Random rnd = new Random();
            int which = rnd.Next(1, 5);

            switch (which)
            {
                case 1: (System.Windows.Forms.Application.OpenForms["Form1"] as Anima.Form1).Pasukonsay("I am fine thank you, always happy to serve");
                    break;

                case 2: (System.Windows.Forms.Application.OpenForms["Form1"] as Anima.Form1).Pasukonsay("A bit sleepy but glad to be helpful");
                    break;

                case 3: (System.Windows.Forms.Application.OpenForms["Form1"] as Anima.Form1).Pasukonsay("I'm really tired, can I go to sleep?");
                    break;

                case 4: (System.Windows.Forms.Application.OpenForms["Form1"] as Anima.Form1).Pasukonsay("I'm feeling great and ready to serve.");
                    break;

                case 5: (System.Windows.Forms.Application.OpenForms["Form1"] as Anima.Form1).Pasukonsay("So, so, I could be better, I could be worse");
                    break;

            }
        }


        public void Clear_Click(object sender, EventArgs e)
        {
            textBox1.Text = "";
        }

        private async void LockComp()
        {
            (System.Windows.Forms.Application.OpenForms["Form1"] as Anima.Form1).Pasukonsay("Locking computer, Don't forget about me!");
            await Task.Delay(1500);
            System.Diagnostics.Process.Start(@"C:\WINDOWS\system32\rundll32.exe", "user32.dll,LockWorkStation");
        }

        private async void TurnOffComp()
        {
            (System.Windows.Forms.Application.OpenForms["Form1"] as Anima.Form1).Pasukonsay("Shutting down the computer, Goodnight " + Properties.Settings.Default.UserName);
            await Task.Delay(1500);
            System.Diagnostics.Process.Start("shutdown", "/s /t 0");
        }

        private async void RestartComp()
        {
            (System.Windows.Forms.Application.OpenForms["Form1"] as Anima.Form1).Pasukonsay("Restarting the computer, Be back in a second.");
            await Task.Delay(1500);
            System.Diagnostics.Process.Start("shutdown", "-r -t 0");
        }

        private void GiveTime()
        {
            DateTime time = DateTime.Now;
            string format = "h   mm";
            string Timenow = time.ToString(format);
            (System.Windows.Forms.Application.OpenForms["Form1"] as Anima.Form1).Pasukonsay("The time is currently " + Timenow);
        }
        private void GiveDate()
        {
            DateTime date = DateTime.Now;
            string datenow = date.ToString("D");
            (System.Windows.Forms.Application.OpenForms["Form1"] as Anima.Form1).Pasukonsay("The date is " + datenow);
        }

        private async void TimeSleep(object sender, EventArgs e)
        {
            if (Properties.Settings.Default.GuardMode == true)
            {
                (System.Windows.Forms.Application.OpenForms["Form1"] as Anima.Form1).Pasukonsay("Haha not a chance");
            }
            else
            {
                (System.Windows.Forms.Application.OpenForms["Form1"] as Anima.Form1).Pasukonsay("Goodbye "+Properties.Settings.Default.UserName);
                await Task.Delay(2500);
                Application.Exit();
            }
        }

        private async void BootChrome()
        {
            (System.Windows.Forms.Application.OpenForms["Form1"] as Anima.Form1).Pasukonsay("Opening chrome.");
            await Task.Delay(1500);
            System.Diagnostics.Process.Start(@"C:\Program Files (x86)\Google\Chrome\Application\chrome.exe");
        }

        private async void BootSK()
        {
            (System.Windows.Forms.Application.OpenForms["Form1"] as Anima.Form1).Pasukonsay("Opening Stronghold Kingdoms, How's your village doing?");
            await Task.Delay(1500);
            System.Diagnostics.Process.Start(@"C:\Program Files (x86)\Firefly Studios\Stronghold Kingdoms\StrongholdKingdoms.exe");
        }


        private async void BootVS()
        {
            (System.Windows.Forms.Application.OpenForms["Form1"] as Anima.Form1).Pasukonsay("Opening Visual Studio, I wonder what I'll get this time.");
            await Task.Delay(1500);
            System.Diagnostics.Process.Start(@"C:\Program Files (x86)\Microsoft Visual Studio 12.0\Common7\IDE\devenv.exe");
        }

        private async void OpenTumblr()
        {
            (System.Windows.Forms.Application.OpenForms["Form1"] as Anima.Form1).Pasukonsay("Opening Tumblr, try not to lose track of time.");
            await Task.Delay(1500);
            System.Diagnostics.Process.Start("https://www.tumblr.com/dashboard");
        }

        private async void OpenAoTgame()
        {
            (System.Windows.Forms.Application.OpenForms["Form1"] as Anima.Form1).Pasukonsay("Opening the Attack on Titan game, go for the neck.");
            await Task.Delay(1500);
            System.Diagnostics.Process.Start("http://fenglee.com/game/aog/");
        }

        private void CloseProgram(string procname)
        {
            (System.Windows.Forms.Application.OpenForms["Form1"] as Anima.Form1).Pasukonsay("Closing " + procname);
            Process[] processes;
            processes = Process.GetProcessesByName(procname);
            try
            {
                foreach (Process proc in processes)
                {
                    proc.CloseMainWindow();
                }
            }
            catch
            {
                (System.Windows.Forms.Application.OpenForms["Form1"] as Anima.Form1).Pasukonsay(procname + " isn't open");
            }
        }

        private async void BootTmnger()
        {
            (System.Windows.Forms.Application.OpenForms["Form1"] as Anima.Form1).Pasukonsay("Opening Task manager.");
            await Task.Delay(1500);
            System.Diagnostics.Process.Start(@"C:\Windows\System32\taskmgr.exe");
        }

        private async void BootTitanfall()
        {
            (System.Windows.Forms.Application.OpenForms["Form1"] as Anima.Form1).Pasukonsay("Running Titanfall, Good luck!");
            await Task.Delay(1500);
            System.Diagnostics.Process.Start(@"C:\Program Files (x86)\Origin Games\Titanfall\Titanfall.exe");
        }

        public static void Alarm()
        {
            // TODO: Fix changable alarms based off settings1 file, something about threads.
            if (Properties.Settings.Default.AlarmIO == true)
            {
                DateTime TimeNow = DateTime.Now;
                string format = "HH:mm";
                string currentTime = TimeNow.ToString(format);
                bool alarmed = false;
                foreach (string time in Properties.Settings1.Default.Times)
                {
                    bool Mon = time.Contains("Monday");
                    bool Tue = time.Contains("Tuesday");
                    bool Wed = time.Contains("Wednesday");
                    bool Thu = time.Contains("Thursday");
                    bool Fri = time.Contains("Friday");
                    bool Sat = time.Contains("Saturday");
                    bool Sun = time.Contains("Sunday");

                    string AlarmTime = time.Remove(5);
                    
                    if (Mon)
                    {
                        if (currentTime == AlarmTime)
                        {
                            alarmed = true;                        
                        }
                    }
                    if (Tue)
                    {
                        if (currentTime == AlarmTime)
                        {
                            alarmed = true;
                        }
                    }
                    if (Wed)
                    {
                        if (currentTime == AlarmTime)
                        {
                            alarmed = true;
                        }
                    }
                    if (Thu)
                    {
                        if (currentTime == AlarmTime)
                        {
                            alarmed = true;
                        }
                    }
                    if (Fri)
                    {
                        if (currentTime == AlarmTime)
                        {
                            alarmed = true;
                        }
                    }
                    if (Sat)
                    {
                        if (currentTime == AlarmTime)
                        {
                            alarmed = true;
                        }
                    }
                    if (Sun)
                    {
                        if (currentTime == AlarmTime)
                        {
                            alarmed = true;
                        }
                    }

                   
                    
                }
                if (alarmed)
                {
                    (System.Windows.Forms.Application.OpenForms["Form1"] as Anima.Form1).Pasukonsay(Properties.Settings.Default.UserName + " you need to do something.");
                }                
            }
        }
        private void Commandlistbtn_Click(object sender, EventArgs e)
        {
            Form CommandList = new Anima.Form2();
            CommandList.Show();
        }

    }
}