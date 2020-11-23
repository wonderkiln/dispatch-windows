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
    }
}
