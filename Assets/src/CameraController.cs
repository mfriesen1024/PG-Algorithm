using UnityEngine;

namespace Assets
{
    [RequireComponent(typeof(Camera))]
    internal class CameraController : MonoBehaviour
    {
        private void Update()
        {
            float delta = Time.deltaTime;
            if (Input.GetKey(KeyCode.W)) { Move(Vector3.forward); }
            if (Input.GetKey(KeyCode.A)) { Move(Vector3.left); }
            if (Input.GetKey(KeyCode.S)) { Move(Vector3.back); }
            if (Input.GetKey(KeyCode.D)) { Move(Vector3.right); }

            void Move(Vector3 direction)
            {
                Vector3 newPos = transform.position + direction*delta*5;
                transform.position = newPos;
            }
        }
    }
}
