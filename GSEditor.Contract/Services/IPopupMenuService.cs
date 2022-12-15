using System;
using System.Collections.Generic;

namespace GSEditor.Contract.Services;

public interface IPopupMenuService
{
  public void Show(params PopupMenuItem[] menuItems);
}

public sealed class PopupMenuItem
{
  public string Caption { get; }
  public Action? OnClickAction { get; }

  public PopupMenuItem(string caption)
  {
    Caption = caption;
    OnClickAction = null;
  }

  public PopupMenuItem(string caption, Action onClickAction)
  {
    Caption = caption;
    OnClickAction = onClickAction;
  }

  public sealed class Builder
  {
    private readonly List<PopupMenuItem> _items = new();

    public Builder Add(string caption)
    {
      _items.Add(new(caption));
      return this;
    }

    public Builder Add(string caption, Action onClickAction)
    {
      _items.Add(new(caption, onClickAction));
      return this;
    }

    public PopupMenuItem[] Create()
    {
      return _items.ToArray();
    }
  }
}
