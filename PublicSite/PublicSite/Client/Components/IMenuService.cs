using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PublicSite.Client.Components
{
    public interface IMenuService
    {
        event Action OnChange;
        event Action OnOpenAccount;

        void OpenAccountModal();
        bool ShouldShowAccountButton();
        void ShowAccountButton(bool show = true);
    }
}
