using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace WinFormsRenaming {
	public class DataGridViewForm : Form {
		internal string TestProperty { get; set; }

		public DataGridViewForm() {
			var grid = new DataGridView();
			grid.Columns.Add(new DataGridViewTextBoxColumn() { DataPropertyName = nameof(DataBoundElement.BoundProperty) });
			grid.DataSource = new List<DataBoundElement>() { new DataBoundElement() { BoundProperty = "Test" } };

			grid.DataBindings.Add(new Binding(nameof(DataBoundElement.BoundProperty), grid.DataSource, nameof(TestProperty)));

			Controls.Add(grid);
		}
	}
}
