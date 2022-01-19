using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public event System.Action OnReachedEndOfLevel;
    public bool hasReachedEndOfLevel = false;


    public float moveSpeed = 7;
    public float smoothMoveTime = 0.1f;
    public float turnSpeed = 8;
    float angle;
    float smoothInputMag;
    float smoothMoveVel;
    Rigidbody rb;
    Vector3 velo;
    bool disabled;

    private void Start() {
        rb = GetComponent<Rigidbody>();
        Guard.OnGuardHasSpottedPlayer += Disable;
        
    }
    

    // Update is called once per frame
    void Update()
    {
        Vector3 inputDirection = Vector3.zero;
        if (!disabled)
        {
            inputDirection = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;
        }
         
        float inputMag = inputDirection.magnitude;
        smoothInputMag = Mathf.SmoothDamp(smoothInputMag, inputMag, ref smoothMoveVel, smoothMoveTime);

       float targetAngle = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg;
       angle = Mathf.LerpAngle(angle, targetAngle, Time.deltaTime * turnSpeed * inputMag);
    
       velo = transform.forward * moveSpeed * smoothInputMag;
    }

    private void OnTriggerEnter(Collider hitCollider)
    {
        if(hitCollider.tag == "Finish")
        {
            Disable();
            if(OnReachedEndOfLevel != null)
            {
                OnReachedEndOfLevel();
            }

            hasReachedEndOfLevel = true;
        }
    }
    void Disable()
    {
        disabled = true;
    }
    private void FixedUpdate() {
        rb.MoveRotation(Quaternion.Euler(Vector3.up * angle));
        rb.MovePosition(rb.position + velo * Time.deltaTime);
    }

    private void OnDestroy()
    {
        Guard.OnGuardHasSpottedPlayer -= Disable;
    }
}
