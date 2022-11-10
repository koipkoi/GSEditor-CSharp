using GSEditor.Core;
using System.Windows;

namespace GSEditor.UI.Windows;

public partial class AppInformationDialog : Window
{
  public static readonly string AppVersions = "" +
    "### 추후 개선 사항\n" +
    "* 안농 이미지, 팔레트 편집\n" +
    "* 기술머신 편집\n" +
    "  \n" +
    "  \n" +
    "### 1.2.0\n" +
    "* UI 개선 및 재설계\n" +
    "* hidpi+ 지원 (배율이 지정되어 있는 높은 해상도의 모니터에서 지원)\n" +
    "* 이제부터 은 버전 편집을 미지원합니다. (오류가 발생할 수 있음)\n" +
    "* 이제부터 성도맵 호환성 지원하지 않습니다. (포켓몬 이름 관련 사항으로 성도맵 툴을 이용시 불편할 수 있음)\n" +
    "* 이제부터 롬 파일 헤더에 관하여 알림을 표시하지 않습니다.\n" +
    "* 포켓몬 이미지, 팔레트 편집 및 추출 추가\n" +
    "* 롬 파일 헤더 체크섬이 기록되도록 업데이트\n" +
    "* 기술 편집 탭 추가\n" +
    "* 목록 검색 기능 추가\n" +
    "  \n" +
    "  \n" +
    "### 1.1.4\n" +
    "* 저장, 텍스트 UI 수정\n" +
    "* 성도맵 호환에 따른 예외처리 추가\n" +
    "* 에뮬레이터 미설정 시 나오는창에서 선택사항 묻기 추가\n" +
    "  \n" +
    "  \n" +
    "### 1.1.0\n" +
    "* 진화, 자력기 편집 버튼 UI 수정\n" +
    "* 자력기 편집 중 가져오기 기능 추가\n" +
    "  \n" +
    "  \n" +
    "### 1.0.5\n" +
    "* 롬을 여러번 불러와도 정상 작동하도록 수정\n" +
    "* 기타 UI 위치 조절\n" +
    "  \n" +
    "  \n" +
    "### 1.0.4\n" +
    "* 롬 파일 헤더 검사를 자세히 하도록 수정\n" +
    "* 은 버전에서 아이템 수정이 불가능한 부분 수정\n" +
    "* 포켓몬 선택 유지 체크항목 기억하도록 수정\n" +
    "  \n" +
    "  \n" +
    "### 1.0.3\n" +
    "* 롬을 열 때 헤더를 검사하도록 수정 (비정상인 경우 경고창 팝업)\n" +
    "* 능력치 입력 순서 변경\n" +
    "* 소스 리팩터링\n" +
    "  \n" +
    "  \n" +
    "### 1.0.2\n" +
    "* 데이터 저장이 재대로 안되는 현상 수정\n" +
    "* 롬을 여러번 불러오지 못하도록 변경\n" +
    "  \n" +
    "  \n" +
    "";

  private AppInformationDialog()
  {
    InitializeComponent();

    VersionLabel.Content = $"버전：{Platform.AppVersion}";
    ContentMarkdown.HereMarkdown = AppVersions;
  }

  public static void Show(DependencyObject owner)
  {
    var window = new AppInformationDialog { Owner = GetWindow(owner), };
    window.ShowDialog();
  }
}
