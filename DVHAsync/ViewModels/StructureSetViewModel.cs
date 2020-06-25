using System.Collections.ObjectModel;
using System.Linq;
using VMS.TPS.Common.Model.API;

namespace DVHAsync
{
    public class StructureSetViewModel
    {
        public ObservableCollection<StructureViewModel> StructureList { get; set; }

        public StructureSetViewModel(StructureSet structureSet)
        {
            var StructureList = new ObservableCollection<StructureViewModel>();
            foreach (Structure structure in structureSet.Structures)
            {
                if (!structure.IsEmpty && structure.DicomType != "SUPPORT")
                {
                    var structureViewModel = new StructureViewModel(structure);
                    StructureList.Add(structureViewModel);
                }
            }
            StructureList = new ObservableCollection<StructureViewModel>(StructureList.OrderBy(x => x.StructureId));
        }
    }
}
