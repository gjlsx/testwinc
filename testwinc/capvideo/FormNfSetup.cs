using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace testwinc.capvideo
{
    public partial class FormNfSetup : Form
    {
        public FormNfSetup()
        {
            InitializeComponent();
            initWindForm();
        }

         //初始化
        private void initWindForm()
        {
            this.StartPosition = FormStartPosition.CenterParent;
            tbnfsetup.MaxLength = 9;

        }

        private void btnnfsetupok_Click(object sender, EventArgs e)
        {
            //add code here
            if (this.tbnfsetup.Text.Trim().Equals(""))
            {
                MessageBox.Show(this, "您输入期数为空！", "提示");
                return;
            }
            int qihao = Int32.Parse(tbnfsetup.Text);
            String shqh = (qihao - 1) + "";

            if (tbnfsetupkj.Text.Trim().Equals(""))
            {
                //是否已经存储当期上一期号码
                Boolean issave = false;
               
                 if(ForNaiStrategy.llKaiJiang.Contains(shqh))
                 {
                    issave = true;
                 }
                //若没有则
                if (!issave)
                {
                    MessageBox.Show(this, "您输入该期上期号码为空！", "提示");
                    return;
                }
                //若有则
                else
                {
                    //获得上期期数，补投，add code here
                    UseStatic.getWindForm().fnfButou(shqh, ForNaiStrategy.htKai[shqh]+"");

                    this.Dispose();
                }
            }
            else
            {
                //获得当期期数，补投
                UseStatic.getWindForm().fnfButou(shqh, tbnfsetupkj.Text);

                this.Dispose();
            }
        }

        private void btnnfsetupcanel_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

    }
}
