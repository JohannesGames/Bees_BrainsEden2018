using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class MovementPC : MonoBehaviour
{
    [SerializeField] float movementSpeed;
    [SerializeField] float currentMovementSpeed;
    [SerializeField] float turnSpeed;
    [Tooltip("How fast an angle of direction resets when the button is released")]
    [SerializeField] float resetAngleSpeed;
    [SerializeField] float acceleration;
    [SerializeField] float outOfBoundsResetSpeed;
    Vector3 moveDirection;
    Vector3 unreliableVector;
    Vector3 unreliableDirection;
    Vector3 nextPosition;
    [HideInInspector]
    public Vector3 lerpTargetOOB = Vector3.zero;

    // Input & move direction
    [HideInInspector]
    public bool up;
    [HideInInspector]
    public bool left;
    [HideInInspector]
    public bool right;
    [HideInInspector]
    public bool down;
    float movementDot;

    [Header("Unreliable movement")]
    public float unreliabilityRate;
    public float addNewUnreliableVectorEvery;
    float unreliableTime;
    public float unreliabilityAmount;
    public float unreliabilityAmountMax;
    CinemachineBasicMultiChannelPerlin camNoiseComponent;
    [Header("Camera")]
    public float camNoiseValue;
    public float camNoiseMaxValue;
    public float camNoiseCurrentValue;
    public float camNoiseUnreliabilityRate;

    void Start()
    {
        moveDirection = transform.TransformDirection(Vector3.forward);
        camNoiseComponent = GameManager.gm.playerCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        camNoiseValue = camNoiseCurrentValue = camNoiseComponent.m_AmplitudeGain;
        currentMovementSpeed = movementSpeed;
    }


    void Update()
    {
        InputManager();
        TranslationPC();
    }

    void InputManager()
    {
        if (!GameManager.gm.isOutOfBounds)
        {
            #region Take Player Input
            // W
            if (Input.GetKeyDown(KeyCode.W))
            {
                up = true;
                down = false;
            }
            else if (Input.GetKeyUp(KeyCode.W))
            {
                up = false;
            }

            // S
            if (Input.GetKeyDown(KeyCode.S))
            {
                down = true;
                up = false;
            }
            else if (Input.GetKeyUp(KeyCode.S))
            {
                down = false;
            }

            // A
            if (Input.GetKeyDown(KeyCode.A))
            {
                left = true;
                right = false;
            }
            else if (Input.GetKeyUp(KeyCode.A))
            {
                left = false;
            }

            // D
            if (Input.GetKeyDown(KeyCode.D))
            {
                right = true;
                left = false;
            }
            else if (Input.GetKeyUp(KeyCode.D))
            {
                right = false;
            }
            #endregion
        }

        ProcessDirectionInput();
    }

    void ProcessDirectionInput()
    {
        // if some input
        if (down)
        {
            // ensure doesn't go too far down
            movementDot = Vector3.Dot(Vector3.down, moveDirection);
            if (movementDot < .5f)
            {
                moveDirection += Vector3.down * Time.deltaTime * (GameManager.gm.isOutOfBounds ? outOfBoundsResetSpeed : turnSpeed);
            }
        }
        else
        {
            // reset angle
            movementDot = Vector3.Dot(Vector3.down, moveDirection);
            if (movementDot > 0)
            {
                moveDirection += Vector3.up * Time.deltaTime * resetAngleSpeed;
            }
        }

        if (up)
        {
            // ensure doesn't go too far up
            movementDot = Vector3.Dot(Vector3.up, moveDirection);
            if (movementDot < .5f)
            {
                moveDirection += Vector3.up * Time.deltaTime * (GameManager.gm.isOutOfBounds ? outOfBoundsResetSpeed : turnSpeed);
            }
        }
        else
        {
            // reset angle
            movementDot = Vector3.Dot(Vector3.up, moveDirection);
            if (movementDot > 0)
            {
                moveDirection += Vector3.down * Time.deltaTime * resetAngleSpeed;
            }
        }

        if (left)
        {
            // ensure doesn't go too far left
            movementDot = Vector3.Dot(Vector3.left, moveDirection);
            if (movementDot < .707f)
            {
                moveDirection += Vector3.left * Time.deltaTime * (GameManager.gm.isOutOfBounds ? outOfBoundsResetSpeed : turnSpeed);
            }
        }
        else
        {
            // reset angle
            movementDot = Vector3.Dot(Vector3.left, moveDirection);
            if (movementDot > 0)
            {
                moveDirection += Vector3.right * Time.deltaTime * resetAngleSpeed;
            }
        }

        if (right)
        {
            // ensure doesn't go too far left
            movementDot = Vector3.Dot(Vector3.right, moveDirection);
            if (movementDot < .707f)
            {
                moveDirection += Vector3.right * Time.deltaTime * (GameManager.gm.isOutOfBounds ? outOfBoundsResetSpeed : turnSpeed);
            }
        }
        else
        {
            // reset angle
            movementDot = Vector3.Dot(Vector3.right, moveDirection);
            if (movementDot > 0)
            {
                moveDirection += Vector3.left * Time.deltaTime * resetAngleSpeed;
            }
        }
    }

    void TranslationPC()
    {
        AddUnreliableMovement();

        moveDirection.Normalize();

        nextPosition = transform.position + (moveDirection * Time.deltaTime * currentMovementSpeed);   // normal movement

        currentMovementSpeed += acceleration * Time.deltaTime;

        transform.position = nextPosition;
    }

    void AddUnreliableMovement()
    {
        // add unreliable movement
        if (unreliabilityRate > 0)
        {
            if (!GameManager.gm.isOutOfBounds)
            {
                if (unreliabilityAmount < unreliabilityAmountMax)
                {
                    unreliabilityAmount += Time.deltaTime * unreliabilityRate;

                    // Camera noise
                    if (camNoiseCurrentValue < camNoiseMaxValue)
                    {
                        camNoiseCurrentValue += Time.deltaTime * camNoiseUnreliabilityRate;
                    }
                    else
                    {
                        camNoiseCurrentValue = camNoiseMaxValue;
                    }
                    camNoiseComponent.m_AmplitudeGain = camNoiseCurrentValue;
                }
                else
                {
                    unreliabilityAmount = unreliabilityAmountMax;
                }

                if (Time.time >= unreliableTime)
                {
                    unreliableTime = Time.time + addNewUnreliableVectorEvery;
                    unreliableVector = Vector3.left * Random.Range(-.5f, .5f) + Vector3.up * Random.Range(-.5f, .5f);
                }
                unreliableDirection = unreliableVector * unreliabilityAmount;
                moveDirection += unreliableDirection;
            }
            //else
            //{
            //    unreliableDirection = Vector3.forward;
            //    unreliableTime = Time.time + addNewUnreliableVectorEvery;
            //    unreliabilityAmount *= .9f;
            //}
        }
    }

    #region Out of Bounds Return Coroutines

    public IEnumerator OutOfBoundsReturnTop()
    {
        while (transform.position.y > GameManager.gm.boundaryTop)
        {
            yield return null;
        }
        down = false;
        GameManager.gm.isOutOfBounds = false;
    }

    public IEnumerator OutOfBoundsReturnBottom()
    {
        while (transform.position.y < GameManager.gm.boundaryBottom)
        {
            yield return null;
        }
        up = false;
        GameManager.gm.isOutOfBounds = false;
    }

    public IEnumerator OutOfBoundsReturnLeft()
    {
        while (transform.position.x < GameManager.gm.boundaryLeft)
        {
            yield return null;
        }
        right = false;
        GameManager.gm.isOutOfBounds = false;
    }

    public IEnumerator OutOfBoundsReturnRight()
    {
        while (transform.position.x > GameManager.gm.boundaryRight)
        {
            yield return null;
        }
        left = false;
        GameManager.gm.isOutOfBounds = false;
    }

    #endregion
}
