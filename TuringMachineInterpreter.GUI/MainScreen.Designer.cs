using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace TuringMachineInterpreter.GUI
{
	partial class MainScreen
	{
		private System.ComponentModel.IContainer components = null;
		private System.Windows.Forms.Label lblTapePosition;

		private string currentState = "q0";   // This is the current state of the Turing Machine (TM)
		private int tapePosition = 0;
		private string intialInput = "";
		private bool simulationStarted = false;

		private List<String> options = new List<String>
		{
			"Provjeri da li je binarni broj paran",
			"Obrni niz znakova",
			"Pretvorba malih u velika slova",
			"Provjeri da li je dekadski broj paran"
		};

		private void FormatInputAsCells(string input)
		{
			var formattedText = string.Join("|", input.ToCharArray()); // Dodaje vertikalne crtice između znakova
			txtTapeView.Text = formattedText+"|_|";
		}


		private void UpdateTapeAndHeadPosition(int tapePosition) => txtHeadPosition.Text = new string(' ', tapePosition * 2) + "^";


		private void BtnStep_Click(object sender, EventArgs e)
		{
			simulationStarted = true;
			txtTape.Enabled = false;

			if (string.IsNullOrEmpty(txtTape.Text))
				txtTape.Text = "_";
			
			if (!txtTapeView.Text.Contains("|"))
				FormatInputAsCells(txtTape.Text);

			UpdateTapeAndHeadPosition(tapePosition);

			// For predefined tasks, execute the corresponding function
			if (cmbTasks.SelectedItem.ToString() == options[0])
			{
				ExecuteEvenNumbersTM(); // This function will check if the input is an even binary number
				return;
			}
			else if (cmbTasks.SelectedItem.ToString() == options[1])
			{
				ExecuteFlipInputCharactersTM();  // This function will copy input text
				return;
			}
			else if (cmbTasks.SelectedItem.ToString() == options[2])
			{
				ExecuteLowerCaseToUpperTM();  // This function will turn small into capital letters
				return;
			}
			else if (cmbTasks.SelectedItem.ToString() == options[3])
			{
				ExecuteEvenNumbersDecimalTM();  // This function will check if the input is an even decimal number
				return;
			}

			char currentSymbol = txtTape.Text[tapePosition];
			lblState.Text = currentState;

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

					string direction = rightParts[2] == "R" ? "Desno" : "Lijevo";
					lblState.Text = currentState;
					txtTransitions.Text = $"Kretanje {direction}, Novi simbol: {rightParts[1]}";
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
				lblState.Text = "Nije pronađena validna tranzicija!";
				txtTransitions.Text = "Nije pronađena validna tranzicija!";
			}
			else
			{
				lblState.Text = currentState;
			}

			string positionMarker = new string(' ', tapePosition * 2) + "^"; // Multiplying by 2 is a simple way to align with characters in a TextBox, assuming a fixed-width font.
			lblTapePosition.Text = positionMarker;
		}

		private void ExecuteEvenNumbersDecimalTM()
		{
			switch (currentState)
			{
				case "q0": // Početno stanje
					if ("0123456789".Contains(txtTape.Text[tapePosition]))
					{
						currentState = "q0"; // Nastavi kretanje udesno dok ne pronađeš '_'
						MoveRight();
						lblState.Text = currentState;
						txtTransitions.Text = "Kretanje udesno do pronalaska završetka broja";
					}
					else if (txtTape.Text[tapePosition] == '_')
					{
						MoveLeft(); // Vrati se na zadnju znamenku broja
						currentState = "q1";
						lblState.Text = currentState;
						txtTransitions.Text = "Pronađen kraj broja, pomicanje za 1 mjesto ulijevo za provjeru zadnje znamenke";
					}
					break;

				case "q1": // Provjera zadnje znamenke
					currentState = "qf";
					if ("02468".Contains(txtTape.Text[tapePosition])) // Ako je zadnja znamenka 0, 2, 4, 6, ili 8
					{
						lblState.Text = currentState;
						txtTransitions.Text = "Zadnja znamenka unesenog broja je " + txtTape.Text[tapePosition] + ". Broj je paran";
					}
					else
					{
						lblState.Text = currentState;
						txtTransitions.Text = "Zadnja znamenka unesenog broja je " + txtTape.Text[tapePosition] + ". Broj je neparan";
					}
					break;
			}
		}


		private void ExecuteLowerCaseToUpperTM()
		{
			switch (currentState)
			{
				case "q0": // Početno stanje
					if (txtTape.Text[tapePosition] != '_') // Ako trenutni znak nije prazna ćelija
					{
						// Provjerite je li trenutni znak malo slovo
						if (txtTape.Text[tapePosition] >= 'a' && txtTape.Text[tapePosition] <= 'z')
						{
							// Zamijenite malo slovo s velikim
							char upperCaseChar = char.ToUpper(txtTape.Text[tapePosition]);
							txtTape.Text = txtTape.Text.Remove(tapePosition, 1).Insert(tapePosition, upperCaseChar.ToString());

							// Osvježite prikaz u txtTapeView nakon svake promjene
							FormatInputAsCells(txtTape.Text);
						}

						// Pomaknite se udesno kako biste provjerili sljedeći znak
						MoveRight();
						lblState.Text = "q0";
						txtTransitions.Text = "Pretvaranje malih slova u velika i kretanje udesno";
					}
					else // Ako naiđete na praznu ćeliju
					{
						// Prelazak u završno stanje
						currentState = "qf";
						lblState.Text = "qf";
						txtTransitions.Text = "Pronađena prazna ćelija, završetak pretvorbe";
					}
					break;
			}
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
						lblState.Text = currentState;
						txtTransitions.Text = "Kretanje udesno do pronalaska završetka broja";
					}
					else if (txtTape.Text[tapePosition] == '_')
					{
						MoveLeft(); // Go to the last symbol of the input
						currentState = "q1";
						lblState.Text = currentState;
						txtTransitions.Text = "Pronađen kraj broja, pomicanje za 1 mjesto ulijevo za provjeru zadnje znamenke";
					}
					break;

				case "q1": // Check last symbol state
					currentState = "qf";
					if (txtTape.Text[tapePosition] == '0')
					{
						lblState.Text = currentState;
						txtTransitions.Text = "Zadnja znamenka unesenog broja je 0. Broj je paran";
					}
					else
					{
						lblState.Text = currentState;
						txtTransitions.Text = "Zadnja znamenka unesenog broja je 1. Broj je neparan";
					}
					break;
			}
		}

		private void ExecuteFlipInputCharactersTM()
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
						lblState.Text = "Tekst je kopiran!";
					}
					UpdateTapeAndHeadPosition(tapePosition);
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
					UpdateTapeAndHeadPosition(tapePosition);
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
					UpdateTapeAndHeadPosition(tapePosition);
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
					UpdateTapeAndHeadPosition(tapePosition);
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
			clearFirstGroupInputs();
			if(cmbTasks.SelectedIndex == 0)
			{
				txtTransitions.Text = "U interpreter je potrebno unijeti binaran broj";
			}
			else if(cmbTasks.SelectedIndex == 1)
			{
				txtTransitions.Text = "U interpreter je moguće unijeti bilokakav znak";
			}
			else if(cmbTasks.SelectedIndex == 2)
			{
				txtTransitions.Text = "U interpreter je potrebno unijeti velika ili mala slova";
			}
			else if(cmbTasks.SelectedIndex == 3)
			{
				txtTransitions.Text = "U interpreter je moguće unijeti prirodne brojeve";
			}
		}

		private void clearFirstGroupInputs()
		{
			// Clear all inputs
			txtTape.Text = string.Empty;
			txtTapeView.Text = string.Empty;
			txtHeadPosition.Text = string.Empty;
			txtTransitions.Text = string.Empty;
			lblState.Text = string.Empty;
			lblTapePosition.Text = string.Empty;
			currentState = "q0";  // reset current state
			tapePosition = 0;     // reset tape position
			txtTape.Enabled = true;
			simulationStarted = false;
		}


		//---------------------------------------------User defined simulations--------------------------------
		private string ycurrentState = "q0";
        private int ytapePosition = 0;
        private bool ysimulationStarted = false;
        private Dictionary<(string, char), (string, char, char)> userDefinedRules;
		private void clearSecondGroupInputs()
		{
			ytxtTape.Text = string.Empty;
			ytxtTapeView.Text = string.Empty;
			ytxtHeadPosition.Text = string.Empty;
			ytxtTransitions.Text = string.Empty;
			ylblState.Text = string.Empty;
			ycurrentState = "q0";
			ytapePosition = 0;
			ytxtTape.Enabled = true;
			ysimulationStarted = false;
			ytxtInstructions.Text = string.Empty;
		}

		private void YMoveRight()
		{
			ytapePosition++;
			if (ytapePosition >= ytxtTape.Text.Length)
			{
				ytxtTape.Text += "_"; // Extend the tape
			}
		}

		private void YMoveLeft()
		{
			ytapePosition--;
			if (ytapePosition < 0)
			{
				ytxtTape.Text = "_" + ytxtTape.Text; // Extend the tape
				ytapePosition = 0; // Adjust the tape position
			}
		}

		private void YWriteSymbol(char symbol)
		{
			ytxtTape.Text = ytxtTape.Text.Remove(ytapePosition, 1).Insert(ytapePosition, symbol.ToString());
		}

		private void ParseUserDefinedRules()
		{
			userDefinedRules = new Dictionary<(string, char), (string, char, char)>();
			var lines = ytxtInstructions.Text.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);
			foreach (var line in lines)
			{
				var parts = line.Split(new[] { "->" }, StringSplitOptions.RemoveEmptyEntries);
				var currentStateSymbol = parts[0].Split(',');
				var newStateSymbolDirection = parts[1].Split(',');

				var key = (currentStateSymbol[0].Trim(), currentStateSymbol[1].Trim()[0]);
				var value = (newStateSymbolDirection[0].Trim(), newStateSymbolDirection[1].Trim()[0], newStateSymbolDirection[2].Trim()[0]);

				userDefinedRules[key] = value;
			}
		}

		private void ExecuteUserDefinedSimulationStep()
		{
			if (ycurrentState == "qf")
			{
				MessageBox.Show("Simulacija završena.");
				return;
			}

			var currentSymbol = ytxtTape.Text.Length > ytapePosition ? ytxtTape.Text[ytapePosition] : '_';
			var key = (ycurrentState, currentSymbol);

			if (userDefinedRules.TryGetValue(key, out var rule))
			{
				YWriteSymbol(rule.Item2);
				ycurrentState = rule.Item1;
				if (rule.Item3 == 'R') YMoveRight();
				else if (rule.Item3 == 'L') YMoveLeft();

				
			}
			else
			{
				MessageBox.Show("Nedostaje pravilo za trenutno stanje i simbol.");
				ycurrentState = "qf";
			}
		}

		private void UpdateUI()
		{
			YFormatInputAsCells(ytxtTape.Text);
			ylblState.Text = ycurrentState;
			ytxtHeadPosition.Text = new string(' ', ytapePosition * 2) + "^";
		}

		private void YFormatInputAsCells(string input)
		{
			var formattedText = string.Join("|", input.ToCharArray());
			ytxtTapeView.Text = formattedText + "|_|";
		}

		private void ybtnStep_Click(object sender, EventArgs e)
		{
			if (!ysimulationStarted)
			{
				ParseUserDefinedRules();
				ysimulationStarted = true;
				ytxtTape.Enabled = false;
			}

			ExecuteUserDefinedSimulationStep();
			UpdateUI();
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
			this.lblTapePosition = new System.Windows.Forms.Label();
			this.tabControl1 = new System.Windows.Forms.TabControl();
			this.tabPage1 = new System.Windows.Forms.TabPage();
			this.label18 = new System.Windows.Forms.Label();
			this.label8 = new System.Windows.Forms.Label();
			this.label7 = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.txtHeadPosition = new System.Windows.Forms.TextBox();
			this.label5 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.txtTapeView = new System.Windows.Forms.TextBox();
			this.label4 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.cmbTasks = new System.Windows.Forms.ComboBox();
			this.txtTape = new System.Windows.Forms.TextBox();
			this.txtTransitions = new System.Windows.Forms.TextBox();
			this.btnStep = new System.Windows.Forms.Button();
			this.lblState = new System.Windows.Forms.Label();
			this.tabPage2 = new System.Windows.Forms.TabPage();
			this.label16 = new System.Windows.Forms.Label();
			this.label17 = new System.Windows.Forms.Label();
			this.ytxtInstructions = new System.Windows.Forms.TextBox();
			this.label9 = new System.Windows.Forms.Label();
			this.label10 = new System.Windows.Forms.Label();
			this.label11 = new System.Windows.Forms.Label();
			this.ytxtHeadPosition = new System.Windows.Forms.TextBox();
			this.label12 = new System.Windows.Forms.Label();
			this.label13 = new System.Windows.Forms.Label();
			this.ytxtTapeView = new System.Windows.Forms.TextBox();
			this.label14 = new System.Windows.Forms.Label();
			this.label15 = new System.Windows.Forms.Label();
			this.ytxtTape = new System.Windows.Forms.TextBox();
			this.ytxtTransitions = new System.Windows.Forms.TextBox();
			this.ybtnStep = new System.Windows.Forms.Button();
			this.ylblState = new System.Windows.Forms.Label();
			this.tabControl1.SuspendLayout();
			this.tabPage1.SuspendLayout();
			this.tabPage2.SuspendLayout();
			this.SuspendLayout();
			// 
			// lblTapePosition
			// 
			this.lblTapePosition.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblTapePosition.Location = new System.Drawing.Point(44, 74);
			this.lblTapePosition.Name = "lblTapePosition";
			this.lblTapePosition.Size = new System.Drawing.Size(78, 32);
			this.lblTapePosition.TabIndex = 5;
			// 
			// tabControl1
			// 
			this.tabControl1.Controls.Add(this.tabPage1);
			this.tabControl1.Controls.Add(this.tabPage2);
			this.tabControl1.Location = new System.Drawing.Point(1, 0);
			this.tabControl1.Name = "tabControl1";
			this.tabControl1.SelectedIndex = 0;
			this.tabControl1.Size = new System.Drawing.Size(679, 456);
			this.tabControl1.TabIndex = 10;
			this.tabControl1.SelectedIndexChanged += new System.EventHandler(this.tabControl1_SelectedIndexChanged);
			// 
			// tabPage1
			// 
			this.tabPage1.Controls.Add(this.label18);
			this.tabPage1.Controls.Add(this.label8);
			this.tabPage1.Controls.Add(this.label7);
			this.tabPage1.Controls.Add(this.label6);
			this.tabPage1.Controls.Add(this.txtHeadPosition);
			this.tabPage1.Controls.Add(this.label5);
			this.tabPage1.Controls.Add(this.label1);
			this.tabPage1.Controls.Add(this.txtTapeView);
			this.tabPage1.Controls.Add(this.label4);
			this.tabPage1.Controls.Add(this.label3);
			this.tabPage1.Controls.Add(this.label2);
			this.tabPage1.Controls.Add(this.cmbTasks);
			this.tabPage1.Controls.Add(this.txtTape);
			this.tabPage1.Controls.Add(this.txtTransitions);
			this.tabPage1.Controls.Add(this.btnStep);
			this.tabPage1.Controls.Add(this.lblState);
			this.tabPage1.Location = new System.Drawing.Point(4, 22);
			this.tabPage1.Name = "tabPage1";
			this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage1.Size = new System.Drawing.Size(671, 430);
			this.tabPage1.TabIndex = 0;
			this.tabPage1.Text = "Predefinirane simulacije";
			this.tabPage1.UseVisualStyleBackColor = true;
			// 
			// label18
			// 
			this.label18.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
			this.label18.Location = new System.Drawing.Point(25, 151);
			this.label18.Name = "label18";
			this.label18.Size = new System.Drawing.Size(113, 18);
			this.label18.TabIndex = 42;
			this.label18.Text = "Pojašnjenja koraka:";
			// 
			// label8
			// 
			this.label8.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
			this.label8.ForeColor = System.Drawing.SystemColors.AppWorkspace;
			this.label8.Location = new System.Drawing.Point(342, 393);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(111, 18);
			this.label8.TabIndex = 24;
			this.label8.Text = "qf  - Završno stanje";
			// 
			// label7
			// 
			this.label7.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
			this.label7.ForeColor = System.Drawing.SystemColors.AppWorkspace;
			this.label7.Location = new System.Drawing.Point(342, 375);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(155, 18);
			this.label7.TabIndex = 23;
			this.label7.Text = "qX - Korak unutar algoritma";
			// 
			// label6
			// 
			this.label6.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
			this.label6.ForeColor = System.Drawing.SystemColors.AppWorkspace;
			this.label6.Location = new System.Drawing.Point(342, 357);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(111, 18);
			this.label6.TabIndex = 22;
			this.label6.Text = "q0 - Početno stanje";
			// 
			// txtHeadPosition
			// 
			this.txtHeadPosition.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.txtHeadPosition.Font = new System.Drawing.Font("Courier New", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
			this.txtHeadPosition.Location = new System.Drawing.Point(30, 134);
			this.txtHeadPosition.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.txtHeadPosition.Name = "txtHeadPosition";
			this.txtHeadPosition.ReadOnly = true;
			this.txtHeadPosition.Size = new System.Drawing.Size(601, 19);
			this.txtHeadPosition.TabIndex = 21;
			this.txtHeadPosition.Text = "^";
			// 
			// label5
			// 
			this.label5.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
			this.label5.ForeColor = System.Drawing.SystemColors.AppWorkspace;
			this.label5.Location = new System.Drawing.Point(451, 84);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(178, 18);
			this.label5.TabIndex = 20;
			this.label5.Text = "^ Označava poziciju glave čitača";
			// 
			// label1
			// 
			this.label1.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
			this.label1.Location = new System.Drawing.Point(25, 84);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(99, 18);
			this.label1.TabIndex = 19;
			this.label1.Text = "Traka stanja";
			// 
			// txtTapeView
			// 
			this.txtTapeView.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.txtTapeView.Enabled = false;
			this.txtTapeView.Font = new System.Drawing.Font("Courier New", 12F);
			this.txtTapeView.Location = new System.Drawing.Point(28, 104);
			this.txtTapeView.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.txtTapeView.Name = "txtTapeView";
			this.txtTapeView.ReadOnly = true;
			this.txtTapeView.Size = new System.Drawing.Size(601, 26);
			this.txtTapeView.TabIndex = 18;
			// 
			// label4
			// 
			this.label4.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
			this.label4.Location = new System.Drawing.Point(25, 10);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(123, 18);
			this.label4.TabIndex = 17;
			this.label4.Text = "Odabir tipa simulacije:";
			// 
			// label3
			// 
			this.label3.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
			this.label3.Location = new System.Drawing.Point(354, 10);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(99, 18);
			this.label3.TabIndex = 16;
			this.label3.Text = "Unos:";
			// 
			// label2
			// 
			this.label2.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
			this.label2.Location = new System.Drawing.Point(153, 357);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(99, 18);
			this.label2.TabIndex = 15;
			this.label2.Text = "Trenutno stanje: ";
			// 
			// cmbTasks
			// 
			this.cmbTasks.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cmbTasks.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.cmbTasks.Location = new System.Drawing.Point(28, 35);
			this.cmbTasks.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.cmbTasks.Name = "cmbTasks";
			this.cmbTasks.Size = new System.Drawing.Size(283, 29);
			this.cmbTasks.TabIndex = 10;
			this.cmbTasks.SelectedIndexChanged += new System.EventHandler(this.CmbTasks_SelectedIndexChanged);
			// 
			// txtTape
			// 
			this.txtTape.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.txtTape.Font = new System.Drawing.Font("Courier New", 12F);
			this.txtTape.Location = new System.Drawing.Point(357, 35);
			this.txtTape.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.txtTape.Name = "txtTape";
			this.txtTape.Size = new System.Drawing.Size(272, 26);
			this.txtTape.TabIndex = 11;
			// 
			// txtTransitions
			// 
			this.txtTransitions.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.txtTransitions.Location = new System.Drawing.Point(28, 171);
			this.txtTransitions.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.txtTransitions.Multiline = true;
			this.txtTransitions.Name = "txtTransitions";
			this.txtTransitions.ReadOnly = true;
			this.txtTransitions.Size = new System.Drawing.Size(601, 157);
			this.txtTransitions.TabIndex = 12;
			this.txtTransitions.Text = "Pojašnjenja koraka ";
			// 
			// btnStep
			// 
			this.btnStep.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.btnStep.Location = new System.Drawing.Point(28, 357);
			this.btnStep.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.btnStep.Name = "btnStep";
			this.btnStep.Size = new System.Drawing.Size(86, 41);
			this.btnStep.TabIndex = 13;
			this.btnStep.Text = "Dalje";
			this.btnStep.Click += new System.EventHandler(this.BtnStep_Click);
			// 
			// lblState
			// 
			this.lblState.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblState.Location = new System.Drawing.Point(152, 375);
			this.lblState.Name = "lblState";
			this.lblState.Size = new System.Drawing.Size(131, 32);
			this.lblState.TabIndex = 14;
			this.lblState.Text = "string";
			// 
			// tabPage2
			// 
			this.tabPage2.Controls.Add(this.label16);
			this.tabPage2.Controls.Add(this.label17);
			this.tabPage2.Controls.Add(this.ytxtInstructions);
			this.tabPage2.Controls.Add(this.label9);
			this.tabPage2.Controls.Add(this.label10);
			this.tabPage2.Controls.Add(this.label11);
			this.tabPage2.Controls.Add(this.ytxtHeadPosition);
			this.tabPage2.Controls.Add(this.label12);
			this.tabPage2.Controls.Add(this.label13);
			this.tabPage2.Controls.Add(this.ytxtTapeView);
			this.tabPage2.Controls.Add(this.label14);
			this.tabPage2.Controls.Add(this.label15);
			this.tabPage2.Controls.Add(this.ytxtTape);
			this.tabPage2.Controls.Add(this.ytxtTransitions);
			this.tabPage2.Controls.Add(this.ybtnStep);
			this.tabPage2.Controls.Add(this.ylblState);
			this.tabPage2.Location = new System.Drawing.Point(4, 22);
			this.tabPage2.Name = "tabPage2";
			this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage2.Size = new System.Drawing.Size(671, 430);
			this.tabPage2.TabIndex = 1;
			this.tabPage2.Text = "Simulacije s vlastitim unosom instrukcija";
			this.tabPage2.UseVisualStyleBackColor = true;
			// 
			// label16
			// 
			this.label16.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
			this.label16.Location = new System.Drawing.Point(34, 254);
			this.label16.Name = "label16";
			this.label16.Size = new System.Drawing.Size(113, 18);
			this.label16.TabIndex = 41;
			this.label16.Text = "Pojašnjenja koraka:";
			// 
			// label17
			// 
			this.label17.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
			this.label17.Location = new System.Drawing.Point(34, 15);
			this.label17.Name = "label17";
			this.label17.Size = new System.Drawing.Size(107, 18);
			this.label17.TabIndex = 39;
			this.label17.Text = "Instrukcije i pravila:";
			// 
			// ytxtInstructions
			// 
			this.ytxtInstructions.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.ytxtInstructions.Font = new System.Drawing.Font("Courier New", 12F);
			this.ytxtInstructions.Location = new System.Drawing.Point(35, 35);
			this.ytxtInstructions.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.ytxtInstructions.Multiline = true;
			this.ytxtInstructions.Name = "ytxtInstructions";
			this.ytxtInstructions.Size = new System.Drawing.Size(179, 146);
			this.ytxtInstructions.TabIndex = 38;
			this.ytxtInstructions.Text = "q0,1 -> q1,1,R q0,1 -> q1,1,R q0,1 -> q1,1,R";
			// 
			// label9
			// 
			this.label9.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
			this.label9.ForeColor = System.Drawing.SystemColors.AppWorkspace;
			this.label9.Location = new System.Drawing.Point(349, 398);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(111, 18);
			this.label9.TabIndex = 37;
			this.label9.Text = "qf  - Završno stanje";
			// 
			// label10
			// 
			this.label10.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
			this.label10.ForeColor = System.Drawing.SystemColors.AppWorkspace;
			this.label10.Location = new System.Drawing.Point(349, 380);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(155, 18);
			this.label10.TabIndex = 36;
			this.label10.Text = "qX - Korak unutar algoritma";
			// 
			// label11
			// 
			this.label11.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
			this.label11.ForeColor = System.Drawing.SystemColors.AppWorkspace;
			this.label11.Location = new System.Drawing.Point(349, 362);
			this.label11.Name = "label11";
			this.label11.Size = new System.Drawing.Size(111, 18);
			this.label11.TabIndex = 35;
			this.label11.Text = "q0 - Početno stanje";
			// 
			// ytxtHeadPosition
			// 
			this.ytxtHeadPosition.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.ytxtHeadPosition.Font = new System.Drawing.Font("Courier New", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
			this.ytxtHeadPosition.Location = new System.Drawing.Point(37, 233);
			this.ytxtHeadPosition.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.ytxtHeadPosition.Name = "ytxtHeadPosition";
			this.ytxtHeadPosition.ReadOnly = true;
			this.ytxtHeadPosition.Size = new System.Drawing.Size(601, 19);
			this.ytxtHeadPosition.TabIndex = 34;
			this.ytxtHeadPosition.Text = "^";
			// 
			// label12
			// 
			this.label12.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
			this.label12.ForeColor = System.Drawing.SystemColors.AppWorkspace;
			this.label12.Location = new System.Drawing.Point(458, 183);
			this.label12.Name = "label12";
			this.label12.Size = new System.Drawing.Size(178, 18);
			this.label12.TabIndex = 33;
			this.label12.Text = "^ Označava poziciju glave čitača";
			// 
			// label13
			// 
			this.label13.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
			this.label13.Location = new System.Drawing.Point(32, 183);
			this.label13.Name = "label13";
			this.label13.Size = new System.Drawing.Size(99, 18);
			this.label13.TabIndex = 32;
			this.label13.Text = "Traka stanja";
			// 
			// ytxtTapeView
			// 
			this.ytxtTapeView.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.ytxtTapeView.Enabled = false;
			this.ytxtTapeView.Font = new System.Drawing.Font("Courier New", 12F);
			this.ytxtTapeView.Location = new System.Drawing.Point(35, 203);
			this.ytxtTapeView.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.ytxtTapeView.Name = "ytxtTapeView";
			this.ytxtTapeView.ReadOnly = true;
			this.ytxtTapeView.Size = new System.Drawing.Size(601, 26);
			this.ytxtTapeView.TabIndex = 31;
			// 
			// label14
			// 
			this.label14.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
			this.label14.Location = new System.Drawing.Point(310, 15);
			this.label14.Name = "label14";
			this.label14.Size = new System.Drawing.Size(99, 18);
			this.label14.TabIndex = 30;
			this.label14.Text = "Unos:";
			// 
			// label15
			// 
			this.label15.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
			this.label15.Location = new System.Drawing.Point(160, 362);
			this.label15.Name = "label15";
			this.label15.Size = new System.Drawing.Size(99, 18);
			this.label15.TabIndex = 29;
			this.label15.Text = "Trenutno stanje: ";
			// 
			// ytxtTape
			// 
			this.ytxtTape.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.ytxtTape.Font = new System.Drawing.Font("Courier New", 12F);
			this.ytxtTape.Location = new System.Drawing.Point(313, 40);
			this.ytxtTape.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.ytxtTape.Name = "ytxtTape";
			this.ytxtTape.Size = new System.Drawing.Size(323, 26);
			this.ytxtTape.TabIndex = 25;
			// 
			// ytxtTransitions
			// 
			this.ytxtTransitions.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ytxtTransitions.Location = new System.Drawing.Point(37, 274);
			this.ytxtTransitions.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.ytxtTransitions.Multiline = true;
			this.ytxtTransitions.Name = "ytxtTransitions";
			this.ytxtTransitions.ReadOnly = true;
			this.ytxtTransitions.Size = new System.Drawing.Size(599, 59);
			this.ytxtTransitions.TabIndex = 26;
			this.ytxtTransitions.Text = "U polje Instrukcije unosite pravila koja definiraju ponašanje Turingovog stroja. " +
    "         U polje Unos upisujete podatke koje Turingov stroj treba obraditi.";
			// 
			// ybtnStep
			// 
			this.ybtnStep.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ybtnStep.Location = new System.Drawing.Point(35, 362);
			this.ybtnStep.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.ybtnStep.Name = "ybtnStep";
			this.ybtnStep.Size = new System.Drawing.Size(86, 41);
			this.ybtnStep.TabIndex = 27;
			this.ybtnStep.Text = "Dalje";
			this.ybtnStep.Click += new System.EventHandler(this.ybtnStep_Click);
			// 
			// ylblState
			// 
			this.ylblState.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ylblState.Location = new System.Drawing.Point(159, 380);
			this.ylblState.Name = "ylblState";
			this.ylblState.Size = new System.Drawing.Size(131, 32);
			this.ylblState.TabIndex = 28;
			this.ylblState.Text = "string";
			// 
			// MainScreen
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(678, 455);
			this.Controls.Add(this.tabControl1);
			this.Controls.Add(this.lblTapePosition);
			this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.Name = "MainScreen";
			this.Text = "Interpreter za rad Turingovog stroja";
			this.Load += new System.EventHandler(this.MainScreen_Load);
			this.tabControl1.ResumeLayout(false);
			this.tabPage1.ResumeLayout(false);
			this.tabPage1.PerformLayout();
			this.tabPage2.ResumeLayout(false);
			this.tabPage2.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private TabControl tabControl1;
		private TabPage tabPage1;
		private TabPage tabPage2;
		private Label label4;
		private Label label3;
		private Label label2;
		private ComboBox cmbTasks;
		private TextBox txtTape;
		private TextBox txtTransitions;
		private Button btnStep;
		private Label lblState;
		private TextBox txtTapeView;
		private Label label1;
		private TextBox txtHeadPosition;
		private Label label5;
		private Label label8;
		private Label label7;
		private Label label6;
		private Label label17;
		private TextBox ytxtInstructions;
		private Label label9;
		private Label label10;
		private Label label11;
		private TextBox ytxtHeadPosition;
		private Label label12;
		private Label label13;
		private TextBox ytxtTapeView;
		private Label label14;
		private Label label15;
		private TextBox ytxtTape;
		private TextBox ytxtTransitions;
		private Button ybtnStep;
		private Label ylblState;
		private Label label18;
		private Label label16;
	}
}

