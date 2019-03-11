using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {

    public float forwardmovementSpeed = 5f;
    public float backwardmovementSpeed = 5f;
    public float rotationSpeed = 3f;
    public float jumpSpeed = 2f;
    public float distToGround;

    float forwardInput, turnInput;
    Quaternion targetRotation;

    Rigidbody rb;

    bool isGrounded=true;
    public Animator playerAnimator;

    protected Joystick joystick;
      
    public Quaternion TargetRotation
    {
        get { return targetRotation; }
    }

    void Start()
    {
        targetRotation = transform.rotation;
        if (GetComponent<Rigidbody>())
        {
            rb = GetComponent<Rigidbody>();
        }
        else
        {
            Debug.LogError("RigidBody Not Attached");
        }
        forwardInput = turnInput = 0;
        distToGround= distToGround = GetComponent<CapsuleCollider>().bounds.extents.y;
        joystick = FindObjectOfType<Joystick>();
    }

    void Update()
    {
        GetInput();
        Turn();
    }

    void FixedUpdate()
    {
        Move();
    }



    

    public void Jump()
    {
        PlayerJump();
    }

    void GetInput()
    {
        turnInput = joystick.Horizontal;
        forwardInput = joystick.Vertical;
    }

    void Turn()
    {
        if(Mathf.Abs(turnInput)>0)
        {
            targetRotation *= Quaternion.AngleAxis(rotationSpeed * turnInput * Time.deltaTime, Vector3.up);
            transform.rotation = targetRotation;
        }
      
    }

    void Move()
    {
        

            if (forwardInput > 0.7f)
            {
                transform.Translate(Vector3.forward * Time.deltaTime * forwardInput * forwardmovementSpeed);
                playerAnimator.Play("Move");
            }
            else if (forwardInput < -0.7f)
            {
                transform.Translate(Vector3.forward * Time.deltaTime * -backwardmovementSpeed);
                playerAnimator.Play("Move");
            }

            if (turnInput == 0 && forwardInput == 0)
            {
                playerAnimator.Play("Idle");

            }
      
    }
    

    void PlayerJump()
    {
        if ( isGroundedCheck())
        {
            rb.velocity = Vector3.up * jumpSpeed;
            SoundManager.instance.PlaySound(SoundManager.instance.allClips[3], false);

        }
        
    }

    bool isGroundedCheck()
    {
        return Physics.Raycast(transform.position, -Vector3.up, distToGround + 0.1f);
    }



    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "coins")
        {
            Destroy(col.gameObject);
            PlayerScore.instance.score = PlayerScore.instance.score + 1;
            SoundManager.instance.PlaySound(SoundManager.instance.allClips[1], false);
        }
        else if (col.gameObject.tag == "hurdle")
        {
            UIManager.instance.GameOver_Finish(); 
        }
    }

    
    void OnCollisionEnter(Collision col)
    {
        if(col.gameObject.tag=="hurdle")
        {

            GetComponent<PlayerHealth>().TakeDamage(20);
            SoundManager.instance.PlaySound(SoundManager.instance.allClips[1], false);
        }
    }

    void OnTriggerExit(Collider col)
    {
        if (col.gameObject.tag == "Door")
        {
            UIManager.instance.GameWin_Finish();
        }
    }
   
}
