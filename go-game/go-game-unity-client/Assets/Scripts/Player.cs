using System;
using System.Threading.Tasks;
using UnityEngine;

public interface IPlayer
{
    PlayerData Data { get; }
}


public class Player : MonoBehaviour, IPlayer
{
    
    public PlayerData Data { get; private set; }
    [SerializeField] private string _id;

    private void Awake()
    {
        Data = new PlayerData(_id);
    }

}

public class PlayerData
{
    public String id { get; private set; }

    public PlayerData(String id)
    {
        this.id = id;
    }
}