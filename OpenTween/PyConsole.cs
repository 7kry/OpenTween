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
                e.Handled = true;
                var codestr = textBox1.Text;
                var src = TweenMain.pyengine.CreateScriptSourceFromString(codestr);
                textBox2.Text += ">>> " + string.Join("\n... ", codestr.Split('\n')) + "\r\n";
                try
                {
                    textBox2.Text += src.Compile().Execute(TweenMain.pyscope) + "\r\n";
                }
                catch (Exception pyex)
                {
                    var eo = TweenMain.pyengine.GetService<ExceptionOperations>();
                    textBox2.Text += eo.FormatException(pyex) + "\r\n";
                }
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
