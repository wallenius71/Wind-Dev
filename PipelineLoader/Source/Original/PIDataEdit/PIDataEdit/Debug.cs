using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace PIDataEdit
{
    public partial class Debug : Form
    {

        public string Input
        {
            set {textBox1.Text = value; }
        }
	
        public Debug()
        {
            InitializeComponent();
        }

        private void Debug_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}