using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Microsoft.Scripting.Hosting;
using System.Windows.Forms;

namespace OpenTween
{
    class PytohnRC
    {
        private const string PythonRCFile = "rc.py";

        public static void initialize(TweenMain mainForm)
        {
            if (File.Exists(PythonRCFile))
            {
                var pyrc = mainForm.pyengine.CreateScriptSourceFromFile(PythonRCFile);
                var mod = new IronPython.Runtime.PythonModule();
                mod.__setattr__(IronPython.Runtime.DefaultContext.Default,
                    "set_opacity",
                    new Action<double>((opacity) => { mainForm.Opacity = opacity; }));
                mod.__setattr__(IronPython.Runtime.DefaultContext.Default, 
                    "set_consumer",
                    new Action<string, string>(ApplicationSettings.SetConsumer));
               mainForm.pyscope.SetVariable("tweenenv", mod);
                try
                {
                    pyrc.Compile().Execute(mainForm.pyscope);
                }
                catch (Exception pyex)
                {
                    var eo = mainForm.pyengine.GetService<ExceptionOperations>();
                    MessageBox.Show(eo.FormatException(pyex), "Error in RC", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }  
        }
    }
}
