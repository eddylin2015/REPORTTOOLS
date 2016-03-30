using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.IO;


namespace ReportTools
{
    public partial class MDIParent1 : Form
    {
        private int childFormNumber = 0;

        private string appPath = System.IO.Path.GetDirectoryName(
System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase);
        public string getAppPath() { return appPath.Substring(6); }
        public static Hashtable tempdata = new Hashtable();
        public MDIParent1()
        {
            InitializeComponent();
            menuStrip.Items.Add("TEST",null,  (Object sender, EventArgs e) =>
            {
                
                ///            DataSet ds = new DataSet();
                ///            OdbcDataAdapter ad = new OdbcDataAdapter(sql, es_dblib.esdb.GetInstance().GetConn());
                ///            ad.Fill(ds, "Table");
                ///            PrtSalaryRec_ESprinterOne ep = new PrtSalaryRec_ESprinterOne(es_lib.Publib.Pub.cfg.getAppPath());
                ///            ep.ShowPageSetup(0, 0);
                ///            ep.Salary_Period = textBox2.Text;
                ///            ep.Remarks = richTextBox1.Text;
                ///            ep.ReptDef_PrintSetting(ds, es_lib.Publib.Pub.cfg.getAppPath() + @"\PayrollAdive_RptFeildPosi.xml", null);
                DataSet ds = new DataSet();
                DataTable tbl = new DataTable();
                tbl.TableName = "Table";
                tbl.Columns.Add("Staf_ref");
                for (int i = 0; i < 30; i++)
                {
                    DataRow dr = tbl.NewRow();
                    dr["Staf_ref"] = "aaaa"+i;
                    tbl.Rows.Add(dr);
                    
                }
                ds.Tables.Add(tbl);
                esprinterOne pl = new esprinterOne(getAppPath());
                pl.ShowPageSetup(0, 0);
                pl.ReptDef_PrintSetting(ds, getAppPath() + @"\aa.xml", null);
                //pl.ShowPrintDialog();

            });

            if(File.Exists("tempdata.xml")){
            FileStream fs = new FileStream("tempdata.xml", FileMode.Open);
            
            XmlTextReader r = new XmlTextReader(fs);
            string tempstr = null;
            while (r.Read())
            {

                if (r.NodeType == XmlNodeType.Element)
                {
                    if (r.Name == "tempdata") continue;
                    tempstr = r.Name;
                }
                else if (r.NodeType == XmlNodeType.Text)
                {
                    tempdata.Add(tempstr, r.Value);
                }
            }
            fs.Close();
            }

        }
        public void updateIniTempdata()
        {
            FileStream fs = new FileStream(this.getAppPath() + @"\tempdata.xml", FileMode.Create);
            XmlTextWriter w = new XmlTextWriter(fs, Encoding.UTF8);
            w.WriteStartDocument();
            w.WriteStartElement("tempdata");
            foreach (DictionaryEntry entry in tempdata)
            {
                w.WriteElementString(entry.Key.ToString(), entry.Value.ToString());
            }
            w.WriteEndElement();
            w.WriteEndDocument();
            w.Flush();
            fs.Close();
        }
        public string TempdataKey(string key)
        {
            if (!tempdata.ContainsKey(key))
            {
                tempdata.Add(key, "");
                return "";
            }
            else
            {
                return tempdata[key].ToString();
            }
        }
        private void ShowNewForm(object sender, EventArgs e)
        {
            // 创建此子窗体的一个新实例。
            Form childForm = new Form();
            // 在显示该窗体前使其成为此 MDI 窗体的子窗体。
            childForm.MdiParent = this;
            childForm.Text = "窗口" + childFormNumber++;
            childForm.Show();
        }




        private void OpenFile(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal);

            openFileDialog.Filter = "文本文件(*.xml)|*.xml|所有文件(*.*)|*.*";
            if (!TempdataKey("filepath").Equals(""))
            {
                openFileDialog.InitialDirectory=tempdata["filepath"].ToString();
            }
            if (openFileDialog.ShowDialog(this) == DialogResult.OK)
            {
                
                string FileName = openFileDialog.FileName;
                ReptDefOpForm frm = new ReptDefOpForm();
                frm.loadReptFile(FileName);
                frm.MdiParent = this;
                frm.Text = FileName;
                frm.Show();
                tempdata["filepath"] = openFileDialog.FileName;
                // TODO: 在此处添加打开文件的代码。
            }
        }

        private void SaveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            saveFileDialog.Filter = "文本文件(*.txt)|*.txt|所有文件(*.*)|*.*";
            if (saveFileDialog.ShowDialog(this) == DialogResult.OK)
            {
                string FileName = saveFileDialog.FileName;
                // TODO: 在此处添加代码，将窗体的当前内容保存到一个文件中。
            }
        }

        private void ExitToolsStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void CutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // TODO: 使用 System.Windows.Forms.Clipboard 将所选的文本或图像插入到剪贴板
        }

        private void CopyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // TODO: 使用 System.Windows.Forms.Clipboard 将所选的文本或图像插入到剪贴板
        }

        private void PasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // TODO: 使用 System.Windows.Forms.Clipboard.GetText() 或 System.Windows.Forms.GetData 从剪贴板中检索信息。
        }

        private void ToolBarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            toolStrip.Visible = toolBarToolStripMenuItem.Checked;
        }

        private void StatusBarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            statusStrip.Visible = statusBarToolStripMenuItem.Checked;
        }

        private void CascadeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.Cascade);
        }

        private void TileVerticalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.TileVertical);
        }

        private void TileHorizontalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.TileHorizontal);
        }

        private void ArrangeIconsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.ArrangeIcons);
        }

        private void CloseAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (Form childForm in MdiChildren)
            {
                childForm.Close();
            }
        }

        private void saveToolStripButton_Click(object sender, EventArgs e)
        {
            if (this.ActiveMdiChild is ReptDefOpForm)
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal);

                saveFileDialog.Filter = "文本文件(*.xml)|*.xml|所有文件(*.*)|*.*";
                saveFileDialog.FileName = TempdataKey("filepath");
                if (saveFileDialog.ShowDialog(this) == DialogResult.OK)
                {
                    string FileName = saveFileDialog.FileName;
                    ReptDefOpForm rf = (ReptDefOpForm)this.ActiveMdiChild;
                    rf.saveFile(FileName);

                    // TODO: 在此处添加代码，将窗体的当前内容保存到一个文件中。
                }

            }
        }

        private void MDIParent1_FormClosed(object sender, FormClosedEventArgs e)
        {
            updateIniTempdata();
        }
    }
}
