using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SnakeGame
{
    public partial class Form1 : Form
    {
        public enum Directon
        {
            Up,
            Down,
            Right,
            Left
        }

        public int score = 0, money = 0;
        public int X = 1, Y = 1;
        public Directon dr = Directon.Right;
        public Location feed = new Location(-1, -1);
        public List<Location> tales = new List<Location>();
        public bool Game = false;
        public Thread getThread;
        public Item[] items;
        Item selectedItem;
        public void ResetEveryThing()
        {
            score = 0; X = 1; Y = 1;
            dr = Directon.Right;
            feed = new Location(-1, -1);
            tales = new List<Location>();
            tales.Add(new SnakeGame.Location(0, 0));

        }

        [Obsolete]
        public Form1()
        {
            InitializeComponent();
            Item mainItem = new Item(color1, null);
            mainItem.status = 1;
            selectedItem = mainItem;
            tales.Add(new SnakeGame.Location(0, 0));
            CalcTable();
            items = new Item[]
            {
                mainItem,
                new Item(color2,price2),
                new Item(color3,price3),
                new Item(color4,price4),
                new Item(color5,price5),
                new Item(color6,price6),
            };
            if (File.Exists("store.ruslan")) LoadStore();
            if (File.Exists("setting.ruslan")) LoadMoney();
            moneyText.Text = money.ToString();
            getThread = new Thread(new ThreadStart(new Action(() =>
              {
                  while (Game)
                  {
                      if (dr == Directon.Right) X++;
                      if (dr == Directon.Down) Y++;
                      if (dr == Directon.Up) Y--;
                      if (dr == Directon.Left) X--;
                      CalcTable();
                      Thread.Sleep(100);
                  }

              })));
        }

        [Obsolete]
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.ToString() == "S") dr = Directon.Down;
            if (e.KeyCode.ToString() == "Down") dr = Directon.Down;
            if (e.KeyCode.ToString() == "Up") dr = Directon.Up;
            if (e.KeyCode.ToString() == "Left") dr = Directon.Left;
            if (e.KeyCode.ToString() == "Right") dr = Directon.Right;
            if (e.KeyCode.ToString() == "W") dr = Directon.Up;
            if (e.KeyCode.ToString() == "A") dr = Directon.Left;
            if (e.KeyCode.ToString() == "D") dr = Directon.Right;

            if (e.KeyCode.ToString() == "Escape")
            {
                ShowMenu(0);
            }

        }

        [Obsolete]
        private void button1_Click(object sender, EventArgs e)
        {
            string str = ((Button)sender).Text;
            menu.Visible = false;
            this.Focus();
            Game = true;
            getThread.Resume();
            if (str == "Play Again") ResetEveryThing();

        }
        private void button2_Click(object sender, EventArgs e)
        {
            pictureBox1.Visible = false;
            mainmenu.Visible = true;
            menu.Visible = false;
            Game = false;
        }

        [Obsolete]
        public void ShowMenu(int status)
        {
            if (status == 0)
            {
                menu.Visible = true;
                pause.Text = "Pause";
                button1.Text = "Continue";
                getThread.Suspend();
                Game = false;
            }
            else if (status == 1)
            {
                Invoke(new Action(() =>
                {
                    pause.Text = "Game Over";
                    button1.Text = "Play Again";
                    money += score;
                    menu.Visible = true;
                    getThread.Suspend();
                    Game = false;
                }));

            }
        }
        [Obsolete]
        private void btn_Play_Click(object sender, EventArgs e)
        {

            mainmenu.Visible = false;
            pictureBox1.Visible = true;
            Game = true;
            this.Focus();
            if (getThread.ThreadState == ThreadState.Unstarted) getThread.Start();
            if (getThread.ThreadState == ThreadState.Suspended) getThread.Resume();
            ResetEveryThing();
        }


        // Drag Form
        [DllImport("user32.DLL", EntryPoint = "ReleaseCapture")]
        private extern static void ReleaseCapture();
        [DllImport("user32.DLL", EntryPoint = "SendMessage")]
        private extern static void SendMessage(System.IntPtr hWnd, int wMsg, int wParam, int lParam);
        private void panel2_MouseDown(object sender, MouseEventArgs e)
        {
            ReleaseCapture();
            SendMessage(this.Handle, 0x112, 0xf012, 0);
        }

        [Obsolete]
        public void CalcTable()
        {
            try
            {
                Invoke(new Action(() => label1.Text = "Score:" + score));
            }
            catch { }
            Random rnd = new Random();
            Bitmap bitmap = new Bitmap(500, 500);
            //Ozune Deyerek Oyunu Dayandirma
            if (tales.Count != 1)
            {
                for (int i = 1; i < tales.Count; i++)
                {
                    if (tales[i].x == X && tales[i].y == Y)
                    {
                        ShowMenu(1);
                    }
                }
            }
            //Yem Yeme
            if (X == feed.x && Y == feed.y)
            {
                score++;
                tales.Add(new SnakeGame.Location(feed.x, feed.y));
                feed = new Location(-1, -1);
            }
            //Kenarlara Deyerek Oyunu Dayandirma 
            if (X <= 0 || Y <= 0 || X == 51 || Y == 51)
            {
                ShowMenu(1);
            }
            //Eks Halda Davam Etme
            else
            {
                for (int i = (X - 1) * 10; i < X * 10; i++)
                {
                    for (int j = (Y - 1) * 10; j < Y * 10; j++)
                    {
                        bitmap.SetPixel(i, j, Color.Yellow);
                    }
                }
            }
            //
            if (tales.Count != 1)
            {
                for (int k = 0; k < tales.Count; k++)
                {
                    for (int i = (tales[k].x - 1) * 10; i < tales[k].x * 10; i++)
                    {
                        for (int j = (tales[k].y - 1) * 10; j < tales[k].y * 10; j++)
                        {
                            bitmap.SetPixel(i, j, selectedItem.box.BackColor);
                        }
                    }
                }
            }
            //
            tales[0] = new Location(X, Y);
            //
            for (int i = tales.Count - 1; i > 0; i--)
            {
                tales[i] = tales[i - 1];
            }
            //
            if (feed.x == -1)
            {
                feed = new Location(rnd.Next(1, 50), rnd.Next(1, 50));

            }
            //
            for (int i = (feed.x - 1) * 10; i < feed.x * 10; i++)
            {
                for (int j = (feed.y - 1) * 10; j < feed.y * 10; j++)
                {
                    bitmap.SetPixel(i, j, selectedItem.box.BackColor);
                }
            }
            pictureBox1.Image = bitmap;

        }


        public void SaveMoney()
        {

            StreamWriter sw = new StreamWriter("setting.ruslan");
            sw.WriteLine(money);
            sw.Close();
        }

        public void LoadMoney()
        {
            StreamReader sr = new StreamReader("setting.ruslan");
            string mny = sr.ReadLine();
            money = int.Parse(mny);
            sr.Close();
        }
        private void btn_Minimize_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void btn_Store_Click(object sender, EventArgs e)
        {
            storePage.Visible = true;
            moneyText.Text = money + "";
        }

        private void button3_Click(object sender, EventArgs e)
        {
            storePage.Visible = false;
            mainmenu.Visible = true;
        }

        private void pic_black_Click(object sender, EventArgs e)
        {
            PictureBox clicked = ((PictureBox)sender);
            foreach (Item item in items)
            {
                if (item.box.Name == clicked.Name)
                {
                    if (item.status == 0)
                    {
                        DialogResult dr = MessageBox.Show("Do you want to buy?", "Information", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                        if (dr == DialogResult.Yes)
                        {
                            if (money >= int.Parse(item.text.Text))
                            {
                                money = money - int.Parse(item.text.Text);
                                moneyText.Text = money + "";
                                SaveMoney();
                                item.status = 2;
                                SaveStore();
                                ReloadStore();
                            }
                            else MessageBox.Show("Have Not Enough Money", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                    else
                    {
                        foreach (Item item1 in items)
                        {
                            if (item1.status == 1) item1.status = 2;
                        }
                        item.status = 1;
                        selectedItem = item;

                        SaveStore();
                        ReloadStore();
                    }
                }
            }

        }

        public void SaveStore()
        {
            StreamWriter sw = new StreamWriter("store.ruslan");
            foreach (Item item in items)
            {
                sw.WriteLine(item.box.Name + "&" + item.status);
            }
            sw.Close();
        }
        public void ReloadStore()
        {
            foreach (Item item in items)
            {
                if (item.status == 0)
                {
                    item.box.Image = Properties.Resources.Lock;
                }
                else if (item.status == 1)
                {
                    item.box.Image = Properties.Resources.Check;
                    selectedItem = item;
                }
                else
                {
                    item.box.Image = null;
                }
            }
        }
        public void LoadStore()
        {
            StreamReader sr = new StreamReader("store.ruslan");
            string str = sr.ReadLine();
            while (str != null)
            {
                string[] splited = str.Split('&');
                items[int.Parse(splited[0].Replace("color", "")) - 1].status = int.Parse(splited[1]);
                ReloadStore();



                str = sr.ReadLine();
            }
            sr.Close();
        }

        [Obsolete]
        private void btn_Exit_Click(object sender, EventArgs e)
        {
            if (getThread.ThreadState == ThreadState.Suspended) getThread.Resume();
            getThread.Abort();
            Environment.Exit(0);
        }


        private void btn_Close_Click(object sender, EventArgs e)
        {
            SaveMoney();
            Environment.Exit(0);
        }
    }


    public class Item
    {
        public PictureBox box;
        public Label text;
        public int status = 0;

        public Item(PictureBox box, Label text)
        {
            this.box = box;
            this.text = text;
        }
    }
    public class Location
    {
        public int x, y;
        public Location(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
    }
}
