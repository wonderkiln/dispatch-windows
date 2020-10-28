using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dispatch.Helpers
{
    public class Constants
    {
        public enum Flavour { Develop, Beta, Stable }

#if DEVELOP || DEBUG
        public static Flavour FLAVOUR = Flavour.Develop;
#elif BETA
        public static Flavour FLAVOUR = Flavour.Beta;
#else
        public static Flavour FLAVOUR = Flavour.Stable;
#endif
    }
}
