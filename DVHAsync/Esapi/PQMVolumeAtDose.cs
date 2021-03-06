﻿using System.Text.RegularExpressions;
using VMS.TPS.Common.Model.API;
using VMS.TPS.Common.Model.Types;

namespace DVHAsyncCompare
{
    class PQMVolumeAtDose
    {
        public static string GetVolumeAtDose(StructureSet structureSet, PlanningItemViewModel planningItem, StructureViewModel evalStructure, MatchCollection testMatch, Group evalunit)
        {
            //check for sufficient sampling and dose coverage
            DVHData dvh = planningItem.PlanningItemObject.GetDVHCumulativeData(evalStructure.Structure, DoseValuePresentation.Absolute, VolumePresentation.Relative, 0.1);
            //MessageBox.Show(evalStructure.Id + "- Eval unit: " + evalunit.Value.ToString() + "Achieved unit: " + dvAchieved.UnitAsString + " - Sampling coverage: " + dvh.SamplingCoverage.ToString() + " Coverage: " + dvh.Coverage.ToString());
            if ((dvh.SamplingCoverage < 0.9) || (dvh.Coverage < 0.9))
            {
                return "Unable to calculate - insufficient dose or sampling coverage";
            }
            Group eval = testMatch[0].Groups["evalpt"];
            Group unit = testMatch[0].Groups["unit"];
            DoseValue.DoseUnit du = (unit.Value.CompareTo("%") == 0) ? DoseValue.DoseUnit.Percent :
                    (unit.Value.CompareTo("cGy") == 0) ? DoseValue.DoseUnit.cGy : DoseValue.DoseUnit.Unknown;
            VolumePresentation vp = (unit.Value.CompareTo("%") == 0) ? VolumePresentation.Relative : VolumePresentation.AbsoluteCm3;
            DoseValue dv = new DoseValue(double.Parse(eval.Value), du);
            double volume = double.Parse(eval.Value);
            VolumePresentation vpFinal = (evalunit.Value.CompareTo("%") == 0) ? VolumePresentation.Relative : VolumePresentation.AbsoluteCm3;
            DoseValuePresentation dvpFinal = (evalunit.Value.CompareTo("%") == 0) ? DoseValuePresentation.Relative : DoseValuePresentation.Absolute;
            if (planningItem.PlanningItemObject is PlanSum)
            {
                double planDoseDouble = 0;
                PlanSum planSum = (PlanSum)planningItem.PlanningItemObject;
                foreach (PlanSetup planSetup in planSum.PlanSetups)
                {
                    planDoseDouble += planSetup.TotalDose.Dose;
                }
                dv = new DoseValue(planDoseDouble, DoseValue.DoseUnit.cGy);
            }

            double volumeAchieved = planningItem.PlanningItemObject.GetVolumeAtDose(evalStructure.Structure, dv, vpFinal);
            return string.Format("{0:0.00} {1}", volumeAchieved, evalunit.Value);   // todo: better formatting based on VolumePresentation
                                                                                    //#if false
                                                                                    //            string message = string.Format("{0} - Dose unit = {1}, Volume Presentation = {2}, vpFinal = {3}, dvpFinal ={4}", 
                                                                                    //               objective.DVHObjective, du.ToString(), vp.ToString(), vpFinal.ToString(), dvpFinal.ToString());
                                                                                    //           MessageBox.Show(message);
                                                                                    //#endif
        }
    }
}
