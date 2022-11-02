using System.Collections.Generic;
using Ultility;
using UnityEngine;

namespace Army
{
    public class SoldierManager : Singleton<SoldierManager>
    {
        public HashSet<Soldier> Soldiers { get; } = new HashSet<Soldier>();

        public Dictionary<TeamColorTypes, int> TeamSoldierCnt { get; } = new Dictionary<TeamColorTypes, int>()
            { [TeamColorTypes.Blue] = 0, [TeamColorTypes.Red] = 0 };

        public void RegisterSoldier(Soldier soldier)
        {
            Soldiers.Add(soldier);
            TeamSoldierCnt[soldier.TeamColor]++;
        }

        public void UnRegisterSoldier(Soldier soldier)
        {
            if (Soldiers.Contains(soldier))
            {
                Soldiers.Remove(soldier);
                TeamSoldierCnt[soldier.TeamColor]--;
            }
        }

        public void DestroyAllSoldier()
        {
            foreach (Soldier soldier in Soldiers)
            {
                Destroy(soldier.gameObject);
            }
        }
    }
}