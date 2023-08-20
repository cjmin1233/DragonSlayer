using UnityEngine;

public class Rotator : MonoBehaviour
{
    [SerializeField] private float rotateSpeed;
    [SerializeField] private Vector3 rotateAxis;
    private void Update()
    {
        transform.Rotate(rotateAxis, rotateSpeed * Time.deltaTime);
    }
}
