namespace Dispatch.Service.Models
{
    public class ResourceProgress
    {
        public int FileIndex { get; set; }

        public int FileCount { get; set; }

        public double Progress { get; set; }

        public double TotalProgress
        {
            get
            {
                return (Progress + 100 * FileIndex) / FileCount;
            }
        }

        public string CurrentPath { get; set; }

        public ResourceProgress(int index, int count, double progress, string path = null)
        {
            FileIndex = index;
            FileCount = count;
            Progress = progress;
            CurrentPath = path;
        }
    }
}
