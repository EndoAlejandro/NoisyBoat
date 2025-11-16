using UnityEngine;

namespace Enemies
{
    public class Node : MonoBehaviour
    {
        [SerializeField] private Node _node;

        private Transform _root;
        private float _maxDistance;
        private float _speed;

        public void Setup(Transform root, float maxDistance, float speed)
        {
            _root = root;
            _maxDistance = maxDistance;
            _speed = speed;

            transform.SetParent(null);

            _node?.Setup(transform, _maxDistance, _speed);
        }

        private void Update()
        {
            if (Vector3.Distance(_root.position, transform.position) < _maxDistance) return;

            var direction = _root.position - transform.position;
            transform.position = Vector3.MoveTowards(transform.position, transform.position + direction.normalized * _maxDistance, Time.deltaTime * _speed);
        }

        public void Move(Vector3 target) => 
            transform.position = Vector3.MoveTowards(transform.position, target, Time.deltaTime * _speed);
    }
}