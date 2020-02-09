using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Threading;

namespace LilyWhite.UI.Type
{
    /// <summary>
    /// 用于将 Console 输出绑定到文本框
    /// </summary>
    public class ControlWriter : TextWriter
    {
        private Dispatcher dispatcher;
        private TextBox tb;
        private Action callback;
        public ControlWriter(Dispatcher dispatcher, TextBox tb, Action callback)
        {
            tb.IsReadOnly = true;
            this.dispatcher = dispatcher;
            this.tb = tb;
            this.callback = callback;
        }
        public override void Write(char value)
        {
            Write(value.ToString());
        }

        public override void Write(string value)
        {
            this.dispatcher.Invoke(DispatcherPriority.Send, (Action)delegate {
                tb.AppendText(value);
                callback.Invoke();
            });


        }
        public override Encoding Encoding
        {
            get { return Encoding.UTF8; }
        }
    }
}
