using System.Collections.Generic;
using Ultility;
using UnityEngine;

namespace Army
{
    public class SoldierManager : Singleton<SoldierManager>
    {
        public HashSet<Soldier> Soldiers { get; } = new HashSet<Soldier>();

        public void RegisterSoldier(Soldier soldier)
        {
            Soldiers.Add(soldier);
        }

        public void UnRegisterSoldier(Soldier soldier)
        {
            Soldiers.Remove(soldier);
        }
    }
}