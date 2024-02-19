using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TuringMachineInterpreter.GUI
{
	public partial class MainScreen : Form
	{
		public MainScreen()
		{
			InitializeComponent();

            cmbTasks.Items.AddRange(options.ToArray());
        }

		private void MainScreen_Load(object sender, EventArgs e)
		{
			if (cmbTasks.Items.Count > 0)
			{
				cmbTasks.SelectedIndex = 0;
			}
		}


		private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
		{
			//First group inputs
			txtTape.Text = string.Empty;
			txtTapeView.Text = string.Empty;
			txtHeadPosition.Text = string.Empty;
			txtTransitions.Text = string.Empty;
			lblState.Text = string.Empty;
			lblTapePosition.Text = string.Empty;
			currentState = "q0";  // reset current state
			tapePosition = 0;     // reset tape position
			simulationStarted = false;
			txtTape.Enabled = true;

			//Second group inputs
			ytxtTape.Text = string.Empty;
			ytxtTapeView.Text = string.Empty;
			ytxtHeadPosition.Text = string.Empty;
			//ytxtTransitions.Text = string.Empty;
			ylblState.Text = string.Empty;
			ylblTapePosition.Text = string.Empty;
			currentState = "q0";
			tapePosition = 0;
			txtTape.Enabled = true;
			simulationStarted = false;
		}


	}
}
