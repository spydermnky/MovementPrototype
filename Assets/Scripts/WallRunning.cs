using UnityEngine;

public class WallRunning : MonoBehaviour
{
    [Header("Wallrunning Params")]
    public LayerMask whatIsWall;
    public LayerMask whatIsGround;
    public float wallRunForce = 200f;
    public float wallStickForce = 60f;
    public float maxWallRunTime;
    private float wallRunTimer;
    public float wallClimbSpeed;
    public float wallJumpUpForce;
    public float wallJumpSideForce;

    [Header("Input Params")]
    private float horizontalInput;
    private float verticalInput;
    public KeyCode upwardsRunKey = KeyCode.LeftShift;
    public KeyCode downwardsRunKey = KeyCode.LeftControl;
    private bool upwardsRunning;
    private bool downwardsRunning;
    public KeyCode jumpKey = KeyCode.Space;

    [Header("Detection Params")]
    public float wallCheckDistance;
    public float minJumpHeight;
    private RaycastHit leftWallHit;
    private RaycastHit rightWallHit;
    private bool wallLeft;
    private bool wallRight;

    [Header("Leaving Params")]
    private bool leavingWall;
    public float leaveWallTime;
    private float leaveWallTimer;

    [Header("References Params")]
    public Transform orientation;
    private PlayerMovement pm;
    private Rigidbody rb;

    public float maxWallRunSpeed = 10f;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        pm = GetComponent<PlayerMovement>();
    }

    private void Update()
    {
        CheckForWall();
        StateMachine();
    }

    private void FixedUpdate()
    {
        if (pm.wallrunning)
        {
            WallRunningMovement();
        }
    }

    private void CheckForWall()
    {
        wallRight = Physics.Raycast(transform.position, orientation.right, out rightWallHit, wallCheckDistance, whatIsWall);
        wallLeft = Physics.Raycast(transform.position, -orientation.right, out leftWallHit, wallCheckDistance, whatIsWall);
    }

    private bool AboveGround()
    {
        return !Physics.Raycast(transform.position, Vector3.down, minJumpHeight, whatIsGround);
    }

    private void StateMachine()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        upwardsRunning = Input.GetKey(upwardsRunKey);
        downwardsRunning = Input.GetKey(downwardsRunKey);

        if ((wallLeft || wallRight) && verticalInput > 0 && AboveGround() && !leavingWall)
        {
            if (!pm.wallrunning)
            {
                StartWallRun();
            }
            else if (wallRunTimer <= 0)
            {
                leavingWall = true;
                leaveWallTimer = leaveWallTime;
            }
            if (Input.GetKeyDown(jumpKey)) WallJump();
        }
        else if (leavingWall)
        {
            if (pm.wallrunning)
            {
                StopWallRun();
            }
            if (leaveWallTimer > 0)
            {
                leaveWallTimer -= Time.deltaTime;
            }
            if (leaveWallTimer <= 0)
            {
                leavingWall = false;
            }
        }
        else if (pm.wallrunning)
        {
            StopWallRun();
        }
    }

    private void StartWallRun()
    {
        pm.wallrunning = true;
        wallRunTimer = maxWallRunTime;
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
    }

    private void WallRunningMovement()
    {
        rb.useGravity = false;

        rb.AddForce(Vector3.down * 20f, ForceMode.Force);

        Vector3 wallNormal = wallRight ? rightWallHit.normal : leftWallHit.normal;
        Vector3 wallForward = Vector3.Cross(wallNormal, transform.up);

        if ((orientation.forward - wallForward).magnitude > (orientation.forward - -wallForward).magnitude)
        {
            wallForward = -wallForward;
        }
        
        rb.AddForce(wallForward * wallRunForce, ForceMode.Force);

        if (upwardsRunning)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, wallClimbSpeed, rb.linearVelocity.z);
        }

        if (downwardsRunning)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, -wallClimbSpeed, rb.linearVelocity.z);
        }
        else
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, rb.linearVelocity.y * 0.94f, rb.linearVelocity.z);
        }

        if (!(wallLeft && horizontalInput > 0) && !(wallRight && horizontalInput < 0))
        {
            rb.AddForce(-wallNormal * wallStickForce, ForceMode.Force);
        }

        RestrictWallRunSpeed();
    }

    private void StopWallRun()
    {
        pm.wallrunning = false;
        rb.useGravity = true;
    }

    private void WallJump()
    {
        leavingWall = true;
        leaveWallTimer = leaveWallTime;

        Vector3 wallNormal = wallRight ? rightWallHit.normal : leftWallHit.normal;
        Vector3 forceToApply = transform.up * wallJumpUpForce + wallNormal * wallJumpSideForce;

        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        rb.AddForce(forceToApply, ForceMode.Impulse);
    }

    private void RestrictWallRunSpeed()
    {
        Vector3 flatVel = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

        if (flatVel.magnitude > maxWallRunSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * maxWallRunSpeed;
            rb.linearVelocity = new Vector3(limitedVel.x, rb.linearVelocity.y, limitedVel.z);
        }
    }
}

