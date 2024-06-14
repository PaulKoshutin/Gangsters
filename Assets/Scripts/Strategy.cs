using System.Linq;
using Random = UnityEngine.Random;

public class Strategy
{
    public Char Prepare(Char c, bool combat=false, Char target = null)
    {
        int mental = 0;
        int social = 0;
        int physical = 0;

        if (c.squadLeader)
        {
            mental += c.mental;
            social += c.social;
            physical += c.physical;

            Char s;
            foreach (string sub in c.subordinates)
            {
                if (c.org != "")
                    s = ActiveEntities.Instance.GetOrg(c.org).GetActive(sub);
                else
                    s = ActiveEntities.Instance.GetDistrict(c.district).policemen.Find(i => i.name == sub);
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
                mental += (sup.mental/ (10 * i) + sup.social/ (20 * i)) * (c.subordinates.Count + 1);
                social += (sup.mental / (10 * i) + sup.social / (20 * i)) * (c.subordinates.Count + 1);
                physical += (sup.mental / (10 * i) + sup.social / (20 * i)) * (c.subordinates.Count + 1);
                if (c.org != "")
                    sup = ActiveEntities.Instance.GetOrg(c.org).GetActive(sup.superior);
                else break;
                i++;
            }
        }
        else
        {
            mental = c.mental;
            social = c.social;
            physical = c.physical;

            if (c.org != "")
            {
                int i = 1;
                Char sup = ActiveEntities.Instance.GetOrg(c.org).GetActive(c.superior);
                while (true)
                {
                    if (sup == null)
                        break;
                    mental += sup.mental / (10 * i) + sup.social / (20 * i);
                    social += sup.mental / (10 * i) + sup.social / (20 * i);
                    physical += sup.mental / (10 * i) + sup.social / (20 * i);
                    sup = ActiveEntities.Instance.GetOrg(c.org).GetActive(sup.superior);
                    i++;
                }
            }
        }
        Char squad = new Char(c, mental, social, physical);
        if (!combat)
            ExecuteStrategy(squad, target);
        return squad;
    }
    public void ExecuteStrategy(Char c, Char target=null)
    {
        switch (c.strategy)
        {
            case "Recruit": Recruit(c); break;
            case "Extort": Extort(c, target); break;
            case "Punish": Punish(c, target); break;
            case "Rob": Rob(c, target); break;
            case "Patrol": Patrol(c); break;
            case "Hunt": Hunt(c, target); break;
        }
    }
    public bool AvoidPatrol(Char us, string otherOrg)
    {
        District district = ActiveEntities.Instance.GetDistrict(us.district);
        Char them = ActiveEntities.Instance.patrols.Find(i => (i.org == otherOrg && i.district == district.name));
        if (them != null)
        {
            int evasionRoll = Random.Range(15, us.mental + us.social / 2);
            int searchRoll = Random.Range(1, them.mental + them.social / 2);
            if (otherOrg == "")  
                searchRoll -= district.criminality / 4; 
            if (us.strategy == "Hunt")
                searchRoll += district.criminality / 4;
            if (evasionRoll > searchRoll)
                return true;
            else
            {
                ActiveEntities.Instance.GetOrg(us.org).AddToKnown(them);
                int deceptionRoll = Random.Range(25, us.mental / 2 + us.social);
                int investigationRoll = Random.Range(1, them.mental / 2 + them.social);
                if (otherOrg == "")  
                    investigationRoll += district.criminality / 4; 
                else if (ActiveEntities.Instance.GetOrg(them.org).GetPolicyTowards(us.org) == "Competition")
                    investigationRoll += district.criminality / 4;
                else if (ActiveEntities.Instance.GetOrg(them.org).GetPolicyTowards(us.org) == "War")
                    investigationRoll += district.criminality / 2;
                if (us.strategy == "Hunt")
                    searchRoll += district.criminality / 4;
                if (deceptionRoll > investigationRoll)
                    return true;
                else
                    return Combat(us, them);
            }
        }
        return true;
    }
    public bool Combat(Char us, Char them)
    {
        District district = ActiveEntities.Instance.GetDistrict(us.district);
        Org org = ActiveEntities.Instance.GetOrg(us.org);
        bool police = false;
        if (them.org == "")
            police = true;
        Org otherOrg = ActiveEntities.Instance.GetOrg(them.org);
        ActiveEntities.Instance.GetOrg(us.org).AddToKnown(them);
        while (true)
        {
            int ourCombatRoll = Random.Range(us.mental / 4 + us.physical / 4, us.mental + us.physical);
            int theirCombatRoll = Random.Range(them.mental / 4 + them.physical / 4, them.mental + them.physical);
            int saveThrow;
            if (ourCombatRoll <= theirCombatRoll)
            {
                if (us.subordinates.Count > 0)
                    foreach (string sub in us.subordinates)
                    {
                        Char c = org.GetActive(sub);
                        saveThrow = Random.Range(c.physical / 4, c.physical + them.physical);
                        if (saveThrow < theirCombatRoll)
                        {
                            if (!c.wounded)
                            {
                                c.Wound();
                                break;
                            }
                            else
                            {
                                c.Fire();
                                district.CriminalityChange(Random.Range(1, 5));
                                if (!police)
                                {
                                    if (otherOrg.GetPolicyTowards(c.org) != "War")
                                        if (Random.Range(1, 100) < 10)
                                            otherOrg.SetPolicyTowards(c.org, "War");
                                    if (otherOrg.player)
                                        Reports.Instance.AddReport("We killed one of the " + c.org + "! Name: " + c.name + ", District: " + c.district);
                                }
                                if (org.player && !police)
                                    Reports.Instance.AddReport("One of our men was killed by the "+otherOrg+"! Name: " + c.name + ", District: " + c.district);
                                else if (org.player)
                                    Reports.Instance.AddReport("One of our men was killed by the policemen! Name: " + c.name + ", District: " + c.district);
                                us.subordinates.Remove(sub);
                                break;
                            }
                        }
                    }
                else
                {
                    saveThrow = Random.Range(us.physical / 4, us.physical + them.physical);
                    if (saveThrow < theirCombatRoll)
                    {
                        if (!org.GetActive(us.name).wounded)
                        {
                            org.GetActive(us.name).Wound();
                            break;
                        }
                        else
                        {
                            org.GetActive(us.name).Fire();
                            district.CriminalityChange(Random.Range(1, 5));
                            org.RespectChange(-Random.Range(1, 5));
                            if (!police)
                            {
                                otherOrg.RespectChange(Random.Range(1, 5));
                                if (otherOrg.GetPolicyTowards(us.org) != "War")
                                    if (Random.Range(1, 100) < 10)
                                        otherOrg.SetPolicyTowards(us.org, "War");
                                if (otherOrg.player)
                                    Reports.Instance.AddReport("We wiped out a squad of " + us.org + "! Name: " + us.name + ", District: " + us.district);
                            }
                            if (org.player)
                                Reports.Instance.AddReport("One of our squads was wiped out by the "+otherOrg+"! Leader name: " + us.name + ", District: " + us.district);
                            else if (org.player)
                                Reports.Instance.AddReport("One of our squads was wiped out by the policemen! Leader name: " + us.name + ", District: " + us.district);
                            return false;
                        }
                    }
                }
            }
            else
            {
                if (them.subordinates.Count > 0)
                    foreach (string sub in them.subordinates)
                    {
                        Char c;
                        if (!police)
                            c = otherOrg.GetActive(sub);
                        else
                            c = ActiveEntities.Instance.GetDistrict(them.district).policemen.Find(i=>i.name == sub);

                        saveThrow = Random.Range(c.physical / 4, c.physical + us.physical);
                        if (saveThrow < theirCombatRoll)
                        {
                            if (!c.wounded)
                            {
                                c.Wound();
                                break;
                            }
                            else
                            {
                                c.Fire();
                                district.CriminalityChange(Random.Range(1, 5));
                                if (!police)
                                {
                                    if (org.GetPolicyTowards(c.org) != "War")
                                        if (Random.Range(1, 100) < 10)
                                            org.SetPolicyTowards(c.org, "War");
                                    if (otherOrg.player)
                                        Reports.Instance.AddReport("One of our men was killed by the "+org+"! Name: " + c.name + ", District: " + c.district);
                                }
                                if (org.player && !police)
                                    Reports.Instance.AddReport("We killed one of the " + c.org + "! Name: " + c.name + ", District: " + c.district);
                                else if (org.player)
                                    Reports.Instance.AddReport("We killed one of the policemen! Name: " + c.name + ", District: " + c.district);
                                them.subordinates.Remove(sub);
                                break;
                            }
                        }
                    }
                else
                {
                    saveThrow = Random.Range(them.physical / 4, them.physical + us.physical);
                    if (saveThrow < theirCombatRoll)
                    {
                        if (!police)
                        {
                            if (!otherOrg.GetActive(them.name).wounded)
                            {
                                otherOrg.GetActive(them.name).Wound();
                                break;
                            }
                            else
                            {
                                otherOrg.GetActive(them.name).Fire();
                                otherOrg.RespectChange(-Random.Range(1, 5));
                                if (org.GetPolicyTowards(them.org) != "War")
                                    if (Random.Range(1, 100) < 10)
                                        org.SetPolicyTowards(them.org, "War");
                                if (otherOrg.player)
                                    Reports.Instance.AddReport("One of our squads was wiped out by the "+org+"! Leader name: " + them.name + ", District: " + them.district);
                                district.CriminalityChange(Random.Range(1, 5));
                                org.RespectChange(Random.Range(1, 5));
                                if (org.player)
                                    Reports.Instance.AddReport("We wiped out a squad of the " + otherOrg + "! Name: " + them.name + ", District: " + them.district);
                                return true;
                            }
                        }
                        else
                        {
                            Char p = ActiveEntities.Instance.GetDistrict(them.district).policemen.Find(i => i.name == them.name);
                            if (!p.wounded)
                            {
                                p.Wound();
                                break;
                            }
                            else
                            {
                                p.Fire();
                                district.CriminalityChange(Random.Range(1, 5));
                                org.RespectChange(Random.Range(1, 5));
                                if (org.player)
                                    Reports.Instance.AddReport("We wiped out a squad of the policemen! Name: " + them.name + ", District: " + them.district);
                                return true;
                            }
                        }
                    }
                }
            }
            us = Prepare(us, true);
            them = Prepare(them, true);
            if (us.physical > them.physical * 2 || us.subordinates.Count > them.subordinates.Count * 2)
            {
                int pursuitRoll = Random.Range(us.mental / 4 + us.physical / 4, us.mental + us.physical);
                int escapeRoll = Random.Range(them.mental / 2 + them.physical / 2, them.mental * 2 + them.physical * 2);
                if (escapeRoll > pursuitRoll)
                {
                    return true;
                }
            }
            else if (them.physical > us.physical * 2 || them.subordinates.Count > us.subordinates.Count * 2)
            {
                int pursuitRoll = Random.Range(them.mental / 4 + them.physical / 4, them.mental + them.physical);
                int escapeRoll = Random.Range(us.mental / 2 + us.physical / 2, us.mental * 2 + us.physical * 2);
                if (escapeRoll > pursuitRoll)
                {
                    return false;
                }
            }
        }
        return false;
    }
    public virtual Char Recruit(Char c) { return null; }
    public virtual void Extort(Char c, Char target = null) { }
    public virtual void Punish(Char c, Char target = null) { }
    public virtual void Rob(Char c, Char target = null) { }
    public virtual void Patrol(Char c) { }
    public virtual void Hunt(Char c, Char target = null) { }

    
}

