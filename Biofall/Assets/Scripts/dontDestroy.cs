/*using UnityEngine;

public class dontDestroy : MonoBehaviour
{
    GameObject[] persistanceObject = new GameObject[3];
    public int index;
    void Awake()
    {
        if (persistanceObject[index] == null)
        {
            persistanceObject[index] = gameObject;
            DontDestroyOnLoad(gameObject);
        }
        else if (persistanceObject[index] != null)
        {
            Destroy(gameObject);
        }
    }
}*/
