using UnityEngine;

public class HandsManager : MonoBehaviour
{
    public static HandsManager Instance;

    public HandAnimatorController[] Hands;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }


    public void SetGrabValue(float value)
    {
        foreach (var hs in Hands)
        {
            hs.GrabValue = value;
        }

    }
}
