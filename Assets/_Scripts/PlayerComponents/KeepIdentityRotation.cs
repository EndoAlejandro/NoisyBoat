using UnityEngine;

namespace PlayerComponents
{
    public class KeepIdentityRotation : MonoBehaviour
    {
        private void Update()
        {
            transform.rotation = Quaternion.identity;
        }
    }
}