public class PlayerGangsterStrategy : Strategy
{
    public override Char Recruit(Char c)
    {
        Org org = ActiveEntities.Instance.GetOrg(c.org);
        Char recruit = org.GetRecruitable();
        int findingRoll = Random.Range(1, c.mental / 2 + c.social);
        int findingDifficultyRoll = Random.Range(15, 400);
        if ((recruit == null && findingRoll > findingDifficultyRoll) || recruit != null) 
        {
            if (recruit == null && findingRoll > findingDifficultyRoll)
                recruit = CharPool.Instance.GetCharFromPool(c.type,c.district,c.org);
            if (recruit != null)
            {
                org.AddToRecruitable(recruit);
                int recruiterRoll = Random.Range(1, c.mental / 2 + c.social + org.respect);
                int recruitRoll = Random.Range(15, recruit.mental + recruit.social + recruit.physical);
                if (recruiterRoll > recruitRoll)
                {
                    if (org.player)
                    {
                        org.AddToReserve(recruit, true);
                        Reports.Instance.AddReport("New member recruited! Name: " + recruit.name + ", District: " + recruit.district + ", Mental: " + recruit.mental + ", Social: " + recruit.social + ", Physical: " + recruit.physical);
                    }
                    else
                        org.AddToReserve(recruit, false);
                    return recruit;
                }
            }
        }
        return null;
    }

