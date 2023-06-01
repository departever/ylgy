using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using System.Xml.Linq;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ProgressBar;


namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        //方块类
        class block
        {
            public int index;
            public Bitmap bitmap;
        }

        //生成一个随机数
        Random random = new Random();
        
        //将选中的按钮加入下面的panel
        private void Button_Click(object sender, EventArgs e)
        {
            Button clickedBtn = (Button)sender;

            AddImageButtonToPanel(clickedBtn);

            this.Controls.Remove(clickedBtn);

            ButtonShow();
        }

        //把Image转换成字节数组
        private byte[] ImageToBytes(Image image)
        {
            // 创建内存中读写数据的流
            using (MemoryStream ms = new MemoryStream())
            {
                // 把图片数据以Png格式写入到流中
                image.Save(ms, ImageFormat.Png);
                // 返回一个包含流中数据的字节数组
                return ms.ToArray();
            }
        }

        //重新排列按钮
        private void ReorderButtons()
        {
            // 获取panel中Button类型的控件的数量
            int buttonCount = panel.Controls.OfType<Button>().Count();
            // 遍历panel中的每个Button类型的控件
            for (int i = 0; i < buttonCount; i++)
            {
                // 获取第i个Button类型的控件
                Button button = panel.Controls.OfType<Button>().ElementAt(i);
                if (button != null)
                {
                    button.Location = new Point((i % 7) * 50 + 5, 0);
                }
            }
        }

        //把选中的按钮加入槽
        private void AddImageButtonToPanel(Button button)
        {
            // 存储相同图片的Button
            List<Button> sameImageButtons = new List<Button>();
            // 把图片转换成字符串
            string imageStr = Convert.ToBase64String(ImageToBytes(button.Image));
            
            // 遍历Button
            for (int i = 0; i < panel.Controls.Count; i++)
            {
                // 获取第i个控件，并转换为Button类型
                Button existButton = (Button)panel.Controls[i];
                // 获取图片的64位编码字符串
                string existImageStr = Convert.ToBase64String(ImageToBytes(existButton.Image));
                // 判断新Button和已存在的Button是否有相同的图片
                if (imageStr == existImageStr)
                {
                    sameImageButtons.Add(existButton);
                }
            }

            // 3消效果
            if (sameImageButtons.Count.Equals(2))
            {
                sameImageButtons.Add(button);
                // 遍历sameImageButtons中的Button
                for (int i = sameImageButtons.Count - 1; i >= 0; i--)
                {
                    // 获取第i个Button
                    Button sameImageButton = sameImageButtons[i];
                    // 从panel中移除这个Button
                    panel.Controls.Remove(sameImageButton);
                }

                ////重新排列按钮
                ReorderButtons();
                return;
            }

            // 将新Button添加到Panel中
            CtrEnabled.SetControlEnabled(button, false);
            panel.Controls.Add(button);

            //重新排列所有Button
            ReorderButtons();

            // 判断游戏结束
            if (panel.Controls.OfType<Button>().Count().Equals(7))
            {
                MessageBox.Show("游戏结束，方块已满", "Sorry", MessageBoxButtons.OK, MessageBoxIcon.Error);
                axWindowsMediaPlayer1.Ctlcontrols.stop();
                this.Close();
                this.Hide();
                Form2 frm2 = new Form2();
                frm2.ShowDialog();
            }
        }

        //获得背景图片
        public Bitmap GetRandomBackground()
        {
            List<Image> images = new List<Image>();
            //16张图片遍历获取
            for (int i = 0; i < 16; i++)
            {
                string fileName = @"C:\Users\asus\Desktop\ylgy\IMAGE\" + i + ".jpg";
                Image _image = Image.FromFile(fileName);
                images.Add(_image);
            }
            int imageIndex = random.Next(0, images.Count);
            // 复制图片来创建新的按钮背景
            Image image = images[imageIndex];
            Bitmap buttonBackground = new Bitmap(image);
            return buttonBackground;
        }

        //显示按钮 灰度化效果
        private void ButtonShow()
        {
            //设置button使用
            for (int i = 0; i < this.Controls.Count; i++) 
            { 
                if (this.Controls[i] is Button) { 
                    this.Controls[i].Enabled = true; 
                } 
            }

            for (int i = 0; i < this.Controls.Count; i++)
            {
                if (this.Controls[i] is Button btn1)
                {
                    for (int j = 0; j < this.Controls.Count; j++)
                    {
                        if (this.Controls[j] is Button btn2)
                        {
                            if (btn1.TabIndex > btn2.TabIndex)
                            {
                                //检测重叠
                                Rectangle b1 = new Rectangle(btn1.Location, btn1.Size);
                                Rectangle b2 = new Rectangle(btn2.Location, btn2.Size);
                                if (b1.IntersectsWith(b2))
                                {
                                    // 比较索引，小的放于上方，大的放于下方
                                    btn1.Enabled = false;
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }

        //音乐播放判断
        bool isPlay = false;

        //加载游戏页面
        private void Form1_Load(object sender, EventArgs e)
        {

            //广告
            AxWMPLib.AxWindowsMediaPlayer myPlayer = new AxWMPLib.AxWindowsMediaPlayer();
            axWindowsMediaPlayer2.PlayStateChange += axWindowsMediaPlayer2_PlayStateChange;

            //启动播放器
            axWindowsMediaPlayer1.URL = @"C:\Users\asus\Desktop\ylgy\Song\普通disco.m4a";
            axWindowsMediaPlayer1.Ctlcontrols.play();

            //生成上方的点击区
            List<block> list1 = new List<block>();

            //生成一个240个不同的随机数的数组
            int[] arr1 = new int[240];
            Random rnd = new Random();
            arr1 = Enumerable.Range(0, 240).
                OrderBy(n => rnd.Next()).
                ToArray();

            //赋予图片背景
            for (int i = 0; i < arr1.Length / 3; i++)
            {
                Bitmap buttonBackground = GetRandomBackground();
                for (int j = 0; j < 3; j++)
                {
                    var pai = new block { bitmap = buttonBackground, index = arr1[i * 3 + j] };
                    list1.Add(pai);
                }
            }

            var blklist1 = list1.OrderBy(x => x.index).ToArray();
            int layers = random.Next(2, 4);//随机生成图片层数 可以调整关卡难度
            for (int i = 0; i < layers; i++)
            {
                //不同层的偏移量
                int offsetX = 20 * random.Next(-1, 2);
                int offsetY = 20 * random.Next(-1, 2);

                int count = random.Next(0, 9) * 3;//*3以保证图片数量为3的倍数 count为限制图片最多显示的数目
                int l = 0;//没有显示的图片数目
                int lx = 0;
                int ly = 0;
                //生成5*5的方块区
                for (int x = 0; x < 5; x++, lx++)
                {
                    for (int y = 0; y < 5; y++)
                    {
                        int isShow = random.Next(0, 2);//随机生成01判断
                        if (l < count)
                        {
                            l++;
                            if (isShow.Equals(0))
                            {
                                continue;
                            }
                        }
                        Button but1 = new Button();
                        but1.Location = new System.Drawing.Point(x: 160 + 50 * x + offsetX, y: 80 + 50 * y + offsetY);
                        but1.Size = new System.Drawing.Size(width: 50, height: 50);
                        but1.Image = blklist1[i * 25 + lx * 5 + ly].bitmap;
                        but1.BackgroundImageLayout = ImageLayout.Stretch;
                        but1.TabIndex = i;//赋予按钮值
                        this.Controls.Add(but1);
                        but1.Click += new EventHandler(Button_Click);
                        ly++;
                    }
                }
            }

            //左下方边的卡片区
            List<block> list2 = new List<block>();
            int size2 = random.Next(1, 20);
            int[] array2 = new int[size2];
            array2 = Enumerable.Range(0, size2)
                        .OrderBy(n => (new Random(n).Next()))
                        .ToArray<int>();
            for (int i = 0; i < array2.Length / 3; i++)
            {
                Bitmap buttonBackground = GetRandomBackground();
                for (int j = 0; j < 3; j++)
                {
                    var block1 = new block { bitmap = buttonBackground, index = array2[i * 3 + j] };
                    list2.Add(block1);
                }
            }
            var blklist2 = list2.OrderByDescending(x => x.index).ToArray();
            foreach (var item in blklist2)
            {
                Button but2 = new Button();
                but2.Location = new System.Drawing.Point(x: 100 + (48 - size2) * 2 + 5 * item.index, y: 430);
                but2.Size = new System.Drawing.Size(width: 50, height: 50);
                but2.Image = item.bitmap;
                this.Controls.Add(but2);
                but2.Click += new EventHandler(Button_Click);
            }

            //右下方的卡片区
            List<block> list3 = new List<block>();
            int size3 = random.Next(1, 20);
            int[] array3 = new int[size3];
            array3 = Enumerable.Range(0, size3).OrderBy(n => (new Random(n).Next())).ToArray<int>();
            for (int i = 0; i < array3.Length / 3; i++)
            {
                Bitmap buttonBackground = GetRandomBackground();
                for (int j = 0; j < 3; j++)
                {
                    var block2 = new block { bitmap = buttonBackground, index = array3[i * 3 + j] };
                    list3.Add(block2);
                }
            }

            var blklist3 = list3.OrderByDescending(x => x.index).ToArray();
            foreach (var item in blklist3)
            {
                Button but3 = new Button();
                but3.Location = new System.Drawing.Point(x: 300 + (48 - size3) * 2 + 5 * item.index, y: 430);
                but3.Size = new System.Drawing.Size(width: 50, height: 50);
                but3.Image = item.bitmap;
                this.Controls.Add(but3);
                but3.Click += new EventHandler(Button_Click);
            }

            //展示按钮
            ButtonShow();
        }


        //结束游戏
        private void btnEnd_Click(object sender, EventArgs e)
        {
            this.Close();
            this.Hide();
            axWindowsMediaPlayer1.Ctlcontrols.stop();
            axWindowsMediaPlayer2.Ctlcontrols.stop();
            Form2 frm2 = new Form2();
            frm2.ShowDialog();
        }

        //刷新
        private void btn_Refresh_Click(object sender, EventArgs e)
        {
            axWindowsMediaPlayer1.Ctlcontrols.stop();
            axWindowsMediaPlayer2.URL = "C:\\Users\\asus\\Desktop\\ylgy\\Vedio\\1.mp4";
            axWindowsMediaPlayer2.Visible = true;
            axWindowsMediaPlayer2.Ctlcontrols.play();
            axWindowsMediaPlayer2.uiMode = "none";
        }

        //广告状态转换
        private void axWindowsMediaPlayer2_PlayStateChange(object sender, AxWMPLib._WMPOCXEvents_PlayStateChangeEvent e)
        {
            if (e.newState == 8) // 播放完成的状态
            {
                // 隐藏控件
                axWindowsMediaPlayer2.Visible = false;
                this.Controls.Clear();
                InitializeComponent();
                Form1_Load(sender, new EventArgs());
            }

        }
        //音乐暂停
        private void btn_music_Click(object sender, EventArgs e)
        {
            isPlay = !isPlay;
            if (isPlay)
            {
                //播放和暂停切换
                axWindowsMediaPlayer1.Ctlcontrols.pause();
            }
            else
            {
                //播放和暂停切换
                axWindowsMediaPlayer1.Ctlcontrols.play();
            }
        }
    }


    //设置控件的可用性
    class CtrEnabled
    {
        [System.Runtime.InteropServices.DllImport("user32.dll ")]
        static extern int SetWindowLong(IntPtr hWnd, int nIndex, int wndproc);
        [System.Runtime.InteropServices.DllImport("user32.dll ")]
        static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        const int GWL_STYLE = -16;
        const int WS_DISABLED = 0x8000000;

        public static void SetControlEnabled(Control c, bool enabled)
        {
            if (enabled)
            {
                SetWindowLong(c.Handle, GWL_STYLE, (~WS_DISABLED) & GetWindowLong(c.Handle, GWL_STYLE));
            }
            else
            {
                SetWindowLong(c.Handle, GWL_STYLE, WS_DISABLED + GetWindowLong(c.Handle, GWL_STYLE));
            }
        }

    }
}



