using UnityEngine;

public static class Extensions
{
    public static bool Whitelist(this Animator animator, params string[] stateNames)
    {
        foreach (string stateName in stateNames)
        {
            if (animator.GetCurrentAnimatorStateInfo(0).IsName(stateName) == true)
            {
                return true;
            }
        }
        return false;
    }
    public static bool Blacklist(this Animator animator, params string[] stateNames)
    {
        foreach (string stateName in stateNames)
        {
            if (animator.GetCurrentAnimatorStateInfo(0).IsName(stateName) == true)
            {
                return false;
            }
        }
        return true;
    }
}

