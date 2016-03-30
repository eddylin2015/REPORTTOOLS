using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.IO;

namespace ReportTools
{
    public partial class ReptDefOpForm : Form
    {
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            bool bHandled = false;
            // switch case is the easy way, a hash or map would be better, 
            // but more work to get set up.
            switch (keyData)
            {
                case Keys.F5:
                    // do whatever
                    List<Control> li = new List<Control>();
                    foreach(Control c in this.Controls)
                    {
                        li.Add(c);

                    }
                    foreach (Control c in li)
                    {
                        this.Controls.Remove(c);
                    }
                    MessageBox.Show("a");
                    loadReptFile(this.Text);
                    bHandled = true;
                    break;
            }
            return bHandled;
        }
        private bool isDragging = false;


        private int clickOffsetX, clickOffsetY;


        public void lblDragger_MouseDown(System.Object sender, System.Windows.Forms.MouseEventArgs e)
        {
            isDragging = true;
            clickOffsetX = e.X;
            clickOffsetY = e.Y;
        }

        public void lblDragger_MouseUp(System.Object sender, System.Windows.Forms.MouseEventArgs e)
        {
            isDragging = false;
        }

        public void lblDragger_MouseMove(System.Object sender,
          System.Windows.Forms.MouseEventArgs e)
        {
            if (isDragging == true)
            {
                if (sender is Control)
                {
                    Control cb = (Control)sender;

                    cb.Left = e.X + cb.Left - clickOffsetX;
                    cb.Top = e.Y + cb.Top - clickOffsetY;
                }
            }
        }
 
        public ReptDefOpForm()
        {
            InitializeComponent();

        }
        private XmlDocument xd=null;
        private XmlNode currpagenode = null;

