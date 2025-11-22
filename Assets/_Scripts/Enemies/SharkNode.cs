using UnityEngine;

namespace Enemies
{
    public class SharkNode
    {
        public Vector3 Position { get; private set; }

        private readonly SphereCollider _collider;

        private SharkNode _head;
        private float _maxDistance;
        private float _speed;

        public SharkNode(Vector3 position, float maxDistance, float speed, SphereCollider collider)
        {
            Position = position;
            _maxDistance = maxDistance;
            _speed = speed;
            _collider = collider;
        }

        public void Tick()
        {
            _collider.transform.position = Position;
            
            if (_head is null) return;
            if (Vector3.Distance(_head.Position, Position) < _maxDistance) return;

            var direction = Position - _head.Position;
            Position = Vector3.MoveTowards(Position, _head.Position + direction.normalized * _maxDistance, Time.deltaTime * _speed);
        }

        public void SetHead(SharkNode head) => _head = head;

        public void SetPosition(Vector3 position) => Position = position;

        public void SetMaxDistance(float maxDistance)
        {
            _maxDistance = maxDistance;
            _collider.radius = _maxDistance;
        }

        public void SetSpeed(float speed) => _speed = speed;
    }
}