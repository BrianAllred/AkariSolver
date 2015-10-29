using System;
using Gtk;
using AkariSolver;

/// <summary>
/// Main window.
/// </summary>
public partial class MainWindow: Gtk.Window
{
    /// <summary>
    /// The board.
    /// </summary>
    public static GameBoard board;

    /// <summary>
    /// Initializes a new instance of the <see cref="MainWindow"/> class.
    /// </summary>
    public MainWindow()
        : base(Gtk.WindowType.Toplevel)
    {
        //GUI Builder
        Build();
        //Custom placement of controls because GTK# GUI Builder is not easy to work with
        global::Gtk.Fixed.FixedChild Generate = ((global::Gtk.Fixed.FixedChild)(this.fixed2[this.buttonGenerate]));
        Generate.X = this.WidthRequest / 2 - 10;
        global::Gtk.Fixed.FixedChild ByLabel = ((global::Gtk.Fixed.FixedChild)(this.fixed2[this.label3]));
        ByLabel.X = this.WidthRequest / 2 + buttonGenerate.WidthRequest / 2 - 20;
        global::Gtk.Fixed.FixedChild RowBox = ((global::Gtk.Fixed.FixedChild)(this.fixed2[this.entryRows]));
        RowBox.X = ByLabel.X - 50;
        global::Gtk.Fixed.FixedChild ColBox = ((global::Gtk.Fixed.FixedChild)(this.fixed2[this.entryCols]));
        ColBox.X = ByLabel.X + 45;
    }

    /// <summary>
    /// Raises the delete event event.
    /// </summary>
    /// <param name="sender">Sender.</param>
    /// <param name="a">The alpha component.</param>
    protected void OnDeleteEvent(object sender, DeleteEventArgs a)
    {
        Application.Quit();
        a.RetVal = true;
    }

    /// <summary>
    /// Generates a grid specified by user
    /// </summary>
    /// <param name="sender">Button Click, not used.</param>
    /// <param name="e">Button Click, not used.</param>
    protected void Generate(object sender, EventArgs e)
    {
        int r, c;
        //Try to convert the text in text boxes to ints
        try
        {
            r = Convert.ToInt32(entryRows.Text);
            c = Convert.ToInt32(entryCols.Text);
        }
        //if it fails, log it, but dont close. Maybe user just mistyped.
        catch (FormatException ex)
        {
            Console.WriteLine(ex.ToString());
            return;
        }
        if (CheckBounds(r, c))
        {
            board = new GameBoard(r, c);
            GridConstructor gc = new GridConstructor(r, c, this);
            gc.Show();
            gc.SetGrid();
        }
        else
        {
            //if bounds check is not met, then display a specific dialog.
            ErrorDialog error = new ErrorDialog(2);
            error.Show();
        }
    }

    /// <summary>
    /// Checks the bounds.
    /// </summary>
    /// <param name="r">Rows</param>
    /// <param name="c">Cols</param>
    bool CheckBounds(int r, int c)
    {
        if (r < 3 || c < 3)
            return false;
        return true;
    }
}
