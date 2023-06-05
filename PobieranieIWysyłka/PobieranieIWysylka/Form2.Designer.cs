
namespace PobieranieIWysylka
{
    partial class Form2
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            button1 = new System.Windows.Forms.Button();
            textBox2 = new System.Windows.Forms.TextBox();
            listBox1 = new System.Windows.Forms.ListBox();
            button2 = new System.Windows.Forms.Button();
            listBox2 = new System.Windows.Forms.ListBox();
            textBox1 = new System.Windows.Forms.TextBox();
            button3 = new System.Windows.Forms.Button();
            button4 = new System.Windows.Forms.Button();
            SuspendLayout();
            // 
            // button1
            // 
            button1.Location = new System.Drawing.Point(318, 390);
            button1.Name = "button1";
            button1.Size = new System.Drawing.Size(91, 41);
            button1.TabIndex = 1;
            button1.Text = "Wyślij wszystko";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // textBox2
            // 
            textBox2.BackColor = System.Drawing.SystemColors.MenuBar;
            textBox2.BorderStyle = System.Windows.Forms.BorderStyle.None;
            textBox2.Font = new System.Drawing.Font("Segoe UI", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            textBox2.Location = new System.Drawing.Point(12, 35);
            textBox2.Name = "textBox2";
            textBox2.ReadOnly = true;
            textBox2.Size = new System.Drawing.Size(137, 24);
            textBox2.TabIndex = 2;
            textBox2.Text = "Potwierdzenia:";
            // 
            // listBox1
            // 
            listBox1.FormattingEnabled = true;
            listBox1.ItemHeight = 15;
            listBox1.Location = new System.Drawing.Point(12, 59);
            listBox1.Name = "listBox1";
            listBox1.Size = new System.Drawing.Size(137, 379);
            listBox1.TabIndex = 4;
            // 
            // button2
            // 
            button2.Location = new System.Drawing.Point(9, 8);
            button2.Name = "button2";
            button2.Size = new System.Drawing.Size(67, 22);
            button2.TabIndex = 5;
            button2.Text = "Powrót";
            button2.UseVisualStyleBackColor = true;
            button2.Click += button2_Click;
            // 
            // listBox2
            // 
            listBox2.FormattingEnabled = true;
            listBox2.ItemHeight = 15;
            listBox2.Location = new System.Drawing.Point(170, 59);
            listBox2.Name = "listBox2";
            listBox2.Size = new System.Drawing.Size(137, 379);
            listBox2.TabIndex = 6;
            // 
            // textBox1
            // 
            textBox1.BackColor = System.Drawing.SystemColors.MenuBar;
            textBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            textBox1.Font = new System.Drawing.Font("Segoe UI", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            textBox1.Location = new System.Drawing.Point(170, 35);
            textBox1.Name = "textBox1";
            textBox1.ReadOnly = true;
            textBox1.Size = new System.Drawing.Size(137, 24);
            textBox1.TabIndex = 7;
            textBox1.Text = "Tracking:";
            // 
            // button3
            // 
            button3.Location = new System.Drawing.Point(318, 331);
            button3.Name = "button3";
            button3.Size = new System.Drawing.Size(91, 41);
            button3.TabIndex = 8;
            button3.Text = "Wyślij tracking";
            button3.UseVisualStyleBackColor = true;
            button3.Click += button3_Click;
            // 
            // button4
            // 
            button4.Location = new System.Drawing.Point(318, 271);
            button4.Name = "button4";
            button4.Size = new System.Drawing.Size(91, 41);
            button4.TabIndex = 9;
            button4.Text = "Wyślij potwierdzenia";
            button4.UseVisualStyleBackColor = true;
            button4.Click += button4_Click;
            // 
            // Form2
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(421, 450);
            Controls.Add(button4);
            Controls.Add(button3);
            Controls.Add(textBox1);
            Controls.Add(listBox2);
            Controls.Add(button2);
            Controls.Add(listBox1);
            Controls.Add(textBox2);
            Controls.Add(button1);
            Name = "Form2";
            Text = "Form2";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.ListBox listBox2;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button4;
    }
}