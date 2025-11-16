using TMPro;
using UnityEngine;

namespace StateMachineComponents
{
    public class StateMachineDebug : MonoBehaviour
    {
        private FiniteStateBehaviour _finiteStateBehaviour;
        private TMP_Text _debugText;

        private void Awake()
        {
            _finiteStateBehaviour = GetComponentInParent<FiniteStateBehaviour>();
            _debugText = GetComponentInChildren<TMP_Text>();
            _finiteStateBehaviour.OnEntityStateChanged += FiniteStateBehaviourOnEntityStateChanged;
        }

        private void Start()
        {
            // FiniteStateBehaviourOnEntityStateChanged(_finiteStateBehaviour.CurrentStateType);
        }

        private void FiniteStateBehaviourOnEntityStateChanged(IState state) =>
            _debugText.SetText(state.ToString());

        private void OnDestroy() =>
            _finiteStateBehaviour.OnEntityStateChanged -= FiniteStateBehaviourOnEntityStateChanged;
    }
}