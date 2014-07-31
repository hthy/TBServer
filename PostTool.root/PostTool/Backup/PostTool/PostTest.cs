using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Sjweb.Common;
using System.Collections;
using System.Data;
using System.IO;
using System.Net;
using BambooCommonApp;

namespace PostTool
{
    public partial class PostTest : Form
    {
        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            if (System.Windows.Forms. MessageBox.Show("确定要关闭?", "提示", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
            }
            else
            {
                e.Cancel = true;
            }
        }

        public PostTest()
        {
            InitializeComponent();

            txtPara.Multiline = true;
            txtPara.ScrollBars = ScrollBars.Horizontal;
            txtPara.AcceptsReturn = true;
            txtPara.AcceptsTab = true;

            
        }


        #region  -  配置存储  -
        /// <summary>
        /// 自动保存配置
        /// </summary>
        /// <returns></returns>
        public bool SaveSetting()
        {
            string strurl = txtURL.Text.Trim();
            string strsection = txtSection.Text.Trim();
            if (strsection == string.Empty)
            {
                strsection = strurl;
                
            }
            string filePath = System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "\\"+this.cbbKey.Text+".ini";
            if (strurl != string.Empty)
            {
                IniClass.WriteValue(filePath,strsection,"url",strurl);
                IniClass.WriteValue(filePath, strsection, "param", txtPara.Text.Replace("\r\n","@@8@"));
            }
            return true;
        }
        /// <summary>
        /// 选择绑定值
        /// </summary>
        /// <param name="section"></param>
        public void BindSetting(string section)
        {
            if (section != string.Empty)
            {
                string filePath = System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "\\" + this.cbbKey.Text + ".ini";
                if (File.Exists(filePath))
                {
                    txtURL.Text = IniClass.ReadValue(filePath, section, "url");
                    txtPara.Text = IniClass.ReadValue(filePath, section, "param").Replace("@@8@", "\r\n");
                    txtSection.Text = section;
                }
            }
 
        }



        public List<string> ReadAllSection(string filePath)
        {
            List<string> lists = new List<string>();
            using (StreamReader reader = new StreamReader(filePath))
            {
                string[] strs=reader.ReadToEnd().Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string str in strs)
                {
                    if (str.StartsWith("[") && str.EndsWith("]"))
                    {
                        lists.Add(str.Trim(new char[]{'[',']'}));
                    }
                }
            }


            lists.Sort(new Comparison<string>(delegate(string str1, string str2)
            {
                //被比较的数
                for (int i = 0; i < str1.Length && i < str2.Length; i++)
                {
                    if (str1[i] > str2[i])
                    {
                        return 1;
                    }
                    else if (str1[i] < str2[i])
                    {
                        return -1;
                    }
                }
                if (str1.Length > str2.Length)
                {
                    return -1;
                }
                else if (str1.Length < str2.Length)
                {
                    return 1;
                }

                return 0;
            }));

            return lists;
        }


