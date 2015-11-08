using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFProject
{
    public interface ISwitchable
    {
        void UtilizeState(object state);

        void sendcommand(int command);
    }
}
