using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Proto;

namespace Sherlock.Support
{
    public static class PIDExtensions
    {
        public static string ExtractName(this PID pid)
        {
            if (pid == null)
            {
                return "null";
            }

            return pid.Id.Split('/').Last();
        }
    }
}
