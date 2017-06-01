using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml.Linq;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            BindData();
        }
        private void BindData()
        {
            //清空控件内容
            txtTitle.Text = "";
            txtDesc.Text = "";
            cbkOK.Checked = false;
            //清除原有节点
            tvShow.Nodes.Clear();
            //加载XML，获取根结点
            XDocument document = XDocument.Load("QQ.xml");
            XElement root = document.Root;
            //加载数据
            //获得所有Date节点
            var dates = root.Elements("Date");
            foreach (var date in dates)
            {
                //获得date节点的编号，并显示到treeview上
                string id = date.Attribute("id").Value;
                TreeNode tn = tvShow.Nodes.Add(id);
                tn.Tag = 1;//标记：这是日期节点
                //获得某个日期的所有问题节点
                var qs = date.Elements("Q");
                foreach (var q in qs)
                {
                    //获得问题节点的标题节点，并显示到treeview的指定日期节点下
                    XElement title = q.Element("title");
                    bool isOk = Boolean.Parse(q.Attribute("isOk").Value.ToLower());//读取是否解决属性，决定了节点的背景色
                    TreeNode tn1 = tn.Nodes.Add(title.Value);
                    tn1.Tag = 2;//标记：问题标题节点
                    tn1.BackColor = isOk ? Color.Green : Color.Red;//设置背景色
                }
            }
            //展开treeview
            tvShow.ExpandAll();
            //设置默认不选中任何内容
            this.ActiveControl = txtTitle;
        }


        private void btnSave_Click(object sender, EventArgs e)
        {
            //加载xml文档
            XDocument document = XDocument.Load("QQ.xml");
            XElement root = document.Root;
            //获取指定的内容
            string title = txtTitle.Text.Trim();
            string desc = txtDesc.Text.Trim();
            string isOk = cbkOK.Checked.ToString();
            //确定要将现有内容添加到哪个节点下
            TreeNode tn = tvShow.SelectedNode;
            if (tn == null)
            {//如果没有选中任何节点，则创建当前日期的节点，并将问题添加
                string date = DateTime.Now.ToString("yyyyMMdd");
                //构造Q节点
                XElement q = new XElement("Q");
                q.SetAttributeValue("isOk", isOk);
                q.SetElementValue("title", title);
                q.SetElementValue("desc", desc);

                if (root.Elements("Date").Where(d => d.Attribute("id").Value == date).Count() > 0)
                {
                    //如果当前日期的节点已经存在，则直接在此节点下添加
                    XElement dateElement = root.Elements("Date").Where(d => d.Attribute("id").Value == date).First();
                    //将q节点加到指定的日期节点下
                    dateElement.Add(q);
                }
                else
                {
                    //如果当前日期的节点不存在，则创建日期节点，并添加到Queation下
                    XElement dateElement = new XElement("Date");
                    dateElement.SetAttributeValue("id", date);
                    dateElement.Add(q);
                    root.Add(dateElement);
                }
            }
            else
            {
                int tag = Convert.ToInt32(tn.Tag);
                if (tag == 1)
                {//向指定日期下添加问题
                    string date = tn.Text;
                    //找到指定日期格式对应的xml节点
                    XElement dateElement = root.Elements("Date").Where(d => d.Attribute("id").Value == date).First();
                    //构造Q节点
                    XElement q = new XElement("Q");
                    q.SetAttributeValue("isOk", isOk);
                    q.SetElementValue("title", title);
                    q.SetElementValue("desc", desc);

                    dateElement.Add(q);
                }
                else if (tag == 2)
                {//修改选中的问题
                    //tn.Text
                    //获取日期节点
                    string date = tn.Parent.Text;
                    //获取原始标题
                    string titleOld = tn.Text;
                    //查找需要修改的Q节点
                    XElement dateElement = root.Elements("Date").Where(d => d.Attribute("id").Value == date).First();
                    XElement qElement = dateElement.Descendants("title").Where(t => t.Value == titleOld).First().Parent;
                    qElement.SetAttributeValue("isOk", isOk);
                    qElement.SetElementValue("title", title);
                    qElement.SetElementValue("desc", desc);
                }
            }
            //完成保存
            document.Save("QQ.xml");
            //保存完成后，重新加载数据
            BindData();
        }

        private void tvShow_AfterSelect(object sender, TreeViewEventArgs e)
        {
            //获取选中节点
            TreeNode tn = e.Node;
            //如果选中的是日期节点，则不需要修改
            if (tn.Tag.ToString() == "1")
            {
                return;
            }
            txtTitle.Text = tn.Text;
            //加载查询
            XDocument document = XDocument.Load("QQ.xml");
            XElement root = document.Root;
            string date = tn.Parent.Text;
            //查找Date节点
            XElement dateElement = root.Elements("Date").Where(d => d.Attribute("id").Value == date).First();
            //查找q节点
            XElement qElement = dateElement.Descendants("title").Where(t => t.Value == tn.Text).First().Parent;
            //读取是否已解决信息
            cbkOK.Checked = Boolean.Parse(qElement.Attribute("isOk").Value);
            //读取描述信息
            txtDesc.Text = qElement.Element("desc").Value;
        }

        private void cbkOK_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            TreeNode tn = tvShow.SelectedNode;
            if (tn == null)
            {
                MessageBox.Show("要闹哪样");
            }
            else
            {
                DialogResult dr= MessageBox.Show("是否要删除吗？", "提示", MessageBoxButtons.YesNo);
                if (dr == DialogResult.Yes)
                {
                    XDocument document = XDocument.Load("QQ.xml");
                    XElement root = document.Root;
                    //查找Q
                    if (tn.Tag.ToString() == "1")
                    {
                        //选中的是日期节点
                        string date = tn.Text;
                        XElement dateElement = root.Elements("Date").Where(d => d.Attribute("id").Value == date).First();
                        dateElement.Remove();
                    }
                    else
                    {
                        //选中的是标题节点
                        string date = tn.Parent.Text;
                        string title = tn.Text;
                        XElement dateElement = root.Elements("Date").Where(d => d.Attribute("id").Value == date).First();
                        XElement qElement = dateElement.Descendants("title").Where(t => t.Value == title).First().Parent;
                        qElement.Remove();
                    }
                    document.Save("QQ.xml");
                    BindData();
                }
            }
        }
    }
}
