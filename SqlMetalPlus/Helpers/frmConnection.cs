using System;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using System.Data.Common;
//using Microsoft.Data.ConnectionUI;
using Microsoft.Data.ConnectionUI;
using System.Data.SqlClient;

namespace SqlMetalPlus
{
	internal partial class frmConnection : Form
	{
		private SqlFileConnectionProperties cp;
		private SqlConnectionUIControl uic;

		//Allows the user to change the title of the dialog 
		public override string Text
		{
			get { return base.Text; }
			set { base.Text = value; }
		}

		//Pass the original connection string or get the resulting connection string 
		public string ConnectionString
		{
			get { return cp.ConnectionStringBuilder.ConnectionString; }
			set { cp.ConnectionStringBuilder.ConnectionString = value; }
		}

		public bool Pluralize
		{
			get { return this.checkBox1.Checked; }
			set { this.checkBox1.Checked = value; }
		}

		public string Serialization
		{
			get { return this.checkBox3.Checked ? "Unidirectional" : "None"; }
			set { this.checkBox3.Checked = (value == "Unidirectional"); }
		}

		public bool IncludeSProcs
		{
			get { return this.checkBox2.Checked; }
			set { this.checkBox2.Checked = value; }
		}
        public bool IncludeViews
        {
            get { return this.chkViews.Checked; }
            set { this.chkViews.Checked = value; }
        }


		private void OK_Button_Click(System.Object sender, System.EventArgs e)
		{
			this.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.Close();
		}

		private void Cancel_Button_Click(System.Object sender, System.EventArgs e)
		{
			this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.Close();
		}

		private void frmConnection_Load(object sender, EventArgs e)
		{
			this.Padding = new Padding(5);
			Button adv = new Button();
			Button Tst = new Button();

			//Size the form and place the uic, Test connection button, and advanced button 
			uic.LoadProperties();
			uic.Dock = DockStyle.Top;
			uic.Parent = this;
			this.ClientSize = Size.Add(uic.MinimumSize, new Size(10, (groupBox1.Height + adv.Height + 25)));
			this.MinimumSize = this.Size;

			adv.Text = "Advanced";
			adv.Dock = DockStyle.None;
			adv.Location = new Point((uic.Width - adv.Width), (uic.Bottom + 10));
			adv.Anchor = (AnchorStyles.Right | AnchorStyles.Top);
			adv.Click += this.Advanced_Click;
			adv.Parent = this;

			groupBox1.Dock = DockStyle.None;
			groupBox1.Width = uic.Width;
			groupBox1.Location = new Point(uic.Left, (uic.Bottom + 10));
			groupBox1.Anchor = (AnchorStyles.Right | AnchorStyles.Top);
			checkBox1.Checked = true;


			Tst.Text = "Test Connection";
			Tst.Width = 100;
			Tst.Dock = DockStyle.None;
			Tst.Location = new Point((uic.Width - Tst.Width) - adv.Width - 10, (uic.Bottom + 10));
			Tst.Anchor = (AnchorStyles.Right | AnchorStyles.Top);
			Tst.Click += this.Test_Click;
			Tst.Parent = this;

		}

		private void Advanced_Click(object sender, EventArgs e)
		{
			//Set up a form to display the advanced connection properties 
			Form frm = new Form();
			PropertyGrid pg = new PropertyGrid();
			pg.SelectedObject = cp;
			pg.Dock = DockStyle.Fill;
			pg.Parent = frm;
			frm.ShowDialog();
		}

		private void Test_Click(object sender, EventArgs e)
		{
			//Test the connection 
			SqlConnection conn = new SqlConnection();
			conn.ConnectionString = cp.ConnectionStringBuilder.ConnectionString;
			try
			{
				conn.Open();
				MessageBox.Show("Test Connection Succeeded.", "Successful", MessageBoxButtons.OK, MessageBoxIcon.Information);
			}
			catch (Exception ex)
			{
				MessageBox.Show("Test Connection Failed.\n" + ex.Message, "Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
			finally
			{
				try
				{
					conn.Close();
				}
				catch (Exception ex)
				{
				}


			}
		}

		private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
		{

		}

		private void groupBox1_Enter(object sender, EventArgs e)
		{

		}

	}
}