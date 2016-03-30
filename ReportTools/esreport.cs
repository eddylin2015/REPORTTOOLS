using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing.Printing;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Xml;
using System.Data;

namespace ReportTools
{
    public class esprinterOne:  esprinter , iesreport
    {
        private int reptdefpageindex = 0;
        private XmlNodeList reptdefpages_nodes = null;
        private string path = "";
        private string mixPath(string filename)
        {
            if (path != null)
            { return path + @"\" + filename; }
            else
            { return filename; }
        }
        public esprinterOne(string apath):base()
        {
            path = apath;
            // oPrintDialog
            // printDoc
            this.printDoc.PrintPage += new System.Drawing.Printing.PrintPageEventHandler(this.printDoc_PrintPage);
            // this.oPrintDialog.PrintToFile = true;
        }
        private Image hjp = null;
        private Image fjp = null;
        public void ReptDef_PrintSetting(DataSet ds, String ReptDefFileName, String SelectStr)
        {
            XmlTextReader xtr = new XmlTextReader(ReptDefFileName); 
            xtr.WhitespaceHandling = WhitespaceHandling.None;
            XmlDocument xd = new XmlDocument();
            xd.Load(xtr);
            XmlNode xnodDE = xd.DocumentElement;
            //reptdef pages nodes & pageindex
            reptdefpages_nodes = xnodDE.SelectNodes("/report/page");
            reptdefpageindex = 0;
            //read main TableName for DataSet ds1
            ds1 = ds;
            XmlNodeList reportnodelist = xnodDE.SelectNodes("/report");
            reptdef_maintablename = reportnodelist[0].Attributes.GetNamedItem("maintablename").Value.ToString();
            try
            {
                defaultfontsize = int.Parse(reportnodelist[0].Attributes.GetNamedItem("fontsize").Value.ToString());
            }
            catch { MessageBox.Show("font size?"); }
            String hjps = reportnodelist[0].Attributes.GetNamedItem("headjpg").Value;
            String fjps = reportnodelist[0].Attributes.GetNamedItem("footjpg").Value;
            if(hjps!="NULL")  hjp = Image.FromFile(mixPath(hjps));
            if(fjps != "NULL") fjp = Image.FromFile(mixPath(fjps));

            rept_CurrRecNo = 0;
            ShowPrintDialog();
            xtr.Close();
        }
        public virtual void RowDataDrawByHand(DataRow dr, Font pFont, int MarginTop, int MarginLeft, Graphics g)
        {
        }
        private int defaultfontsize = 8;
        private DataSet ds1;
        private String reptdef_maintablename = null;
        private int rept_CurrRecNo = 0;
        public override void printDoc_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            DataRow currDR = ds1.Tables[reptdef_maintablename].Rows[rept_CurrRecNo];
            PrinterSettings pSettings = new PrinterSettings();
            int nPosY = e.MarginBounds.Top;
            int nPosX = e.MarginBounds.Left;
            Image pagebg;
            Font pFont = new Font("細明體", defaultfontsize);
            try
            {
                String bgstring=reptdefpages_nodes[reptdefpageindex].Attributes.GetNamedItem("bg").Value;
                pagebg = Image.FromFile(mixPath(bgstring));
                //e.Graphics.DrawImage(pagebg, new PointF(nPosX, nPosY),);
                if (bgstring != "BLANK.jpg")
                {
                    e.Graphics.DrawImage(pagebg, nPosX, nPosY, pagebg.Width / 2, pagebg.Height / 2);
                }
                if (hjp != null) { e.Graphics.DrawImage(hjp, nPosX+10, nPosY+10, hjp.Width / 3, hjp.Height / 3); }
                if (fjp != null) { e.Graphics.DrawImage(fjp, nPosX+10, 1100, fjp.Width / 3, fjp.Height / 3); }

            }
            catch
            {
            }
            foreach (XmlNode node in reptdefpages_nodes[reptdefpageindex])
            {
                if (node.Name == "dbtext")
                {
                    string textsize = null; try { textsize = node.Attributes.GetNamedItem("size").Value.ToString(); }
                    catch { } if (textsize == null) { textsize = ""; } else { textsize = "," + textsize; }
                    e.Graphics.DrawString(
                        String.Format("{0" + textsize + "}",
                        currDR[node.Attributes.GetNamedItem("fieldname").Value.ToString()].ToString()),
                                         pFont,
                                         Brushes.Black,
                                         new PointF(e.MarginBounds.Left + float.Parse(node.Attributes.GetNamedItem("x").Value),
                                                  e.MarginBounds.Top + float.Parse(node.Attributes.GetNamedItem("y").Value)));
                }
                else if (node.Name == "drawrect")
                {
                    Pen myPen = new Pen(System.Drawing.Color.Black, 2);
                    int x = int.Parse(node.Attributes.GetNamedItem("x").Value) ;
                    int y = int.Parse(node.Attributes.GetNamedItem("y").Value) ;
                    int x1 = int.Parse(node.Attributes.GetNamedItem("x1").Value);
                    int y1 = int.Parse(node.Attributes.GetNamedItem("y1").Value);
                    e.Graphics.DrawRectangle(myPen, x, y, x1 - x, y1 - y);
                }
                else if (node.Name == "drawline")
                {
                    Pen myPen = new Pen(System.Drawing.Color.Black, 1);
                    int x = int.Parse(node.Attributes.GetNamedItem("x").Value) ;
                    int y = int.Parse(node.Attributes.GetNamedItem("y").Value) ;
                    int x1 = int.Parse(node.Attributes.GetNamedItem("x1").Value);
                    int y1 = int.Parse(node.Attributes.GetNamedItem("y1").Value);
                    e.Graphics.DrawLine(myPen, x, y, x1, y1);
                }
                else if (node.Name == "drawtext")
                {
                    Font f = new Font(node.Attributes.GetNamedItem("fontname").Value, float.Parse(node.Attributes.GetNamedItem("fontsize").Value));
                    if (node.Attributes.GetNamedItem("fontstyle").Value == "bold")
                        f = new Font(node.Attributes.GetNamedItem("fontname").Value, float.Parse(node.Attributes.GetNamedItem("fontsize").Value), FontStyle.Bold);
                    int x = int.Parse(node.Attributes.GetNamedItem("x").Value) ;
                    int y = int.Parse(node.Attributes.GetNamedItem("y").Value) ;
                    e.Graphics.DrawString(node.Attributes.GetNamedItem("text").Value,f,
                         Brushes.Black,
                            x, y);

                }
            }
            RowDataDrawByHand(currDR, pFont, e.MarginBounds.Top, e.MarginBounds.Left, e.Graphics);
            reptdefpageindex++;
            e.HasMorePages = true;
            if (reptdefpageindex >= reptdefpages_nodes.Count)
            {
                rept_CurrRecNo++;

                if (rept_CurrRecNo >= ds1.Tables[reptdef_maintablename].Rows.Count)
                {
                    e.HasMorePages = false;
                    rept_CurrRecNo = 0;
                }
            }
            else
            {
            }
            reptdefpageindex = reptdefpageindex % reptdefpages_nodes.Count;
            return;
        }
    }
    /// <summary>
    /// 打印糧單
    /// </summary>
    class PrtSalaryRec_ESprinterOne : esprinterOne
    {
        public string Salary_Period = null;
        public string Remarks = null;
        public PrtSalaryRec_ESprinterOne(string path)
            : base(path)
        {
        }
        public override void RowDataDrawByHand(DataRow dr, Font pFont, int MarginTop, int MarginLeft, Graphics g)
        {

            base.RowDataDrawByHand(dr, pFont, MarginTop, MarginLeft, g);
            String[] listItem = { "a", "b" };
            Font drawFont = new Font("細明體", 10);
            float fontsize = drawFont.GetHeight();
            float gridY = 733;
            float alloc_x = 460;
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(dr["Note"].ToString());
            XmlNodeList fa_List = xmlDoc.SelectNodes("dd/FAllo");
            XmlNodeList va_List = xmlDoc.SelectNodes("dd/VAllo");
            XmlNodeList adj_List = xmlDoc.SelectNodes("dd/ADJU");
            decimal substitueteachpay = decimal.Parse(dr["SubstituteTeachPay"].ToString());
            if (decimal.Parse(dr["SubstituteTeachPay"].ToString()) > 0.0m)
            {
                int[] awidth = { 200, 80 };
                g.DrawString("代課費", drawFont, Brushes.Black, new RectangleF(MarginLeft + alloc_x, MarginTop + gridY, awidth[0], fontsize * 1.5f));
                g.DrawString(String.Format("{0,10}", substitueteachpay), drawFont, Brushes.Black, new RectangleF(MarginLeft + alloc_x + awidth[0], MarginTop + gridY, awidth[1], fontsize * 1.5f));
                gridY += fontsize * 1.5f;
            }
            gridY = 960;
            foreach (XmlNode node in adj_List)
            {
                XmlNode a = node.SelectSingleNode("T");
                XmlNode b = node.SelectSingleNode("A");
                XmlNode N = node.SelectSingleNode("N");
                int[] awidth = { 150, 100, 200 };//x="358" y="640"
                g.DrawString(string.Format("{0}", a.InnerText), drawFont, Brushes.Black, new RectangleF(MarginLeft + 210, MarginTop + gridY, awidth[0], fontsize * 1.5f));
                g.DrawString(string.Format("{0,10}", b.InnerText), drawFont, Brushes.Black, new PointF(MarginLeft + 380, MarginTop + gridY));
                g.DrawString(string.Format("{0}", N.InnerText), drawFont, Brushes.Black, new RectangleF(MarginLeft + alloc_x, MarginTop + gridY, awidth[2], fontsize * 1.5f));
                gridY += fontsize * 1.5f;
            }
            decimal income0 = decimal.Parse(dr["baseSalary"].ToString()) +
                decimal.Parse(dr["Seniority"].ToString()) +
                decimal.Parse(dr["F_allowance"].ToString()) +
                decimal.Parse(dr["V_allowance"].ToString()) +
                decimal.Parse(dr["SubstituteTeachPay"].ToString()) +
                decimal.Parse(dr["FixExtraWorkPay"].ToString());
            g.DrawString(
                        String.Format("{0,10}",
                        income0),
                                         pFont,
                                         Brushes.Black,
                                         new PointF(MarginLeft + 650,
                                                  MarginTop + 800));
            //    <dbtext name="cb14" fieldname="F_allowance" x="650" y="478" size="10" />
            decimal withhold = decimal.Parse(dr["PensionFund_withhold"].ToString()) +
                decimal.Parse(dr["FSS_Fee"].ToString()) +
                decimal.Parse(dr["Leave_withhold"].ToString()) +
                decimal.Parse(dr["Tax"].ToString());
            g.DrawString(
                        String.Format("{0,10}",
                        withhold),
                                         pFont,
                                         Brushes.Black,
                                         new PointF(MarginLeft + 650,
                                                  MarginTop + 890));
            //    <dbtext name="cb15" fieldname="V_allowance" x="652" y="588" size="10" />
            decimal Adj_total = decimal.Parse(dr["Adjust_tax"].ToString()) +
                decimal.Parse(dr["AdjustAmount"].ToString());
            g.DrawString(
                        String.Format("{0,10}",
                        Adj_total),
                                         pFont,
                                         Brushes.Black,
                                         new PointF(MarginLeft + 650,
                                                  MarginTop + 990));
            g.DrawString(String.Format("{0,10}", income0), pFont, Brushes.Black, new PointF(MarginLeft + 120, MarginTop + 1044));
            g.DrawString(String.Format("{0,10}", withhold), pFont, Brushes.Black, new PointF(MarginLeft + 202, MarginTop + 1044));
            g.DrawString(String.Format("{0,10}", Adj_total), pFont, Brushes.Black, new PointF(MarginLeft + 290, MarginTop + 1044));
            //Salary Period x="162" y="280"
            g.DrawString(Salary_Period, pFont, Brushes.Black, new PointF(MarginLeft + 260, MarginTop + 533));
            //Remarks
            g.DrawString(Remarks, pFont, Brushes.Black, new RectangleF(MarginLeft + 120, MarginTop + 1116, 600, 150));
        }
    }
    //////////////
    public interface iesreport
    {
         void printDoc_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e);
         void ShowPrintDialog();
         void ShowPageSetup();
         void ShowPageSetup(int MarginTop, int MarginLeft);
    }
    public class esprinter:iesreport
    {
        public static esprinter ainstance = null;
        protected PageSettings oPageSettings;
        protected int m_nCurrPrinter;
        protected int m_nCurrPage;
        public System.Drawing.Printing.PrintDocument printDoc;
        protected System.Windows.Forms.PrintPreviewDialog ppDialog;
        protected System.Windows.Forms.PrintDialog oPrintDialog;
        protected System.Windows.Forms.PageSetupDialog oPageSetup;
        public esprinter()
        {
            this.printDoc = new System.Drawing.Printing.PrintDocument();
            this.ppDialog = new System.Windows.Forms.PrintPreviewDialog();
            this.oPrintDialog = new System.Windows.Forms.PrintDialog();
            this.oPageSetup = new System.Windows.Forms.PageSetupDialog();
            oPageSettings = new PageSettings();
            // oPrintDialog
            // printDoc
            this.printDoc.PrintPage += new System.Drawing.Printing.PrintPageEventHandler(this.printDoc_PrintPage);
            this.oPrintDialog.AllowSomePages = true;
            // this.oPrintDialog.PrintToFile = true;
        }
        public virtual void printDoc_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            return;
        }
        public void ShowPrintDialog()
        {
            //Set to defaults 
            m_nCurrPrinter = 0;
            m_nCurrPage = 1;
            oPrintDialog.Document = printDoc;
            if (oPrintDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    //Ensure the correct PrinterSettings object is used
                    oPageSettings.PrinterSettings = printDoc.PrinterSettings;
                    //Assign PageSettings object to all pages
                    printDoc.DefaultPageSettings = oPageSettings;
                    ppDialog.Document = printDoc;
                    ppDialog.ShowDialog();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }
        }
        public void ShowPageSetup()
        {
            oPageSettings.Margins.Top = 30;
            oPageSetup.PageSettings = oPageSettings;
            if (oPageSetup.ShowDialog() == DialogResult.OK)
            {
                oPageSettings = oPageSetup.PageSettings;
            }
        }
        public void ShowPageSetup(int MarginTop, int MarginLeft)
        {
            oPageSettings.Margins.Top = MarginTop;
            oPageSettings.Margins.Left = MarginLeft;
            oPageSetup.PageSettings = oPageSettings;
            if (oPageSetup.ShowDialog() == DialogResult.OK)
            {
                oPageSettings = oPageSetup.PageSettings;
            }
        }
        public static esprinter getInst
        {
            get
            {
                if (ainstance == null)
                {
                    ainstance = new esprinter();
                }
                return ainstance;
            }
        }
    }
    class printerlist : esprinter
    {
        public printerlist()
            : base()
        {
        }
        public override void printDoc_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            PrinterSettings pSettings = new PrinterSettings();
            Font printFont = new Font("Arial", 12);
            //Use the Margins
            int nTextPosY = e.MarginBounds.Top;
            int nTextPosX = e.MarginBounds.Left;
            int nHeight = (int)printFont.GetHeight(e.Graphics);
            //Height of a printer block 
            int nBlockHeight = 9 * nHeight;
            if (e.PageSettings.PrinterSettings.PrintRange == PrintRange.SomePages)
            {
                while (m_nCurrPage < e.PageSettings.PrinterSettings.FromPage)
                {
                    //Move printer to next page block
                    m_nCurrPrinter += (int)(e.MarginBounds.Height / nBlockHeight);
                    m_nCurrPage++;
                    if (m_nCurrPrinter > PrinterSettings.InstalledPrinters.Count)
                        return;
                }
                if (m_nCurrPage > e.PageSettings.PrinterSettings.ToPage)
                {
                    //Don't print anything more
                    return;
                }
            }
            //Print Background Graphic
            LinearGradientBrush aBrush =
                new LinearGradientBrush(e.MarginBounds, Color.FromArgb(100, Color.LightBlue),
                    Color.FromArgb(100, Color.Blue), LinearGradientMode.ForwardDiagonal);
            e.Graphics.FillRectangle(aBrush, e.MarginBounds);
            //Loop through using indexor now
            //Start with the previous index in m_nCurrPrinter
            for (int x = m_nCurrPrinter; x < PrinterSettings.InstalledPrinters.Count; x++)//each(string sPtr in PrinterSettings.InstalledPrinters)
            {
                pSettings.PrinterName = PrinterSettings.InstalledPrinters[x];
                if (pSettings.IsValid)
                {
                    //Ensure this printer block can fit on the page
                    if (nTextPosY + nBlockHeight < e.MarginBounds.Bottom)
                    {
                        //Print the caps of the printer
                        e.Graphics.DrawString(PrinterSettings.InstalledPrinters[x], printFont, Brushes.Black, nTextPosX, nTextPosY + 5);
                        e.Graphics.DrawString("Can Duplex: " + pSettings.CanDuplex.ToString(),
                            printFont, Brushes.Black, nTextPosX + 10, nTextPosY + (5 + nHeight));
                        e.Graphics.DrawString("Is Default: " + pSettings.IsDefaultPrinter.ToString(),
                            printFont, Brushes.Black, nTextPosX + 10, nTextPosY + (5 + nHeight * 2));
                        e.Graphics.DrawString("Is Plotter: " + pSettings.IsPlotter.ToString(),
                            printFont, Brushes.Black, nTextPosX + 10, nTextPosY + (5 + nHeight * 3));
                        e.Graphics.DrawString("Landscape Angle: " + pSettings.LandscapeAngle.ToString(),
                            printFont, Brushes.Black, nTextPosX + 10, nTextPosY + (5 + nHeight * 4));
                        e.Graphics.DrawString("Maximum Copies: " + pSettings.MaximumCopies.ToString(),
                            printFont, Brushes.Black, nTextPosX + 10, nTextPosY + (5 + nHeight * 5));
                        e.Graphics.DrawString("Maximum Page: " + pSettings.MaximumPage.ToString(),
                            printFont, Brushes.Black, nTextPosX + 10, nTextPosY + (5 + nHeight * 6));
                        e.Graphics.DrawString("Minimum Page: " + pSettings.MinimumPage.ToString(),
                            printFont, Brushes.Black, nTextPosX + 10, nTextPosY + (5 + nHeight * 7));
                        e.Graphics.DrawString("Supports Color: " + pSettings.SupportsColor.ToString(),
                            printFont, Brushes.Black, nTextPosX + 10, nTextPosY + (5 + nHeight * 8));
                        nTextPosY = nTextPosY + ((5 + nHeight * 8) + nHeight);
                        //Draw line after each
                        e.Graphics.DrawLine(System.Drawing.Pens.Black, nTextPosX, nTextPosY, e.MarginBounds.Right - 10, nTextPosY);
                        e.Graphics.FillEllipse(System.Drawing.Brushes.Black, e.MarginBounds.Right - 10, nTextPosY - 5, 10, 10);
                    }
                    else
                    {
                        //Couldn't fit block on the page - need more pages
                        m_nCurrPrinter = x;
                        e.HasMorePages = true;
                        return;
                    }
                }
            }
            //Last page if we reached here
            e.HasMorePages = false;
            m_nCurrPrinter = 0;
            return;
        }
    }

}