        public void BindCbSetting()
        {
            cbSetting.Items.Clear();
            string filePath = System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "\\" + this.cbbKey.Text + ".ini";
            if (File.Exists(filePath))
            {
                List<string> lists = ReadAllSection(filePath);
                for (int i = 0; i < lists.Count; i++)
                {
                    cbSetting.Items.Add(lists[i]);
                }
            }
 
        }


       
        private void cbSetting_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindSetting(cbSetting.Text);
        }

        #endregion



        private void btnOK_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(this.txtURL.Text))
            {
                string[] strs = this.txtPara.Text.Split(new string[] { "\r\n" }, StringSplitOptions.None);  //不忽略空行                  
                object[] objs = new object[strs.Length];
                for (int i = 0; i < strs.Length; i++)
                {
                    if (strs[i].IndexOf('@') == 0)  //是文件的,处理方式不一样
                    {
                        objs[i] = File.ReadAllBytes(strs[i].TrimStart('@'));
                    }
                    else
                    {
                        objs[i] = System.Text.Encoding.UTF8.GetBytes(strs[i]);
                    }
                }

                byte[] postData = BooBufferHelper.PackBuffer(objs);
                switch (this.cbbKey.Text) //加密
                {
                    case "同步壁纸":
                        postData = SecurityHelper.DESEncoder(postData, new byte[] { 0xa2, 0x66, 0x17, 0x2D, 0x51, 0x54, 0x3D, 0x2A });
                        break;
                    case "同步推Store":
                        postData = SecurityHelper.DESEncoder(postData, new byte[] { 0xB3, 0xb4, 0x62, 0x51, 0xc8, 0x49, 0x1D, 0xaC });
                        break;
                    case "同步推越狱":
                    case "同步推正版":
                        postData = SecurityHelper.DESEncoder(postData, SecurityHelper.KeyServices);
                        break;
                    case "Android":
                        postData = SecurityHelper.DESEncoder(postData, new byte[] { 0xa1, 0x68, 0x47, 0x5D, 0x61, 0x59, 0x3D, 0x4A });
                        break;
                    case "简版助手":
                        postData = SecurityHelper.DESEncoder(postData, new byte[] { 0xa3, 0x76, 0x28, 0x5D, 0x23, 0x19, 0x4D, 0x1A });
                        break;
                    case "userDownload":
                        postData = SecurityHelper.DESEncoder(postData, new byte[] { 0xa2, 0xB6, 0x18, 0x2D, 0x1B, 0x13, 0x2C, 0x2B });
                        break;
                    case "通用接口":
                        postData = SecurityHelper.DESEncoder(postData, new byte[] { 0x13, 0x1, 0x2a, 0x3b, 0xc6, 0xe3, 0x4a, 0xb7 });
                        break;
                    default:
                        break;
                }




                //postData2 = SecurityHelper.DESEncoder(postData2, new byte[] { 0xa2, 0x66, 0x17, 0x2D, 0x51, 0x54, 0x3D, 0x2A });

                byte[] byteReturn = Bamboo.ClientLib.WebRequestHelper.PostDataReturnBytes(this.txtURL.Text, postData, false);
                if (byteReturn != null)
                {
                    #region -datatable-

                    DataTable dt = new DataTable();
                    dt.Columns.Add("result1", typeof(string));
                    dt.Columns.Add("result2", typeof(string));
                    dt.Columns.Add("result3", typeof(string));
                    dt.Columns.Add("result4", typeof(string));
                    dt.Columns.Add("result5", typeof(string));
                    dt.Columns.Add("result6", typeof(string));
                    dt.Columns.Add("result7", typeof(string));
                    dt.Columns.Add("result8", typeof(string));
                    dt.Columns.Add("result9", typeof(string));
                    dt.Columns.Add("result10", typeof(string));
                    dt.Columns.Add("result11", typeof(string));
                    dt.Columns.Add("result12", typeof(string));

                    #endregion -datatable-

                    byte[] tempBytes = null;
                    string[] tempStrs = null;
                    ArrayList tempAL1 = new ArrayList();

                    #region 开始解包-第一层
                    tempAL1 = BooBufferHelper.UnPackBuffer(byteReturn);
                    for (int i = 0; i < tempAL1.Count; i++)
                    {
                        tempBytes = (byte[])tempAL1[i];
                        if (TryUnPack(tempBytes))
                        {
                            #region  第二层
                            ArrayList tempAL2 = new ArrayList();
                            tempAL2 = BooBufferHelper.UnPackBuffer(tempBytes);
                            for (int j = 0; j < tempAL2.Count; j++)
                            {
                                tempBytes = (byte[])tempAL2[j];
                                if (TryUnPack(tempBytes))
                                {
                                    #region  第三层
                                    ArrayList tempAl3 = new ArrayList();
                                    tempAl3 = BooBufferHelper.UnPackBuffer(tempBytes);
                                    for (int m = 0; m < tempAl3.Count; m++)
                                    {
                                        tempBytes = (byte[])tempAl3[m];
                                        if (TryUnPack(tempBytes))
                                        {
                                            #region  第四层
                                            ArrayList tempAl4 = new ArrayList();
                                            tempAl4 = BooBufferHelper.UnPackBuffer(tempBytes);
                                            for (int n = 0; n < tempAl4.Count; n++)
                                            {
                                                tempBytes = (byte[])tempAl4[n];
                                                if (TryUnPack(tempBytes))
                                                {
                                                    #region  第五层
                                                    ArrayList tempAl5 = new ArrayList();
                                                    tempAl5 = BooBufferHelper.UnPackBuffer(tempBytes);
                                                    for (int z = 0; z < tempAl5.Count; z++)
                                                    {
                                                        tempBytes = (byte[])tempAl5[z];
                                                        if (!TryUnPack(tempBytes))
                                                        {
                                                            if (z == 0)
                                                            {
                                                                tempStrs = new string[] { "", "", "", "", BooBufferHelper.BytesToString(tempBytes) };
                                                            }
                                                            else
                                                            {
                                                                tempStrs = new string[] { "", "", "", "", BooBufferHelper.BytesToString(tempBytes) };
                                                            }
                                                            dt.Rows.Add(tempStrs);
                                                        }

                                                    }
                                                    #endregion  第五层
                                                }
                                                else
                                                {
                                                    if (n == 0)
                                                    {
                                                        if (m == 0)
                                                        {
                                                            tempStrs = new string[] { "L0" + i, "L1" + j, "L2" + m, BooBufferHelper.BytesToString(tempBytes) };
                                                        }
                                                        else
                                                        {
                                                            tempStrs = new string[] { "", "", "L2" + m, BooBufferHelper.BytesToString(tempBytes) };
                                                        }
                                                    }
                                                    else
                                                    {
                                                        tempStrs = new string[] { "", "", "", BooBufferHelper.BytesToString(tempBytes) };

                                                    }
                                                    dt.Rows.Add(tempStrs);
                                                }
                                            }
                                            #endregion  第四层
                                        }
                                        else
                                        {
                                            if (m == 0 && j == 0)
                                            {
                                                tempStrs = new string[] { "L0" + i, "L" + i + j, BooBufferHelper.BytesToString(tempBytes) };
                                            }
                                            else if (m == 0)
                                            {
                                                tempStrs = new string[] { "", "L" + i + j, BooBufferHelper.BytesToString(tempBytes) };
                                            }
                                            else
                                            {
                                                tempStrs = new string[] { "", "", BooBufferHelper.BytesToString(tempBytes) };
                                            }
                                            dt.Rows.Add(tempStrs);
                                        }
                                    }
                                    #endregion  第三层
                                }
                                else
                                {
                                    if (j == 0)
                                    {
                                        tempStrs = new string[] { "L0" + i, "L1" + j, BooBufferHelper.BytesToString(tempBytes) };
                                    }
                                    else
                                    {
                                        tempStrs = new string[] { "", "L1" + j, BooBufferHelper.BytesToString(tempBytes) };
                                    }
                                    dt.Rows.Add(tempStrs);
                                }
                            }
                            #endregion 第二层
                        }
                        else
                        {
                            tempStrs = new string[] { "L0" + i, BooBufferHelper.BytesToString(tempBytes) };
                            dt.Rows.Add(tempStrs);
                        }
                    }
                    #endregion

                    this.dgvResult.DataSource = dt.DefaultView;
                }
                else
                {
                    this.dgvResult.DataSource = null;
                }

            }
            else
            {
                System.Windows.Forms.MessageBox.Show("URL不能为空");
            }
        }

        public string GetGb2312(string str)
        {
            try
            {
                return System.Text.Encoding.GetEncoding("gb2312").GetString(System.Text.Encoding.GetEncoding("gb2312").GetBytes(str)); ;
            }
            catch (Exception ex)
            { return null; }

        }

        /// <summary>
        /// 移动数组
        /// </summary>
        /// <param name="strs"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public string[] StringMove(string[] strs, int count)
        {
            string[] tempStrs = new string[strs.Length + count];
            for (int i = 0; i < count; i++)
            {
                tempStrs[i] = string.Empty;
            }
            for (int i = 0; i < strs.Length; i++)
            {
                tempStrs[i + count - 1] = strs[i];
            }
            return tempStrs;
        }

        public bool TryUnPack(byte[] bufferr)
        {
            ArrayList al = null;
            try
            {
                al = BooBufferHelper.UnPackBuffer(bufferr);
                if (al != null && al.Count > 0)
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        //获取byte数组，密钥长度只能为八个
        private byte[] GetByte(string str)
        {
            byte[] bytes = null;
            str = str.Replace("，", ",").Trim(',');
            string[] strs = str.Split(',');
            if (strs.Length < 2)
            {
                bytes = new byte[] { Convert.ToByte(str) };
            }
            else
            {
                bytes = new byte[] { Convert.ToByte(strs[0]), Convert.ToByte(strs[1]), Convert.ToByte(strs[2]),
                    Convert.ToByte(strs[3]), Convert.ToByte(strs[4]), Convert.ToByte(strs[5]),
                    Convert.ToByte(strs[6]), Convert.ToByte(strs[7])};
            }
            return bytes;
        }

        private void cbbKey_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtSection.Text = string.Empty;
            BindCbSetting();
        }

        private void PostTest_Load(object sender, EventArgs e)
        {
            BindCbSetting();
        }

        private void txtPara_TextChanged(object sender, EventArgs e)
        {
            txtSection.Text = string.Empty;
        }

        private void txtURL_TextChanged(object sender, EventArgs e)
        {
            txtSection.Text = string.Empty;
        }

        private void btnSaveSetting_Click(object sender, EventArgs e)
        {
            SaveSetting();
            BindCbSetting();
        }

    }
}
