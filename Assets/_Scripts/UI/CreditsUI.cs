using UnityEngine;

namespace UI
{
    public class CreditsUI : BaseUI
    {
        [SerializeField] private BaseUI _mainMenu;

        protected override void LeftButtonOnClick() { }

        protected override void MidButtonOnClick() => Hide(() => _mainMenu.Show());

        protected override void RightButtonOnClick() { }
    }
}