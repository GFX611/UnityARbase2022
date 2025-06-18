using UnityEngine;
using System.Collections;
using UnityEngine.Android;

public class androidCameraSwitch : MonoBehaviour
{
    WebCamTexture frontCameraTexture;
    WebCamTexture backCameraTexture;

    void Start()
    {
        if (Application.isMobilePlatform)
        {
            if (!Permission.HasUserAuthorizedPermission(Permission.Camera))
            {
                Permission.RequestUserPermission(Permission.Camera);
            }
            else
            {
                StartCoroutine(InitializeCameras());
            }
        }
    }

    IEnumerator InitializeCameras()
    {
        frontCameraTexture = new WebCamTexture(WebCamTexture.devices[1].name);
        backCameraTexture = new WebCamTexture(WebCamTexture.devices[0].name);

        yield return new WaitForEndOfFrame();

        frontCameraTexture.Play();
        backCameraTexture.Play();
    }

    public void SwitchCamera()
    {
        if (Camera.main.targetTexture == frontCameraTexture)
        {
            RenderTexture renderTexture = new RenderTexture(frontCameraTexture.width, frontCameraTexture.height, 24);
            Camera.main.targetTexture = renderTexture;
            renderTexture.Create();
            Camera.main.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, 0));
        }
        else
        {
            RenderTexture renderTexture = new RenderTexture(backCameraTexture.width, backCameraTexture.height, 24);
            Camera.main.targetTexture = renderTexture;
            renderTexture.Create();
            Camera.main.transform.localRotation = Quaternion.Euler(new Vector3(0, 180, 0));
        }
    }
}