        private int multiple_rate = 4;
        public void saveFile(String Filename)
        {
  //          FileStream f = new FileStream(Filename, FileMode.Create);
            //StreamWriter s = new StreamWriter(f);
            int dbtext_rate = multiple_rate / 2;
            foreach (Component c in this.Controls)
            {
                if (c is CheckBox)
                {
                    CheckBox cb = (CheckBox)c;
//                    s.WriteLine("<dbtext name=\"{0}\" fieldname=\"{1}\" x=\"{2}\" y=\"{3}\" />", cb.Name.ToString(),cb.Text, cb.Location.X * 2, cb.Location.Y * 2);


                    foreach (XmlNode xnod in currpagenode.ChildNodes)
                    {
                        if (xnod.NodeType == XmlNodeType.Element)
                        {try{
                            if (xnod.Name == "dbtext" && xnod.Attributes.GetNamedItem("name").Value==cb.Name)
                            {
                                xnod.Attributes.GetNamedItem("x").Value = String.Format("{0}", cb.Location.X * dbtext_rate);
                                xnod.Attributes.GetNamedItem("y").Value = String.Format("{0}", cb.Location.Y * dbtext_rate);
                            }
                        }catch{
                            MessageBox.Show(cb.Name.ToString()+cb.Text);
                        }
                        }
                      }
                }
            }
            //s.Close();
            //f.Close();
            XmlTextWriter myXmlTextWriter = new XmlTextWriter(Filename, null);
            myXmlTextWriter.Formatting = Formatting.Indented;
            xd.Save(myXmlTextWriter);
            myXmlTextWriter.Flush();
            myXmlTextWriter.Close();
        }
        private int loadcnt=0;
        private NumericUpDowns nud = null;
        public void loadReptFile(string filestr)
        {
                if (loadcnt == 0)
                {
                nud = new NumericUpDowns();
                nud.Text = "縮放比例";
                nud.nupdwn.Value = multiple_rate;
                nud.ShowDialog();
                loadcnt++;
                };
                multiple_rate = nud.num;
            
            
            PictureBox pb = new PictureBox();
            pb.MouseMove += ReptDefOpForm_MouseMove;
            
            //pb.Dock = DockStyle.Fill;
            XmlTextReader xtr = new XmlTextReader(filestr);
            xtr.WhitespaceHandling = WhitespaceHandling.None;
            xd = new XmlDocument();
            xd.Load(xtr);
            XmlNode xnodDE = xd.DocumentElement;
            XmlNodeList nodes = xnodDE.SelectNodes("/report/page");
            if (nodes.Count > 1)
            {
                nud.Text = "報表第几頁";
                nud.nupdwn.Value = 1;
                nud.ShowDialog();
                currpagenode = nodes[nud.num - 1];
            }
            else
            {
                currpagenode = nodes[0];
            }

            Graphics g = null;
            try
            {
                Image pagebg = Image.FromFile(currpagenode.Attributes.GetNamedItem("bg").Value);
                Bitmap bm = new Bitmap(pagebg.Width / multiple_rate, pagebg.Height / multiple_rate);
                g = Graphics.FromImage(bm);
                g.DrawImage(pagebg, 0, 0, pagebg.Width / multiple_rate, pagebg.Height / multiple_rate);
                pb.Size = new Size(pagebg.Width / multiple_rate, pagebg.Height / multiple_rate);
                pb.Image = bm;
            }
            catch
            {
            }
            int dbtext_rate = multiple_rate / 2;
            foreach (XmlNode xnod in currpagenode.ChildNodes)
            {
                if (xnod.NodeType == XmlNodeType.Element)
                {
                    if (xnod.Name == "dbtext")
                    {
                        CheckBox cb = new CheckBox();
                        cb.Location = new System.Drawing.Point(int.Parse(xnod.Attributes.GetNamedItem("x").Value) / dbtext_rate, int.Parse(xnod.Attributes.GetNamedItem("y").Value) / dbtext_rate);
                        cb.Size = new System.Drawing.Size(59, 16);
                        cb.Name = xnod.Attributes.GetNamedItem("name").Value;
                        cb.Text = xnod.Attributes.GetNamedItem("fieldname").Value;
                        cb.AutoSize = true;
                        cb.MouseUp += new System.Windows.Forms.MouseEventHandler(this.lblDragger_MouseUp);
                        cb.MouseMove += new System.Windows.Forms.MouseEventHandler(this.lblDragger_MouseMove);
                        cb.MouseDown += new System.Windows.Forms.MouseEventHandler(this.lblDragger_MouseDown);
                        cb.Visible = true;
                        Controls.Add(cb);
                    }
                    else if (xnod.Name == "drawrect")
                    {
                        Pen myPen = new Pen(System.Drawing.Color.Black, 2);
                        int x=int.Parse(xnod.Attributes.GetNamedItem("x").Value) / dbtext_rate; 
                        int y=int.Parse(xnod.Attributes.GetNamedItem("y").Value) / dbtext_rate;
                        int x1 = int.Parse(xnod.Attributes.GetNamedItem("x1").Value) / dbtext_rate;
                        int y1 = int.Parse(xnod.Attributes.GetNamedItem("y1").Value) / dbtext_rate;
                        g.DrawRectangle(myPen, x, y, x1 - x, y1 - y);
                    }
                    else if (xnod.Name == "drawline")
                    {
                        Pen myPen = new Pen(System.Drawing.Color.Black, 1);
                        int x = int.Parse(xnod.Attributes.GetNamedItem("x").Value) / dbtext_rate;
                        int y = int.Parse(xnod.Attributes.GetNamedItem("y").Value) / dbtext_rate;
                        int x1 = int.Parse(xnod.Attributes.GetNamedItem("x1").Value) / dbtext_rate;
                        int y1 = int.Parse(xnod.Attributes.GetNamedItem("y1").Value) / dbtext_rate;
                        g.DrawLine(myPen, x, y, x1, y1);

                    }
                    else if (xnod.Name == "drawtext")
                    {
                        int x = int.Parse(xnod.Attributes.GetNamedItem("x").Value) / dbtext_rate;
                        int y = int.Parse(xnod.Attributes.GetNamedItem("y").Value) / dbtext_rate;

                        Font f = new Font(xnod.Attributes.GetNamedItem("fontname").Value, float.Parse(xnod.Attributes.GetNamedItem("fontsize").Value));
                        if(xnod.Attributes.GetNamedItem("fontstyle").Value=="bold")
                            f = new Font(xnod.Attributes.GetNamedItem("fontname").Value, float.Parse(xnod.Attributes.GetNamedItem("fontsize").Value),FontStyle.Bold);
                        g.DrawString(xnod.Attributes.GetNamedItem("text").Value, f, Brushes.Black,x, y);
                    }
                }
            }
            xtr.Close();
            Controls.Add(pb);
        }

        private void ReptDefOpForm_MouseMove(object sender, MouseEventArgs e)
        {
            MDIParent1 p = (MDIParent1)this.MdiParent;
            p.toolStripStatusLabel.Text = String.Format("x={0},y={1}", e.X * multiple_rate / 2, e.Y * multiple_rate/2);
        }
    }
}