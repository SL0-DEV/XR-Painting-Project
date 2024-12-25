using UnityEngine;
using UnityEngine.InputSystem;
public class HandAnimatorController : MonoBehaviour
{
    public InputActionProperty PicnhInput;
    public InputActionProperty GrabInput;
    public float GrabValue = 1;
    private Animator anim;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        float pinch = PicnhInput.action.ReadValue<float>();
        float grab = GrabInput.action.ReadValue<float>();
        anim.SetFloat("Pinch", pinch);
        anim.SetFloat("Grab", grab * GrabValue);
    }
}
