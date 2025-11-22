using PlayerComponents;
using UnityEngine;

namespace UI
{
    public class MainMenuUI : BaseUI
    {
        [SerializeField] private Player _playerPrefab;
        [SerializeField] private Transform _spawnPoint;

        // Credits
        protected override void LeftButtonOnClick() { }

        // Play
        protected override void MidButtonOnClick() => Hide(InstantiatePlayer);

        // Tutorial
        protected override void RightButtonOnClick() { }

        private void InstantiatePlayer() => Instantiate(_playerPrefab, _spawnPoint.position, Quaternion.identity);
    }
}