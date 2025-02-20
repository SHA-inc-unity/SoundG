using UnityEngine;

// Убирает обьект из релизной версии
public class DebugCleaner : MonoBehaviour
{
    void Awake()
    {
#if !UNITY_EDITOR
        Destroy(gameObject);
#endif
    }
}
