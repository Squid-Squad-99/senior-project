using System;
using System.Threading.Tasks;
using UnityEngine;

public interface ILocalPlayer
{
    PlayerData Data { get; }
}

public class LocalPlayer : MonoBehaviour, ILocalPlayer
{
    
    public PlayerData Data { get; private set; }
    [SerializeField] private string _id;

    private void Awake()
    {
        Data = new PlayerData(_id);
    }

}