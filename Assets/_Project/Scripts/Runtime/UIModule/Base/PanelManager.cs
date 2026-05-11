using System.Collections.Generic;
using Zenject;

namespace GoldenSpin.UIModule
{
    public class PanelManager : IInitializable
    {
        private readonly List<BasePanel> activePanels = new List<BasePanel>();

        public void Initialize()
        {
        }

        public void ShowPanel(BasePanel panel)
        {
            panel.Show();

            if (!activePanels.Contains(panel))
            {
                activePanels.Add(panel);
            }
        }

        public void HidePanel(BasePanel panel)
        {
            panel.Hide();
            activePanels.Remove(panel);
        }

        public void HideAllPanels()
        {
            foreach (BasePanel panel in activePanels)
            {
                panel.Hide();
            }

            activePanels.Clear();
        }
    }
}
