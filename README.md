# BarcordProject
최대 5개의 바코드와 TAG를 1:1로 연결해 바코드로 입력된 값을 연결된 TAG로 보내는 프로그램.   
바코드 입력 시 RawInput을 이용해 해당 바코드의 장치이름을 찾아서 연결된 TAG로 데이터 전송. 

# 구성
* BarcodeProject : WPF 기반의 응용프로그램.
* CIMON_Helper : CIMON에서 제공하는 코드. `SetTagVal`을 추가해 사용.
* RawInput : 참고 사이트의 코드를 WPF에서 동작하도록 수정해 사용. 여러 입력 장비가 있을 시 해당 장비의 정보를 가지고옴.

# 참고 사이트
https://www.codeproject.com/Articles/17123/Using-Raw-Input-from-C-to-handle-multiple-keyboard
