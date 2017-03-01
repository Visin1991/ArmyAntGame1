using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeiPlayerController : MonoBehaviour {

    public float walkspeed = 2;
    public float runSpeed = 6;

    public bool immediateTurnAround = false;
    [Tooltip("the bigger the turnSpeed slower. the smaller the turnSpeed fast")]
    public float turnSmoothTime = 0.2f;
    float turnSmoothVelocity;

    public bool immediateChangeSpeed = false;
    public float speedSmoothTime = 0.1f;
    float speedSmoothVelocity;
    float currentSpeed;

    Animator animator;
    Vector2 input;
    Vector2 inputDir;
    bool running;
    float speed;
    float animationSpeedPercent;

    Transform cameraT;

    void Start () {
        animator = GetComponent<Animator>();
        cameraT = Camera.main.transform;
	}
	
	void Update () {
#if UNITY_XBOXONE
        input = new Vector2(Input.GetAxisRaw("Whatever"),Input.GetAxisRaw("Whatever"));
        running = Input.GetKey(KeyCode.Whatever);
#else
        input = new Vector2(Input.GetAxisRaw("Horizontal"),Input.GetAxisRaw("Vertical"));
        running = Input.GetKey(KeyCode.LeftShift);
#endif
        //Movemnet TurnAround postporcess
        inputDir = input.normalized;
        if (inputDir != Vector2.zero){
            if (immediateTurnAround){
                transform.eulerAngles = Vector3.up * (Mathf.Atan2(inputDir.x, inputDir.y) * Mathf.Rad2Deg + cameraT.eulerAngles.y);
            }else{
                float targetRotation = Mathf.Atan2(inputDir.x, inputDir.y) * Mathf.Rad2Deg + cameraT.eulerAngles.y;
                transform.eulerAngles = Vector3.up * Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation,ref turnSmoothVelocity,turnSmoothTime);
            }
        }
        //Movement movespeed postprocess
        if (immediateChangeSpeed){
            currentSpeed = ((running) ? runSpeed : walkspeed) * inputDir.magnitude;
        }else{
            float targetSpeed = ((running) ? runSpeed : walkspeed) * inputDir.magnitude;
            currentSpeed = Mathf.SmoothDamp(currentSpeed, targetSpeed, ref speedSmoothVelocity, speedSmoothTime);
        }
        
        transform.Translate(transform.forward * currentSpeed * Time.deltaTime, Space.World);

        animationSpeedPercent = ((running) ? (1.0f / runSpeed): (0.5f/walkspeed)) * currentSpeed;

        //Animation Postprocess
        if (immediateChangeSpeed){
            animator.SetFloat("speedPercent", animationSpeedPercent);
        }else{
            animator.SetFloat("speedPercent", animationSpeedPercent,speedSmoothVelocity,Time.deltaTime);
        }
    }
}
