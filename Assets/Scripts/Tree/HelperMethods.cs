using UnityEngine;

public class HelperMethods : MonoBehaviour
{
    private const float MAX_ANGLE = 45;

    private const int NODES_COUNT = 5;
    public static Vector3[] GetRandomRotationsForBones()
    {
        Vector3[] randomNodeVectors = new Vector3[NODES_COUNT];
        for (int i = 0; i < NODES_COUNT; i++)
        {
            randomNodeVectors[i].x = Random.Range(-MAX_ANGLE, MAX_ANGLE);
            randomNodeVectors[i].y = 0f;
            randomNodeVectors[i].z = Random.Range(-MAX_ANGLE, MAX_ANGLE);
        }
        return randomNodeVectors;
    }
}
