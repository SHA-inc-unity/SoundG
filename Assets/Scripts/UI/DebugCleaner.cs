using UnityEngine;

// Убирает обьект из релизной версии
public class DebugCleaner : MonoBehaviour
{
    void Awake()
    {
#if !UNITY_EDITOR && !DEVELOPMENT_BUILD
        Destroy(gameObject);
#endif
    }
}
