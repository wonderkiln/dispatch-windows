using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Dispatch.Helpers
{
    public class FileIconHelper
    {
        private static readonly ImageSource DRIVE_ICON = new BitmapImage(new Uri("/Resources/icons/ic_drive.png", UriKind.Relative));
        private static readonly ImageSource DIRECTORY_ICON = new BitmapImage(new Uri("/Resources/icons/ic_folder.png", UriKind.Relative));
        private static readonly ImageSource FILE_ICON = new BitmapImage(new Uri("/Resources/icons/ic_file.png", UriKind.Relative));
        private static readonly ImageSource IMAGE_ICON = new BitmapImage(new Uri("/Resources/icons/ic_image.png", UriKind.Relative));
        private static readonly ImageSource VIDEO_ICON = new BitmapImage(new Uri("/Resources/icons/ic_video.png", UriKind.Relative));

        private static readonly List<string> IMAGE_EXTENSIONS = new List<string>() { "ase", "art", "bmp", "blp", "cd5", "cit", "cpt", "cr2", "cut", "dds", "dib", "djvu", "egt", "exif", "gif", "gpl", "grf", "icns", "ico", "iff", "jng", "jpeg", "jpg", "jfif", "jp2", "jps", "lbm", "max", "miff", "mng", "msp", "nitf", "ota", "pbm", "pc1", "pc2", "pc3", "pcf", "pcx", "pdn", "pgm", "PI1", "PI2", "PI3", "pict", "pct", "pnm", "pns", "ppm", "psb", "psd", "pdd", "psp", "px", "pxm", "pxr", "qfx", "raw", "rle", "sct", "sgi", "rgb", "int", "bw", "tga", "tiff", "tif", "vtf", "xbm", "xcf", "xpm", "3dv", "amf", "ai", "awg", "cgm", "cdr", "cmx", "dxf", "e2d", "egt", "eps", "fs", "gbr", "odg", "svg", "stl", "vrml", "x3d", "sxd", "v2d", "vnd", "wmf", "emf", "art", "xar", "png", "webp", "jxr", "hdp", "wdp", "cur", "ecw", "iff", "lbm", "liff", "nrrd", "pam", "pcx", "pgf", "sgi", "rgb", "rgba", "bw", "int", "inta", "sid", "ras", "sun", "tga" };
        private static readonly List<string> VIDEO_EXTENSIONS = new List<string>() { "3g2", "3gp", "aaf", "asf", "avchd", "avi", "drc", "flv", "m2v", "m4p", "m4v", "mkv", "mng", "mov", "mp2", "mp4", "mpe", "mpeg", "mpg", "mpv", "mxf", "nsv", "ogg", "ogv", "qt", "rm", "rmvb", "roq", "svi", "vob", "webm", "wmv", "yuv" };

        public static ImageSource GetDriveIcon()
        {
            return DRIVE_ICON;
        }

        public static ImageSource GetDirectoryIcon()
        {
            return DIRECTORY_ICON;
        }

        public static ImageSource GetFileIcon(string path)
        {
            var extension = Path.GetExtension(path);

            if (!string.IsNullOrEmpty(extension))
                extension = extension.Substring(1);

            if (IMAGE_EXTENSIONS.Contains(extension))
                return IMAGE_ICON;

            if (VIDEO_EXTENSIONS.Contains(extension))
                return VIDEO_ICON;

            return FILE_ICON;
        }
    }
}
