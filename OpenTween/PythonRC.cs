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
    class PythonRC
    {
        private const string PythonRCFile = "rc.py";

        public static void initialize(TweenMain mainForm)
        {
            var mod = new IronPython.Runtime.PythonModule();
            mod.__setattr__(IronPython.Runtime.DefaultContext.Default,
                "__name__",
                "tweenenv");
            mod.__setattr__(IronPython.Runtime.DefaultContext.Default,
                "TweenMain",
                mainForm);
            mod.__setattr__(IronPython.Runtime.DefaultContext.Default,
                "set_consumer",
                new Action<string, string>(ApplicationSettings.SetConsumer));
            mod.__setattr__(IronPython.Runtime.DefaultContext.Default,
                "set_mutefilter",
                new Action<Func<PostClass, bool>>((func) => TabInformations.GetInstance().MuteFilter = func));
            mod.__setattr__(IronPython.Runtime.DefaultContext.Default,
                "get_statustext",
                new Func<Tuple<string, string, long?>>(() => mainForm.StatusTextInfo));
            mod.__setattr__(IronPython.Runtime.DefaultContext.Default,
                "set_statustext",
                new Action<string, string, long?>((text, screen_name, id) => mainForm.StatusTextInfo = new Tuple<string, string, long?>(text, screen_name, id)));
            mod.__setattr__(IronPython.Runtime.DefaultContext.Default,
                "clear_statustext",
                new Action(mainForm.ClearStatusText));
            mod.__setattr__(IronPython.Runtime.DefaultContext.Default,
                "show_user",
                new Action<string>((screen_name) => mainForm.ShowUserStatus(screen_name, false)));
            mainForm.pyscope.SetVariable("tweenenv", mod);

            if (File.Exists(PythonRCFile))
            {
                var pyrc = mainForm.pyengine.CreateScriptSourceFromFile(PythonRCFile);
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
