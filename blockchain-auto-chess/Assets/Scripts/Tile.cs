using Army;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public Vector3 Position => transform.position;
    public Vector2Int Index;
    public Soldier Occupier { get; set; }

    [SerializeField] private GameObject _focusFrame;

    public void Focus(bool focus)
    {
        _focusFrame.SetActive(focus);
    }
}