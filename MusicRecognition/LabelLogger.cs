using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Threading;

namespace MusicRecognition
{
    class LabelLogger
    {
        private Label _label;

        /// <summary>
        /// Object enables writing easily logs to Label control
        /// </summary>
        /// <param name="label">Label in which logs will be written</param>
        public LabelLogger(Label label)
        {
            _label = label;
        }

        /// <summary>
        /// Writes log line
        public void WriteLog(string line)
        {
            _label.Content += line + "\n";
        }

        /// <summary>
        /// Clears label content
        /// </summary>
        public void Clear()
        {
            _label.Content = string.Empty;
        }
    }
}
