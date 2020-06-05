# 2018_ICT_Transfer_Script
2018 한이음 ICT 출품작 'Transfer' 소스 코드

![alt text](https://github.com/maniaKj/2018_ICT_Transfer_Script/blob/master/title.png)


# Youtube Link

[소개 영상](https://youtu.be/BaW5Ap3TsZw)

[플레이 & 기능 설명 영상](https://youtu.be/1iOuXr54z6U)


# Script

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
