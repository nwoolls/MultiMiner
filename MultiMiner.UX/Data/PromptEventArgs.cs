using System;

namespace MultiMiner.UX.Data
{
    public class PromptEventArgs : EventArgs
    {
        public string Caption { get; set; }
        public string Text { get; set; }
        public PromptButtons Buttons { get; set; }
        public PromptIcon Icon { get; set; }
        public PromptResult Result { get; set; }
    }
}
