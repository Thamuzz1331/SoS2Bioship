using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using SaveOurShip2;
using RimWorld.Planet;
using UnityEngine;
using Verse.AI.Group;

namespace RimWorld
{
    public class ShipBody
    {
        public Building_ShipHeart heart = null;
        public HashSet<Thing> shipFlesh = new HashSet<Thing>();
        public HashSet<Thing> otherFlesh = new HashSet<Thing>();
        public HashSet<Thing> adjacentMechanicals = new HashSet<Thing>();
        public HashSet<CompShipNutritionConsumer> consumers = new HashSet<CompShipNutritionConsumer>();
        public HashSet<CompShipNutritionStore> stores = new HashSet<CompShipNutritionStore>();
        public HashSet<CompShipNutritionSource> source = new HashSet<CompShipNutritionSource>();
        private int maxDepth = 5;
        public float currentNutrition = 0;
        public float nutritionCapacity = 0;
        public float passiveConsumption = 0;
        public float nutritionGen = 0;
        public float tempHunger = 0;

        public void Register(Building_ShipHeart _heart)
        {
            heart = _heart;
            heart.body = this;
            CompShipfleshConversion converter = ((ThingWithComps)_heart).TryGetComp<CompShipfleshConversion>();
            foreach (Thing flesh in shipFlesh)
            {
                CompShipBodyPart bodyComp = ((ThingWithComps)flesh).TryGetComp<CompShipBodyPart>();
                foreach(Thing t in bodyComp.GetScaff())
                {
                    converter.toConvert.Enqueue(t);
                }
                bodyComp.ClearScaff();
            }
            foreach (IMutation mutation in heart.mutations)
            {
                if (mutation.RunOnBodyParts())
                {
                    ApplyMutationToAll(mutation);
                }
            }

            Register(heart.GetComp<CompShipNutritionConsumer>());
            Register(heart.GetComp<CompShipNutritionStore>());
        }

        public void Register(CompShipBodyPart comp)
        {
            shipFlesh.Add(comp.parent);
            comp.body = this;
            if (heart != null)
            {
                ApplyMutations(comp.parent);
            }
            foreach (Thing t in comp.adjBodypart)
            {
                otherFlesh.Add(t);
            }
            comp.adjBodypart.Clear();
            foreach (Thing t in comp.adjMechs)
            {
                adjacentMechanicals.Add(t);
            }
            comp.adjMechs.Clear();
        }
        public void Register(CompShipNutrition comp)
        {
            if (comp is CompShipNutritionConsumer)
            {
                consumers.Add((CompShipNutritionConsumer)comp);
                passiveConsumption += ((CompShipNutritionConsumer)comp).getConsumptionPerPulse();
            }
            if (comp is CompShipNutritionSource)
            {
                source.Add((CompShipNutritionSource)comp);
                nutritionGen += ((CompShipNutritionSource)comp).getNutritionPerPulse();
            }
            if (comp is CompShipNutritionStore)
            {
                stores.Add((CompShipNutritionStore)comp);
                nutritionCapacity += ((CompShipNutritionStore)comp).getNutrientCapacity();
                currentNutrition += ((CompShipNutritionStore)comp).getCurrentNutrition();
            }
            comp.body = this;
        }

        public void ApplyMutationToAll(IMutation mutation)
        {
            foreach (Thing bodypart in shipFlesh)
            {
                mutation.Apply(bodypart);
            }
        }

        public void ApplyMutations(Thing target)
        {
            foreach (IMutation mutation in heart.mutations)
            {
                if (mutation.RunOnBodyParts())
                {
                    mutation.Apply(target);
                }
            }
        }

        public void DeRegister(CompShipBodyPart comp)
        {
            shipFlesh.Remove(comp.parent);
        }
        public void DeRegister(CompShipNutrition comp)
        {
            if (comp is CompShipNutritionConsumer)
            {
                consumers.Remove((CompShipNutritionConsumer)comp);
                passiveConsumption -= ((CompShipNutritionConsumer)comp).getConsumptionPerPulse();
            }
            if (comp is CompShipNutritionStore)
            {
                stores.Remove((CompShipNutritionStore)comp);
                nutritionCapacity -= ((CompShipNutritionStore)comp).getNutrientCapacity();
                currentNutrition -= ((CompShipNutritionStore)comp).getNutrientCapacity();
            }
            if (comp is CompShipNutritionSource)
            {
                source.Add((CompShipNutritionSource)comp);
                nutritionGen += ((CompShipNutritionSource)comp).getNutritionPerPulse();
            }
        }

        public void Regen(CompShipBodyPart _toRegen)
        {
            if (heart != null)
            {
                ((ThingWithComps)heart).TryGetComp<CompShipfleshConversion>().toRegen.Push(_toRegen.parent);
            }
        }

