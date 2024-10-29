using Acme.UserInfoCollector.Middleware.Enumerations;
using CommunityToolkit.Mvvm.ComponentModel;
namespace Acme.UserInfoCollector.Middleware
{
    public class PersonVM : ObservableObject
    {
        private string _FirstName = string.Empty;
        private string _Surname = string.Empty;
        private DateTime _DateOfBirth = DateTime.Now;
        private MaritalStatus? _MaritalStatus;
        private PersonVM? _PartnerInfo;

        /// <summary>
        /// First name of the user whose information is being collected
        /// </summary>
        public string FirstName { get => _FirstName; set => SetProperty(ref _FirstName, value); }

        /// <summary>
        /// Sirname/last name of the user whose information is being collected
        /// </summary>
        public string Surname { get => _Surname; set => SetProperty(ref _Surname, value); }

        /// <summary>
        /// User's date of birth
        /// </summary>
        public DateTime DateOfBirth { get => _DateOfBirth; set => SetProperty(ref _DateOfBirth, value); }

        /// <summary>
        /// Marital status of the user
        /// </summary>
        public MaritalStatus? MaritalStatus { get => _MaritalStatus; set => SetProperty(ref _MaritalStatus, value); }

        /// <summary>
        /// Partner information if user is married
        /// </summary>
        public PersonVM? PartnerInfo {  get => _PartnerInfo; set => SetProperty(ref _PartnerInfo, value); }
    }
}
