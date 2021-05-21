using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Dispatch.Helpers
{
    public class Icons
    {
        public static readonly ImageSource Icon = new BitmapImage(new Uri("/Resources/Icon.ico", UriKind.Relative));

        public static readonly ImageSource Bolt = new BitmapImage(new Uri("/Resources/Images/ic_bolt.png", UriKind.Relative));
        public static readonly ImageSource Ftp = new BitmapImage(new Uri("/Resources/Images/ic_ftp.png", UriKind.Relative));
        public static readonly ImageSource Sftp = new BitmapImage(new Uri("/Resources/Images/ic_sftp.png", UriKind.Relative));
        public static readonly ImageSource S3 = new BitmapImage(new Uri("/Resources/Images/ic_s3.png", UriKind.Relative));
    }
}
