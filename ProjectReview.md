# Transfer(유니티 기반 퍼즐게임)

> #### 서울과학기술대학교 게임 프로그래밍 동아리 Plum 작품, 2018 한이음 ICT 출품작

#### 아래 링크에서 유튜브 플레이 영상을 확인하실 수 있습니다.

- [소개 영상](https://youtu.be/BaW5Ap3TsZw)
- [플레이 & 기능 설명 영상](https://youtu.be/1iOuXr54z6U)

![alt text](https://github.com/maniaKj/2018_ICT_Transfer_Script/blob/master/title.png)

### 한이음 ICT 공모전 출품작으로, 4명의 인원으로 팀을 구성해 Unity 엔진 기반 퍼즐게임을 개발하였습니다.
메모리와 성능 그리고 프로그래밍 언어 등 기술적인 면에서 깊이 있는 고민을 해가며 개발한 작품은 아닙니다. 해당 프로젝트를 진행할 때는 프로그래밍에 대한 깊이 있는 이해가 부족했던 시기였습니다. 하지만 팀원들이 협동하여 만들고 싶었던 것을 위해 밤낮없이 노력한 경험은 제게 하나의 추억이기도 하면서 지금까지의 저 자신이 가진 장단점이 무엇인지 되새겨보는 디딤돌이 되었습니다.

### 팀장 역할을 맡았으며 게임 기획, shader 프로그래밍, 플레이어 상호작용과 관련된 기능 개발을 담당했습니다.
밸브사의 ‘포탈’에 인상을 받은 1인칭 FPS 퍼즐게임을 개발하였습니다. 대학교 3학년 재학 중, 약 7개월 정도의 기간 동안 진행하였습니다. 유저들이 보다 직관적으로 이해할 수 있는 퍼즐 로직과 단순 간결한 조작/진행 방식을 구상하는데 공을 들였습니다. 이후 ‘Transfer’는 한이음 ICT 공모전에서 입상, 서울과학기술대학교 졸업작품전시회에서 우수상을 수상할 수 있었습니다.

##### 활용 언어 및 엔진
-	Unity
-	C
-	shaderLab

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
