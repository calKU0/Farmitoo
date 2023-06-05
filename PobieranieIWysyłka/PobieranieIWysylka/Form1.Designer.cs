
namespace PobieranieIWysylka
{
    partial class Form1
    {
        private const string V = "Form1";

        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            button1 = new System.Windows.Forms.Button();
            button2 = new System.Windows.Forms.Button();
            textBox1 = new System.Windows.Forms.TextBox();
            maskedTextBox1 = new System.Windows.Forms.MaskedTextBox();
            SuspendLayout();
            // 
            // button1
            // 
            button1.Location = new System.Drawing.Point(56, 144);
            button1.Name = "button1";
            button1.Size = new System.Drawing.Size(218, 55);
            button1.TabIndex = 0;
            button1.Text = "Pobierz";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // button2
            // 
            button2.Location = new System.Drawing.Point(56, 262);
            button2.Name = "button2";
            button2.Size = new System.Drawing.Size(218, 55);
            button2.TabIndex = 1;
            button2.Text = "Wyślij";
            button2.UseVisualStyleBackColor = true;
            button2.Click += button2_Click;
            // 
            // textBox1
            // 
            textBox1.BackColor = System.Drawing.SystemColors.Menu;
            textBox1.Font = new System.Drawing.Font("Segoe UI", 25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            textBox1.Location = new System.Drawing.Point(27, 12);
            textBox1.Name = "textBox1";
            textBox1.Size = new System.Drawing.Size(285, 52);
            textBox1.TabIndex = 2;
            textBox1.Text = "Farmitoo";
            textBox1.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // maskedTextBox1
            // 
            maskedTextBox1.BackColor = System.Drawing.SystemColors.MenuBar;
            maskedTextBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            maskedTextBox1.ForeColor = System.Drawing.Color.Red;
            maskedTextBox1.Location = new System.Drawing.Point(57, 201);
            maskedTextBox1.Name = "maskedTextBox1";
            maskedTextBox1.ReadOnly = true;
            maskedTextBox1.Size = new System.Drawing.Size(218, 16);
            maskedTextBox1.TabIndex = 3;
            // 
            // Form1
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(335, 400);
            Controls.Add(maskedTextBox1);
            Controls.Add(textBox1);
            Controls.Add(button2);
            Controls.Add(button1);
            Name = "Form1";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.MaskedTextBox maskedTextBox1;
    }
}

