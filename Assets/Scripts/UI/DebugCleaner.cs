using UnityEngine;

// ������� ������ �� �������� ������
public class DebugCleaner : MonoBehaviour
{
    void Awake()
    {
#if !UNITY_EDITOR && !DEVELOPMENT_BUILD
        Destroy(gameObject);
#endif
    }
}
