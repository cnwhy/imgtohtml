using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace IMGtoHTML
{
    public partial class Form1 : Form
    {
        private string imgfile = "";
        private Image pic;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //domainUpDown1.SelectedIndex = 4;
        }

        private void button1_Click(object sender, EventArgs e)
        {

            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                imgfile = openFileDialog1.FileName;
                pic = Image.FromFile(imgfile);
                获取图片信息();
            }
        }
        private void 获取图片信息()
        {
            string _text = "";
            _text = "尺寸: " + pic.Width + "*" + pic.Height;
            textBox1.Text = _text;
            pictureBox1.BackgroundImage = Image.FromFile(imgfile);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (imgfile == "") { MessageBox.Show("请选择图片!"); return; }

            if (saveFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                try
                {
                    生成(saveFileDialog1.FileName);
                }
                catch (Exception err)
                {
                    MessageBox.Show("生成时出现了问题!\n" + err.Message);
                    return;
                }
                if (MessageBox.Show("你要现在打开生成的HTML吗?", "生成HTML成功!", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
                {
                    System.Diagnostics.Process.Start(saveFileDialog1.FileName);
                }
            }

        }

        /// <summary>
        /// 生成缩略图副本
        /// </summary>
        public static Bitmap CreateThumbnail(Bitmap source, int thumbWi, int thumbHi)
        {
            System.Drawing.Bitmap ret = null;
            try
            {
                int wi, hi;
                wi = thumbWi;
                hi = thumbHi;
                ret = new Bitmap(wi, hi);
                using (Graphics g = Graphics.FromImage(ret))
                {
                    g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
                    g.FillRectangle(Brushes.White, 0, 0, wi, hi);
                    g.DrawImage(source, 0, 0, wi, hi);
                }
            }
            catch
            {
                ret = null;
            }
            return ret;
        }
        /// <summary>
        /// 把Color转成HEX形式
        /// </summary>
        private string ColorToHEX(Color a)
        {
            return Convert.ToString(a.ToArgb(), 16).Substring(2);
        }
        /// <summary>
        /// 
        /// </summary>
        private string IMGtoHTML(Bitmap b, int KLsize)
        {
            StringBuilder html = new StringBuilder();
            html.Append("<!DOCTYPE html>");
            html.Append("<html>");
            html.Append("<head>");
            html.Append("<meta http-equiv=\"Content-Type\" content=\"text/html; charset=gb2312\" />");
            html.Append("<title>无标题文档</title>");
            html.Append("<style type=\"text/css\">");
            html.Append("*,b,p{padding:0;margin:0;font-size:0;line-height:0}");
            html.Append("body{background:#333;}");
            html.Append(string.Format("div.htmlimg b,td{{display:block;float:left;width:{0}px;height:{1}px}}", KLsize, KLsize));
            html.Append("div.htmlimg p{display:block;clear:both;height:0}");
            html.Append("</style>");
            html.Append("</head>");
            html.Append("<body>");
            html.Append(string.Format("<div class=\"htmlimg\" style=\"width:{0}px\">", KLsize * b.Width));
            for (int i = 0; i < b.Height - 1; i++)
            {
                for (int z = 0; z < b.Width - 1; z++)
                {
                    html.Append(string.Format("<b style=\"background:#{0}\"></b>", ColorToHEX(b.GetPixel(z, i))));
                    //html.Append(string.Format("<td bgcolor=\"#{0}\"></td>", ColorToHEX(b.GetPixel(z, i))));
                }
                html.Append("<p></p>");
            }

            html.Append("");
            html.Append("</div>");
            html.Append("</body>");
            html.Append("</html>");
            return html.ToString();
        }
        private string imgtohtml_script(Bitmap b, int KLsize, int w)
        {
            StringBuilder html = new StringBuilder();
            html.Append("<!DOCTYPE html>");
            html.Append("<html>");
            html.Append("<head>");
            html.Append("<meta http-equiv=\"Content-Type\" content=\"text/html; charset=gb2312\" />");
            html.Append("<title></title>");
            html.Append("<style type=\"text/css\">");
            html.Append("*,b,p{padding:0;margin:0;font-size:0;line-height:0}");
            html.Append("body{background:#333;}");
            html.Append(string.Format("div.htmlimg b{{display:block;float:left;width:{0}px;height:{1}px}}", KLsize, KLsize));
            html.Append("div.htmlimg p{display:block;clear:both;height:0}");
            html.Append("</style>");
            html.Append("<script>");
            html.Append(string.Format("var $imgtohtml_d ={{s:{0},w:{1},cs:[", KLsize, w));
            for (int i = 0; i <= b.Height - 1; i++)
            {
                for (int z = 0; z <= b.Width - 1; z++)
                {
                    html.Append(string.Format("\"{0}\"", ColorToHEX(b.GetPixel(z, i))));
                    if (i != b.Height - 1 || z != b.Width - 1) { html.Append(","); }
                }
            }
            html.Append("]}");
            html.Append("</script>");
            html.Append("</head>");
            html.Append("<body>");
            html.Append(string.Format("<div class=\"htmlimg\" style=\"width:{0}px\">", KLsize * b.Width));
            html.Append("<script>");
            html.Append("for(i=0; i<=$imgtohtml_d.cs.length-1;i++){");
            html.Append("document.write(\"<b style=\\\"background:#\"+$imgtohtml_d.cs[i]+\";\\\"></b>\");");
            html.Append("if((i+1)%$imgtohtml_d.w==0) document.write(\"<p></p>\")");
            html.Append("};");
            html.Append("</script>");
            html.Append("</div>");
            html.Append("</body>");
            html.Append("</html>");
            return html.ToString();
        }
        private string imgtohtml_CSS3(Bitmap b, int KLsize, int w)
        {
            StringBuilder html = new StringBuilder();
            html.Append("<!DOCTYPE html>");
            html.Append("<html>");
            html.Append("<head>");
            html.Append("<meta http-equiv=\"Content-Type\" content=\"text/html; charset=gb2312\" />");
            html.Append("<title></title>");
            html.Append("<style type=\"text/css\">");
            html.Append("*{padding:0;margin:0;font-size:0;line-height:0}");
            html.Append("body{background:#333;}");
            html.Append("#pic{ position: absolute; top: 0px;width: 0;height: 0;box-shadow:");
            for (int i = 0; i <= b.Height - 1; i++)
            {
                for (int z = 0; z <= b.Width - 1; z++)
                {
                    html.Append(string.Format("{0}px {1}px {2}px {3}px #{4}", z * KLsize, i * KLsize, (KLsize % 2 == 0 || KLsize==1) ? KLsize - 1 : KLsize, Math.Ceiling(KLsize / 2f), ColorToHEX(b.GetPixel(z, i))));
                    if (i != b.Height - 1 || z != b.Width - 1) { html.Append(","); }
                }
            }
            html.Append("}");
            html.Append("</style>");
            html.Append("</head>");
            html.Append("<body>");
            html.Append("<div id=\"pic\"></div>");
            html.Append("</body>");
            html.Append("</html>");
            return html.ToString();
        }

        private string imgtohtml_CSS3_js(Bitmap b, int KLsize, int w)
        {
            StringBuilder html = new StringBuilder();
            html.Append("<!DOCTYPE html>");
            html.Append("<html>");
            html.Append("<head>");
            html.Append("<meta http-equiv=\"Content-Type\" content=\"text/html; charset=gb2312\" />");
            html.Append("<title></title>");
            html.Append("<style type=\"text/css\">");
            html.Append("*{padding:0;margin:0;font-size:0;line-height:0}");
            html.Append("body{background:#999;}");
            html.Append("#pic{position:absolute;top:0px;width:0;height:0}");
            html.Append("</style>");
            html.Append("<script>");
            html.Append(string.Format("var $imgtohtml_d ={{s:{0},w:{1},cs:'", KLsize, w));
            for (int i = 0; i <= b.Height - 1; i++)
            {
                for (int z = 0; z <= b.Width - 1; z++)
                {
                    html.Append(string.Format("{0}", ColorToHEX(b.GetPixel(z, i))));
                    if (i != b.Height - 1 || z != b.Width - 1) { html.Append(","); }
                }
            }
            html.Append("'}");
            html.Append("</script>");
            html.Append("</head>\n");
            html.Append("<body>");
            html.Append("<div id=\"pic\"></div>");
            html.Append("<script>");
            html.Append("var box_s=\"\",x=0,y=0,$color=$imgtohtml_d.cs.split(',');");
            html.Append("for(i=0; i<=$color.length-1;i++){");
            html.Append("box_s+= x+'px '+y+'px ' + ($imgtohtml_d.s%2==0 || $imgtohtml_d.s==1?$imgtohtml_d.s-1:$imgtohtml_d.s) +'px '+ Math.ceil($imgtohtml_d.s/2) + 'px #'+$color[i]+',';");
            html.Append("x+=$imgtohtml_d.s;if((i+1)%$imgtohtml_d.w==0){x=0;y+=$imgtohtml_d.s}");
            html.Append("};");
            html.Append(@"box_s=box_s.replace(/\,*$/,'');");
            html.Append(@"document.getElementById('pic').style.boxShadow=box_s;");
            html.Append("</script>");
            html.Append("</body>");
            html.Append("</html>");
            return html.ToString();
        }

        private void 生成(string filename)
        {
            Bitmap b = new Bitmap(pic);
            Size y = new Size((int)nUD_x.Value, (int)nUD_y.Value);
            Size n_size = b.Size;
            int 块大小 = (int)nUD_kl.Value;
            if (true)//保持比例
            {
                if (((decimal)y.Width / y.Height) < ((decimal)n_size.Width / n_size.Height))
                {
                    decimal xx = ((decimal)y.Width / n_size.Width) * n_size.Height;
                    n_size.Height = (int)Math.Round(xx, 0);
                    n_size.Width = y.Width;
                }
                else
                {
                    decimal xx = ((decimal)y.Height / n_size.Height) * n_size.Width;
                    n_size.Width = (int)Math.Round(xx, 0);
                    n_size.Height = y.Height;
                }
            }
            b = CreateThumbnail(b, n_size.Width, n_size.Height);
            //string filePath = html.ToString();
            using (StreamWriter sw = new StreamWriter(filename, false, Encoding.GetEncoding("GB2312")))
            {
                if (radioButton1.Checked) { sw.Write(IMGtoHTML(b, 块大小)); }
                else if (radioButton2.Checked) { sw.Write(imgtohtml_script(b, 块大小, n_size.Width)); }
                else if (radioButton3.Checked) { sw.Write(imgtohtml_CSS3(b, 块大小, n_size.Width)); }
                else { sw.Write(imgtohtml_CSS3_js(b, 块大小, n_size.Width)); }
            }
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("tencent://message/?uin=51474146");
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://www.cnblogs.com/whyoop");
        }
    }
}
