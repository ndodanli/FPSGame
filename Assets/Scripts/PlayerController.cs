using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerMotor))]
[RequireComponent(typeof(ConfigurableJoint))]
[RequireComponent(typeof(Animator))]
public class PlayerController : MonoBehaviour
{
    [SerializeField]    //show up in inspector even if it is private
    private float speed = 5f;
    [SerializeField]    //show up in inspector even if it is private
    private float lookSensitivity = 3f;
    private PlayerMotor motor;
    [SerializeField]
    private float thrusterForce = 2000f;
    [SerializeField]
    private float thrusterFuelBurnSpeed = 1f;
    [SerializeField]
    private float thrusterFuelRegenSpeed = 0.3f;
    private float thrusterFuelAmount = 1f;
    private ConfigurableJoint joint;

    [SerializeField]
    private LayerMask environmentMask;

    [Header("Spring Settings:")]
    [SerializeField]
    private float jointSpring = 30;
    [SerializeField]
    private float jointMaxForce = 40f;
    private Animator animator;

    public float GetThrusterAmount()
    {
        return thrusterFuelAmount;
    }
    private void Start()
    {
        motor = GetComponent<PlayerMotor>();
        joint = GetComponent<ConfigurableJoint>();
        animator = GetComponent<Animator>();
        SetJointSettings(jointSpring);
    }
    void Update()
    {
        //setting target position for spring. this makes the physics acts right when i comes to appliying gravity when fliying
        //over objects
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 100f, environmentMask))
        {
            joint.targetPosition = new Vector3(0, -hit.point.y, 0f);
        }
        else
        {
            joint.targetPosition = new Vector3(0, -0, 0f);
        }
        //calculate our movement as a 3D vector
        float XMov = Input.GetAxis("Horizontal");    //-1 and 1
        float ZMov = Input.GetAxis("Vertical");  //-1 and 1

        Vector3 movHorizontal = transform.right * XMov;
        Vector3 movVertical = transform.forward * ZMov;
        //final movement vector
        Vector3 velocity = (movHorizontal + movVertical).normalized * speed;

        //Animate movement
        if (ZMov != 0.0f && XMov == 0.0f) 
        {
            animator.SetFloat("ForwardVelocity", ZMov);

        }
        else if (XMov != 0.0f && ZMov != 0.0f)
        {
            animator.SetFloat("ForwardVelocity", 4f);
            if (ZMov == 1.0f) XMov++;
            animator.SetFloat("ForwardSideVelocity", XMov + ZMov);

        }
        else
        {
            animator.SetFloat("ForwardVelocity", 3f);
            animator.SetFloat("SideVelocity", XMov);
        }

        //apply movement
        motor.Move(velocity);

        //calculate rotation as a 3D vector(turning around)
        float yRot = Input.GetAxisRaw("Mouse X");
        Vector3 playerRotation = new Vector3(0f, yRot, 0f) * lookSensitivity;

        

        //calculate camera rotation as a 3D vector(turning around)
        float xRot = Input.GetAxisRaw("Mouse Y");
        float cameraRotationX = xRot * lookSensitivity;

        //apply rotation
        motor.Rotate(playerRotation, cameraRotationX);

        //calculate the thrusterforce based on player input
        Vector3 _thrusterForce = Vector3.zero;
        if (Input.GetButton("Jump") && thrusterFuelAmount > 0f)
        {
            thrusterFuelAmount -= thrusterFuelBurnSpeed * Time.deltaTime;
            if (thrusterFuelAmount >= 0.01f)
            {
                _thrusterForce = Vector3.up * thrusterForce;
                SetJointSettings(0f);
            }
            
        }
        else
        {
            thrusterFuelAmount += thrusterFuelRegenSpeed * Time.deltaTime;
            SetJointSettings(jointSpring);
        }
        thrusterFuelAmount =  Mathf.Clamp(thrusterFuelAmount, 0f, 1f);
        //apply the thruster force
        motor.ApplyThruster(_thrusterForce);
    }

    private void SetJointSettings(float _jointSpring)
    {
        joint.yDrive = new JointDrive
        {
            positionSpring = _jointSpring,
            maximumForce = jointMaxForce
        };
    }
}