    public override void Extort(Char c, Char target = null)
    {
        Org org = ActiveEntities.Instance.GetOrg(c.org);
        Char bizz = org.GetExtortable();
        int findingRoll = Random.Range(1, c.mental / 2 + c.social);
        int findingDifficultyRoll = Random.Range(15, 400);
        if (bizz == null && findingRoll > findingDifficultyRoll)
            bizz = ActiveEntities.Instance.GetDistrict(c.district).GetExtortable(org);
        if ((bizz == null && findingRoll > findingDifficultyRoll) || (bizz != null && bizz.org == ""))
        {
            if (bizz == null && findingRoll > findingDifficultyRoll)
            {
                bizz = CharPool.Instance.GetCharFromPool("businessman", c.district, "");
                if (bizz != null)
                    ActiveEntities.Instance.GetDistrict(c.district).businessmen.Add(bizz);
            }
            if (bizz != null && AvoidPatrol(c, ""))
            {
                org.AddToKnown(bizz);
                int bizzerRoll = Random.Range(1, c.mental / 2 + c.social + org.respect);
                int bizzRoll = Random.Range(15, bizz.mental / 2 + bizz.social + bizz.physical / 2 + bizz.pay / 4);
                if (bizzerRoll > bizzRoll)
                {
                    if (target != null)
                        org.GetActive(c.name).order = "";

                    org.controlled.Add(bizz);
                    bizz.org = c.org;
                    if (org.player)
                        Reports.Instance.AddReport("New businessman extorted! Name: " + bizz.name + ", District: " + bizz.district + ", Pay: " + bizz.pay);
                }
                else
                {
                    org.RespectChange(-Random.Range(1, 5));
                    if (org.player)
                        Reports.Instance.AddReport("Businessman declined our protection! Name: " + bizz.name + ", District: " + bizz.district + ", Pay: " + bizz.pay);
                }
            }
        }
        else if (bizz != null && AvoidPatrol(c, bizz.org)) 
        {
            org.AddToKnown(bizz);
            Org otherOrg = ActiveEntities.Instance.GetOrg(bizz.org);
            int bizzerRoll = Random.Range(1, c.mental / 2 + c.social + org.respect);
            int bizzRoll = Random.Range(15, bizz.mental / 2 + bizz.social + bizz.physical / 2 + bizz.pay / 4 + otherOrg.respect);
            if (bizzerRoll > bizzRoll)
            {
                if (target != null)
                    org.GetActive(c.name).order = "";

                org.controlled.Add(bizz);
                otherOrg.controlled.Remove(bizz);
                org.RespectChange(Random.Range(1, 5));
                otherOrg.RespectChange(-Random.Range(1, 5));
                if (otherOrg.GetPolicyTowards(c.org) == "Peace")
                    if (Random.Range(1, 100) < 10)
                        otherOrg.SetPolicyTowards(c.org, "Competition");
                if (org.player)
                    Reports.Instance.AddReport("New businessman taken from the "+otherOrg.name+"! Name: " + bizz.name + ", District: " + bizz.district + ", Pay: " + bizz.pay);
                else if (otherOrg.player)
                    Reports.Instance.AddReport("One of our businessmen was taken by the " + org.name + "! Name: " + bizz.name + ", District: " + bizz.district + ", Pay: " + bizz.pay);
            }
            else
            {
                org.RespectChange(-Random.Range(1, 5));
                otherOrg.RespectChange(Random.Range(1, 5));
                if (org.player)
                    Reports.Instance.AddReport("Businessman under the "+otherOrg.name+" declined our protection! Name: " + bizz.name + ", District: " + bizz.district + ", Pay: " + bizz.pay);
            }
        }
    }

