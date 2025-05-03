using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    private Vector2 moveDirection = Vector2.zero;
    private bool interactPressed = false;
    private bool submitPressed = false;
    private bool attackPressed = false; // New: Attack flag

    private static InputManager instance;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("Found more than one Input Manager in the scene.");
        }
        instance = this;
    }

    public static InputManager GetInstance()
    {
        return instance;
    }

    private void Update()
    {
        // Movement
        moveDirection = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

        // Interact
        if (Input.GetKeyDown(KeyCode.I))
        {
            interactPressed = true;
        }
        else if (Input.GetKeyUp(KeyCode.I))
        {
            interactPressed = false;
        }

        // Submit
        if (Input.GetKeyDown(KeyCode.Return))
        {
            submitPressed = true;
        }
        else if (Input.GetKeyUp(KeyCode.Return))
        {
            submitPressed = false;
        }

        // Attack
        if (Input.GetKeyDown(KeyCode.J)) // Replace with your attack key
        {
            attackPressed = true;
            Debug.Log("Attack action triggered! J pressed!");
        }
        else if (Input.GetKeyUp(KeyCode.J))
        {
            attackPressed = false;
        }
    }

    public Vector2 GetMoveDirection()
    {
        return moveDirection;
    }

    public bool GetInteractPressed()
    {
        bool result = interactPressed;
        interactPressed = false; // Reset after being read
        return result;
    }

    public bool GetSubmitPressed()
    {
        bool result = submitPressed;
        submitPressed = false; // Reset after being read
        return result;
    }

    public bool GetAttackPressed() // New: Attack getter
    {
        bool result = attackPressed;
        attackPressed = false; // Reset the state after being read
        return result;
    }

    public void RegisterSubmitPressed()
    {
        submitPressed = false;
    }
}