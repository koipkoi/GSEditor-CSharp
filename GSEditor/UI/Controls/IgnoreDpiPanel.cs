using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace GSEditor.UI.Controls;

public sealed class IgnoreDpiPanel : Decorator
{
  protected override void OnInitialized(EventArgs e)
  {
    base.OnInitialized(e);
    Loaded += (_, __) => Adjustment();
  }

  protected override void OnDpiChanged(DpiScale oldDpi, DpiScale newDpi)
  {
    base.OnDpiChanged(oldDpi, newDpi);
    Adjustment();
  }

  private void Adjustment()
  {
    var m = PresentationSource.FromVisual(this).CompositionTarget.TransformToDevice;
    var dpiTransform = new ScaleTransform(1 / m.M11, 1 / m.M22);
    if (dpiTransform.CanFreeze)
      dpiTransform.Freeze();
    LayoutTransform = dpiTransform;
  }
}
