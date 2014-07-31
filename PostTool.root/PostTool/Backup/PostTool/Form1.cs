using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using Sjweb.Common;

namespace PostTool
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            
        }

        private void button1_Click(object sender, EventArgs e)
        {



           // string url = "http://localhost:19978/Wallpaper.aspx?type=4&p=iPhone3,1&v=5.0&sv=1.0&sn=test&w=320&h=480&lan=0&uni=553F860F-F33E-5F81-8F80-21849FC0AA4F";



            //string url = "http://192.168.40.89/Wallpaper/Wallpaper.aspx?type=6&p=iPhone3,1&v=5.0&sv=1.0&sn=test&w=320&h=480&lan=0&uni=553F860F-F33E-5F81-8F80-21849FC0AA4F";
           // string url = "http://localhost:2406/TrainProject/INotice.aspx?type=2";



            //byte[] imgData = File.ReadAllBytes(@"F:\复件 img\216fc546-9.jpg");

            string url = "http://localhost:9341/userLoadurl/UserDownload.aspx?type=2";
            /*
            byte[] postData = BooBufferHelper.PackBuffer(Encoding.UTF8.GetBytes("account"), Encoding.UTF8.GetBytes("nickname"),
                Encoding.UTF8.GetBytes("123"),Encoding.UTF8.GetBytes("tag"),
                imgData, Encoding.UTF8.GetBytes("320"), Encoding.UTF8.GetBytes("480"), Encoding.UTF8.GetBytes("jpg"));
            */
            byte[] postData = null;
            byte[] tempbyte = null;
            object[] objs = new object[9];
            //objs[0] = BooBufferHelper.PackBuffer(Encoding.UTF8.GetBytes("www.tongbu.com"), Encoding.UTF8.GetBytes("670B14728AD9902AECBA32E22FA4F6BD"));
            //objs[1] = BooBufferHelper.PackBuffer(Encoding.UTF8.GetBytes("www.tongbu.com"), Encoding.UTF8.GetBytes("670B14728AD9902AECBA32E22FA4F6BD"));
            //objs[2] = BooBufferHelper.PackBuffer(Encoding.UTF8.GetBytes("tui.tongbu.com"), Encoding.UTF8.GetBytes("670B14728AD9902AECBA32E22FA4F6BD"));
            objs[0] = Encoding.UTF8.GetBytes("http://dl.dbank.com/c0prhg1u43");
            objs[1] = Encoding.UTF8.GetBytes("1.0");
            objs[2] = Encoding.UTF8.GetBytes("www.tongbu.com");
            objs[3] = Encoding.UTF8.GetBytes("11.2");
            objs[4] = Encoding.UTF8.GetBytes("3.0");
            objs[5] = Encoding.UTF8.GetBytes("1");
            objs[6] = Encoding.UTF8.GetBytes("tongb'u");
            objs[7] = Encoding.UTF8.GetBytes("34534533");
            objs[8] = Encoding.UTF8.GetBytes("tongbu@tong.com");

            tempbyte = BooBufferHelper.PackBuffer(objs);


            postData = BooBufferHelper.PackBuffer(Encoding.UTF8.GetBytes("2"), Encoding.UTF8.GetBytes("2"), Encoding.UTF8.GetBytes("包无效"), tempbyte);
            postData = SecurityHelper.DESEncoder(postData, new byte[] { 0xa2, 0xB6, 0x18, 0x2D, 0x1B, 0x13, 0x2C, 0x2B });

            string str= Bamboo.ClientLib.WebRequestHelper.PostData(url, postData, false);
        }

        public void Case(int money)
        {
            money = money / 50;
            switch(money)
            {
                case 0:
                    println("nothing");
                    break;
                case 1:
                    println("0.0075");
                    break;
                case 2:
                    println("0.1");
                    break;
                case 3:
                    println("0.1");
                    break;
                default:
                    break;

            }
        }
        public void println(string str)
        {
            Console.WriteLine(str);
        }
    }
}
