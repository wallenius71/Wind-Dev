using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace PIDataEdit
{
    public partial class Error : Form
    {
        public Error()
        {
            InitializeComponent();
        }

        public string ErrorMsg
        {
            get
            {
                return _errorMsg;
            }
            set
            {
                _errorMsg = value;
            }
        }

        private string _errorMsg = string.Empty;


        private string _title = "Error";

        public string Title
        {
            get { return _title; }
            set { _title = value; }
        }

        public bool btnOkVisible
        {
            set
            {
                btnOk.Visible = value;
            }
        }

        public int progressBarValue
        {
            set
            {
                progressBar.Value = value;
            }
        }

        public int progressBarMaximum
        {
            set
            {
                progressBar.Maximum= value;
            }
        }

        public bool progressBarVisible
        {
            set
            {
                progressBar.Visible = value;
            }
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Error_Load(object sender, EventArgs e)
        {
            lblErrorMsg.Text = _errorMsg;
            this.Text = _title;
        }

    }
}