using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TouchLib {

	static Ray ray;
	static RaycastHit hit;
	static float maxTime = 1.0f;
	static float minSwipDist= 20.0f;
	static float maxSwipDist= 100.0f;

	static float startTime;
	static float endTime;

	static Vector3 startPos;
	static Vector3 endPos;
	static Vector2 move;
	static float swipeDistance;
	static float swipeTime;

	#region GlobalFunc

	public static void GetSwipeTest()
	{
		if(Input.touchCount > 0)
		{
			Touch touch = Input.GetTouch(0);
			if(touch.phase == TouchPhase.Began)
			{
				startTime = Time.time;
				startPos = touch.position;

			}
			else if(touch.phase == TouchPhase.Ended)
			{
				endTime = Time.time;
				endPos = touch.position;
				swipeDistance = (endPos - startPos).magnitude;
				swipeTime = endTime - startTime;
				if(swipeTime < maxTime && swipeDistance > minSwipDist)
				{
					CheckSwipeDir();
				}
			}

		}
	}

	public static float GetSwipeVertical()
	{
		if(Input.touchCount > 0)
		{
			Touch touch = Input.GetTouch(0);
			if(touch.phase == TouchPhase.Began)
			{
				startTime = Time.time;
				startPos = touch.position;
				return 0.0f;
			}
			else if(touch.phase == TouchPhase.Ended)
			{
				endTime = Time.time;
				endPos = touch.position;
				move = endPos - startPos;
				swipeDistance = move.magnitude;
				swipeTime = endTime - startTime;
				if(Mathf.Abs(move.x) > Mathf.Abs(move.y))
				{
					return 0.0f; //if this swip is tend to horizontal
				}
				if(swipeTime < maxTime && swipeDistance > minSwipDist)
				{
					return Mathf.Clamp(move.y/maxSwipDist,-1.0f,1.0f);
				}
				return 0.0f;
			}
			return 0.0f;
		}
		return 0.0f;
	}

	public static float GetSwipeHorizontal()
	{
		if(Input.touchCount > 0)
		{
			Touch touch = Input.GetTouch(0);
			if(touch.phase == TouchPhase.Began)
			{
				startTime = Time.time;
				startPos = touch.position;
				return 0.0f;
			}
			else if(touch.phase == TouchPhase.Ended)
			{
				endTime = Time.time;
				endPos = touch.position;
				move = endPos - startPos;
				swipeDistance = move.magnitude;
				swipeTime = endTime - startTime;
				if(Mathf.Abs(move.x) < Mathf.Abs(move.y))
				{
					return 0.0f; //if this swip is tend to vertical
				}
				if(swipeTime < maxTime && swipeDistance > minSwipDist)
				{
					return Mathf.Clamp(move.x/maxSwipDist,-1.0f,1.0f);
				}
				return 0.0f;
			}
			return 0.0f;
		}
		return 0.0f;
	}

	public static Vector2 GetSwipe2D()
	{
        //Debug.Log(Input.touchCount);
		if(Input.touchCount > 0)
		{
			Touch touch = Input.GetTouch(0);
			if(touch.phase == TouchPhase.Began)
			{
				startTime = Time.time;
				startPos = touch.position;
				return Vector2.zero;
			}
			else if(touch.phase == TouchPhase.Ended)
			{
				endTime = Time.time;
				endPos = touch.position;
				move = endPos - startPos;
				swipeDistance = move.magnitude;
				swipeTime = endTime - startTime;
				if(swipeTime < maxTime && swipeDistance > minSwipDist)
				{
					return new Vector2(Mathf.Clamp(move.x/maxSwipDist,-1.0f,1.0f),Mathf.Clamp(move.y/maxSwipDist,-1.0f,1.0f));
				}
				return Vector2.zero;
			}
			return Vector2.zero;
		}
		return Vector2.zero;
	}

    static float perspectiveZoomSpeed = 0.5f;
    static float orthZoomSpeed = 0.5f;

    public static void ZoomInOut()
    {
        if (Input.touchCount == 2)
        {
            Touch t1 = Input.GetTouch(0);
            Touch t2 = Input.GetTouch(1);
            Vector2 t1PrevPos = t1.position - t1.deltaPosition;
            Vector2 t2PrevPos = t2.position - t2.deltaPosition;

            float prevTouchDeltaMag = (t1PrevPos - t2PrevPos).magnitude;
            float touchDeltaMag = (t1.position - t2.position).magnitude;

            float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;
            if (Camera.main.orthographic)
            {
                Camera.main.orthographicSize += deltaMagnitudeDiff * orthZoomSpeed;
                Camera.main.orthographicSize = Mathf.Max(Camera.main.orthographicSize, 0.1f);
            }
            else
            {
                Camera.main.fieldOfView += deltaMagnitudeDiff * perspectiveZoomSpeed;
                Camera.main.fieldOfView = Mathf.Clamp(Camera.main.fieldOfView,0.1f,179.9f);
            }
        }
    }
	/// <summary>
	/// Use it as Gravity Sensor
	/// </summary>
	public static void AccelerationTest()
	{
		float temp = Input.acceleration.x;
		Debug.Log(temp);

	}

	public static void TouchRayCastTest()
	{
		if(Input.touchCount >0 && Input.GetTouch(0).phase == TouchPhase.Began)
		{
			ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
			if(Physics.Raycast(ray,out hit,float.MaxValue))
			{
				Debug.Log(hit.collider.name);
			}
		}

	}

	public static void PrintAllTouchesPosition()
	{
		if(Input.touchCount > 0)
		{

			foreach(Touch touch in Input.touches)
			{
				Debug.Log(touch.position);

			}
		}
	}

	public static void TouchActionTest()
	{
		if(Input.touchCount > 0)
		{

			if(Input.GetTouch(0).phase == TouchPhase.Began)
			{
				Debug.Log("Touch Began");
			}
			if(Input.GetTouch(0).phase == TouchPhase.Moved)
			{
				Debug.Log("Touch Moved");
			}
			if(Input.GetTouch(0).phase == TouchPhase.Ended)
			{
				Debug.Log("Touch Ended");
			}

		}
	}

	#endregion

	#region PrivateFunc

	static void  CheckSwipeDir()
	{
		Vector2 distance = endPos - startPos;
		if(Mathf.Abs(distance.x) > Mathf.Abs(distance.y))
		{
			Debug.Log("Horizontal Swipe");
			if(distance.x > 0)
				Debug.Log("Right Swipe");
			else
				Debug.Log("Left Swipe");

		}else
		{
			Debug.Log("Vertical Swipe");
			if(distance.y > 0){
				Debug.Log("Up Swipe");
			}else
			{
				Debug.Log("Down Swipe");
			}

		}
	}
		
	#endregion
}
