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
        private ScriptEngine pyengine;
        private ScriptScope pyscope;

        public PyConsole(ScriptEngine pyengine, ScriptScope pyscope)
        {
            this.pyengine = pyengine;
            this.pyscope = pyscope;

            InitializeComponent();
            textBox1.Focus();        
        }

        private void TextBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && (e.Control || e.Shift || e.Alt))
            {
                e.SuppressKeyPress = true;
                var codestr = textBox1.Text;
                var src = pyengine.CreateScriptSourceFromString(codestr);
                textBox2.AppendText(string.Join("\n... ", codestr.Split('\n')) + "\r\n");
                try
                {
                    var ret = src.Compile().Execute(pyscope);
                    if (ret != null)
                    {
                        int retry = 0;
                    retrying:
                        try
                        {
                            switch(retry)
                            {
                                case 0:
                                    textBox2.AppendText(ret.__repr__(IronPython.Runtime.DefaultContext.Default) + "\r\n");
                                    break;
                                case 1:
                                    textBox2.AppendText(ret.ToString() + "\r\n");
                                    break;
                                case 2:
                                    var tmpscope = pyengine.CreateScope();
                                    tmpscope.SetVariable("hoge", ret);
                                    textBox2.AppendText(pyengine.Execute<string>("str(hoge)", tmpscope) + "\r\n");
                                    break;
                            }
                        }
                        catch (Exception)
                        {
                            ++retry;
                            goto retrying;
                        }
                    }
                }
                catch (Exception pyex)
                {
                    var eo = pyengine.GetService<ExceptionOperations>();
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
