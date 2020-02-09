using System;
using System.Collections.Generic;
using System.Text;

namespace LilyWhite.Lib.Type
{
    public enum BuildStatus
    {
        Idle,
        Compiling,
        Failed,
        Success
    }
    public delegate void BuildProgressHandler(object sender, BuildProgressEventArgs e);
    public class BuildProgressEventArgs:EventArgs
    {
        public double Progress { get; private set; }
        public string Message { get; private set; }
        public BuildStatus BuildStatus { get; private set; }
        public BuildProgressEventArgs(BuildStatus  status, double progress, string message = "")
        {
            this.BuildStatus = status;
            progress = Normalize(progress);
            this.Progress = progress;
            this.Message = message;
        }

        private static double Normalize(double progress)
        {
            if (progress > 1.0d)
            {
                progress = 1.0d;
            }
            if (progress < 0.0d)
            {
                progress = 0.0d;
            }

            return progress;
        }

        public double GetPercent(int decimals = 1)
        {
            return Math.Round(this.Progress * 100d, decimals);
        }
    }
}
