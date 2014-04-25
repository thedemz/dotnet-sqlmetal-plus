using System.Configuration;
namespace SqlMetalPlus
{
	public class ConnectionStringHelper
	{

		private frmConnection _frmConnection;

		public ConnectionStringHelper()
			: base()
		{
			_frmConnection = new frmConnection();
		}

		public string Title
		{
			get { return this._frmConnection.Text; }
			set { this._frmConnection.Text = value; }
		}

		public string ConnectionString
		{
			get { return this._frmConnection.ConnectionString; }
			set { this._frmConnection.ConnectionString = value; }
		}

		public bool Pluralize
		{
			get { return this._frmConnection.Pluralize; }
			set { this._frmConnection.Pluralize = value; }
		}

        public bool IncludeViews
        {
            get { return this._frmConnection.IncludeViews; }
            set { this._frmConnection.IncludeViews = value; }
        }

		public string Serialization
		{
			get { return this._frmConnection.Serialization; }
			set { this._frmConnection.Serialization = value; }
		}

		public bool IncludeSProcs
		{
			get { return this._frmConnection.IncludeSProcs; }
			set { this._frmConnection.IncludeSProcs = value; }
		}

		public System.Windows.Forms.DialogResult ShowDialog()
		{

			return this._frmConnection.ShowDialog();
		}

	}
}