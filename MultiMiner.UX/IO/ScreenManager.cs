using System;
using System.Collections.Generic;
using System.Linq;

namespace MultiMiner.UX.IO
{
    public class ScreenManager
    {
        private readonly Dictionary<string, Action> screenRenderers = new Dictionary<string, Action>();

        private string currentScreen = String.Empty;
        public string CurrentScreen { get { return currentScreen; } }

        public void RegisterScreen(string name, Action renderer)
        {
            screenRenderers[name.ToLower()] = renderer;
            if (String.IsNullOrEmpty(CurrentScreen))
                currentScreen = name.ToLower();
        }

        public bool RenderScreen()
        {
            screenRenderers[CurrentScreen]();
            return true;
        }

        public void AdvanceCurrentScreen()
        {
            if (screenRenderers.Count == 0) return;

            var keyList = screenRenderers.Keys.ToList();
            var index = keyList.IndexOf(currentScreen);
            index++;
            if (index >= keyList.Count) index = 0;
            currentScreen = keyList[index];
            RenderScreen();
        }

        public bool SetCurrentScreen(string name)
        {
            if (!screenRenderers.ContainsKey(name.ToLower())) return false;
            currentScreen = name.ToLower();
            RenderScreen();
            return true;
        }
    }
}
