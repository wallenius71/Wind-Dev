using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ExcelApplication
{
    public partial class Load : Form
    {

        private int _UFLDetailId;

        public int UFLDetailId
        {
            get { return _UFLDetailId; }
            set { _UFLDetailId = value; }
        }
	
        public Load()
        {
            InitializeComponent();
        }

        private void Load_Load(object sender, EventArgs e)
        {
            // Select name, id from db
            List<AddValue> towerNames = new List<AddValue>();
            db myDb = new db();

            towerNames = myDb.loadSavedMetTower();

            if (towerNames == null)
            {
                this.Close();
                return;
            }
            cbxTowerNames.DataSource = towerNames;
            cbxTowerNames.DisplayMember = "Display";
            cbxTowerNames.ValueMember= "Value";

        }

        private void loadSavedMetTower()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            //this.UFLDetailId = 

            try
            {
                object id = cbxTowerNames.SelectedValue;
                _UFLDetailId = (int)id;
            }
            catch
            {
            }
            this.Close();

        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}