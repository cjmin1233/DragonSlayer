using UnityEngine;

public class ChildRandomPositioner : MonoBehaviour
{
    [SerializeField] private float minRadius;
    [SerializeField] private float maxRadius;
    
    private void OnEnable()
    {
        int childCount = transform.childCount;
        for (int i = 0; i < childCount; i++)
        {
            transform.GetChild(i).transform.position = MyUtility.GetRandomPointBet2Circles(transform.position, minRadius, maxRadius);
        }
    }
}
