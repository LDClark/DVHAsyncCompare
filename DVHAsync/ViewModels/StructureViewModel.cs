using System.Linq;
using VMS.TPS.Common.Model.API;
using VMS.TPS.Common.Model.Types;

namespace DVHAsync
{
    public class StructureViewModel
    {
        public string StructureId { get; set; }
        public string StructureCode { get; set; }
        public string StructureIdWithCode { get; set; }
        public Structure Structure { get; set; }
        public string VolumeValue { get; set; }
        public string VolumeUnit { get; set; }

        public StructureViewModel(Structure structure)
        {
            
            if (structure != null)
            {
                StructureId = structure.Id;
                StructureCode = structure.StructureCodeInfos.FirstOrDefault().Code;
                StructureIdWithCode = StructureId + " : " + StructureCode;
                Structure = structure;
                VolumeValue = structure.Volume.ToString("0.0");
                VolumeUnit = VolumePresentation.AbsoluteCm3.ToString();
            }
        }
    }
}
