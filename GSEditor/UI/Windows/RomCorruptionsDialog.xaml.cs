using GSEditor.Contract.Services;
using GSEditor.Models.Pokegold;
using GSEditor.ViewModels.Selectors;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;

namespace GSEditor.UI.Windows;

public partial class RomCorruptionsDialog : Window
{
  public RomCorruptionsDialog()
  {
    InitializeComponent();

    var cnt = 0;
    var pokegold = App.Services.GetRequiredService<IPokegoldService>();
    pokegold.Data.Corruptions.ForEach(e =>
    {
      var pokemonName = pokegold.Data.Strings.PokemonNames[e.Index];
      var alphabat = ((char)('A' + e.Index)).ToString();
      LogsListBiew.Items.Add(new RomCorruptionViewModel
      {
        No = $"{cnt++}",
        Content = e.Type switch
        {
          Corruption.LearnMoveCorrupted => $"{pokemonName}의 배우는 기술 데이터가 손상되어 일부분만 남게됩니다.",
          Corruption.EvolutionCorrupted => $"{pokemonName}의 진화 데이터가 손상되어 일부분만 남게됩니다.",
          Corruption.ImageCorrupted => $"{pokemonName}의 이미지가 손상되어 빈 이미지로 대체됩니다.",
          Corruption.ImageBacksideCorrupted => $"{pokemonName}의 뒷모습 이미지가 손상되어 빈 이미지로 대체됩니다.",
          Corruption.UnownImageCorrupted => $"{alphabat}의 이미지가 손상되어 빈 이미지로 대체됩니다.",
          Corruption.UnownImageBacksideCorrupted => $"{alphabat}의 뒷모습 이미지가 손상되어 빈 이미지로 대체됩니다.",
          Corruption.TrainerImageCorrupted => $"트레이너 {e.Index}번의 이미지가 손상되어 빈 이미지로 대체됩니다.",
          _ => "",
        },
      });
    });
  }

  private void OnConfirmClick(object _, RoutedEventArgs __)
  {
    Close();
  }
}
