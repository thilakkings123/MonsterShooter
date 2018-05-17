using UnityEngine;
/// <summary>
/// This make brackground scroll
/// </summary>
public class BackgroundController : MonoBehaviour {

    public float backgroundSpeed;       //speed of scrolling
    public Renderer backgroundTexture;  //ref to renderer
    float offset;

    // Update is called once per frame
    void Update ()
    {
        ScrollBackground(backgroundSpeed, backgroundTexture);	
	}

    void ScrollBackground(float scrollSpeed, Renderer rend) 
    {
        offset -= Time.deltaTime * scrollSpeed;  //set the offset with respective to camera
        rend.material.SetTextureOffset("_MainTex", new Vector2(offset, -0.001f));  //set it in renderer
    }

}
