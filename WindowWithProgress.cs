using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScheduleVis
{
    public interface IWindowWithProgress
    {
        void DoWorkStep(byte fractionOf255done);
        void DisplayMessage(string message);
        void Complete(string message);
    }
}