    public override void Punish(Char c, Char target = null)
    {
        Org org = ActiveEntities.Instance.GetOrg(c.org);
        District district = ActiveEntities.Instance.GetDistrict(c.district);
        Char bizz = org.GetExtortable();
        if (bizz != null && bizz.org == "" && AvoidPatrol(c, ""))
        {
            int bizzerRoll = Random.Range(1, c.social / 2 + c.physical);
            int bizzRoll = Random.Range(15, bizz.mental/2 + bizz.social/2 + bizz.physical);
            if (bizzerRoll > bizzRoll)
            {
                if (target != null)
                    org.GetActive(c.name).order = "";
                if (bizzerRoll > bizzRoll * 2 && Random.Range(1, 100) < 20)
                {
                    if (bizz.wounded)
                    {
                        district.CriminalityChange(Random.Range(5, 10));
                        org.RespectChange(Random.Range(5, 10));
                        bizz.Fire();
                        if (org.player)
                            Reports.Instance.AddReport("Businessman killed during assault! Name: " + bizz.name + ", District: " + bizz.district + ", Pay: " + bizz.pay);
                    }
                    else
                    {
                        district.CriminalityChange(Random.Range(2, 7));
                        org.RespectChange(Random.Range(2, 7));
                        bizz.Wound();
                        if (org.player)
                            Reports.Instance.AddReport("Businessman wounded during assault! Name: " + bizz.name + ", District: " + bizz.district + ", Pay: " + bizz.pay);
                    }
                }
                else
                {
                    district.CriminalityChange(Random.Range(1, 5));
                    org.RespectChange(Random.Range(1, 5));
                    bizz.Punishment(Random.Range(5, 10));
                    if (org.player)
                        Reports.Instance.AddReport("Businessman assaulted! Name: " + bizz.name + ", District: " + bizz.district + ", Pay: " + bizz.pay);
                }
            }
            else
            {
                org.RespectChange(-Random.Range(1, 5));
                if (org.player)
                    Reports.Instance.AddReport("Failed to assualt a businessman! Name: " + bizz.name + ", District: " + bizz.district + ", Pay: " + bizz.pay);
            }
        }
        else if (bizz != null && AvoidPatrol(c, bizz.org))
        {
            Org otherOrg = ActiveEntities.Instance.GetOrg(bizz.org);
            int bizzerRoll = Random.Range(1, c.social / 2 + c.physical);
            int bizzRoll = Random.Range(15, bizz.mental / 2 + bizz.social / 2 + bizz.physical);
            if (bizzerRoll > bizzRoll)
            {
                if (target != null)
                    org.GetActive(c.name).order = "";
                if (bizzerRoll > bizzRoll * 2 && Random.Range(1, 100) < 20)
                {
                    if (bizz.wounded)
                    {
                        district.CriminalityChange(Random.Range(5, 10));
                        org.RespectChange(Random.Range(5, 10));
                        otherOrg.RespectChange(-Random.Range(5, 10));
                        if (otherOrg.GetPolicyTowards(c.org) != "War")
                            if (Random.Range(1, 100) < 10)
                                otherOrg.SetPolicyTowards(c.org, "War");
                        bizz.Fire();
                        if (org.player)
                            Reports.Instance.AddReport("Businessman under the " + otherOrg.name + " killed during assault! Name: " + bizz.name + ", District: " + bizz.district + ", Pay: " + bizz.pay);
                        if (otherOrg.player)
                            Reports.Instance.AddReport("One of our businessmen killed by the " + org.name + " during assault! Name: " + bizz.name + ", District: " + bizz.district + ", Pay: " + bizz.pay);
                    }
                    else
                    {
                        district.CriminalityChange(Random.Range(2, 7));
                        org.RespectChange(Random.Range(2, 7));
                        otherOrg.RespectChange(-Random.Range(2, 7));
                        if (otherOrg.GetPolicyTowards(c.org) != "War")
                            if (Random.Range(1, 100) < 1)
                                otherOrg.SetPolicyTowards(c.org, "War");
                        bizz.Wound();
                        if (org.player)
                            Reports.Instance.AddReport("Businessman under the " + otherOrg.name + " wounded during assault! Name: " + bizz.name + ", District: " + bizz.district + ", Pay: " + bizz.pay);
                        if (otherOrg.player)
                            Reports.Instance.AddReport("One of our businessmen wounded by the " + org.name + " during assault! Name: " + bizz.name + ", District: " + bizz.district + ", Pay: " + bizz.pay);
                    }
                }
                else
                {
                    district.CriminalityChange(Random.Range(1, 5));
                    org.RespectChange(Random.Range(1, 5));
                    otherOrg.RespectChange(-Random.Range(1, 5));
                    bizz.Punishment(Random.Range(5, 10));
                    if (otherOrg.GetPolicyTowards(c.org) == "Peace")
                        if (Random.Range(1, 100) < 10)
                            otherOrg.SetPolicyTowards(c.org, "Competition");
                    if (org.player)
                        Reports.Instance.AddReport("Businessman under the " + otherOrg.name + " assaulted! Name: " + bizz.name + ", District: " + bizz.district + ", Pay: " + bizz.pay);
                }
            }
            else
            {
                org.RespectChange(-Random.Range(1, 5));
                otherOrg.RespectChange(Random.Range(1, 5));
                if (org.player)
                    Reports.Instance.AddReport("Failed to assualt a businessman under the " + otherOrg.name + "! Name: " + bizz.name + ", District: " + bizz.district + ", Pay: " + bizz.pay);
            }
        }
    }

