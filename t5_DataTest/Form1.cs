using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace t5_DataTest
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //为gridview添加列
            gvList.Columns.Add("id", "编号");
            gvList.Columns.Add("name", "用户名");
            gvList.Columns.Add("password", "密码");
            //加载数据，显示到gridview上
            BindData();
        }

        private void groupBox4_Enter(object sender, EventArgs e)
        {

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
        }
        private void BindData()
        {
            //加载
            XDocument document = XDocument.Load("UserData.xml");
            XElement root = document.Root;
            //查找所有的user节点
            var users = root.Elements("user");
            //清除原有数据
            gvList.Rows.Clear();
            foreach (var user in users)
            {
                //获取用户的数据信息
                string id = user.Attribute("id").Value;
                string name = user.Element("name").Value;
                string password = user.Element("password").Value;
                //添加行数据
                gvList.Rows.Add(id, name, password);
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            //注意：用户名不可以重复
            XDocument document = XDocument.Load("UserData.xml");
            XElement root = document.Root;
            //先查询用户名是否存在
            if (root.Descendants("name").Where(n => n.Value == txtName.Text.Trim()).Count() > 0)
            {
                var name = root.Descendants("name").Where(n => n.Value == txtName.Text.Trim()).First();
                var user = name.Parent;
                if (user.Attribute("id").Value == lblID.Text)
                {//当名称相同，并且编号也相同时，则进行密码修改
                    user.SetElementValue("password", txtPwd.Text.Trim());
                }
                else
                {
                    //提示用户名已经存在，不允许修改
                    MessageBox.Show("用户名已存在");
                    return;
                }
            }
            else
            {//用户名不存在，直接根据id进行修改即可
                var user = root.Elements("user").Where(u => u.Attribute("id").Value == lblID.Text).First();
                user.SetElementValue("name", txtName.Text.Trim());
                user.SetElementValue("password", txtPwd.Text.Trim());
            }
            document.Save("UserData.xml");
            BindData();
        }

        private void gvList_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            var row = gvList.Rows[e.RowIndex];
            lblID.Text = row.Cells[0].Value.ToString();
            txtName.Text = row.Cells[1].Value.ToString();
            txtPwd.Text = row.Cells[2].Value.ToString();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            XDocument document = XDocument.Load("UserData.xml");
            XElement root = document.Root;
            //根据用户名、密码进行查询
            if (root.Elements("user").Where(u => u.Element("name").Value == loginName.Text.Trim() && u.Element("password").Value == loginPwd.Text.Trim()).Count() > 0)
            {
                MessageBox.Show("登录成功");
            }
            else
            {
                MessageBox.Show("登录失败");
            }
        }
    }
}
