# Team 04 Project
추후 프로젝트 제안 문서의 내용을 반영할 예정
## 팀원들을 위한 워크플로우
### 개발 준비
- 수업 슬라이드의 git 세팅
- 우측 상단 Fork를 클릭하고, 그대로 Create Fork 버튼 클릭
- Fork 받은 리포지토리로 가서, 상단 ``<> Code``를 클릭하고, HTTPS의 링크를 우측 버튼으로 Copy, 다운로드 받을 경로에 가서 터미널에 다음과 같이 입력

```
git clone https://github.com/(자신의 GitHub 아이디)/team-project-for-2025-spring-swpp-team-04.git
```
- 이제 수업 슬라이드를 참조하여 다음과 같이 ``upstream``을 설정

```
git remote add upstream https://github.com/SWPP-2025SPRING/team-project-for-2025-spring-swpp-team-04.git
```
- 이제 ``git branch (브랜치 이름)``을 입력하면 신규 브랜치 생성과 함께 해당 브랜치로 checkout됨
- Unity에서 기능 구현 및 ``commit``
- 다음 커맨드로 ``main``에 ``merge``하여 ``conflict``가 없으면 코드 통합 완료

```
git checkout main
git merge (브랜치 이름)
```
- 구현 완료 후 ``git push``
- Fork 받은 자신의 리포 페이지에서 Contribute > Open pull request
- Create pull request
- 봇이 잠시 후 자동 코드 리뷰를 진행해 줌
  - 다른 팀원도 코드 리뷰 진행 가능
  - 리뷰를 반영하여 코드 수정
  - 리뷰가 완료되면 merge 완료. 모든 팀원들과 구현 내용을 공유 가능
### 디렉토리 구조
- ``Assets``에 게임을 이루고 있는 애셋과 코드들이 위치하고 있습니다.
  - (현재 프로젝트 세팅 진행 중)
- ``meetings``에는 회의록들이 정리됩니다.
- ``gdd``에는 게임 디자인 문서가 정리됩니다.
- ``Packages``, ``ProjectSettings``에는 패키지 매니징을 위한 파일들이 있습니다. (고려사항 아님)
