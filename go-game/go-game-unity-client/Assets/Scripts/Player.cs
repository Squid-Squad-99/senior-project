using System;
using System.Threading.Tasks;
using UnityEngine;

public interface IPlayer
{
    string Id { get; }
}


public class Player : MonoBehaviour, IPlayer
{
    public string Id => _id;
    [SerializeField] private string _id;


}