    public override void Rob(Char c, Char target = null)
    {
        Org org = ActiveEntities.Instance.GetOrg(c.org);
        District district = ActiveEntities.Instance.GetDistrict(c.district);
        Char bizz = org.GetExtortable();
        int findingRoll = Random.Range(1, c.mental / 2 + c.social);
        int findingDifficultyRoll = Random.Range(15, 400);
        if (bizz == null && findingRoll > findingDifficultyRoll)
            bizz = ActiveEntities.Instance.GetDistrict(c.district).GetExtortable(org);
        if ((bizz == null && findingRoll > findingDifficultyRoll) || (bizz != null && bizz.org == ""))
        {
            if (bizz == null && findingRoll > findingDifficultyRoll)
            {
                bizz = CharPool.Instance.GetCharFromPool("businessman", c.district, "");
                if (bizz != null)
                    ActiveEntities.Instance.GetDistrict(c.district).businessmen.Add(bizz);
            }
            if (bizz != null && AvoidPatrol(c, ""))
            {
                org.AddToKnown(bizz);
                int bizzerRoll = Random.Range(1, c.mental / 2 + c.physical);
                int bizzRoll = Random.Range(15, bizz.mental + bizz.social / 2 + bizz.physical / 2);
                if (bizzerRoll > bizzRoll)
                {
                    if (target != null)
                        org.GetActive(c.name).order = "";
                    if (bizzerRoll > bizzRoll * 2 && Random.Range(1, 100) < 20)
                    {
                        if (bizz.wounded)
                        {
                            district.CriminalityChange(Random.Range(5, 10));
                            org.RespectChange(Random.Range(1, 5));
                            org.money += Random.Range(bizz.pay, bizz.pay * 4);
                            bizz.Fire();
                            if (org.player)
                                Reports.Instance.AddReport("Businessman killed during robbery! Name: " + bizz.name + ", District: " + bizz.district + ", Pay: " + bizz.pay);
                        }
                        else
                        {
                            district.CriminalityChange(Random.Range(2, 7));
                            org.money += Random.Range(bizz.pay, bizz.pay * 2);
                            bizz.Wound();
                            if (org.player)
                                Reports.Instance.AddReport("Businessman wounded during robbery! Name: " + bizz.name + ", District: " + bizz.district + ", Pay: " + bizz.pay);
                        }
                    }
                    else
                    {
                        district.CriminalityChange(Random.Range(1, 5));
                        bizz.Punishment(Random.Range(bizz.pay / 10, bizz.pay / 5), true);
                        org.money += Random.Range(bizz.pay / 2, bizz.pay * 2);
                        if (org.player)
                            Reports.Instance.AddReport("Businessman robbed! Name: " + bizz.name + ", District: " + bizz.district + ", Pay: " + bizz.pay);
                    }
                }
            }
        }
        else if (bizz != null && AvoidPatrol(c, bizz.org))
        {
            org.AddToKnown(bizz);
            Org otherOrg = ActiveEntities.Instance.GetOrg(bizz.org);
            int bizzerRoll = Random.Range(1, c.mental / 2 + c.physical);
            int bizzRoll = Random.Range(15, bizz.mental + bizz.social / 2 + bizz.physical/2);
            if (bizzerRoll > bizzRoll)
            {
                if (target != null)
                    org.GetActive(c.name).order = "";
                if (bizzerRoll > bizzRoll * 2 && Random.Range(1, 100) < 20)
                {
                    if (bizz.wounded)
                    {
                        district.CriminalityChange(Random.Range(5, 10));
                        org.RespectChange(Random.Range(1, 5));
                        otherOrg.RespectChange(-Random.Range(5, 10));
                        if (otherOrg.GetPolicyTowards(c.org) != "War")
                            if (Random.Range(1, 100) < 10)
                                otherOrg.SetPolicyTowards(c.org, "War");
                        org.money += Random.Range(bizz.pay, bizz.pay * 4);
                        bizz.Fire();
                        if (org.player)
                            Reports.Instance.AddReport("Businessman under the " + otherOrg.name + " killed during robbery! Name: " + bizz.name + ", District: " + bizz.district + ", Pay: " + bizz.pay);
                        if (otherOrg.player)
                            Reports.Instance.AddReport("One of our businessmen killed by the " + org.name + " during robbery! Name: " + bizz.name + ", District: " + bizz.district + ", Pay: " + bizz.pay);
                    }
                    else
                    {
                        district.CriminalityChange(Random.Range(2, 7));
                        otherOrg.RespectChange(-Random.Range(1, 5));
                        if (otherOrg.GetPolicyTowards(c.org) != "War")
                            if (Random.Range(1, 100) < 1)
                                otherOrg.SetPolicyTowards(c.org, "War");
                        org.money += Random.Range(bizz.pay, bizz.pay * 2);
                        bizz.Wound();
                        if (org.player)
                            Reports.Instance.AddReport("Businessman under the " + otherOrg.name + " wounded during robbery! Name: " + bizz.name + ", District: " + bizz.district + ", Pay: " + bizz.pay);
                        if (otherOrg.player)
                            Reports.Instance.AddReport("One of our businessmen wounded by the " + org.name + " during robbery! Name: " + bizz.name + ", District: " + bizz.district + ", Pay: " + bizz.pay);
                    }
                }
                else
                {
                    district.CriminalityChange(Random.Range(1, 5));
                    otherOrg.RespectChange(-Random.Range(1, 5));
                    bizz.Punishment(Random.Range(bizz.pay / 10, bizz.pay / 5), true);
                    org.money += Random.Range(bizz.pay / 2, bizz.pay * 2);
                    if (otherOrg.GetPolicyTowards(c.org) == "Peace")
                        if (Random.Range(1, 100) < 10)
                            otherOrg.SetPolicyTowards(c.org, "Competition");
                    if (org.player)
                        Reports.Instance.AddReport("Businessman under the " + otherOrg.name + " robbed! Name: " + bizz.name + ", District: " + bizz.district + ", Pay: " + bizz.pay);
                }
            }
        }
    }
    public override void Patrol(Char c)
    { 
        ActiveEntities.Instance.patrols.Add(c);
    }
    public override void Hunt(Char c, Char target=null)
    {
        Org org = ActiveEntities.Instance.GetOrg(c.org);
        int findingRoll = Random.Range(1, c.mental / 2 + c.social);
        int findingDifficultyRoll = Random.Range(15, 400);
        if (findingRoll > findingDifficultyRoll)
        {   
            if (target == null)
                target = org.GetHuntable();
            if (target == null)
                target = org.FindHuntable(c);
            if (target != null && AvoidPatrol(c, target.org))
            {
                bool win = false;
                org.AddToKnown(target);
                if (c.org != "")
                {
                    win = Combat(c, target);
                    if (win)
                    {
                        if (target != null)
                            org.GetActive(c.name).order = "";
                    }
                }
                else
                {
                    win = Combat(target, c);

                    if (win)
                        ActiveEntities.Instance.GetDistrict(c.district).CriminalityChange(-Random.Range(5, 10));
                }
            }
        }
    }
}
public class AIGangsterStrategy : PlayerGangsterStrategy
{
    public override Char Recruit(Char c)
    {
        Char recruit = base.Recruit(c);
        if (recruit != null)
        {
            Org o = ActiveEntities.Instance.GetOrg(c.org);
            foreach (Char r in o.reserve)
                if (recruit.mental+recruit.social+recruit.physical<r.mental+r.social+r.physical && r.pay < o.money)
                    recruit = r;
            if (o.active.Count >= 33 || o.reserve.Count >= 10)
            { 
                foreach (Char r in o.reserve)
                    if (recruit.mental + recruit.social + recruit.physical > r.mental + r.social + r.physical)
                        recruit = r;
                recruit.Fire();
            }
            else if (o.active.Count >= 33) { }
            else if (o.money >= recruit.pay)
            {
                o.AddToActive(recruit);
                if (o.active[0].subordinates.Count > 0 && (o.GetActive(o.active[0].subordinates.LastOrDefault()).squadLeader))
                {
                    recruit.superior = o.active[0].name;
                    foreach (string s in o.active[0].subordinates)
                        if (o.GetActive(s).squadLeader)
                        {
                            recruit.AddSub(o.GetActive(s));
                            o.active[0].subordinates.Remove(s);
                        }
                }
                else if (o.active[0].subordinates.Count < 2)
                {
                    o.active[0].AddSub(recruit);
                }
                else
                {
                    for (int i = 0; i < 2; i++)
                    {
                        if (o.GetActive(o.active[0].subordinates[i]).subordinates.Count < 3)
                        {
                            o.GetActive(o.active[0].subordinates[i]).AddSub(recruit);
                            return null;
                        }
                        for (int j = 0; j < 3; j++)
                            if (o.GetActive(o.GetActive(o.active[0].subordinates[i]).subordinates[j]).subordinates.Count < 2)
                            {
                                o.active[0].strategy = "";
                                o.GetActive(o.active[0].subordinates[i]).strategy = "";
                                Char capitan = o.GetActive(o.GetActive(o.active[0].subordinates[i]).subordinates[j]);
                                if (i == 0)
                                {
                                    switch (j)
                                    {
                                        case 0: capitan.strategy = "Recruit"; break;
                                        case 1: capitan.strategy = "Extort"; break;
                                        case 2: capitan.strategy = "Punish"; break;
                                    }
                                }
                                else if (i == 1)
                                {
                                    switch (j)
                                    {
                                        case 0: capitan.strategy = "Rob"; break;
                                        case 1: capitan.strategy = "Patrol"; break;
                                        case 2: capitan.strategy = "Hunt"; break;
                                    }
                                }
                                capitan.AddSub(recruit);
                                if (capitan.subordinates.Count > 0)
                                    capitan.squadLeader = true;
                                else
                                    capitan.solo = true;
                                return null;
                            }
                    }
                    for (int i = 0; i < 2; i++)
                    {
                        if (o.GetActive(o.active[0].subordinates[i]).subordinates.Count < 3)
                        {
                            o.GetActive(o.active[0].subordinates[i]).AddSub(recruit);
                            return null;
                        }
                        for (int j = 0; j < 3; j++)
                            if (o.GetActive(o.GetActive(o.active[0].subordinates[i]).subordinates[j]).subordinates.Count < 4)
                            {
                                o.active[0].strategy = "";
                                o.GetActive(o.active[0].subordinates[i]).strategy = "";
                                Char capitan = o.GetActive(o.GetActive(o.active[0].subordinates[i]).subordinates[j]);
                                if (i == 0)
                                {
                                    switch (j)
                                    {
                                        case 0: capitan.strategy = "Recruit"; break;
                                        case 1: capitan.strategy = "Extort"; break;
                                        case 2: capitan.strategy = "Punish"; break;
                                    }
                                }
                                else if (i == 1)
                                {
                                    switch (j)
                                    {
                                        case 0: capitan.strategy = "Rob"; break;
                                        case 1: capitan.strategy = "Patrol"; break;
                                        case 2: capitan.strategy = "Hunt"; break;
                                    }
                                }
                                capitan.AddSub(recruit);
                                if (capitan.subordinates.Count > 0)
                                    capitan.squadLeader = true;
                                else
                                    capitan.solo = true;
                                return null;
                            }
                    }
                }
            }
        }
        return null;
    }
}