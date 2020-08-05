using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PublicSite.Client.Components
{
    public class MenuService : IMenuService
    {
        public event Action OnChange;
        public event Action OnOpenAccount;

        private bool showAccountButton;

        public void ShowAccountButton(bool show = true)
        {
            showAccountButton = show;
            OnChange?.Invoke();
        }

        public bool ShouldShowAccountButton()
        {
            return showAccountButton;
        }

        public void OpenAccountModal()
        {
            OnOpenAccount?.Invoke();
        }
    }
}
