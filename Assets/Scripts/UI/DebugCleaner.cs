using UnityEngine;

// ������� ������ �� �������� ������
public class DebugCleaner : MonoBehaviour
{
    void Awake()
    {
#if !UNITY_EDITOR
        Destroy(gameObject);
#endif
    }
}
