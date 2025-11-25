using UnityEngine.SceneManagement;

namespace UI
{
    public class WinnerUI : BaseUI
    {
        protected override void Start()
        {
            base.Start();
            EnemyManager.OnAllSharksCaptured += EnemyManagerOnAllSharksCaptured;
        }

        private void OnDestroy()
        {
            EnemyManager.OnAllSharksCaptured -= EnemyManagerOnAllSharksCaptured;
        }

        private void EnemyManagerOnAllSharksCaptured() => Invoke(nameof(ShowGameOver), 2f);
        private void ShowGameOver() => Show();
        protected override void LeftButtonOnClick() { }

        protected override void MidButtonOnClick()
        {
            Hide(() =>
            {
                TransitionManager.Instance.Show(() =>
                {
                    SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                });
            });
        }

        protected override void RightButtonOnClick() { }
    }
}