using System;
using PlayerComponents;
using UnityEngine.SceneManagement;

namespace UI
{
    public class GameOverUI : BaseUI
    {
        protected override void Start()
        {
            Player.OnDead += PlayerOnDead;
            base.Start();
        }

        private void OnDestroy() => Player.OnDead -= PlayerOnDead;

        private void PlayerOnDead() => Invoke(nameof(ShowGameOver), 2f);

        private void ShowGameOver() => Show();

        protected override void LeftButtonOnClick() { }

        protected override void MidButtonOnClick() =>
            Hide(() =>
            {
                TransitionManager.Instance.Show(() =>
                {
                    SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                });
            });

        protected override void RightButtonOnClick() { }
    }
}