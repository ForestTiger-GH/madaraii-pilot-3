using Microsoft.Win32;
using MiniDoc.Docx;
using MiniDoc.Editor;
using MiniDoc.Pdf;
using MiniDoc.Storage;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Ribbon;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace MiniDoc;

public partial class MainWindow : RibbonWindow
{
    private enum DocumentMode { None, EditableDocx, ReadOnlyDocx, Pdf }

    private DocumentMode _mode;
    private DocxSession? _docxSession;
    private PdfSession? _pdfSession;
    private string? _currentPath;
    private bool _dirty;
    private bool _suppressDirty;
    private bool _pdfRendering;
    private uint _pdfPage;
    private int _zoomIndex = 2;
    private static readonly double[] ZoomLevels = [0.50, 0.75, 1.00, 1.25, 1.50, 2.00, 3.00, 4.00];

    public MainWindow()
    {
        InitializeComponent();
        _suppressDirty = true;
        FontFamilyBox.SelectedIndex = 0;
        FontSizeBox.SelectedIndex = 3;
        ColorBox.SelectedIndex = 0;
        HighlightBox.SelectedIndex = 0;
        ParagraphFillBox.SelectedIndex = 0;
        CellFillBox.SelectedIndex = 0;
        _suppressDirty = false;
        StartNewDocument();
    }

    private bool IsEditableDocx => _mode == DocumentMode.EditableDocx;
    private bool IsDocxView => _mode is DocumentMode.EditableDocx or DocumentMode.ReadOnlyDocx;

    private void New_Click(object sender, RoutedEventArgs e)
    {
        if (ResolveUnsavedChanges()) StartNewDocument();
    }

    private async void Open_Click(object sender, RoutedEventArgs e)
    {
        if (!ResolveUnsavedChanges()) return;
        var dialog = new OpenFileDialog
        {
            Title = "Open document",
            Filter = "Supported documents (*.docx;*.pdf)|*.docx;*.pdf|Word documents (*.docx)|*.docx|PDF documents (*.pdf)|*.pdf",
            Multiselect = false,
            CheckFileExists = true
        };
        if (dialog.ShowDialog(this) == true) await OpenPathAsync(dialog.FileName);
    }

    private void Save_Click(object sender, RoutedEventArgs e) => SaveCurrent(false);
    private void SaveAs_Click(object sender, RoutedEventArgs e) => SaveCurrent(true);
    private void Exit_Click(object sender, RoutedEventArgs e) => Close();

    private void StartNewDocument()
    {
        ReleasePdf();
        var opened = DocxCodec.CreateNew();
        _docxSession = opened.Session;
        _currentPath = null;
        _mode = DocumentMode.EditableDocx;
        SetEditorDocument(opened.Document, false);
        _dirty = false;
        ShowNotice(null);
        StatusText.Text = "New DOCX";
        UpdateUi();
        Editor.Focus();
    }

    private async Task OpenPathAsync(string path)
    {
        try
        {
            var extension = Path.GetExtension(path);
            if (extension.Equals(".docx", StringComparison.OrdinalIgnoreCase))
            {
                ReleasePdf();
                var opened = DocxCodec.Open(path);
                _docxSession = opened.Session;
                _currentPath = path;
                _mode = opened.IsEditable ? DocumentMode.EditableDocx : DocumentMode.ReadOnlyDocx;
                SetEditorDocument(opened.Document, !opened.IsEditable);
                _dirty = false;
                ShowNotice(opened.IsEditable ? null : opened.Message);
                StatusText.Text = opened.IsEditable ? "Editable DOCX" : "DOCX compatibility view (read-only)";
                UpdateUi();
                return;
            }

            if (extension.Equals(".pdf", StringComparison.OrdinalIgnoreCase))
            {
                await OpenPdfAsync(path);
                return;
            }

            MessageBox.Show(this, "MiniDoc supports .docx and .pdf files.", "Unsupported file", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, ex.Message, "Open failed", MessageBoxButton.OK, MessageBoxImage.Error);
            StatusText.Text = "Open failed";
        }
    }

    private void SetEditorDocument(FlowDocument document, bool readOnly)
    {
        _suppressDirty = true;
        Editor.Document = document;
        Editor.IsReadOnly = readOnly;
        Editor.Visibility = Visibility.Visible;
        PdfHost.Visibility = Visibility.Collapsed;
        _suppressDirty = false;
    }

    private async Task OpenPdfAsync(string path)
    {
        ReleasePdf();
        _docxSession = null;
        _currentPath = path;
        _dirty = false;
        _pdfSession = await PdfSession.OpenAsync(path);
        _pdfPage = 0;
        _zoomIndex = 2;
        _mode = DocumentMode.Pdf;
        Editor.Visibility = Visibility.Collapsed;
        PdfHost.Visibility = Visibility.Visible;
        ShowNotice(null);
        UpdateUi();
        await RenderPdfAsync();
    }

