using System;

namespace Target.Utility.ViewModels
{
    public class WorkerReportViewModel
    {
        public bool Success { get; set; }
        public string ImageFileName { get; set; }
        public TimeSpan Time { get; set; }
    }
}
