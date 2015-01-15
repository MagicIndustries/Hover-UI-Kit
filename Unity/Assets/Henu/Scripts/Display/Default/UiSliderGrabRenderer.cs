﻿using Henu.Navigation;
using Henu.Settings;
using Henu.State;
using UnityEngine;
using UnityEngine.UI;

namespace Henu.Display.Default {

	/*================================================================================================*/
	public class UiSliderGrabRenderer : UiBaseIconRenderer {

		private static readonly Texture2D IconTex = Resources.Load<Texture2D>("Slider");

		private NavItemSlider vNavSlider;
		private Text vText;

		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		protected override Texture2D GetIconTexture() {
			return IconTex;
		}

		/*--------------------------------------------------------------------------------------------*/
		protected override Vector3 GetIconScale() {
			float sx = vSettings.TextSize*vTextScale;
			float sy = vSettings.TextSize*1.25f*vTextScale;
			return new Vector3(sx, sy, 1);
		}


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public override void Build(ArcState pArcState, ArcSegmentState pSegState,																									
										float pAngle0, float pAngle1, ArcSegmentSettings pSettings) {
			base.Build(pArcState, pSegState, pAngle0, pAngle1, pSettings);
			vNavSlider = (NavItemSlider)vSegState.NavItem;
			vText = vTextObj.GetComponent<Text>();
		}

		/*--------------------------------------------------------------------------------------------*/
		public override void Update() {
			base.Update();
			vText.text = vNavSlider.ValueToLabel(vNavSlider.CurrentValue);
		}

	}

}