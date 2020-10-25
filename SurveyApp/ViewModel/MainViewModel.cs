using Structures.Tree;
using SurveyApp.Helper;
using SurveyApp.Model;

namespace SurveyApp.ViewModel
{
    public class MainViewModel
    {
        public SearchCriteria SearchCriteria { get; set; }

        public Location SelectedLocation { get; set; }

        public IBSPTree<Location> Properties { get; }

        public IBSPTree<Location> Sites { get; }

        public RelayCommand CanSearch { get; }

        public RelayCommand Search { get; }

        public RelayCommand Reset { get; }

        public RelayCommand New { get; }

        public RelayCommand CanUpdate { get; }

        public RelayCommand Update { get; }

        public RelayCommand CanDelete { get; }

        public RelayCommand Delete { get; }

        public RelayCommand Load { get; }

        public RelayCommand Save { get; }

        public MainViewModel()
        {

        }
    }
}
