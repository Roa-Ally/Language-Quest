using UnityEngine;

[CreateAssetMenu(fileName = "New Fragment", menuName = "Fragment System/Fragment")]
public class Fragment : ScriptableObject
{
    [TextArea(3, 10)]
    public string fragmentText;
} 