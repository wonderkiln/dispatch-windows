namespace Dispatch.Service.Model
{
    public class ProgressStatus
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

        public ProgressStatus(int index, int count, double progress)
        {
            FileIndex = index;
            FileCount = count;
            Progress = progress;
        }
    }
}
