using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace testwinc.tools
{
    //只能输入数字的textbox控件
    public class TextBoxNum: TextBox
    {
        public TextBoxNum() 
        {
             
            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(keypress_num);              
        }


        private void keypress_num(object sender, KeyPressEventArgs e)
        {
            // KeyPress中用 e.KeyChar == '\b'判断。
            //KeyDown和KeyUp中用 e.KeyCode == Keys.Back判断回退键盘    
            //第一个字符能输入负号-
            if(e.KeyChar.Equals('-') && (!this.Text.Equals("")))
            	{
            		 e.Handled = true;
            	}
			
            else if ("1234567890-".IndexOf(e.KeyChar) == -1 && (e.KeyChar != '\b'))
            {
                e.Handled = true;
            }                      
        }

    }
}
