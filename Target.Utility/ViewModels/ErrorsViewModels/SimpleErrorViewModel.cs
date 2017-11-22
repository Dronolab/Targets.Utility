using System;

namespace Target.Utility.ViewModels.ErrorsViewModels
{
    public class SimpleErrorViewModel : ViewModel
    {

        #region Fields

        private string _exceptionSimpleDetails;

        #endregion

        #region Constructors

        public SimpleErrorViewModel(Exception exception)
        {
            this.ExceptionSimpleDetails = exception.Message;
        }

        #endregion

        #region Properties

        public string ExceptionSimpleDetails
        {
            get => _exceptionSimpleDetails;
            set
            {
                _exceptionSimpleDetails = value;
                OnPropertyChanged(nameof(ExceptionSimpleDetails));
            }
        }

        #endregion

    }
}
