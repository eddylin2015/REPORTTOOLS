using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Xml;


namespace ReportTools
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void toolStripContainer1_TopToolStripPanel_Click(object sender, EventArgs e)
        {

        }



        private void button1_Click(object sender, EventArgs e)
        {
            FileStream f = new FileStream(@"c:\output.txt", FileMode.Create);
            StreamWriter s = new StreamWriter(f);
            foreach(Component c in this.Controls)
            {
                if (c is CheckBox)
                {
                   CheckBox cb= (CheckBox)c;
                   s.WriteLine("<dbtext fieldname=\"{0}\" x=\"{1}\" y=\"{2}\" />",cb.Text,cb.Location.X*2 ,cb.Location.Y*2);
                }
            }
            s.Close();
            f.Close();
        }

        private void checkBox26_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {


        }
        public XmlDocument xd = null;
        Image pagebg = null;
        private void button2_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            if(ofd.ShowDialog()==DialogResult.OK)
            {
                XmlTextReader xtr=new XmlTextReader(ofd.FileName);
                xtr.WhitespaceHandling = WhitespaceHandling.None;
                xd = new XmlDocument();
                xd.Load(xtr);
                XmlNode xnodDE = xd.DocumentElement;
                XmlNodeList nodes = xnodDE.SelectNodes("/report/page");
                XmlNode currpagenode = null; 
                if (nodes.Count > 1)
                {
                    NumericUpDowns nud = new NumericUpDowns();
                    nud.ShowDialog();
                    currpagenode = nodes[nud.num - 1];
                }
                else
                {
                    currpagenode = nodes[0];
                }
                
                try
                {
                    pagebg = Image.FromFile(currpagenode.Attributes.GetNamedItem("bg").Value);
                    Bitmap bm = new Bitmap(pagebg.Width / 4, pagebg.Height / 4);
                    Graphics g = Graphics.FromImage(bm);
                    g.DrawImage(pagebg,0,0,pagebg.Width/4, pagebg.Height / 4);
                    this.pictureBox1.Image = bm;
                    
                }
                catch
                {
                }

                ChildDisplay(currpagenode, 0);
                   xtr.Close();
            }
        }
        private  void ChildDisplay(XmlNode xnod, int level)
        {
            XmlNode xnodWorking;
            if (xnod.NodeType == XmlNodeType.Element)
            {
                if (xnod.Name == "dbtext")
                {
                    Button myb = new Button();
                    myb.Text = "aaa";
                    myb.Left = 10;
                    myb.Top = 10;
                    myb.Height = 20;
                    myb.Width = 20;
                    myb.Visible = true;
                    myb.Location = new Point(10, 10);

                    Controls.Add(myb);
 

                 /*   cb.Location = new System.Drawing.Point( int.Parse(xnod.Attributes.GetNamedItem("x").Value),  int.Parse(xnod.Attributes.GetNamedItem("y").Value));
                    MessageBox.Show(cb.Location.X + " :" + cb.Location.Y);
                    cb.Size = new System.Drawing.Size(59, 16);
                    cb.Text = xnod.Attributes.GetNamedItem("fieldname").Value;
                    cb.Visible = true;
                    this.Controls.Add(cb);
                  * */
                }
            }

/*            if (xnod.HasChildNodes)
            {
                xnodWorking = xnod.FirstChild;
                while (xnodWorking != null)
                {
                    ChildDisplay(xnodWorking, level + 1);
                    xnodWorking = xnodWorking.NextSibling;
                }
            }
 * */
        }



    }
    public class NumericUpDowns : Form
    {
        public NumericUpDown nupdwn;
        public int num = 1;
        public NumericUpDowns()
        {
            Size = new Size(480, 580);

            nupdwn = new NumericUpDown();
            nupdwn.Parent = this;
            nupdwn.Location = new Point(50, 50);
            nupdwn.Size = new Size(60, 20);
            nupdwn.Value = 1;
            nupdwn.Minimum = -10;
            nupdwn.Maximum = 10;
            nupdwn.Increment = 1;
            //nupdwn.Increment = .25m;    //  decimal
            nupdwn.DecimalPlaces = 0;
            nupdwn.ReadOnly = true;
            nupdwn.TextAlign = HorizontalAlignment.Right;
            nupdwn.ValueChanged += new EventHandler(nupdwn_OnValueChanged);
        }

        private void nupdwn_OnValueChanged(object sender, EventArgs e)
        {
            num=(int)nupdwn.Value;
        }

    }
}