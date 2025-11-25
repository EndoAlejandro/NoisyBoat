using UnityEngine;

namespace UI
{
    public class TutorialUI : BaseUI
    {
        [Header("Tutorial")]
        [SerializeField] private BaseUI _mainMenu;

        [SerializeField] private GameObject[] _tutorialSlides;

        private int _index;

        private void OnEnable() => _index = 0;

        private void UpdatePage()
        {
            for (int i = 0; i < _tutorialSlides.Length; i++)
            {
                _tutorialSlides[i].SetActive(i == _index);
            }

            _leftButton.interactable = _index > 0;
            _rightButton.interactable = _index < _tutorialSlides.Length - 1;
        }

        protected override void LeftButtonOnClick()
        {
            if (_index > 0) _index--;
            UpdatePage();
        }

        protected override void MidButtonOnClick() => Hide(() => _mainMenu.Show());

        protected override void RightButtonOnClick()
        {
            if (_index < _tutorialSlides.Length - 1) _index++;
            UpdatePage();
        }
    }
}