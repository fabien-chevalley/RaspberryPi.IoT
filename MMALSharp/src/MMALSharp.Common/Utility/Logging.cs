using NLog;
using NLog.Config;
using NLog.Targets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMALSharp
{
    public static class MMALLog
    {
        public static Logger Logger { get; set; }

        public static void ConfigureLogConfig()
        {
            Logger = LogManager.GetCurrentClassLogger();
        }
    }
}
