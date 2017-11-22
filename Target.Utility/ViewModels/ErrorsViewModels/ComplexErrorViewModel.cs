using System;

namespace Target.Utility.ViewModels.ErrorsViewModels
{
    public class ComplexErrorViewModel : SimpleErrorViewModel
    {

        #region Fields

        private string _complexExceptionDetails;

        #endregion

        #region Constructors
        public ComplexErrorViewModel(Exception exception) : base(exception)
        {
            this.ComplexExceptionDetails = exception.ToString();
        }

        #endregion

        #region Properties

        public string ComplexExceptionDetails
        {
            get => _complexExceptionDetails;
            set
            {
                _complexExceptionDetails = value; 
                OnPropertyChanged(nameof(ComplexExceptionDetails));
            }
        }


        #endregion

    }
}
