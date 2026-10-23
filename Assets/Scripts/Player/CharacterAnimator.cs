using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

// TODO:
// Update animated entity to allow for looping anims until action is done (fluttering, jumping etc.)
// Handles 2D sprite animation playback and directional changes for the player. Inehrits AnimatedEntity.cs
public class CharacterAnimator : AnimatedEntity
{
    // data container to group 4-directional sprite frames together in the Inspector. Serializable just makes it so we can see it in the inspector
    [System.Serializable]
    public class DirectionalAnimations {
        public List<Sprite> front;
        public List<Sprite> back;
        public List<Sprite> side;

        // helper method to get the direction 
        public List<Sprite> GetList(int direction) 
        {
            if (direction == 1) return back;
            if (direction == 2) return side;
            return front; // default
        }
    }

    public DirectionalAnimations idle;
    public DirectionalAnimations walk;

    [Header("Jump Phases")]
    public DirectionalAnimations jumpRise;
    public DirectionalAnimations jumpFall;
    public DirectionalAnimations jumpLand;

    public DirectionalAnimations flutterRise;
    public DirectionalAnimations flutterFall;

    private PlayerController playerController;
    private int currentFacing = 0; // 0 is front, 1 is back, 2 is side.

    private bool wasGrounded = true;

    void Start()
    {
        // finds sprite renderer component on this object or any of its children
        SpriteRenderer = GetComponentInChildren<SpriteRenderer>();

        // default is front facing idle
        DefaultAnimationCycle = idle.GetList(0);

        // get playerController
        playerController = GetComponentInParent<PlayerController>();

        base.AnimationSetup();
    }
    void Update()
    {
        int previousIndex = index;
        base.AnimationUpdate();

        bool isGrounded = playerController.IsGrounded;
        if (isGrounded)
        {
            Looping = true;
        }
        if (!wasGrounded && isGrounded)
        {
            TriggerLand();
        }

        // plays jump rising or jump falling animation
        if (!isGrounded)
        {
            // Fluttering
            if (playerController.IsFluttering)
            {
                // Jump Rise
                if (playerController.currentVerticalVelocity > 0)
                {
                    SetCycle(flutterRise.GetList(currentFacing), true);
                }
                // Jump fall
                else
                {
                    SetCycle(flutterFall.GetList(currentFacing), true);
                }
                
            }
            // Jump Rise
            else if (playerController.currentVerticalVelocity > 0)
            {
                SetCycle(jumpRise.GetList(currentFacing), false);
            }
            // Jump fall
            else
            {
                SetCycle(jumpFall.GetList(currentFacing), false);
            }
        }

        wasGrounded = isGrounded;

        // If the frame just changed and we are currently playing the walk cycle
        if (previousIndex != index && DefaultAnimationCycle == walk.GetList(currentFacing))
        {
            // frames where player foot hits the ground
            if ((index == 2 || index == 6) && playerController.IsGrounded)
            {
                AudioController.Instance.PlayFootstep();
            }
        }
    }

    // Reads input then changes direction of the sprite
    public void UpdateFacing(Vector2 input)
    {
        // decoupled it so pressing a side key always update sthe sprite facing direction, before pressing W or S then pressing A or D locks the facing direction.
        if (Mathf.Abs(input.x) > 0.01f)
        {
            SpriteRenderer.flipX = (input.x < 0);
        }

        if (Mathf.Abs(input.y) > Mathf.Abs(input.x))
        {
            currentFacing = input.y > 0 ? 1 : 0;
        }
        else
        {
            currentFacing = 2;
        }
    }

    // Swaps between idle or walk animation cycles
    public void SetMoving(bool isMoving) {
        DirectionalAnimations set = isMoving ? walk : idle;
        SetCycle(set.GetList(currentFacing), true);
    }

    // to determine whats looping and whats not so jumping cycles arent looping.
    private void SetCycle(List<Sprite> newCycle, bool loop)
    {
        Looping = loop;
        if (DefaultAnimationCycle != newCycle)
        {
            DefaultAnimationCycle = newCycle;
            index = 0;
            if (DefaultAnimationCycle != null && DefaultAnimationCycle.Count > 0)
            {
                SpriteRenderer.sprite = DefaultAnimationCycle[0];
            }
        }
    }

    // Plays the landing animation
    public void TriggerLand()
    {
        List<Sprite> landCycle = jumpLand.GetList(currentFacing);
        base.Interrupt(landCycle);
    }
}
