using System;
using System.Net.NetworkInformation;
using UnityEditor.Experimental;
using UnityEngine;
using Random = UnityEngine.Random;

public class Strategy
{
    protected int mental;
    protected int social;
    protected int physical;
    public void Prepare(Char c)
    {
        if (c.squadLeader)
        {
            mental += c.mental;
            social += c.social;
            physical += c.physical;
            foreach (string sub in c.subordinates)
            {
                Char s = ActiveEntities.Instance.GetOrg(c.org).GetActive(sub);
                mental += s.mental;
                social += s.social;
                physical += s.physical;
            }
            mental /= c.subordinates.Count + 1;
            social /= c.subordinates.Count + 1;
            physical /= c.subordinates.Count + 1;

            int i = 1;
            Char sup = c;
            while(true)
            {
                if (sup == null)
                    break;
                mental += sup.mental/ (7 * i) + sup.social/ (14 * i);
                social += sup.mental / (7 * i) + sup.social / (14 * i);
                physical += sup.mental / (7 * i) + sup.social / (14 * i);
                sup = ActiveEntities.Instance.GetOrg(c.org).GetActive(sup.superior);
                i++;
            }
        }
        else
        {
            mental = c.mental;
            social = c.social;
            physical = c.physical;

            int i = 1;
            Char sup = ActiveEntities.Instance.GetOrg(c.org).GetActive(c.superior);
            while (true)
            {
                if (sup == null)
                    break;
                mental += sup.mental / (7 * i) + sup.social / (14 * i);
                social += sup.mental / (7 * i) + sup.social / (14 * i);
                physical += sup.mental / (7 * i) + sup.social / (14 * i);
                sup = ActiveEntities.Instance.GetOrg(c.org).GetActive(sup.superior);
                i++;
            }
        }
        if (c.order == "")
            ExecuteStrategy(c);
        else
            ExecuteOrder(c);
    }
    public virtual void ExecuteStrategy(Char c) { }
    public virtual void Recruit(Char c) { }
    public virtual void Extort(Char c) { }
    public virtual void Collect(Char c) { }
    public virtual void Punish(Char c) { }
    public virtual void Rob(Char c) { }
    public virtual void Patrol(Char c) { }
    public virtual void Hunt(Char c) { }

    public void ExecuteOrder(Char c) 
    {
        switch (c.order)
        {
            case "Kill": Kill(c); break;
        }
    }
    private void Kill(Char c) 
    { 

    }
}

public class PlayerGangsterStrategy : Strategy
{
    public override void ExecuteStrategy(Char c)
    {
        switch(c.strategy) 
        {
            case "Recruit": Recruit(c); break;
        }
    }
    public override void Recruit(Char c)
    {
        Org org = ActiveEntities.Instance.GetOrg(c.org);
        Char recruit = org.GetRecruitable();
        int findingRoll = Random.Range(1, mental / 2 + social);
        int findingDifficultyRoll = Random.Range(10, 200);
        if (recruit == null && findingRoll > findingDifficultyRoll) 
        {
            recruit = CharPool.Instance.GetCharFromPool(c.type,c.district,c.org);
            if (recruit != null)
            {
                org.AddToRecruitable(recruit);
                int recruiterRoll = Random.Range(1, mental / 2 + social + org.respect);
                int recruitRoll = Random.Range(10, recruit.mental + recruit.social + recruit.physical);
                if (recruiterRoll > recruitRoll)
                {
                    org.AddToReserve(recruit, true);
                    if (org.player)
                        Reports.Instance.AddReport("New member recruited! Name: "+recruit.name+", Mental: "+recruit.mental+", Social: "+recruit.social+", Physical: "+recruit.physical);
                }
            }
        }
        else if (recruit != null) 
        {
            int recruiterRoll = Random.Range(1, mental / 2 + social + org.respect);
            int recruitRoll = Random.Range(10, recruit.mental + recruit.social + recruit.physical);
            if (recruiterRoll > recruitRoll)
            {
                org.AddToReserve(recruit, true);
                if (org.player)
                    Reports.Instance.AddReport("New member recruited! Name: " + recruit.name + ", Mental: " + recruit.mental + ", Social: " + recruit.social + ", Physical: " + recruit.physical);
            }
        }
    }

    public override void Extort(Char c)
    {
    }

    public override void Collect(Char c)
    {
    }

    public override void Punish(Char c)
    {
    }

    public override void Rob(Char c)
    {
    }
    public override void Patrol(Char c)
    { }
    public override void Hunt(Char c)
    { }
}
public class AIGangsterStrategy : Strategy
{
    public override void ExecuteStrategy(Char c)
    {

    }
    public override void Recruit(Char c)
    { }

    public override void Extort(Char c)
    {
    }

    public override void Collect(Char c)
    {
    }

    public override void Punish(Char c)
    {
    }

    public override void Rob(Char c)
    {
    }
    public override void Patrol(Char c)
    { }
    public override void Hunt(Char c)
    { }
}
public class PolicemanStrategy : Strategy
{
    public override void ExecuteStrategy(Char c)
    {

    }
    public override void Recruit(Char c)
    { }

    public override void Extort(Char c)
    {
    }

    public override void Collect(Char c)
    {
    }

    public override void Punish(Char c)
    {
    }

    public override void Rob(Char c)
    {
    }
    public override void Patrol(Char c)
    { }
    public override void Hunt(Char c)
    { }
}