using UnityEngine;
using UnityEngine.UI;

/*
 * MyViewer.cs
 * 受信したメッセージを元に情報の管理・UIへの表示などをする
 * 通信用の非同期スレッドから直接Unityのメインスレッド呼ぶとエラーになるので一枚噛ませている
 */
namespace Script
{
	public class MyViewer : MonoBehaviour {
		// 受信した値など集約用のシステム
		public static MyViewer Instance;

		private int _num = -9999;  // 安易な初期値
		private string _ipPort = "none"; // 接続先情報保持用

#pragma warning disable 0649
		// 画面表示用
		[SerializeField]private Text _ipportField;
		[SerializeField]private Text _textField;
#pragma warning restore 0649

		private void Awake(){
			Instance = this;
		}

		private void Update () {
			// 接続先表示
			_ipportField.text = _ipPort;

			// 初期値なら更新しない
			if(_num == -9999){
				return;
			}

			// 例）数字が送られてきたらその温度帯の燗酒の温度表現を表示する
			string str;
			if (_num > -5 && _num < 0) {
				str = "雪どけ";
			} else if (_num >= 0 && _num < 7) {
				str = "雪冷え";
			} else if (_num >= 7 && _num < 12) {
				str = "花冷え";
			} else if (_num >= 12 && _num < 17) {
				str = "涼冷え";
			} else if (_num >= 17 && _num < 30) {
				str = "冷や";
			} else if (_num >= 30 && _num < 35) {
				str = "日向燗";
			} else if (_num >= 35 && _num < 38) {
				str = "人肌燗";
			} else if (_num >= 38 && _num < 42) {
				str = "ぬる燗";
			} else if (_num >= 42 && _num < 48) {
				str = "上燗";
			} else if (_num >= 48 && _num < 53) {
				str = "熱燗";
			} else if (_num >= 53 && _num < 80) {
				str = "飛び切り燗";
			} else if (_num >= 80 && _num < 90) {
				// 玉川酒造のフィリップ・ハーパー杜氏が好きな温度で正式なものではない
				// 著者もハーパー氏に倣ってここまで上げて飲んでみるなどして楽しんでいる
				str = "ハーパー燗"; 
			} else {
				str = "オススメしない";
			}

			_textField.text = _num + "℃は...\n"+str;

		}

		// 受信した数値セット
		public void SetNum(int n){
			_num = n;
		}

		// 接続情報セット
		public void SetIpAddressPort(string ipport){
			_ipPort = ipport;
		}
	}
}