        public void UpdateNutritionGeneration()
        {
            nutritionGen = 0;
            foreach(CompShipNutritionSource c in source)
            {
                nutritionGen += c.getNutritionPerPulse();
            }
        }
        public void UpdateNutritionCapacity()
        {
            nutritionCapacity = 0;
            foreach (CompShipNutritionStore c in stores)
            {
                nutritionCapacity += c.getNutrientCapacity();
            }
        }
        public void UpdateCurrentNutrition()
        {
            currentNutrition = 0;
            foreach (CompShipNutritionStore c in stores)
            {
                currentNutrition += c.getCurrentNutrition();
            }
        }
        public void UpdatePassiveConsumption()
        {
            passiveConsumption = 1*shipFlesh.Count;
            foreach (CompShipNutritionConsumer c in consumers)
            {
                passiveConsumption += c.getConsumptionPerPulse();
            }
        }
        public bool RequestNutrition(float qty)
        {
            float available = nutritionGen + currentNutrition - passiveConsumption - tempHunger;
            if (qty > available)
                return false;

            ExtractNutrition(stores, qty, 0);
            currentNutrition -= qty;
            return true;
        }
        public float ConsumeNutrition(float qty)
        {
            tempHunger += qty;

            return qty;
        }

        public void RunNutrition()
        {
            UpdatePassiveConsumption();
            UpdateNutritionGeneration();
            UpdateCurrentNutrition();
            float net = nutritionGen - passiveConsumption - tempHunger;
            if (net > 0)
            {
                float toStore = net * 0.5f;
                if (heart != null)
                {
                    toStore = toStore * heart.getStatMultiplier("storageEfficiency", null);
                }
                float leftover = 0;
                if ((nutritionCapacity - currentNutrition) <= 0)
                {
                    leftover = toStore;
                } 
                else if (toStore >= (nutritionCapacity - currentNutrition)) 
                {
                    leftover = toStore - (nutritionCapacity - currentNutrition);
                    currentNutrition = nutritionCapacity;
                    foreach (CompShipNutritionStore store in stores)
                    {
                        store.currentNutrition = store.getNutrientCapacity();
                    }
                } else
                {
                    leftover = StoreNutrition(stores, toStore, 0);
                }
            }
            if (net < 0)
            {
                float deficit = 0;
                net = net * -1;
                if (net > currentNutrition)
                {
                    deficit = net - currentNutrition;
                    currentNutrition = 0;
                    foreach (CompShipNutritionStore store in stores)
                    {
                        store.currentNutrition = 0;
                    }
                    while (deficit > 0 && shipFlesh.Count > 0)
                    {
                        int hurtIndex = Rand.Range(0, shipFlesh.Count);
                        Building b = ((Building)shipFlesh.ElementAt(hurtIndex));
                        b.HitPoints -= 100;
                        if (b.HitPoints <= 0)
                        {
                            b.Destroy(DestroyMode.KillFinalize);
                        }
                        deficit -= 100;
                    }

                }
                else
                {
                    ExtractNutrition(stores, net, 0);
                }
            }
            tempHunger = 0;
        }

        public float StoreNutrition(HashSet<CompShipNutritionStore> _stores, float toStore, int depth)
        {
            if (_stores.Count == 0 || depth > maxDepth)
            {
                return toStore;
            }
            float leftOver = 0;
            HashSet<CompShipNutritionStore> retainCapactiy = new HashSet<CompShipNutritionStore>();
            float storeEach = toStore/_stores.Count;
            foreach (CompShipNutritionStore s in _stores)
            {
                leftOver += s.storeNutrition(storeEach);
                if (s.currentNutrition < s.getNutrientCapacity())
                {
                    retainCapactiy.Add(s);
                }
            }

            if (leftOver <= 0)
            {
                return 0;
            } 
            else
            {
                return StoreNutrition(retainCapactiy, leftOver, depth+1);
            }
        }
        public float ExtractNutrition(HashSet<CompShipNutritionStore> _stores, float toExtract, int depth)
        {
            if (_stores.Count <= 0 || depth > maxDepth)
            {
                return toExtract;
            }
            HashSet<CompShipNutritionStore> retainNutrition = new HashSet<CompShipNutritionStore>();
            float localExtract = toExtract/_stores.Count;
            float remainingHunger = 0;
            foreach (CompShipNutritionStore s in _stores)
            {
                remainingHunger += s.consumeNutrition(localExtract);
                if (s.currentNutrition > 0)
                {
                    retainNutrition.Add(s);
                }
            }
            if (remainingHunger <= 0 || retainNutrition.Count == 0)
            {
                return 0;
            } else
            {
                return ExtractNutrition(retainNutrition, remainingHunger, depth+1);
            }
        }
    }
}