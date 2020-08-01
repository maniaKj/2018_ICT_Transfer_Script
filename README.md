# Transfer

> #### 서울과학기술대학교 게임 프로그래밍 동아리 Plum 작품, 2018 한이음 ICT 출품작

1인칭 FPS 퍼즐게임, 유니티 엔진을 기반으로 개발되었습니다.   

밸브 '포탈'에서 인상을 받고 기획하였으며, 특정 도구를 플레이어가 데이터 형태로 전송(transfer)받거나 전송하여 퍼즐을 해결합니다.


#### 아래 링크에서 유튜브 플레이 영상을 확인하실 수 있습니다.

- [소개 영상](https://youtu.be/BaW5Ap3TsZw)
- [플레이 & 기능 설명 영상](https://youtu.be/1iOuXr54z6U)

![alt text](https://github.com/maniaKj/2018_ICT_Transfer_Script/blob/master/title.png)




# Script
<details>
 <summary>fold/unfold</summary> <br>
 
[FirstPerson_move(이동)](https://github.com/wlsvy/2018_ICT_Transfer_Script/blob/master/script/Player/FirstPerson_move.cs)
 : 플레이어 이동 스크립트

[Photo_Identification(홀로그램 인식)](https://github.com/wlsvy/2018_ICT_Transfer_Script/blob/master/script/Player/Photo_Identification.cs)
 : 플레이어 전방의 이미지를 캡쳐해서 서버로 전송시킵니다. 서버의 응답을 받으면 해당 데이터를 처리합니다. Unity의 www form을 활용하였습니다.

[HologramObject (이펙트, 플레이어 상호작용)](https://github.com/wlsvy/2018_ICT_Transfer_Script/blob/master/script/Object/HologramObject.cs)
 : 게임 내 퍼즐을 해결하기 위한 단서를 가지고 있는 홀로그램 오브젝트 입니다.

[TransferableDataObject(전송데이터)](https://github.com/wlsvy/2018_ICT_Transfer_Script/blob/master/script/Object/TransferableDataObject.cs)
 : 플레이어의 총을 통해 흡수 / 전송 하는 전송데이터 오브젝트 입니다.

[3D_Hologram (홀로그램 쉐이더)](https://github.com/wlsvy/2018_ICT_Transfer_Script/blob/master/script/Shader/3D_Hologram.shader)
 : 홀로그램 Material 에 사용되는 쉐이더입니다. 홀로그램의 사라질 때 Dissolve 효과를 활용할 수 있도록 구현하였습니다.
 
 </details>