    private async Task RenderPdfAsync()
    {
        if (_pdfSession is null || _pdfRendering) return;
        _pdfRendering = true;
        try
        {
            StatusText.Text = "Rendering PDF page...";
            PdfImage.Source = await _pdfSession.RenderPageAsync(_pdfPage, ZoomLevels[_zoomIndex]);
            StatusText.Text = "PDF view (read-only)";
        }
        catch (Exception ex)
        {
            StatusText.Text = "PDF render failed";
            MessageBox.Show(this, ex.Message, "PDF render failed", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        finally
        {
            _pdfRendering = false;
            UpdatePdfControls();
        }
    }

    private async void PrevPdf_Click(object sender, RoutedEventArgs e)
    {
        if (_pdfSession is null || _pdfPage == 0) return;
        _pdfPage--;
        await RenderPdfAsync();
    }

    private async void NextPdf_Click(object sender, RoutedEventArgs e)
    {
        if (_pdfSession is null || _pdfPage + 1 >= _pdfSession.PageCount) return;
        _pdfPage++;
        await RenderPdfAsync();
    }

    private async void ZoomOut_Click(object sender, RoutedEventArgs e)
    {
        if (_zoomIndex == 0) return;
        _zoomIndex--;
        await RenderPdfAsync();
    }

    private async void ZoomIn_Click(object sender, RoutedEventArgs e)
    {
        if (_zoomIndex + 1 >= ZoomLevels.Length) return;
        _zoomIndex++;
        await RenderPdfAsync();
    }

    private bool SaveCurrent(bool forceSaveAs)
    {
        if (!IsEditableDocx || _docxSession is null) return false;
        var path = _currentPath;

        if (forceSaveAs || string.IsNullOrWhiteSpace(path))
        {
            var dialog = new SaveFileDialog
            {
                Title = "Save DOCX",
                Filter = "Word document (*.docx)|*.docx",
                AddExtension = true,
                DefaultExt = ".docx",
                FileName = string.IsNullOrWhiteSpace(path) ? "Document.docx" : Path.GetFileName(path),
                OverwritePrompt = true
            };
            if (dialog.ShowDialog(this) != true) return false;
            path = dialog.FileName;
        }

        try
        {
            var bytes = DocxCodec.SaveToBytes(_docxSession, Editor.Document);
            DocumentWriter.WriteExplicitTarget(path!, bytes);
            var rebased = DocxCodec.LoadBytes(bytes);
            if (!rebased.IsEditable || rebased.Session is null)
                throw new InvalidOperationException("The saved document failed MiniDoc's compatibility check.");

            _docxSession = rebased.Session;
            _currentPath = path;
            _dirty = false;
            StatusText.Text = "Saved";
            UpdateUi();
            return true;
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, ex.Message, "Save failed", MessageBoxButton.OK, MessageBoxImage.Error);
            StatusText.Text = "Save failed";
            return false;
        }
    }

    private bool ResolveUnsavedChanges()
    {
        if (!IsEditableDocx || !_dirty) return true;
        var result = MessageBox.Show(this, "Save changes to the current DOCX?", "MiniDoc", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
        return result switch
        {
            MessageBoxResult.Yes => SaveCurrent(false),
            MessageBoxResult.No => true,
            _ => false
        };
    }

    private void Editor_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (!_suppressDirty && IsEditableDocx) MarkDirty();
    }

    private void Editor_SelectionChanged(object sender, RoutedEventArgs e) => UpdateTableControls();

    private void Editor_PreviewExecuted(object sender, ExecutedRoutedEventArgs e)
    {
        if (e.Command == ApplicationCommands.Paste && IsEditableDocx)
        {
            if (Clipboard.ContainsText(TextDataFormat.UnicodeText))
            {
                Editor.Selection.Text = Clipboard.GetText(TextDataFormat.UnicodeText);
                MarkDirty();
            }
            e.Handled = true;
        }
    }

    private void FontFamilyBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (_suppressDirty || !IsEditableDocx || FontFamilyBox.SelectedItem is not ComboBoxItem item) return;
        Editor.Selection.ApplyPropertyValue(TextElement.FontFamilyProperty, new FontFamily(item.Content?.ToString() ?? "Segoe UI"));
        MarkDirty();
    }

