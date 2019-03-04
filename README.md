# 2018_ICT_Transfer_Script
2018 한이음 ICT 출품작 'Transfer' 소스 코드

https://github.com/maniaKj/2018_ICT_Transfer_Script/blob/master/title.png

영상 url :
https://youtu.be/BaW5Ap3TsZw
https://youtu.be/1iOuXr54z6U

코드 중 핵심 오브젝트의 컴포넌트에 대해서만 간략 정리해 보았습니다.

플레이어 컴포넌트-----------

FirstPerson_move(이동) - https://github.com/maniaKj/2018_ICT_Transfer_Script/blob/master/Unity_Transfer/script/Player/FirstPerson_move.cs

Photo_Identify_Result(홀로그램 인식) - https://github.com/maniaKj/2018_ICT_Transfer_Script/blob/master/Unity_Transfer/script/Player/Photo_Identification.cs

Show_Identigy_Result(인식 결과 표시) - https://github.com/maniaKj/2018_ICT_Transfer_Script/blob/master/Unity_Transfer/script/Player/Show_Identify_Result.cs

Aim_UI(조준점 UI) - https://github.com/maniaKj/2018_ICT_Transfer_Script/blob/master/Unity_Transfer/script/Player/Aim_UI.cs
Player_shot(데이터 흡수 & 전송 상호작용) - https://github.com/maniaKj/2018_ICT_Transfer_Script/blob/master/Unity_Transfer/script/Player/player_shot.cs

Player_RM function(손 애니메이션) - https://github.com/maniaKj/2018_ICT_Transfer_Script/blob/master/Unity_Transfer/script/Player/player_RMfunction.cs
///////////////////////////////////

홀로그램 컴포넌트-------------

Obj_Module_Operation (입력 데이터 체크) - https://github.com/maniaKj/2018_ICT_Transfer_Script/blob/master/Unity_Transfer/script/Object/Obj_Module_Operation.cs

Puzzle_Hologram_Repository_2 (초기화 & 오브젝트 관리) - https://github.com/maniaKj/2018_ICT_Transfer_Script/blob/master/Unity_Transfer/script/Object/Puzzle_Hologram_Repository_2.cs

Data_Hologram (이펙트, 플레이어 상호작용) - https://github.com/maniaKj/2018_ICT_Transfer_Script/blob/master/Unity_Transfer/script/Object/Data_Hologram.cs

Module_Lable(데이터 전송 & 흡수 상호작용) - https://github.com/maniaKj/2018_ICT_Transfer_Script/blob/master/Unity_Transfer/script/Object/Module_Lable.cs
//////////////////////////////

데이터 노드-----------

Obj_Module_Operation(입력 데이터 체크) - https://github.com/maniaKj/2018_ICT_Transfer_Script/blob/master/Unity_Transfer/script/Object/Obj_Module_Operation.cs

Circuit_Node(데이터/타 오브젝트 상호작용) - https://github.com/maniaKj/2018_ICT_Transfer_Script/blob/master/Unity_Transfer/script/Object/Circuit_Node.cs

Module_Lable(데이터 전송 & 흡수 상호작용) - https://github.com/maniaKj/2018_ICT_Transfer_Script/blob/master/Unity_Transfer/script/Object/Module_Lable.cs
/////////////////////////////

쉐이더 
홀로그램 Dissolve - https://github.com/maniaKj/2018_ICT_Transfer_Script/blob/master/Shader/3D_Hologram.shader

초보라 코드가 너저분하기 짝이 없습니다 
