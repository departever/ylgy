using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
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
        Random random = new Random();

        private void Button_Click(object sender, EventArgs e)
        {
            var clickedButton = (Button)sender;

            AddButtonToPanel(clickedButton);

            this.Controls.Remove(clickedButton);

            EnableButton();

            RemoveButtons();
        }


        private void AddButtonToPanel(Button Button)
        {
            int sameImageCount = 0; // 相同图片的数量
            List<Button> sameImageButtons = new List<Button>(); // 存储相同图片的 Button
            int count = panel.Controls.Count;
            MemoryStream ms = new MemoryStream();
            Button.Image.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
            String secondBitmap = Convert.ToBase64String(ms.ToArray());
            // 遍历 panel 中的 Button
            for (int i = count - 1; i <= count && i >= 0; i--)
            {
                Button existButton = panel.Controls[i] as Button;

                if (existButton != null)
                {
                    //判断两张图片是否一致的两种方法
                    ms.Position = 0;
                    existButton.Image.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                    String firstBitmap = Convert.ToBase64String(ms.ToArray());
                    if (secondBitmap.Equals(firstBitmap))
                    {
                        sameImageCount++;
                        if (sameImageButtons.Count == 0)
                        {
                            sameImageButtons.Add(Button);
                        }
                        int index = panel.Controls.IndexOf(existButton);
                        panel.Controls.Add(Button);
                        sameImageButtons.Add(existButton);
                        Button.Location = new Point(existButton.Location.X + 50, 0);
                        panel.Controls.SetChildIndex(Button, index + 1);
                    }
                }
            }
            if (sameImageCount == 2)
            {
                foreach (Button button in sameImageButtons)
                {
                    panel.Controls.Remove(button);
                }
                sameImageButtons.Clear();
                // 对 panel 中的 Button 进行重新排序
                for (int j = 0; j < panel.Controls.Count; j++)
                {
                    Button button = panel.Controls[j] as Button;
                    if (button != null)
                    {
                        button.Location = new Point((j % 7) * 50, 0);
                    }
                }
                return;
            }
            if (!panel.Controls.Contains(Button))
            {
                panel.Controls.Add(Button);
                Button.Location = new Point((count % 7) * 50, 0);
            }

            //对 panel 中的 Button 进行重新排序
            for (int i = 0; i < panel.Controls.Count; i++)
            {
                Button button = panel.Controls[i] as Button;
                if (button != null)
                {
                    button.Location = new Point((i % 7) * 50, 0);
                }
            }
            if (panel.Controls.Count > 7)
            {
                MessageBox.Show("游戏结束！");
                return;
            }
        }
        private void RemoveButtons()
        {

        }
        public Bitmap GetRandomButtonBackground()
        {
            List<Image> images = new List<Image>();
            for (int i = 0; i < 10; i++)
            {
                string imageFileName = @"C:\Users\asus\Desktop\ylgy\图片素材\" + i + ".png";//修改这一行即可
                Image _image = Image.FromFile(imageFileName);
                images.Add(_image);
            }
            int imageIndex = random.Next(0, images.Count);
            // 复制图片来创建新的按钮背景
            Image image = images[imageIndex];
            Bitmap buttonBackground = new Bitmap(image);
            return buttonBackground;
        }

        private void EnableButton()
        {
            foreach (Button but in this.Controls.OfType<Button>())
            {
                but.Enabled = true;
            }

            foreach (Button btn1 in this.Controls.OfType<Button>())
            {
                foreach (Button btn2 in this.Controls.OfType<Button>())
                {
                    if (btn1.TabIndex > btn2.TabIndex)
                    {
                        // 检查是否有重叠
                        Rectangle rect1 = new Rectangle(btn1.Location, btn1.Size);
                        Rectangle rect2 = new Rectangle(btn2.Location, btn2.Size);
                        if (rect1.IntersectsWith(rect2))
                        {
                            // 比较 TabIndex
                            btn1.Enabled = false;
                            break;
                        }
                    }
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            List<diepai1> list1 = new List<diepai1>();

            int[] array1 = new int[60];
            array1 = Enumerable.Range(0, 60)
                        .OrderBy(n => (new Random(n).Next()))
                        .ToArray<int>();
            button1.Text = array1.ToString();
            for (int i = 0; i < array1.Length / 3; i++)
            {
                Bitmap buttonBackground = GetRandomButtonBackground();
                for (int j = 0; j < 3; j++)
                {
                    var pai = new diepai1 { bitmap = buttonBackground, index = array1[i * 3 + j] };
                    list1.Add(pai);
                }
            }
            var pailist1 = list1.OrderBy(x => x.index).ToArray();

            for (int i = 0; i < 5; i++)
            {
                int offsetX = 25 * random.Next(-1, 1);
                int offsetY = 25 * random.Next(-1, 1);
                for (int x = 0; x < 5; x++)
                {
                    for (int y = 0; y < 5; y++)
                    {
                        Button but1 = new Button();
                        but1.Location = new System.Drawing.Point(x: 50 + 50 * x + offsetX, y: 50 + 50 * y + offsetY);
                        but1.Size = new System.Drawing.Size(width: 50, height: 50);
                        but1.Image = pailist1[i * 6 + x * 6 + y].bitmap;
                        but1.TabIndex = i;
                        but1.Text = Convert.ToString(i * 36 + x * 6 + y);
                        this.Controls.Add(but1);
                        but1.Click += new EventHandler(Button_Click);
                    }
                }
            }

            List<diepai2> list = new List<diepai2>();

            int[] array2 = new int[3];
            array2 = Enumerable.Range(0, 3)
                        .OrderBy(n => (new Random(n).Next()))
                        .ToArray<int>();
            for (int i = 0; i < array2.Length / 3; i++)
            {
                Bitmap buttonBackground = GetRandomButtonBackground();
                for (int j = 0; j < 3; j++)
                {
                    var pai = new diepai2 { bitmap = buttonBackground, index = array2[i * 3 + j] };
                    list.Add(pai);
                }
            }

            var pailist2 = list.OrderByDescending(x => x.index).ToArray();
            foreach (var item in pailist2)
            {
                Button but2 = new Button();
                but2.Location = new System.Drawing.Point(x: 50 + 5 * item.index, y: 370);
                but2.Size = new System.Drawing.Size(width: 50, height: 50);
                but2.Image = item.bitmap;
                this.Controls.Add(but2);
                but2.Click += new EventHandler(Button_Click);
            }
            EnableButton();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //使用Application.Restart()方法
            Application.Restart();
        }
    }
    class diepai1
    {
        public Bitmap bitmap;
        public int index;
    }
    class diepai2
    {
        public Bitmap bitmap;
        public int index;
    }

}




