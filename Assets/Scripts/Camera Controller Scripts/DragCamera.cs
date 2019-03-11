

#pragma warning disable 0414

using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;


public class DragCamera : MonoBehaviour, IDragHandler, IEndDragHandler{

	private bool isPressing = false;

	public void OnDrag(PointerEventData data){

		isPressing = true;

		CameraController.instance.OnDrag (data);

	}

	public void OnEndDrag(PointerEventData data){

		isPressing = false;

	}

}
