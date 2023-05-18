﻿using HarmonyLib;
using Kingmaker.Blueprints.Classes.Prerequisites;
using Kingmaker.UnitLogic;
using Kingmaker.UnitLogic.Class.LevelUp;
using System.Linq;

namespace Swashbuckler.Patches
{
    /// <summary>
    /// Logic for FeatureForPrerequisite. Pretense unit has a feature during feature selection.
    /// </summary>
    [HarmonyPatch(typeof(Prerequisite), nameof(Prerequisite.Check))]
    public class Patch_Prerequisite
    {
        public static void Postfix(FeatureSelectionState selectionState, UnitDescriptor unit, LevelUpState state, Prerequisite __instance, ref bool __result)
        {
            if (__result)
                return;

            var fakes = unit.Progression.Features.SelectFactComponents<FeatureForPrerequisite>();
            if (!fakes.Any())
                return;

            switch (__instance)
            {
                case PrerequisiteFeature p:
                    __result = fakes.Any(a => a.FakeFact.Equals(p.m_Feature));
                    return;
                case PrerequisiteFeaturesFromList p:
                    int num = 0;
                    foreach (var featurePrerequisite in p.Features)
                    {
                        if ((selectionState == null || !selectionState.IsSelectedInChildren(featurePrerequisite)) && (unit.HasFact(featurePrerequisite) || fakes.Any(a => a.FakeFact.Is(featurePrerequisite))))
                        {
                            num++;
                            if (num >= p.Amount)
                            {
                                __result = true;
                                return;
                            }
                        }
                    }
                    return;
            }
        }
    }
}