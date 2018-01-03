using testwinc.tools;

namespace testwinc.capvideo
{
    partial class FormNfSetup
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
        	this.lblnfsetupqh = new System.Windows.Forms.Label();
            this.tbnfsetup = new testwinc.tools.TextBoxNum();//new System.Windows.Forms.TextBox();
        	this.btnnfsetupok = new System.Windows.Forms.Button();
        	this.btnnfsetupcanel = new System.Windows.Forms.Button();
        	this.groupboxnfsetup = new System.Windows.Forms.GroupBox();
        	this.label2 = new System.Windows.Forms.Label();
        	this.tbnfsetupkj = new System.Windows.Forms.TextBox();
        	this.label1 = new System.Windows.Forms.Label();
        	this.groupboxnfsetup.SuspendLayout();
        	this.SuspendLayout();
        	// 
        	// lblnfsetupqh
        	// 
        	this.lblnfsetupqh.AutoSize = true;
        	this.lblnfsetupqh.Location = new System.Drawing.Point(11, 28);
        	this.lblnfsetupqh.Name = "lblnfsetupqh";
        	this.lblnfsetupqh.Size = new System.Drawing.Size(158, 15);
        	this.lblnfsetupqh.TabIndex = 2;
        	this.lblnfsetupqh.Text = "请输入要补投的期号: ";
        	// 
        	// tbnfsetup
        	// 
        	this.tbnfsetup.Location = new System.Drawing.Point(175, 18);
        	this.tbnfsetup.Name = "tbnfsetup";
        	this.tbnfsetup.Size = new System.Drawing.Size(149, 25);
        	this.tbnfsetup.TabIndex = 3;
        	// 
        	// btnnfsetupok
        	// 
        	this.btnnfsetupok.Location = new System.Drawing.Point(68, 153);
        	this.btnnfsetupok.Name = "btnnfsetupok";
        	this.btnnfsetupok.Size = new System.Drawing.Size(75, 23);
        	this.btnnfsetupok.TabIndex = 4;
        	this.btnnfsetupok.Text = "确定";
        	this.btnnfsetupok.UseVisualStyleBackColor = true;
        	this.btnnfsetupok.Click += new System.EventHandler(this.btnnfsetupok_Click);
        	// 
        	// btnnfsetupcanel
        	// 
        	this.btnnfsetupcanel.Location = new System.Drawing.Point(197, 153);
        	this.btnnfsetupcanel.Name = "btnnfsetupcanel";
        	this.btnnfsetupcanel.Size = new System.Drawing.Size(75, 23);
        	this.btnnfsetupcanel.TabIndex = 5;
        	this.btnnfsetupcanel.Text = "取消";
        	this.btnnfsetupcanel.UseVisualStyleBackColor = true;
        	this.btnnfsetupcanel.Click += new System.EventHandler(this.btnnfsetupcanel_Click);
        	// 
        	// groupboxnfsetup
        	// 
        	this.groupboxnfsetup.Controls.Add(this.label2);
        	this.groupboxnfsetup.Controls.Add(this.tbnfsetupkj);
        	this.groupboxnfsetup.Controls.Add(this.label1);
        	this.groupboxnfsetup.Controls.Add(this.lblnfsetupqh);
        	this.groupboxnfsetup.Controls.Add(this.tbnfsetup);
        	this.groupboxnfsetup.Location = new System.Drawing.Point(1, -6);
        	this.groupboxnfsetup.Name = "groupboxnfsetup";
        	this.groupboxnfsetup.Size = new System.Drawing.Size(332, 153);
        	this.groupboxnfsetup.TabIndex = 6;
        	this.groupboxnfsetup.TabStop = false;
        	// 
        	// label2
        	// 
        	this.label2.AutoSize = true;
        	this.label2.Location = new System.Drawing.Point(193, 100);
        	this.label2.Name = "label2";
        	this.label2.Size = new System.Drawing.Size(125, 15);
        	this.label2.TabIndex = 6;
        	this.label2.Text = "(例：1,4,6,7,8)";
        	// 
        	// tbnfsetupkj
        	// 
        	this.tbnfsetupkj.Location = new System.Drawing.Point(175, 72);
        	this.tbnfsetupkj.Name = "tbnfsetupkj";
        	this.tbnfsetupkj.Size = new System.Drawing.Size(151, 25);
        	this.tbnfsetupkj.TabIndex = 5;
        	// 
        	// label1
        	// 
        	this.label1.AutoSize = true;
        	this.label1.Location = new System.Drawing.Point(67, 82);
        	this.label1.Name = "label1";
        	this.label1.Size = new System.Drawing.Size(165, 15);
        	this.label1.TabIndex = 4;
        	this.label1.Text = "输入该期的上期开奖号:";
        	// 
        	// FormNfSetup
        	// 
        	this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
        	this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        	this.ClientSize = new System.Drawing.Size(337, 188);
        	this.Controls.Add(this.btnnfsetupcanel);
        	this.Controls.Add(this.btnnfsetupok);
        	this.Controls.Add(this.groupboxnfsetup);
        	this.Name = "FormNfSetup";
        	this.Text = "设置";
        	this.groupboxnfsetup.ResumeLayout(false);
        	this.groupboxnfsetup.PerformLayout();
        	this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Label lblnfsetupqh;
        private System.Windows.Forms.Button btnnfsetupok;
        private System.Windows.Forms.Button btnnfsetupcanel;
        private System.Windows.Forms.GroupBox groupboxnfsetup;
        private System.Windows.Forms.TextBox tbnfsetupkj;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tbnfsetup;
        private System.Windows.Forms.Label label2;
    }
}