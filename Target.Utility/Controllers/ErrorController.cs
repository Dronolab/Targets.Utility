using System;
using Target.Utility.ViewModels.ErrorsViewModels;
using Target.Utility.Windows.ErrorWindows;

namespace Target.Utility.Controllers
{
    public class ErrorController : IErrorController
    {
        public void ShowException(Exception ex)
        {
            var viewModel = new SimpleErrorViewModel(ex);
            var window = new SimpleErrorWindow(viewModel);
            viewModel.Window = window;

            window.ShowDialog();
        }

        public void ShowExceptionDetailed(Exception ex)
        {
            var viewModel = new ComplexErrorViewModel(ex);
            var window = new ComplexErrorWindow(viewModel);
            viewModel.Window = window;

            window.ShowDialog();
        }
    }
}
