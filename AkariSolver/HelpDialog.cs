using System;

namespace AkariSolver
{
    /// <summary>
    /// Help dialog.
    /// </summary>
    public partial class HelpDialog : Gtk.Dialog
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AkariSolver.HelpDialog"/> class.
        /// </summary>
        public HelpDialog()
        {
            this.Build();
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

