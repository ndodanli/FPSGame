using UnityEngine;

public class Util
{
    public static void SetLayerRecursively(GameObject obj, int newLayer)
    {

        obj.layer = newLayer;
        foreach (Transform child in obj.transform)
        {
            if (child == null) continue;
            SetLayerRecursively(child.gameObject, newLayer);
        }
    }
}
