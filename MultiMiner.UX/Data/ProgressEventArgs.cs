using System;

namespace MultiMiner.UX.Data
{
    public class ProgressEventArgs : EventArgs
    {
        public string Text;
        public bool IsDownload;
    }
}
