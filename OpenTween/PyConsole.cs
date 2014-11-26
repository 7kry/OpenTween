using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Scripting.Hosting;
using System.Text.RegularExpressions;

namespace OpenTween
{
    public partial class PyConsole : Form
    {
        public PyConsole()
        {
            InitializeComponent();
            textBox1.Focus();        
        }

        private void TextBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && (e.Control || e.Shift || e.Alt))
            {
                e.SuppressKeyPress = true;
                var codestr = textBox1.Text;
                var src = TweenMain.pyengine.CreateScriptSourceFromString(codestr);
                textBox2.AppendText(string.Join("\n... ", codestr.Split('\n')) + "\r\n");
                try
                {
                    var ret = src.Compile().Execute(TweenMain.pyscope);
                    if (ret != null)
                    {
                        try
                        {
                            textBox2.AppendText(ret.__repr__(IronPython.Runtime.DefaultContext.Default) + "\r\n");
                        }
                        catch (Exception)
                        {
                            textBox2.AppendText(ret.ToString() + "\r\n");
                        }
                    }
                }
                catch (Exception pyex)
                {
                    var eo = TweenMain.pyengine.GetService<ExceptionOperations>();
                    textBox2.AppendText(eo.FormatException(pyex) + "\r\n");
                }
                textBox2.AppendText(">>> ");
            }
            else if (e.KeyCode == Keys.Delete && e.Control)
            {
                textBox1.Clear();
            }
            else if (e.KeyCode == Keys.Escape)
            {
                this.Close();
            }
        }
    }
}
