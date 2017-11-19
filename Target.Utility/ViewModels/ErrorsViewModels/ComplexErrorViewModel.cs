using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            get { return _complexExceptionDetails; }
            set
            {
                _complexExceptionDetails = value; 
                OnPropertyChanged(nameof(ComplexExceptionDetails));
            }
        }


        #endregion

        #region Methods

        #region Public
        #endregion

        #region Protected
        #endregion

        #region Private
        #endregion

        #endregion

    }
}
