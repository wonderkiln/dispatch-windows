using System;
using System.Reflection;

namespace Dispatch.Helpers
{
    public class Constants
    {
        public enum Channel { Nightly, Beta, Stable }

#if NIGHTLY || DEBUG
        public static Channel CHANNEL = Channel.Nightly;
#elif BETA
        public static Channel CHANNEL = Channel.Beta;
#else
        public static Channel CHANNEL = Channel.Stable;
#endif

        public static Version VERSION
        {
            get
            {
                return Assembly.GetExecutingAssembly().GetName().Version;
            }
        }
    }
}
