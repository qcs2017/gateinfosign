using SimAirport.Modding.Base;
using SimAirport.Modding.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
// using System.Text;
// using System.Threading.Tasks;
using UnityEngine;
using SimAirport.Modding.Data;
using TMPro;

namespace qcs2017.gateinfosign {
	public class Mod : BaseMod {
		public override string Name => "Gate Info Sign";

		public override string InternalName => "qcs2017.gateinfosign";

		public override string Description => "Gate Info Sign showing Gate information";

		public override string Author => "qcs2017";

		public override SettingManager SettingManager { get; set; } //will be filled by game

		private double lastgametime = 0;

		private Dictionary<SmartObject, SignOverlay> overlays = new Dictionary<SmartObject, SignOverlay>();

		public override void OnAirportLoaded(Dictionary<string, object> saveData) {
			doWork();
		}

		public override void OnTick() {
			double gametime = GameTime.Instance.TotalGameSeconds;
			if (lastgametime == 0) 
				lastgametime = gametime;

			if (gametime - lastgametime > 60) {
				lastgametime = gametime;
				
				doWork();
			}
		}

		private void doWork() {
            List<SmartObject> list = Game.current.objectCache.smartObjects.FindAll(so => so.iprefab.path== "chick10000/gatesign/sign1");
			Debug.Log("Found " + list.Count + " Gate Info Signs");
			list.ForEach(sign => {
				SignOverlay overlay;
				if (!overlays.TryGetValue(sign, out overlay)){
					overlay = sign.gameObject.AddComponent<SignOverlay>();
					overlay.createOverlay(sign);
					overlays.Add(sign, overlay);
				}
				overlay.updateContent();
			});			
		}
	}

	public class SignOverlay : MonoBehaviour {

		public TextMeshPro txtObject;

		// private static Quaternion q = Quaternion.Euler(0f, 0f, 0f);

 		public void createOverlay(SmartObject sign) {
			Debug.Log("Creating overlay");
			txtObject = gameObject.AddComponent<TextMeshPro>();
			txtObject.fontSize = 2f;
			txtObject.renderer.sortingLayerName="UI";
			txtObject.renderer.sortingOrder=32760;
			txtObject.color = Color.green;
			// sign.gameObject.transform.SetParent(txtObject.rectTransform);
			txtObject.gameObject.layer = UILevelSelector.LevelToLayer(sign.iprefab.level);
			// txtObject.rectTransform.localPosition = new Vector3(-1f, 0f, sign.iprefab.level);
			// txtObject.rectTransform.localPosition = new Vector3(0.5f, 0.5f, sign.iprefab.level);
			txtObject.rectTransform.position = sign.iprefab.v3i;
			txtObject.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 3f);
			txtObject.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 3f);
			txtObject.alignment = TextAlignmentOptions.TopLeft;
			// txtObject.fontMaterial.SetFloat(ShaderUtilities.ID_FaceDilate, 0.3f);
			// txtObject.fontMaterial.SetFloat(ShaderUtilities.ID_OutlineSoftness, 0.3f);
			// txtObject.fontMaterial.SetFloat(ShaderUtilities.ID_OutlineWidth, 0.3f);
			// txtObject.UpdateMeshPadding();
			Debug.Log("Created overlay");
		}

		public void updateContent() {
            Agent.Aircraft aircraft = Game.current.aircraft_on_tarmac.RandomElementByWeight(aircraft1 => 1);
			txtObject.text = "WWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWW "
				+ "Gate " + aircraft.flight.flightSchema.gate.Name + "\n"
				+ aircraft.flight.flightSchema.DisplayName + "\n"
				+ "WWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWW "
				;
		}
	};
}
