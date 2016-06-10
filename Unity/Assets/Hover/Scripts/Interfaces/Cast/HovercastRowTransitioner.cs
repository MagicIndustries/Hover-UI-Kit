﻿using System.Diagnostics;
using Hover.Layouts.Arc;
using UnityEngine;

namespace Hover.Interfaces.Cast {

	/*================================================================================================*/
	[ExecuteInEditMode]
	[RequireComponent(typeof(HovercastInterface))]
	public class HovercastRowTransitioner : MonoBehaviour {

		public float RowThickness = 0.06f;
		public float InnerRadius = 0.12f;

		[Range(0, 1)]
		public float TransitionProgress = 1;

		[Range(1, 10000)]
		public float TransitionMilliseconds = 400;

		public HovercastRowSwitcher.RowEntryType RowEntryTransition;

		private Stopwatch vTimer;


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public void Start() {
			GetComponent<HovercastInterface>().OnRowTransitionEvent.AddListener(HandleTransitionEvent);
		}
		
		/*--------------------------------------------------------------------------------------------*/
		public void Update() {
			UpdateTimedProgress();
			UpdateRows();
		}


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		private void HandleTransitionEvent(HovercastRowSwitcher.RowEntryType pEntryType) {
			RowEntryTransition = pEntryType;

			if ( pEntryType == HovercastRowSwitcher.RowEntryType.Immediate ) {
				TransitionProgress = 1;
				vTimer = null;
			}
			else {
				TransitionProgress = 0;
				vTimer = Stopwatch.StartNew();
			}

			Update();
		}

		
		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		private void UpdateTimedProgress() {
			if ( vTimer == null ) {
				return;
			}

			TransitionProgress = (float)vTimer.Elapsed.TotalMilliseconds/TransitionMilliseconds;

			if ( TransitionProgress >= 1 ) {
				TransitionProgress = 1;
				vTimer = null;
			}
		}

		/*--------------------------------------------------------------------------------------------*/
		private void UpdateRows() {
			HovercastInterface cast = GetComponent<HovercastInterface>();
			bool hasPrevRow = (cast.PreviousRow != null);
			bool isTransitionDone = (TransitionProgress >= 1 || !hasPrevRow);
			float radOffset = 0;
			int childOrder = 0;

			cast.ArcStack.InnerRadius = InnerRadius;
			cast.ArcStack.OuterRadius = InnerRadius + RowThickness*(isTransitionDone ? 1 : 2);

			if ( !isTransitionDone ) {
				childOrder = cast.ArcStack.GetChildOrder(cast.PreviousRow, cast.ActiveRow);
			}

			switch ( RowEntryTransition ) {
				case HovercastRowSwitcher.RowEntryType.Immediate:
					break;

				case HovercastRowSwitcher.RowEntryType.FromInside:
					radOffset = (isTransitionDone ? 0 : TransitionProgress-1);
					cast.ArcStack.Arrangement = (childOrder > 0 ?
						HoverLayoutArcStack.ArrangementType.InnerToOuter :
						HoverLayoutArcStack.ArrangementType.OuterToInner);
					break;
					
				case HovercastRowSwitcher.RowEntryType.FromOutside:
					radOffset = (isTransitionDone ? 0 : -TransitionProgress);
					cast.ArcStack.Arrangement = (childOrder > 0 ?
						HoverLayoutArcStack.ArrangementType.OuterToInner :
						HoverLayoutArcStack.ArrangementType.InnerToOuter);
					break;
			}

			if ( hasPrevRow ) {
				HoverLayoutArcRelativeSizer prevSizer = GetRelativeSizer(cast.PreviousRow);
				prevSizer.RelativeRadiusOffset = radOffset;
				//prevSizer.RelativeArcAngle = Mathf.Lerp(1, 0, TransitionProgress);
				cast.PreviousRow.gameObject.SetActive(!isTransitionDone);
			}

			HoverLayoutArcRelativeSizer activeSizer = GetRelativeSizer(cast.ActiveRow);
			activeSizer.RelativeRadiusOffset = radOffset;
			//activeSizer.RelativeArcAngle = Mathf.Lerp(0, 1, TransitionProgress);
			cast.ActiveRow.gameObject.SetActive(true);
		}
		

		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		private HoverLayoutArcRelativeSizer GetRelativeSizer(HoverLayoutArcRow pRow) {
			HoverLayoutArcRelativeSizer sizer = 				
				pRow.gameObject.GetComponent<HoverLayoutArcRelativeSizer>();
			return (sizer ? sizer : pRow.gameObject.AddComponent<HoverLayoutArcRelativeSizer>());
		}

	}

}