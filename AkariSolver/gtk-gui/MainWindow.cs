
// This file has been generated by the GUI designer. Do not modify.

public partial class MainWindow
{
	private global::Gtk.Fixed fixed2;
	
	private global::Gtk.Label lbl1;
	
	private global::Gtk.Entry entryCols;
	
	private global::Gtk.Label label3;
	
	private global::Gtk.Button buttonGenerate;
	
	private global::Gtk.Entry entryRows;

	protected virtual void Build ()
	{
		global::Stetic.Gui.Initialize (this);
		// Widget MainWindow
		this.WidthRequest = 200;
		this.Name = "MainWindow";
		this.Title = global::Mono.Unix.Catalog.GetString ("Akari Solver");
		this.WindowPosition = ((global::Gtk.WindowPosition)(1));
		// Container child MainWindow.Gtk.Container+ContainerChild
		this.fixed2 = new global::Gtk.Fixed ();
		this.fixed2.Name = "fixed2";
		this.fixed2.HasWindow = false;
		// Container child fixed2.Gtk.Fixed+FixedChild
		this.lbl1 = new global::Gtk.Label ();
		this.lbl1.Name = "lbl1";
		this.lbl1.LabelProp = global::Mono.Unix.Catalog.GetString ("Enter size of Akari grid to generate:");
		this.fixed2.Add (this.lbl1);
		global::Gtk.Fixed.FixedChild w1 = ((global::Gtk.Fixed.FixedChild)(this.fixed2 [this.lbl1]));
		w1.X = 14;
		w1.Y = 26;
		// Container child fixed2.Gtk.Fixed+FixedChild
		this.entryCols = new global::Gtk.Entry ();
		this.entryCols.CanFocus = true;
		this.entryCols.Name = "entryCols";
		this.entryCols.Text = global::Mono.Unix.Catalog.GetString ("C");
		this.entryCols.IsEditable = true;
		this.entryCols.WidthChars = 2;
		this.entryCols.MaxLength = 2;
		this.entryCols.InvisibleChar = '●';
		this.fixed2.Add (this.entryCols);
		global::Gtk.Fixed.FixedChild w2 = ((global::Gtk.Fixed.FixedChild)(this.fixed2 [this.entryCols]));
		w2.X = 151;
		w2.Y = 68;
		// Container child fixed2.Gtk.Fixed+FixedChild
		this.label3 = new global::Gtk.Label ();
		this.label3.Name = "label3";
		this.label3.LabelProp = global::Mono.Unix.Catalog.GetString ("by");
		this.fixed2.Add (this.label3);
		global::Gtk.Fixed.FixedChild w3 = ((global::Gtk.Fixed.FixedChild)(this.fixed2 [this.label3]));
		w3.X = 108;
		w3.Y = 71;
		// Container child fixed2.Gtk.Fixed+FixedChild
		this.buttonGenerate = new global::Gtk.Button ();
		this.buttonGenerate.WidthRequest = 75;
		this.buttonGenerate.CanFocus = true;
		this.buttonGenerate.Name = "buttonGenerate";
		this.buttonGenerate.UseUnderline = true;
		this.buttonGenerate.Label = global::Mono.Unix.Catalog.GetString ("Generate");
		this.fixed2.Add (this.buttonGenerate);
		global::Gtk.Fixed.FixedChild w4 = ((global::Gtk.Fixed.FixedChild)(this.fixed2 [this.buttonGenerate]));
		w4.X = 84;
		w4.Y = 131;
		// Container child fixed2.Gtk.Fixed+FixedChild
		this.entryRows = new global::Gtk.Entry ();
		this.entryRows.CanFocus = true;
		this.entryRows.Name = "entryRows";
		this.entryRows.Text = global::Mono.Unix.Catalog.GetString ("R");
		this.entryRows.IsEditable = true;
		this.entryRows.WidthChars = 2;
		this.entryRows.MaxLength = 2;
		this.entryRows.InvisibleChar = '●';
		this.fixed2.Add (this.entryRows);
		global::Gtk.Fixed.FixedChild w5 = ((global::Gtk.Fixed.FixedChild)(this.fixed2 [this.entryRows]));
		w5.X = 54;
		w5.Y = 68;
		this.Add (this.fixed2);
		if ((this.Child != null)) {
			this.Child.ShowAll ();
		}
		this.DefaultWidth = 260;
		this.DefaultHeight = 196;
		this.Show ();
		this.DeleteEvent += new global::Gtk.DeleteEventHandler (this.OnDeleteEvent);
		this.buttonGenerate.Clicked += new global::System.EventHandler (this.Generate);
	}
}