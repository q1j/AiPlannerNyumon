using System.Collections;
using UnityEngine;

public class EaterActions : MonoBehaviour
{
    public IEnumerator Move(GameObject target)
    {
        Debug.Log("Move");

        while (target != null && !IsArrival(target))
        {
            transform.position = Vector3.Lerp(transform.position, target.transform.position, 0.03f);
            transform.LookAt(target.transform.position);
            yield return null;
        }
    }

    bool IsArrival(GameObject target)
    {
        return Vector3.Distance(transform.position, target.transform.position) < 0.01f;
    }

    public IEnumerator DestroyObject(GameObject target)
    {
        Debug.Log("DestroyObject");

        DestroyImmediate(target);
        yield return new WaitForSeconds(0.3f);
    }
}
