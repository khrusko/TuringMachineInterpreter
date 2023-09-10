using System;
using System.Windows.Forms;

namespace TuringMachineInterpreter.GUI
{
	partial class MainScreen
	{
		private System.ComponentModel.IContainer components = null;

		private System.Windows.Forms.ComboBox cmbTasks;
		private System.Windows.Forms.TextBox txtTape;
		private System.Windows.Forms.TextBox txtTransitions;
		private System.Windows.Forms.Button btnStep;
		private System.Windows.Forms.Label lblState;
		private System.Windows.Forms.Label lblTapePosition;

        private string currentState = "q0";   // This is the current state of the Turing Machine (TM)
		private int tapePosition = 0;


		private void BtnStep_Click(object sender, EventArgs e)
		{
			if (string.IsNullOrEmpty(txtTape.Text))
			{
				txtTape.Text = "_";
			}

			// For predefined tasks, execute the corresponding function
			if (cmbTasks.SelectedItem.ToString() == "Prebroji parne brojeve")
			{
				ExecuteEvenNumbersTM(); // This function will count even numbers
				return;
			}
			else if (cmbTasks.SelectedItem.ToString() == "Kopiraj uneseni tekst")
			{
				ExecuteCopyInputTM();  // This function will copy input text
				return;
			}

			char currentSymbol = txtTape.Text[tapePosition];

			bool transitionFound = false;
			foreach (string line in txtTransitions.Lines)
			{
				var parts = line.Split(new string[] { "->" }, StringSplitOptions.None);
				var leftParts = parts[0].Split(',');
				var rightParts = parts[1].Split(',');

				if (leftParts[0] == currentState && leftParts[1][0] == currentSymbol)
				{
					currentState = rightParts[0];
					txtTape.Text = txtTape.Text.Remove(tapePosition, 1).Insert(tapePosition, rightParts[1]);
					transitionFound = true;

					if (rightParts[2] == "R")
						tapePosition++;
					else if (rightParts[2] == "L")
						tapePosition--;

					break;
				}
			}

			// Edge cases
			if (tapePosition < 0)
			{
				txtTape.Text = "_" + txtTape.Text;
				tapePosition = 0; // Reset to beginning
			}
			else if (tapePosition >= txtTape.Text.Length)
			{
				txtTape.Text += "_"; // Expand tape by adding a blank symbol at the end
			}

			if (!transitionFound)
			{
				lblState.Text = "No valid transition found!";
			}
			else
			{
				lblState.Text = $"State: {currentState}, Position: {tapePosition}";
			}

			string positionMarker = new string(' ', tapePosition * 2) + "^"; // Multiplying by 2 is a simple way to align with characters in a TextBox, assuming a fixed-width font.
			lblTapePosition.Text = positionMarker;
		}

		private void ExecuteEvenNumbersTM()
		{
			// TM Rules for counting even numbers in binary
			switch (currentState)
			{
				case "q0": // Initial state
					if (txtTape.Text[tapePosition] == '0' || txtTape.Text[tapePosition] == '1')
					{
						currentState = "q0"; // Keep moving right until you find '_'
						MoveRight();
					}
					else if (txtTape.Text[tapePosition] == '_')
					{
						MoveLeft(); // Go to the last symbol of the input
						currentState = "q1";
					}
					lblState.Text = "Checking the last digit of the binary number.";
					break;

				case "q1": // Check last symbol state
					if (txtTape.Text[tapePosition] == '0')
					{
						lblState.Text = "It's an even number. Computation completed!";
					}
					else
					{
						lblState.Text = "Not an even number. Computation completed!";
					}
					break;
			}
		}

		private void ExecuteCopyInputTM()
		{
			// TM Rules for copying string made up of 'a's and 'b's
			switch (currentState)
			{
				case "q0": // Initial state
					if (txtTape.Text[tapePosition] == 'a')
					{
						WriteSymbol('X'); // Mark the symbol as read
						currentState = "q1"; // Go to copy state for 'a'
						MoveRight();
					}
					else if (txtTape.Text[tapePosition] == 'b')
					{
						WriteSymbol('Y'); // Mark the symbol as read
						currentState = "q2"; // Go to copy state for 'b'
						MoveRight();
					}
					else if (txtTape.Text[tapePosition] == '_')
					{
						currentState = "qf"; // Final state
						lblState.Text = "String copied!";
					}
					break;

				case "q1": // Copy 'a' state
					if (txtTape.Text[tapePosition] != '#')
					{
						MoveRight(); // Keep moving until you find the delimiter
					}
					else
					{
						MoveRight(); // Move past the delimiter
						while (txtTape.Text[tapePosition] == 'a' || txtTape.Text[tapePosition] == 'b')
						{
							MoveRight(); // Find the spot to write the copy
						}
						WriteSymbol('a');
						currentState = "q3"; // Return to beginning of string
						MoveLeft();
					}
					break;

				case "q2": // Copy 'b' state
					if (txtTape.Text[tapePosition] != '#')
					{
						MoveRight();
					}
					else
					{
						MoveRight();
						while (txtTape.Text[tapePosition] == 'a' || txtTape.Text[tapePosition] == 'b')
						{
							MoveRight();
						}
						WriteSymbol('b');
						currentState = "q3";
						MoveLeft();
					}
					break;

				case "q3": // Return state
					if (txtTape.Text[tapePosition] != '#')
					{
						MoveLeft();
					}
					else
					{
						MoveLeft(); // Move past the delimiter
						currentState = "q0"; // Return to the initial state
					}
					break;
			}
		}

