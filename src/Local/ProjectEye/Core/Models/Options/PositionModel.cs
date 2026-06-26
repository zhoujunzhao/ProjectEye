using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Project1.UI.Controls;

namespace ProjectEye.Core.Models.Options
{
    /// <summary>
    /// 位置模型
    /// </summary>
    public class PositionModel
    {
        public Size WindowsSize {  get; set; }
        public Point Position { get; set; }

        public ControlBase TipImage { get; set; }

        public ControlBase TipText { get; set; }

        public ControlBase RestBtn { get; set; }

        public ControlBase BreakBtn { get; set; }

        public ControlBase CountDownText { get; set; }

    }
}
