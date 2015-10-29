using System;

namespace AkariSolver
{
    /// <summary>
    /// Victory dialog.
    /// </summary>
    public partial class VictoryDialog : Gtk.Dialog
    {
        /// <summary>
        /// The parent Grid constructor.
        /// </summary>
        protected GridConstructor gc;

        /// <summary>
        /// Initializes a new instance of the <see cref="AkariSolver.VictoryDialog"/> class.
        /// </summary>
        /// <param name="gridConstructor">The parent Grid constructor.</param>
        public VictoryDialog(GridConstructor gridConstructor)
        {
            this.Build();
            gc = gridConstructor;
        }

        /// <summary>
        /// Raises the button OK clicked event. Closes only this dialog.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">E.</param>
        protected void OnButtonOKClicked(object sender, EventArgs e)
        {
            this.Destroy();
        }

        /// <summary>
        /// Raises the button again clicked event. Presents the parent Grid constructor's parent, destroys parent Grid constructor, destroys this dialog.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">E.</param>
        protected void OnButtonAgainClicked(object sender, EventArgs e)
        {
            gc.main.Present();
            gc.Destroy();
            this.Destroy();
        }
    }
}

