using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Dispatch.Helpers
{
    public class Images
    {
        public static readonly ImageSource Bolt = new BitmapImage(new Uri("/Resources/ic_bolt.png", UriKind.Relative));
        public static readonly ImageSource Ftp = new BitmapImage(new Uri("/Resources/ic_ftp.png", UriKind.Relative));
        public static readonly ImageSource Sftp = new BitmapImage(new Uri("/Resources/ic_sftp.png", UriKind.Relative));
    }
}
