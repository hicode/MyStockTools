﻿namespace OrganizeSelection
{
    partial class MainForm
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
            this.btnMove = new System.Windows.Forms.Button();
            this.btnUnion = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnMove
            // 
            this.btnMove.Location = new System.Drawing.Point(51, 100);
            this.btnMove.Name = "btnMove";
            this.btnMove.Size = new System.Drawing.Size(75, 23);
            this.btnMove.TabIndex = 0;
            this.btnMove.Text = "移动";
            this.btnMove.UseVisualStyleBackColor = true;
            this.btnMove.Click += new System.EventHandler(this.btnMove_Click);
            // 
            // btnUnion
            // 
            this.btnUnion.Location = new System.Drawing.Point(155, 100);
            this.btnUnion.Name = "btnUnion";
            this.btnUnion.Size = new System.Drawing.Size(75, 23);
            this.btnUnion.TabIndex = 1;
            this.btnUnion.Text = "合并";
            this.btnUnion.UseVisualStyleBackColor = true;
            this.btnUnion.Click += new System.EventHandler(this.btnUnion_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Controls.Add(this.btnUnion);
            this.Controls.Add(this.btnMove);
            this.Name = "MainForm";
            this.Text = "整理自选股";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnMove;
        private System.Windows.Forms.Button btnUnion;
    }
}

