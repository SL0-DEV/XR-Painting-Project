using UnityEngine;

public class WorldPositionChecker : MonoBehaviour
{
    public float World_Y_pos;
    private Vector3 startPos;
    private Rigidbody rb;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startPos = transform.position;
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if(transform.position.y < World_Y_pos){
            rb.linearVelocity = Vector3.zero;
            transform.position = startPos;
        }
    }
}
