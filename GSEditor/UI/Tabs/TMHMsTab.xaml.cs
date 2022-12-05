﻿using GSEditor.Core;
using GSEditor.Utilities;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Xceed.Wpf.Toolkit;
using Xceed.Wpf.Toolkit.Primitives;

namespace GSEditor.UI.Tabs;

public partial class TMHMsTab : UserControl
{
  private readonly Pokegold _pokegold = App.Services.GetRequiredService<Pokegold>();
  private readonly List<CheckListBox> _pokemonList;

  public TMHMsTab()
  {
    InitializeComponent();

    _pokemonList = new()
    {
      Pokemon0,
      Pokemon1,
      Pokemon2,
      Pokemon3,
      Pokemon4,
      Pokemon5,
      Pokemon6,
      Pokemon7,
      Pokemon8,
      Pokemon9,
      Pokemon10,
      Pokemon11,
      Pokemon12,
      Pokemon13,
      Pokemon14,
      Pokemon15,
      Pokemon16,
      Pokemon17,
      Pokemon18,
      Pokemon19,
      Pokemon20,
      Pokemon21,
      Pokemon22,
      Pokemon23,
      Pokemon24,
      Pokemon25,
    };

    Loaded += (_, __) => OnNeedTabUpdate();
    _pokegold.RegisterRomChanged(this, (_, _) => OnNeedTabUpdate());
  }

  private void OnNeedTabUpdate()
  {
    if (_pokegold.IsOpened)
    {
      var previousSelection = TMHMsListBox.SelectedIndex;

      this.RunSafe(() =>
      {
        TMHMsListBox.Items.Clear();
        for (var i = 0; i < 57; i++)
        {
          var label = (i < 50 ? "기술" : "비전");
          var number = (i < 50 ? i + 1 : i - 49).ToString().PadLeft(2, '0');
          var name = _pokegold.Strings.MoveNames[_pokegold.TMHMs[i] - 1];
          TMHMsListBox.Items.Add($"{label}{number} [{name}]");
        }

        MoveComboBox.Items.Clear();
        foreach (var e in _pokegold.Strings.MoveNames)
          MoveComboBox.Items.Add(e);

        if (_pokemonList.Select(e => e.Items.Count).Sum() != 251)
        {
          _pokemonList.ForEach(e => e.Items.Clear());
          for (var i = 0; i < 251; i++)
          {
            var name = _pokegold.Strings.PokemonNames[i];
            _pokemonList[i / 10].Items.Add(name);
          }
        }
        else
        {
          for (var i = 0; i < 251; i++)
          {
            var name = _pokegold.Strings.PokemonNames[i];
            _pokemonList[i / 10].Items[i % 10] = name;
          }
        }
      });

      TMHMsListBox.SelectedIndex = previousSelection;
    }
  }

  private void OnSizeChanged(object _, SizeChangedEventArgs __)
  {
    PokemonContainer.Columns = (int)(ActualWidth / 160);
  }

  private void NestedScrollImplEvent(object sender, MouseWheelEventArgs e)
  {
    if (sender is CheckListBox listBox && !e.Handled)
    {
      var border = (VisualTreeHelper.GetChild(listBox, 0) as Border)!;
      var scrollViewer = (VisualTreeHelper.GetChild(border, 0) as ScrollViewer)!;
      if ((e.Delta > 0 && scrollViewer.VerticalOffset == 0) || (e.Delta < 0 && scrollViewer.VerticalOffset == scrollViewer.ScrollableHeight))
      {
        e.Handled = true;

        var parent = (scrollViewer.Parent as UIElement)!;
        parent.RaiseEvent(new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta)
        {
          RoutedEvent = MouseWheelEvent,
          Source = scrollViewer,
        });
      }
    }
  }

  private void OnTMHMsSelectionChanged(object _, SelectionChangedEventArgs __)
  {
    var index = TMHMsListBox.SelectedIndex;
    if (index != -1)
    {
      this.RunSafe(() =>
      {
        MoveComboBox.SelectedIndex = _pokegold.TMHMs[index] - 1;

        foreach (var e in _pokemonList)
          e.SelectedItems.Clear();
        for (var i = 0; i < 251; i++)
        {
          var item = _pokemonList[i / 10].Items[i % 10];
          if (_pokegold.Pokemons[i].TMHMs[index])
            _pokemonList[i / 10].SelectedItems.Add(item);
        }
      });
    }

    ContentBorder.IsEnabled = index != -1;
  }

  private void OnComboBoxSelectionChanged(object _, SelectionChangedEventArgs __)
  {
    var previousSelection = TMHMsListBox.SelectedIndex;

    var index = TMHMsListBox.SelectedIndex;
    if (index != -1)
    {
      this.RunSafe(() =>
      {
        _pokegold.TMHMs[index] = (byte)(MoveComboBox.SelectedIndex + 1);

        var label = (index < 50 ? "기술" : "비전");
        var number = (index < 50 ? index + 1 : index - 49).ToString().PadLeft(2, '0');
        var name = _pokegold.Strings.MoveNames[_pokegold.TMHMs[index] - 1];
        TMHMsListBox.Items[index] = $"{label}{number} [{name}]";

        _pokegold.NotifyDataChanged();
      });
    }

    TMHMsListBox.SelectedIndex = previousSelection;
  }

  private void OnPokemonItemSelectionChanged(object _, ItemSelectionChangedEventArgs __)
  {
    var index = TMHMsListBox.SelectedIndex;
    if (index != -1)
    {
      this.RunSafe(() =>
      {
        for (var i = 0; i < 251; i++)
          _pokegold.Pokemons[i].TMHMs[index] = false;

        for (var i = 0; i < _pokemonList.Count; i++)
        {
          foreach (var e in _pokemonList[i].SelectedItems)
            _pokegold.Pokemons[(i * 10) + _pokemonList[i].Items.IndexOf(e)].TMHMs[index] = true;
        }

        _pokegold.NotifyDataChanged();
      });
    }
  }

  private void OnPokemonButtonClick(object sender, RoutedEventArgs _)
  {
    var index = TMHMsListBox.SelectedIndex;
    if (index != -1)
    {
      if (sender is Button button)
      {
        if (button.Name == nameof(PokemonCheckAllButton))
        {
          foreach (var e in _pokemonList)
            e.SelectedItems.Clear();

          for (var i = 0; i < 57; i++)
          {
            var item = _pokemonList[i / 10].Items[i % 10];
            _pokemonList[i / 10].SelectedItems.Add(item);
          }
        }

        if (button.Name == nameof(PokemonClearButton))
        {
          foreach (var e in _pokemonList)
            e.SelectedItems.Clear();
        }
      }
    }
  }
}
