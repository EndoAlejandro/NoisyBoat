using UnityEngine;

namespace Enemies
{
    public class SharkNode
    {
        public Vector3 Position { get; private set; }

        private SharkNode _head;
        private float _maxDistance;
        private float _speed;

        public SharkNode(Vector3 position, float maxDistance, float speed)
        {
            Position = position;
            _maxDistance = maxDistance;
            _speed = speed;
        }

        public void Tick()
        {
            if (_head is null) return;
            if (Vector3.Distance(_head.Position, Position) < _maxDistance) return;

            var direction = Position - _head.Position;
            Position = Vector3.MoveTowards(Position, _head.Position + direction.normalized * _maxDistance, Time.deltaTime * _speed);
        }

        public void SetHead(SharkNode head) => _head = head;
        public void SetPosition(Vector3 position) => Position = position;
        public void SetMaxDistance(float maxDistance) => _maxDistance = maxDistance;
        public void SetSpeed(float speed) => _speed = speed;
    }
}