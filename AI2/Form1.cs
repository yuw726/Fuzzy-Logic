using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ZedGraph;


namespace AI2
{  
    public partial class Form1 : Form
    {
        LinguisticVariable L = new LinguisticVariable("Height of human", 150, 210);
        int CurveCount;
        
        public Form1()
        {
            InitializeComponent();

            //string[] terms = { "little", "normal", "big" };
            //L.InitTerms(terms, true);
            CurveCount = 0;

            InitGraph();

        }

        private void InitGraph()
        {
            GraphPane myPane = zedGraphControl1.GraphPane;
            myPane.Title.Text = L.Name;
            myPane.XAxis.Title.Text = "Рост человека";
            myPane.YAxis.Title.Text = "Значение функции принадлежности";

            myPane.XAxis.Scale.Min = L.Min;
            myPane.XAxis.Scale.Max = L.Max;
            myPane.YAxis.Scale.Min = 0.0;
            myPane.YAxis.Scale.Max = 1.0;
            zedGraphControl1.AxisChange();
        }


        private void DrawGraph(FuzzyVariable V)
        {
            GraphPane pane = zedGraphControl1.GraphPane;
            PointPairList list1 = new PointPairList();

            for (int i = 0; i < V.Set.Count; i++)
            {
                list1.Add(V.Set[i].Value, V.Set[i].MembershipFunction);
            }

            if (pane.CurveList.Count == CurveCount + 1)
                pane.CurveList.RemoveAt(CurveCount);
            pane.AddCurve("", list1, Color.Red, SymbolType.None);

            zedGraphControl1.Invalidate();
        }

        
        // ----------------------------------------------------------------------
        // Interface
        
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked == false)
            {
                groupBox1.Enabled = false;
            }
            else
            {
                groupBox1.Enabled = true;
            }
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox2.Checked == false || checkBox2.Enabled == false)
            {
                groupBox4.Enabled = false;
            }
            else
            {
                groupBox4.Enabled = true;
            }
        }

        private void radioButton9_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton9.Checked == true)
            {
                checkBox2.Enabled = false;
                groupBox4.Enabled = false;
                groupBox5.Enabled = false;
            }
            else
            {
                checkBox2.Enabled = true;
                if (checkBox2.Checked == true)
                {
                    groupBox4.Enabled = true;
                }
                groupBox5.Enabled = true;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string[] terms = { "little", "normal", "big" };
            bool man = true;
            if (radioButton16.Checked) man = true;
            else 
                if (radioButton17.Checked) man = false;
            L.InitTerms(terms, man);
            
            string tocreate = "";
            if (checkBox1.Checked)
            {
                if (radioButton1.Checked) tocreate = "not";
                if (radioButton2.Checked) tocreate += " very";
                if (radioButton3.Checked) tocreate += " slightly";
            }
            if (radioButton4.Checked) tocreate += " little";
            if (radioButton5.Checked) tocreate += " normal";
            if (radioButton6.Checked) tocreate += " big";
            if (radioButton7.Checked) tocreate += " or";
            if (radioButton8.Checked) tocreate += " and";
            if (!radioButton9.Checked)
            {
                if (checkBox2.Checked)
                {
                    if (radioButton10.Checked) tocreate += " slightly";
                    if (radioButton11.Checked) tocreate += " very";
                    if (radioButton12.Checked) tocreate += " not";
                }
                if (radioButton13.Checked) tocreate += " little";
                if (radioButton14.Checked) tocreate += " normal";
                if (radioButton15.Checked) tocreate += " big";
            }
            
            L.Generate(tocreate);
            InitGraph();
            DrawGraph(L.Value);
        }
    }
}
