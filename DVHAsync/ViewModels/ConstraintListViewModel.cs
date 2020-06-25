using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;

namespace DVHAsync
{
    class ConstraintListViewModel
    {
        static public ObservableCollection<ConstraintViewModel> GetConstraintList(string constraintDir)
        {
            var ConstraintComboBoxList = new ObservableCollection<ConstraintViewModel>();
            foreach (string file in Directory.EnumerateFiles(constraintDir, "*.csv"))
            {
                var constraintViewModel = new ConstraintViewModel(file);
                ConstraintComboBoxList.Add(constraintViewModel);
            }
            return ConstraintComboBoxList;
        }
    }
}
