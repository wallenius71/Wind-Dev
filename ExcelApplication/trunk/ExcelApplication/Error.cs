using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ExcelApplication
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
	

        private void btnOk_Click(object sender, EventArgs e)
        {
            this.Close();
            //Form.ActiveForm.Close();
        }

        private void Error_Load(object sender, EventArgs e)
        {
            lblErrorMsg.Text = _errorMsg;
            this.Text = _title;
        }

        private void lblErrorMsg_Click(object sender, EventArgs e)
        {

        }
    }
}