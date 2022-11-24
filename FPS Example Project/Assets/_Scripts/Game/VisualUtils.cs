using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisualUtils : MonoBehaviour
{
   public static void CreateVFX(GameObject sfx, Transform parent, float lifeTime)
    {
        GameObject sfxObject = Instantiate(sfx, parent);
        TimedSelfDestruct selfDestruct = sfxObject.AddComponent<TimedSelfDestruct>();
        selfDestruct.LifeTime = lifeTime;
    }
    public static void CreateVFX(GameObject sfx, Vector3 pos, float lifeTime)
    {
        GameObject sfxObject = Instantiate(sfx, pos, Quaternion.identity);
        TimedSelfDestruct selfDestruct = sfxObject.AddComponent<TimedSelfDestruct>();
        selfDestruct.LifeTime = lifeTime;
    }
    public static void CreateImpactVFX(GameObject sfx, RaycastHit hit, float lifeTime)
    {
        GameObject sfxObject = Instantiate(sfx, hit.point, Quaternion.LookRotation(hit.normal));
        TimedSelfDestruct selfDestruct = sfxObject.AddComponent<TimedSelfDestruct>();
        selfDestruct.LifeTime = lifeTime;
    }
}
