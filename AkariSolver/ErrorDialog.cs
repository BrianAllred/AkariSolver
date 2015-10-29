using System;

namespace AkariSolver
{
	/// <summary>
	/// Error dialog.
	/// </summary>
    public partial class ErrorDialog : Gtk.Dialog
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AkariSolver.ErrorDialog"/> class.
        /// </summary>
        /// <param name="mode">Mode.</param>
        public ErrorDialog(int mode)
        {
            this.Build();
            switch (mode)
            {
                case 1:
                    ErrorLabel1.Text = "Not solved!";
                    break;
                case 2:
                    ErrorLabel1.Text = "Parameters out of bounds!";
                    break;
                default:
                    ErrorLabel1.Text = "Critical error! Contact developer!";
                    break;
            }
        }

        /// <summary>
        /// Raises the button ok clicked event.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">E.</param>
        protected void OnButtonOkClicked(object sender, EventArgs e)
        {
            this.Destroy();
        }
    }
}

