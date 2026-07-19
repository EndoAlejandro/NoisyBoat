using System;
using PlayerComponents;
using UnityEngine;

namespace UI
{
    public class MainMenuUI : BaseUI
    {
        public static event Action GameStarted;
        
        [Header("Main Menu")]
        [SerializeField] private Player _playerPrefab;

        [SerializeField] private Transform _spawnPoint;
        [SerializeField] private BaseUI _credits;
        [SerializeField] private BaseUI _tutorial;

        // Credits
        protected override void LeftButtonOnClick() => Hide(() => _credits.Show());

        // Play
        protected override void MidButtonOnClick() => Hide(InstantiatePlayer);

        // Tutorial
        protected override void RightButtonOnClick() => Hide(() => _tutorial.Show());

        private void InstantiatePlayer()
        {
            Instantiate(_playerPrefab, _spawnPoint.position, Quaternion.identity);
            GameStarted?.Invoke();
        }
    }
}