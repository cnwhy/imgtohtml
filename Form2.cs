using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using MongoDB;

namespace IMGtoHTML
{
    public partial class Form2 : Form
    {
        private Bitmap imgBit;
        private Image img;
        private string classname;
        private Boolean border;
        private string savefolder;

        private Size size = new Size(200, 200);
        //文件格式
        static string[] geshi = { "jpg", "jpeg", "bmp", "png", "gif" };
        public Form2()
        {
            InitializeComponent();

        }

        private void Form2_Load(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "图片文件|*." + string.Join(";*.", geshi);
        }


        private void button1_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                textBox1.Text = openFileDialog1.FileName;
                if (textBox2.Text == "")
                {
                    textBox2.Text = openFileDialog1.SafeFileName;
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            int int_ = 300 ;
            Int32.TryParse(textBox4.Text,out int_);
            size = new Size(int_,int_);
            try
            {
                if (textBox3.Text.Trim() != "")
                {
                    if (!Directory.Exists(textBox3.Text.Trim()))
                    {
                        Directory.CreateDirectory(textBox3.Text.Trim());
                    }
                    config(textBox3.Text);
                    split();
                }
                else if (folderBrowserDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    textBox3.Text = folderBrowserDialog1.SelectedPath;
                    config(folderBrowserDialog1.SelectedPath);
                    split();
                }
                 MessageBox.Show("完成!");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void config(string SelectedPath)
        {
            savefolder = SelectedPath;
            textBox3.Text = SelectedPath;
            imgBit = new Bitmap(textBox1.Text);
            img = Image.FromFile(textBox1.Text);
            classname = textBox2.Text;
            border = checkBox1.Checked;
        }

        private void split()
        {
            Bitmap splitImg;
            int sX = (int)Math.Ceiling(1.0 * imgBit.Width / size.Width);
            int sY = (int)Math.Ceiling(1.0 * imgBit.Height / size.Height);
            for (int y = 0; y < sY; y++)
            {
                for (int x = 0; x < sX; x++)
                {
                    splitImg = new Bitmap(size.Width, size.Height);
                    Graphics g = Graphics.FromImage(splitImg);
                    //g.DrawImage(img, 0, 0, new Rectangle(x * size.Width, y * size.Height, size.Width, size.Height), GraphicsUnit.Pixel);
                    g.DrawImage(img, -1 * x * size.Width, -1 * y * size.Height);
                    if (border)
                    {
                        g.DrawRectangle(new Pen(Color.Blue, 1f), 0, 0, splitImg.Width - 1, splitImg.Height - 1);
                    }
                    saveimg(splitImg, savefolder + "/" + classname + "_" + y + "_" + x + ".jpg");
                }
            }
            img.Dispose();
            imgBit.Dispose();
        }

        private void saveimg(Bitmap img, string filename)
        {
            img.Save(filename, System.Drawing.Imaging.ImageFormat.Jpeg);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                textBox3.Text = folderBrowserDialog1.SelectedPath;
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            //链接字符串
            string connectionString = "mongodb://localhost";
            //数据库名
            string databaseName = "myDatabase";
            //集合名
            string collectionName = "myCollection";
            //定义Mongo服务
            Mongo mongo = new Mongo(connectionString);
            //获取databaseName对应的数据库，不存在则自动创建
            MongoDatabase mongoDatabase = mongo.GetDatabase(databaseName) as MongoDatabase;
            //获取collectionName对应的集合，不存在则自动创建
            MongoCollection<Document> mongoCollection = mongoDatabase.GetCollection<Document>(collectionName) as MongoCollection<Document>;
            //链接数据库
            mongo.Connect();
            try
            {
                //定义一个文档对象，存入两个键值对
                Document doc = new Document();
                doc["ID"] = 1;
                doc["Msg"] = "Hello World!";

                //将这个文档对象插入集合
                mongoCollection.Insert(doc);

                //在集合中查找键值对为ID=1的文档对象
                Document docFind = mongoCollection.FindOne(new Document { { "ID", 1 } });

                //输出查找到的文档对象中键“Msg”对应的值，并输出
                //Console.WriteLine(Convert.ToString(docFind["Msg"]));
                MessageBox.Show(Convert.ToString(docFind["Msg"]));
            }
            finally
            {
                //关闭链接
                mongo.Disconnect();
            }
        }
    }
}
