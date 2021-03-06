﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class MovementPC : MonoBehaviour
{
    public Animator anim;
    [SerializeField] Transform groundPlane;

    [Header("Basic movement")]
    [SerializeField] float movementSpeed;
    [SerializeField] float currentMovementSpeed;
    float movementSpeedLerpValue;
    [SerializeField] float turnSpeed;
    [Tooltip("How fast an angle of direction resets when the button is released")]
    [SerializeField] float resetAngleSpeed;
    [SerializeField] float acceleration;
    [SerializeField] float outOfBoundsResetSpeed;
    Vector3 moveDirection;
    Vector3 unreliableVectorLeft;
    Vector3 unreliableVectorUp;
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
    public float addNewUnreliableVectorOver;
    float unreliableTime;
    float unreliableProgress;
    public float unreliabilityAmountCurrent;
    public float unreliabilityAmount;
    float unreliabilityAmountLerpFromValue;
    public float unreliabilityAmountMax;
    CinemachineBasicMultiChannelPerlin camNoiseComponent;
    [Header("Camera Noise")]
    public float camNoiseValue;
    float camNoiseValueNext;
    public float camNoiseMaxValue;
    public float camNoiseCurrentValue;
    public float camNoiseUnreliabilityRate;
    float camNoiseLerpFromValue;
    [Header("Camera FOV")]
    public float camMaxFOV;
    float camFOVNext;
    public float currentCamFOV;
    float camFOVLerpValue;
    float camFOVNormal;

    [Header("Nectar Pickup")]
    [SerializeField] float nectarResetLength;
    [SerializeField] AnimationCurve resetCurve;
    float nectarResetTimer;
    float nectarResetProgress;
    bool isNectarResetting;

    // score
    float gameTimer = 0;

    void Start()
    {
        moveDirection = transform.TransformDirection(Vector3.forward);
        camNoiseComponent = GameManager.gm.playerCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        currentCamFOV = camFOVNormal = GameManager.gm.playerCam.m_Lens.FieldOfView;
        camNoiseValue = camNoiseCurrentValue = camNoiseComponent.m_AmplitudeGain;
        currentMovementSpeed = movementSpeed;
    }


    void Update()
    {
        if (GameManager.gm.gameState == GameState.InGame)
        {
            UpdateScore();
            InputManager();
            TranslationPC();
        }
        else
        {
            gameTimer = 0;
        }
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

    void UpdateScore()
    {
        gameTimer += Time.deltaTime;

        if (gameTimer >= 1)
        {
            GameManager.gm.scoreValue += 10;
            GameManager.gm.scoreText.text = GameManager.gm.scoreValue.ToString();
            gameTimer = 0;
        }
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

        #region Movement Animation
        // Movement animations
        if (!up && !down && !left && !right)
        {
            // if no input
            // reset up
            if (Mathf.Abs(anim.GetFloat("up")) < .001f)
            {
                anim.SetFloat("up", 0);
            }
            else
                anim.SetFloat("up", Mathf.Lerp(anim.GetFloat("up"), 0, Time.deltaTime * turnSpeed));

            // reset left
            if (Mathf.Abs(anim.GetFloat("left")) < .001f)
            {
                anim.SetFloat("left", 0);
            }
            else
                anim.SetFloat("left", Mathf.Lerp(anim.GetFloat("left"), 0, Time.deltaTime * turnSpeed));
        }

        // up and down
        if (up)
        {
            if (anim.GetFloat("up") > .999f)
            {
                anim.SetFloat("up", 1);
            }
            else
                anim.SetFloat("up", Mathf.Lerp(anim.GetFloat("up"), 1, Time.deltaTime * turnSpeed));
        }
        else if (down)
        {
            if (anim.GetFloat("up") < -.999f)
            {
                anim.SetFloat("up", -1);
            }
            else
                anim.SetFloat("up", Mathf.Lerp(anim.GetFloat("up"), -1, Time.deltaTime * turnSpeed));
        }

        // left and right
        if (left)
        {
            if (anim.GetFloat("left") > .999f)
            {
                anim.SetFloat("left", 1);
            }
            else
                anim.SetFloat("left", Mathf.Lerp(anim.GetFloat("left"), 1, Time.deltaTime * turnSpeed));
        }
        else if (right)
        {
            if (anim.GetFloat("left") < -.999f)
            {
                anim.SetFloat("left", -1);
            }
            else
                anim.SetFloat("left", Mathf.Lerp(anim.GetFloat("left"), -1, Time.deltaTime * turnSpeed));
        }

        #endregion
    }

    void TranslationPC()
    {
        AddUnreliableMovement();

        moveDirection.Normalize();

        nextPosition = transform.position + (moveDirection * Time.deltaTime * currentMovementSpeed);   // normal movement

        currentMovementSpeed += acceleration * Time.deltaTime;

        transform.position = nextPosition;

        nextPosition.y = nextPosition.x = 0;
        groundPlane.position = nextPosition;
    }

    void AddUnreliableMovement()
    {
        // add unreliable movement
        if (unreliabilityRate > 0)
        {
            if (!GameManager.gm.isOutOfBounds)
            {
                CamNoise();
                if (unreliableTime == 0)
                {
                    if (unreliabilityAmount < unreliabilityAmountMax)
                    {
                        unreliabilityAmount += Time.deltaTime * unreliabilityRate;
                    }
                    else
                    {
                        unreliabilityAmount = unreliabilityAmountMax;
                    }
                    unreliableVectorLeft = Vector3.left * Random.Range(.25f, .5f);
                    int ran = Random.Range(0, 2);
                    if (ran == 1)
                    {
                        unreliableVectorLeft *= -1;
                    }
                    unreliableVectorUp = Vector3.up * Random.Range(.25f, .5f);
                    ran = Random.Range(0, 2);
                    if (ran == 1)
                    {
                        unreliableVectorUp *= -1;
                    }
                    unreliableDirection = unreliableVectorUp + unreliableVectorLeft;
                }

                unreliableTime += Time.deltaTime;

                if (unreliableProgress < 1)
                {
                    unreliableProgress = unreliableTime / addNewUnreliableVectorOver;
                    unreliabilityAmountCurrent = Mathf.Lerp(0, unreliabilityAmount, unreliableProgress);
                }
                else
                {
                    unreliableTime = unreliableProgress = 0;
                }

                moveDirection += unreliableDirection * unreliabilityAmount;
            }
            else
            {
                unreliableTime = unreliableProgress = 0;
            }
        }
    }

    void CamNoise()
    {
        if (unreliableTime == 0)
        {
            camNoiseValueNext = Mathf.Lerp(camNoiseCurrentValue, camNoiseMaxValue, unreliabilityAmount / unreliabilityAmountMax);
            camFOVNext = Mathf.Lerp(camFOVNormal, camMaxFOV, unreliabilityAmount / unreliabilityAmountMax);
        }
        // Camera noise
        camNoiseCurrentValue = Mathf.Lerp(camNoiseCurrentValue, camNoiseValueNext, Time.deltaTime);
        camNoiseComponent.m_AmplitudeGain = camNoiseCurrentValue;

        // Camera FOV
        GameManager.gm.playerCam.m_Lens.FieldOfView = Mathf.Lerp(GameManager.gm.playerCam.m_Lens.FieldOfView, camFOVNext, Time.deltaTime);
    }

    #region Out of Bounds Return Coroutines

    public IEnumerator OutOfBoundsReturnTop()
    {
        while (transform.position.y > GameManager.gm.boundaryTop)
        {
            yield return null;
        }

        if (!Input.GetKey(KeyCode.S))
        {
            down = false;
        }
        GameManager.gm.isOutOfBounds = false;
    }

    public IEnumerator OutOfBoundsReturnBottom()
    {
        while (transform.position.y < GameManager.gm.boundaryBottom)
        {
            yield return null;
        }

        if (!Input.GetKey(KeyCode.W))
        {
            up = false;
        }
        GameManager.gm.isOutOfBounds = false;
    }

    public IEnumerator OutOfBoundsReturnLeft()
    {
        while (transform.position.x < GameManager.gm.boundaryLeft)
        {
            yield return null;
        }

        if (!Input.GetKey(KeyCode.D))
        {
            right = false;
        }
        GameManager.gm.isOutOfBounds = false;
    }

    public IEnumerator OutOfBoundsReturnRight()
    {
        while (transform.position.x > GameManager.gm.boundaryRight)
        {
            yield return null;
        }

        if (!Input.GetKey(KeyCode.A))
        {
            left = false;
        }
        GameManager.gm.isOutOfBounds = false;
    }

    #endregion

    #region On nectar pickup

    public void OnNectarPickup()
    {
        isNectarResetting = true;
        nectarResetProgress = nectarResetTimer = 0;
        camNoiseLerpFromValue = camNoiseComponent.m_AmplitudeGain;
        camFOVLerpValue = GameManager.gm.playerCam.m_Lens.FieldOfView;
        unreliableDirection = Vector3.forward;
        StartCoroutine(NectarReset());
    }

    IEnumerator NectarReset()
    {
        while (nectarResetProgress < 1)
        {
            nectarResetTimer += Time.deltaTime;
            nectarResetProgress = nectarResetTimer / nectarResetLength;

            camNoiseComponent.m_AmplitudeGain = camNoiseCurrentValue = camNoiseValueNext = Mathf.Lerp(camNoiseLerpFromValue, camNoiseValue, resetCurve.Evaluate(nectarResetProgress));
            unreliabilityAmount = Mathf.Lerp(unreliabilityAmountLerpFromValue, 0, resetCurve.Evaluate(nectarResetProgress));
            currentMovementSpeed = Mathf.Lerp(movementSpeedLerpValue, movementSpeed, resetCurve.Evaluate(nectarResetProgress));
            GameManager.gm.playerCam.m_Lens.FieldOfView = camFOVNext = Mathf.Lerp(camFOVLerpValue, camFOVNormal, resetCurve.Evaluate(nectarResetProgress));
            yield return null;
        }

        isNectarResetting = false;
    }

    #endregion

    public void ResetVariables()
    {
        camNoiseComponent.m_AmplitudeGain = camNoiseCurrentValue = camNoiseValueNext = camNoiseValue;
        unreliabilityAmount = 0;
        currentMovementSpeed = movementSpeed;
        GameManager.gm.playerCam.m_Lens.FieldOfView = camFOVNext = camFOVNormal;
    }
}
