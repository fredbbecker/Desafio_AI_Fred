using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ChallengeAI {
  public enum CameraView {
    None, SW,S,SE,W,Top,E,NW,N,NE
  }
  public class CameraController : MonoBehaviour
  {
    public float[] itpr = new float[3] {0.02f,0.02f,0.02f};
    private Transform containerTransform;
    private Transform cameraTransform;
    private CameraViewInfo currentViewInfo = new CameraViewInfo() {
      ContainerPosition = new Vector3(),
      ContainerRotation = Quaternion.identity,
      CameraLocalPosition = new Vector3(0,0,-26),
    };
    private Vector3 southContainerPosition = new Vector3(0,1,-7);
    private List<CameraViewInfo> viewInfos;
    private ObservableProperty<CameraView> View = new ObservableProperty<CameraView>();
    private void Awake() {
      containerTransform = GetComponent<Transform>();
      cameraTransform = containerTransform.GetChild(0);

      // containerTransform.position = new Vector3(0,1,-7);
      // containerTransform.rotation = Quaternion.Euler(45f,0,0);
      // cameraTransform.localPosition = currentViewInfo.CameraLocalPosition;

      viewInfos = new List<CameraViewInfo>() {
        new CameraViewInfo(),
        // SW
        new CameraViewInfo() {
          ContainerPosition = Quaternion.Euler(0,45f,0) * southContainerPosition,
          ContainerRotation = Quaternion.Euler(45f,45f,0),
          CameraLocalPosition = currentViewInfo.CameraLocalPosition,
        },
        // S
        new CameraViewInfo() {
          ContainerPosition = new Vector3(0,1,-7),
          ContainerRotation = Quaternion.Euler(45f,0,0),
          CameraLocalPosition = currentViewInfo.CameraLocalPosition,
        },
        // SE
        new CameraViewInfo() {
          ContainerPosition = Quaternion.Euler(0,-45f,0) * southContainerPosition,
          ContainerRotation = Quaternion.Euler(45f,-45f,0),
          CameraLocalPosition = currentViewInfo.CameraLocalPosition,
        },
        // W
        new CameraViewInfo() {
          ContainerPosition = new Vector3(-7,1,-0),
          ContainerRotation = Quaternion.Euler(45f,90f,0),
          CameraLocalPosition = currentViewInfo.CameraLocalPosition,
        },
        // Top
        new CameraViewInfo() {
          ContainerPosition = new Vector3(),
          ContainerRotation = Quaternion.Euler(90f,0,0),
          CameraLocalPosition = currentViewInfo.CameraLocalPosition,
        },
        // E
        new CameraViewInfo() {
          ContainerPosition = new Vector3(7,1,-0),
          ContainerRotation = Quaternion.Euler(45f,-90f,0),
          CameraLocalPosition = currentViewInfo.CameraLocalPosition,
        },
        // NW
        new CameraViewInfo() {
          ContainerPosition = Quaternion.Euler(0,135f,0) * southContainerPosition,
          ContainerRotation = Quaternion.Euler(45f,135f,0),
          CameraLocalPosition = currentViewInfo.CameraLocalPosition,
        },
        // N
        new CameraViewInfo() {
          ContainerPosition = new Vector3(0,1,7),
          ContainerRotation = Quaternion.Euler(45f,180f,0),
          CameraLocalPosition = currentViewInfo.CameraLocalPosition,
        },
        // NE
        new CameraViewInfo() {
          ContainerPosition = Quaternion.Euler(0,-135f,0) * southContainerPosition,
          ContainerRotation = Quaternion.Euler(45f,-135f,0),
          CameraLocalPosition = currentViewInfo.CameraLocalPosition,
        },
      };

      View.OnChange += (view) => {
        var vi = viewInfos[(int)view];
        currentViewInfo = vi; 
      };

      View.Value = CameraView.S;
    }
    private void Update() {
      InputKeys();

      containerTransform.position = Vector3.Lerp(containerTransform.position,currentViewInfo.ContainerPosition,itpr[0]);
      containerTransform.rotation = Quaternion.Lerp(containerTransform.rotation,currentViewInfo.ContainerRotation,itpr[1]);
      cameraTransform.localPosition = Vector3.Lerp(cameraTransform.localPosition, currentViewInfo.CameraLocalPosition,itpr[2]);
    }

    void InputKeys() {
      if(Input.GetKeyDown(KeyCode.KeypadPlus)) {
        currentViewInfo.CameraLocalPosition = currentViewInfo.CameraLocalPosition - Vector3.forward;
      }
      if(Input.GetKeyDown(KeyCode.KeypadMinus)) {
        currentViewInfo.CameraLocalPosition = currentViewInfo.CameraLocalPosition + Vector3.forward;
      }
      if(Input.GetKeyDown(KeyCode.Keypad1)) {
        View.Value = CameraView.SW;
      }
      if(Input.GetKeyDown(KeyCode.Keypad2)) {
        View.Value = CameraView.S;
      }
      if(Input.GetKeyDown(KeyCode.Keypad3)) {
        View.Value = CameraView.SE;
      }
      if(Input.GetKeyDown(KeyCode.Keypad4)) {
        View.Value = CameraView.W;
      }
      if(Input.GetKeyDown(KeyCode.Keypad5)) {
        View.Value = CameraView.Top;
      }
      if(Input.GetKeyDown(KeyCode.Keypad6)) {
        View.Value = CameraView.E;
      }
      if(Input.GetKeyDown(KeyCode.Keypad7)) {
        View.Value = CameraView.NW;
      }
      if(Input.GetKeyDown(KeyCode.Keypad8)) {
        View.Value = CameraView.N;    
      }
      if(Input.GetKeyDown(KeyCode.Keypad9)) {
        View.Value = CameraView.NE;
      }
    }
  }

  public class CameraViewInfo {
    public Vector3 ContainerPosition;
    public Quaternion ContainerRotation;
    public Vector3 CameraLocalPosition;
  }
}
