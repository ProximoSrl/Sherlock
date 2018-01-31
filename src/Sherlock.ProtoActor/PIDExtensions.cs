using System.Linq;
using Proto;

namespace Sherlock.ProtoActor
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
