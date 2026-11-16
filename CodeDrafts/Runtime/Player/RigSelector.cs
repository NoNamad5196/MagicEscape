using System.Collections;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Management;
namespace MagicEscape
{
    public enum RigMode { Automatic, Desktop, VR }
    public sealed class RigSelector : MonoBehaviour
    {
        public RigMode mode;
        public GameObject desktopRig, vrRig, simulator;
        public Camera desktopCamera, vrCamera;
        public bool IsVR { get; private set; }
        IEnumerator Start() => InitializeAndSelect();
        public IEnumerator InitializeAndSelect()
        {
            // OpenXR initialization is manual so missing PC runtimes remain a supported desktop case.
            var manager = XRGeneralSettings.Instance != null ? XRGeneralSettings.Instance.Manager : null;
            if (mode != RigMode.Desktop && manager != null && manager.activeLoader == null)
            {
                yield return manager.InitializeLoader();
                if (manager.activeLoader != null) manager.StartSubsystems();
            }
            // Let the display subsystem publish device activity after StartSubsystems.
            yield return null;
            Apply(mode == RigMode.VR || (mode == RigMode.Automatic && XRSettings.isDeviceActive));
        }
        public void Apply(bool useVR)
        {
            var driver = desktopRig.GetComponentInChildren<DesktopXriDriver>(true);
            if (driver != null) driver.Drop();
            if (useVR && !IsVR) vrRig.transform.SetPositionAndRotation(desktopRig.transform.position, desktopRig.transform.rotation);
            else if (!useVR && IsVR) desktopRig.transform.SetPositionAndRotation(vrRig.transform.position, Quaternion.Euler(0,vrCamera.transform.eulerAngles.y,0));
            IsVR = useVR;
            // A loaded OpenXR runtime must not turn the manual desktop camera into a stereo camera.
            desktopCamera.stereoTargetEye = StereoTargetEyeMask.None;
            vrCamera.stereoTargetEye = StereoTargetEyeMask.Both;
            // Keep PlayerInteraction as the shared inventory/UI facade on the desktop root.
            desktopRig.GetComponent<PlayerController>().enabled = !useVR;
            desktopRig.GetComponent<CharacterController>().enabled = !useVR;
            desktopCamera.gameObject.SetActive(!useVR);
            vrRig.SetActive(useVR);
            if (simulator != null) simulator.SetActive(useVR && mode == RigMode.VR && !XRSettings.isDeviceActive);
            var facade = desktopRig.GetComponent<PlayerInteraction>();
            facade.SetViewCamera(useVR ? vrCamera : desktopCamera);
            foreach (var canvas in FindObjectsByType<Canvas>(FindObjectsInactive.Include, FindObjectsSortMode.None))
            {
                if (!canvas.isRootCanvas) continue;
                if (useVR) { canvas.renderMode = RenderMode.WorldSpace; canvas.worldCamera = vrCamera; canvas.transform.SetParent(vrCamera.transform, false); canvas.transform.localPosition = new Vector3(0, 0, 1.3f); canvas.transform.localRotation = Quaternion.identity; canvas.transform.localScale = Vector3.one * .001f; }
                else { canvas.transform.SetParent(null, true); canvas.renderMode = RenderMode.ScreenSpaceOverlay; canvas.worldCamera = null; }
            }
        }
        void Update()
        {
            var input = desktopRig.GetComponent<PlayerInputReader>();
            if (Debug.isDebugBuild && input != null && input.RigTogglePressed) { mode = IsVR ? RigMode.Desktop : RigMode.VR; Apply(!IsVR); }
        }
        void OnDestroy()
        {
            var manager = XRGeneralSettings.Instance != null ? XRGeneralSettings.Instance.Manager : null;
            if (manager != null && manager.activeLoader != null) { manager.StopSubsystems(); manager.DeinitializeLoader(); }
        }
    }
}