    private void FontSizeBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (_suppressDirty || !IsEditableDocx || FontSizeBox.SelectedItem is not ComboBoxItem item) return;
        if (!double.TryParse(item.Content?.ToString(), out var points)) return;
        Editor.Selection.ApplyPropertyValue(TextElement.FontSizeProperty, points * 96.0 / 72.0);
        MarkDirty();
    }

    private void ColorBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (_suppressDirty || !IsEditableDocx || ColorBox.SelectedItem is not ComboBoxItem item || item.Tag is not string value) return;
        Editor.Selection.ApplyPropertyValue(TextElement.ForegroundProperty, ParseBrush(value));
        MarkDirty();
    }

    private void HighlightBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (_suppressDirty || !IsEditableDocx || HighlightBox.SelectedItem is not ComboBoxItem item || item.Tag is not string value) return;
        DocumentFormatting.ApplyTextHighlight(Editor, ParseBrush(value));
        MarkDirty();
    }

    private void ParagraphFillBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (_suppressDirty || !IsEditableDocx || ParagraphFillBox.SelectedItem is not ComboBoxItem item || item.Tag is not string value) return;
        DocumentFormatting.ApplyParagraphFill(Editor, ParseBrush(value));
        MarkDirty();
    }

    private void CellFillBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (_suppressDirty || !IsEditableDocx || CellFillBox.SelectedItem is not ComboBoxItem item || item.Tag is not string value) return;
        var context = TableEditor.GetContext(Editor.CaretPosition);
        if (context is null) return;
        TableEditor.SetCurrentCellFill(context, ParseBrush(value));
        MarkDirty();
    }

    private void IndentIncrease_Click(object sender, RoutedEventArgs e) { if (IsEditableDocx) { DocumentFormatting.ChangeIndent(Editor, 24); MarkDirty(); } }
    private void IndentDecrease_Click(object sender, RoutedEventArgs e) { if (IsEditableDocx) { DocumentFormatting.ChangeIndent(Editor, -24); MarkDirty(); } }
    private void SpaceIncrease_Click(object sender, RoutedEventArgs e) { if (IsEditableDocx) { DocumentFormatting.ChangeSpaceAfter(Editor, 8); MarkDirty(); } }
    private void SpaceDecrease_Click(object sender, RoutedEventArgs e) { if (IsEditableDocx) { DocumentFormatting.ChangeSpaceAfter(Editor, -8); MarkDirty(); } }

    private void InsertTable_Click(object sender, RoutedEventArgs e)
    {
        if (!IsEditableDocx) return;
        try
        {
            var table = TableEditor.InsertTable(Editor.Document, Editor.CaretPosition, 3, 3);
            MarkDirty();
            table.BringIntoView();
            UpdateTableControls();
        }
        catch (Exception ex) { MessageBox.Show(this, ex.Message, "Table", MessageBoxButton.OK, MessageBoxImage.Information); }
    }

    private void AddTableRow_Click(object sender, RoutedEventArgs e)
    {
        var context = TableEditor.GetContext(Editor.CaretPosition); if (!IsEditableDocx || context is null) return;
        TableEditor.AddRow(context); MarkDirty(); UpdateTableControls();
    }

    private void DeleteTableRow_Click(object sender, RoutedEventArgs e)
    {
        var context = TableEditor.GetContext(Editor.CaretPosition); if (!IsEditableDocx || context is null) return;
        TableEditor.DeleteCurrentRow(context, Editor.Document); MarkDirty(); UpdateTableControls();
    }

    private void AddTableColumn_Click(object sender, RoutedEventArgs e)
    {
        var context = TableEditor.GetContext(Editor.CaretPosition); if (!IsEditableDocx || context is null) return;
        TableEditor.AddColumn(context); MarkDirty(); UpdateTableControls();
    }

    private void DeleteTableColumn_Click(object sender, RoutedEventArgs e)
    {
        var context = TableEditor.GetContext(Editor.CaretPosition); if (!IsEditableDocx || context is null) return;
        TableEditor.DeleteCurrentColumn(context, Editor.Document); MarkDirty(); UpdateTableControls();
    }

    private void FindNext_Click(object sender, RoutedEventArgs e) => FindNext();

    private void FindBox_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter) { FindNext(); e.Handled = true; }
    }

    private void FindNext()
    {
        if (!IsDocxView || string.IsNullOrEmpty(FindBox.Text)) return;
        var match = TextSearch.FindNext(Editor.Document, FindBox.Text, MatchCase(), Editor.Selection.End);
        if (match is null)
        {
            StatusText.Text = "Text not found";
            return;
        }
        Editor.Selection.Select(match.Start, match.End);
        Editor.Focus();
        StatusText.Text = "Match selected";
    }

    private void Replace_Click(object sender, RoutedEventArgs e)
    {
        if (!IsEditableDocx || string.IsNullOrEmpty(FindBox.Text)) return;
        var comparison = MatchCase() ? StringComparison.CurrentCulture : StringComparison.CurrentCultureIgnoreCase;
        if (string.Equals(Editor.Selection.Text, FindBox.Text, comparison))
        {
            Editor.Selection.Text = ReplaceBox.Text;
            MarkDirty();
        }
        FindNext();
    }

    private void ReplaceAll_Click(object sender, RoutedEventArgs e)
    {
        if (!IsEditableDocx || string.IsNullOrEmpty(FindBox.Text)) return;
        var count = TextSearch.ReplaceAll(Editor.Document, FindBox.Text, ReplaceBox.Text, MatchCase());
        if (count > 0) MarkDirty();
        StatusText.Text = $"Replaced {count} occurrence{(count == 1 ? string.Empty : "s")}";
    }

    private bool MatchCase() => false;

    private static Brush ParseBrush(string value) =>
        value.Equals("Transparent", StringComparison.OrdinalIgnoreCase)
            ? Brushes.Transparent
            : (Brush)new BrushConverter().ConvertFromString(value)!;

    private void MarkDirty()
    {
        _dirty = true;
        UpdateTitle();
    }

    private void UpdateUi()
    {
        var editable = IsEditableDocx;
        FontGroup.IsEnabled = editable;
        ParagraphGroup.IsEnabled = editable;
        TableInsertGroup.IsEnabled = editable;
        SaveMenuItem.IsEnabled = editable;
        SaveAsMenuItem.IsEnabled = editable;
        QuickSaveButton.IsEnabled = editable;
        UpdateTableControls();
        UpdatePdfControls();
        UpdateTitle();
    }

    private void UpdateTableControls()
    {
        var inTable = IsEditableDocx && TableEditor.GetContext(Editor.CaretPosition) is not null;
        TableAddRowButton.IsEnabled = inTable;
        TableDeleteRowButton.IsEnabled = inTable;
        TableAddColumnButton.IsEnabled = inTable;
        TableDeleteColumnButton.IsEnabled = inTable;
        CellFillBox.IsEnabled = inTable;
    }

    private void UpdatePdfControls()
    {
        var hasPdf = _pdfSession is not null;
        if (!hasPdf)
        {
            PdfPageText.Text = string.Empty;
            PdfZoomText.Text = string.Empty;
        }
        else
        {
            PdfPageText.Text = $"{_pdfPage + 1} / {_pdfSession!.PageCount}";
            PdfZoomText.Text = $"{ZoomLevels[_zoomIndex] * 100:0}%";
        }

        var canPrev = hasPdf && !_pdfRendering && _pdfPage > 0;
        var canNext = hasPdf && !_pdfRendering && _pdfPage + 1 < _pdfSession!.PageCount;
        PrevButton.IsEnabled = canPrev;
        NextButton.IsEnabled = canNext;
        RibbonPrevButton.IsEnabled = canPrev;
        RibbonNextButton.IsEnabled = canNext;
        RibbonZoomOutButton.IsEnabled = hasPdf && !_pdfRendering && _zoomIndex > 0;
        RibbonZoomInButton.IsEnabled = hasPdf && !_pdfRendering && _zoomIndex + 1 < ZoomLevels.Length;
    }

    private void UpdateTitle()
    {
        var name = string.IsNullOrWhiteSpace(_currentPath) ? "Untitled" : Path.GetFileName(_currentPath);
        Title = $"{name}{(_dirty ? " *" : string.Empty)} — MiniDoc";
        MainRibbon.Title = Title;
    }

    private void ShowNotice(string? message)
    {
        NoticeText.Text = message ?? string.Empty;
        NoticeBorder.Visibility = string.IsNullOrWhiteSpace(message) ? Visibility.Collapsed : Visibility.Visible;
    }

    private void ReleasePdf()
    {
        PdfImage.Source = null;
        _pdfSession?.Dispose();
        _pdfSession = null;
    }

    private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
    {
        if ((Keyboard.Modifiers & ModifierKeys.Control) == 0) return;
        if (e.Key == Key.N) { New_Click(sender, e); e.Handled = true; }
        else if (e.Key == Key.O) { Open_Click(sender, e); e.Handled = true; }
        else if (e.Key == Key.S && (Keyboard.Modifiers & ModifierKeys.Shift) != 0) { SaveCurrent(true); e.Handled = true; }
        else if (e.Key == Key.S) { SaveCurrent(false); e.Handled = true; }
        else if (e.Key == Key.F) { FindBox.Focus(); FindBox.SelectAll(); e.Handled = true; }
        else if (e.Key == Key.H) { ReplaceBox.Focus(); ReplaceBox.SelectAll(); e.Handled = true; }
    }

    private void Window_Closing(object? sender, CancelEventArgs e)
    {
        if (!ResolveUnsavedChanges()) e.Cancel = true;
    }

    private void Window_Closed(object? sender, EventArgs e)
    {
        ReleasePdf();
        Environment.Exit(0);
    }
}