		private void MoveRight()
		{
			tapePosition++;
			if (tapePosition >= txtTape.Text.Length)
			{
				txtTape.Text += "_"; // Extend the tape
			}
		}

		private void MoveLeft()
		{
			tapePosition--;
			if (tapePosition < 0)
			{
				txtTape.Text = "_" + txtTape.Text; // Extend the tape
				tapePosition = 0; // Adjust the tape position
			}
		}

		private void WriteSymbol(char symbol)
		{
			txtTape.Text = txtTape.Text.Remove(tapePosition, 1).Insert(tapePosition, symbol.ToString());
		}


		private void CmbTasks_SelectedIndexChanged(object sender, EventArgs e)
		{
			// Clear all inputs
			txtTape.Text = string.Empty;
			txtTransitions.Text = string.Empty;
			lblState.Text = string.Empty;
			lblTapePosition.Text = string.Empty;
			currentState = "q0";  // reset current state
			tapePosition = 0;     // reset tape position
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code
		private void InitializeComponent()
		{
			this.cmbTasks = new System.Windows.Forms.ComboBox();
			this.txtTape = new System.Windows.Forms.TextBox();
			this.txtTransitions = new System.Windows.Forms.TextBox();
			this.btnStep = new System.Windows.Forms.Button();
			this.lblState = new System.Windows.Forms.Label();
			this.lblTapePosition = new System.Windows.Forms.Label();
			this.SuspendLayout();
			this.Load += new System.EventHandler(this.MainScreen_Load);
			// 
			// cmbTasks
			// 
			this.cmbTasks.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cmbTasks.Items.AddRange(new object[] {
            "Prebroji parne brojeve",
            "Kopiraj uneseni tekst"});
			
			this.cmbTasks.Location = new System.Drawing.Point(50, 15);
			this.cmbTasks.Name = "cmbTasks";
			this.cmbTasks.Size = new System.Drawing.Size(200, 21);
			this.cmbTasks.TabIndex = 0;
			this.cmbTasks.SelectedIndexChanged += new System.EventHandler(this.CmbTasks_SelectedIndexChanged);
			// 
			// txtTape
			// 
			this.txtTape.Location = new System.Drawing.Point(50, 50);
			this.txtTape.Name = "txtTape";
			this.txtTape.Size = new System.Drawing.Size(700, 20);
			this.txtTape.TabIndex = 1;
			// 
			// txtTransitions
			// 
			this.txtTransitions.Location = new System.Drawing.Point(50, 100);
			this.txtTransitions.Multiline = true;
			this.txtTransitions.Name = "txtTransitions";
			this.txtTransitions.Size = new System.Drawing.Size(700, 200);
			this.txtTransitions.TabIndex = 2;
			// 
			// btnStep
			// 
			this.btnStep.Location = new System.Drawing.Point(50, 350);
			this.btnStep.Name = "btnStep";
			this.btnStep.Size = new System.Drawing.Size(75, 23);
			this.btnStep.TabIndex = 3;
			this.btnStep.Text = "Step";
			this.btnStep.Click += new System.EventHandler(this.BtnStep_Click);
			// 
			// lblState
			// 
			this.lblState.Location = new System.Drawing.Point(150, 350);
			this.lblState.Name = "lblState";
			this.lblState.Size = new System.Drawing.Size(600, 20);
			this.lblState.TabIndex = 4;
			// 
			// lblTapePosition
			// 
			this.lblTapePosition.Location = new System.Drawing.Point(50, 75);
			this.lblTapePosition.Name = "lblTapePosition";
			this.lblTapePosition.Size = new System.Drawing.Size(700, 20);
			this.lblTapePosition.TabIndex = 5;
			// 
			// MainScreen
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(800, 450);
			this.Controls.Add(this.cmbTasks);
			this.Controls.Add(this.txtTape);
			this.Controls.Add(this.txtTransitions);
			this.Controls.Add(this.btnStep);
			this.Controls.Add(this.lblState);
			this.Controls.Add(this.lblTapePosition);
			this.Name = "MainScreen";
			this.Text = "Interpreter za rad Turingovog stroja";
			this.Load += new System.EventHandler(this.MainScreen_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion
	}
}

