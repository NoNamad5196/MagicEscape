// 서버 컴파일 검사 전용 최소 Unity API 스텁. 실제 Unity 동작을 대신하지 않으며, 시그니처 정합성만 확인한다.
// 각 멤버는 Unity 6 공개 API 시그니처를 따른다. Unity 프로젝트에 복사하지 말 것.
using System;
using System.Collections;

namespace UnityEngine
{
    public class Object { public string name { get; set; } public static void Destroy(Object o) { } }
    public class Component : Object
    {
        public GameObject gameObject => null;
        public Transform transform => null;
        public T GetComponent<T>() => default(T);
        public T GetComponentInChildren<T>() => default(T);
        public T GetComponentInParent<T>() => default(T);
    }
    public class Behaviour : Component { public bool enabled { get; set; } public bool isActiveAndEnabled => true; }
    public class MonoBehaviour : Behaviour
    {
        public Coroutine StartCoroutine(IEnumerator routine) => null;
        public void StopCoroutine(Coroutine routine) { }
    }
    public class ScriptableObject : Object { }
    public sealed class Coroutine { }
    public class YieldInstruction { }
    public sealed class WaitForSeconds : YieldInstruction { public WaitForSeconds(float s) { } }
    public class CustomYieldInstruction : IEnumerator
    {
        public object Current => null; public bool MoveNext() => false; public void Reset() { }
    }
    public class WaitForSecondsRealtime : CustomYieldInstruction { public WaitForSecondsRealtime(float t) { } }

    public sealed class GameObject : Object
    {
        public bool activeSelf => true;
        public void SetActive(bool v) { }
        public Transform transform => null;
    }
    public class Transform : Component
    {
        public Vector3 position { get; set; }
        public Quaternion rotation { get; set; }
        public Vector3 localPosition { get; set; }
        public Quaternion localRotation { get; set; }
        public Vector3 right => default; public Vector3 forward => default;
        public void Rotate(float x, float y, float z, Space s) { }
        public void SetPositionAndRotation(Vector3 p, Quaternion r) { }
    }
    public enum Space { World, Self }

    public struct Vector2
    {
        public float x, y;
        public Vector2(float x, float y) { this.x = x; this.y = y; }
        public static Vector2 zero => default;
        public static Vector2 ClampMagnitude(Vector2 v, float m) => v;
        public static Vector2 operator *(Vector2 a, float d) => a;
    }
    public struct Vector3
    {
        public float x, y, z;
        public Vector3(float x, float y, float z) { this.x = x; this.y = y; this.z = z; }
        public static Vector3 up => default;
        public static Vector3 Lerp(Vector3 a, Vector3 b, float t) => a;
        public static Vector3 operator *(Vector3 a, float d) => a;
        public static Vector3 operator *(float d, Vector3 a) => a;
        public static Vector3 operator +(Vector3 a, Vector3 b) => a;
    }
    public struct Quaternion
    {
        public static Quaternion Euler(float x, float y, float z) => default;
        public static Quaternion Slerp(Quaternion a, Quaternion b, float t) => a;
        public static Quaternion operator *(Quaternion a, Quaternion b) => a;
    }
    public struct Color
    {
        public float r, g, b, a;
        public Color(float r, float g, float b) { this.r = r; this.g = g; this.b = b; a = 1f; }
        public static Color black => default;
        public float maxColorComponent => 0f;
        public static Color operator *(Color c, float d) => c;
    }
    public struct Ray { }
    public struct RaycastHit { public Collider collider => null; }
    public struct LayerMask { public static implicit operator LayerMask(int v) => default; public static implicit operator int(LayerMask m) => 0; }
    public enum QueryTriggerInteraction { UseGlobal, Ignore, Collide }
    public enum CursorLockMode { None, Locked, Confined }

    public static class Mathf
    {
        public static float Clamp(float v, float a, float b) => v;
        public static int Clamp(int v, int a, int b) => v;
        public static float Max(float a, float b) => a;
        public static float SmoothStep(float a, float b, float t) => t;
        public static int FloorToInt(float f) => 0;
    }
    public static class Time { public static float deltaTime => 0f; public static float unscaledTime => 0f; public static float timeScale { get; set; } }
    public static class Debug
    {
        public static void Log(object m) { } public static void Log(object m, Object c) { }
        public static void LogWarning(object m) { } public static void LogWarning(object m, Object c) { }
        public static void LogError(object m) { } public static void LogError(object m, Object c) { }
    }
    public static class Physics
    {
        public static bool Raycast(Ray ray, out RaycastHit hit, float maxDistance, int layerMask, QueryTriggerInteraction q) { hit = default; return false; }
    }
    public static class Cursor { public static CursorLockMode lockState { get; set; } public static bool visible { get; set; } }
    public static class Application { public static void Quit() { } }
    public static class Shader { public static int PropertyToID(string n) => 0; }

    public class Camera : Behaviour { public Ray ViewportPointToRay(Vector3 p) => default; }
    public class Collider : Component { public bool isTrigger { get; set; } }
    public class CharacterController : Collider { public bool isGrounded => true; public void Move(Vector3 m) { } }
    public class Renderer : Component
    {
        public void GetPropertyBlock(MaterialPropertyBlock b) { }
        public void SetPropertyBlock(MaterialPropertyBlock b) { }
    }
    public sealed class MaterialPropertyBlock { public void SetColor(int id, Color c) { } }
    public sealed class Light : Behaviour { public Color color { get; set; } }
    public sealed class ParticleSystem : Component { public void Play() { } }
    public sealed class AudioClip : Object { }
    public sealed class AudioSource : Behaviour
    {
        public AudioClip clip { get; set; } public bool loop { get; set; } public bool isPlaying => false;
        public void Play() { } public void PlayOneShot(AudioClip c, float v) { }
    }
    public sealed class Sprite : Object { }

    [AttributeUsage(AttributeTargets.Field)] public sealed class SerializeField : Attribute { }
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)] public class PropertyAttribute : Attribute { }
    public sealed class HeaderAttribute : PropertyAttribute { public HeaderAttribute(string h) { } }
    public sealed class TooltipAttribute : PropertyAttribute { public TooltipAttribute(string t) { } }
    public sealed class TextAreaAttribute : PropertyAttribute { public TextAreaAttribute(int a, int b) { } }
    public sealed class RangeAttribute : PropertyAttribute { public RangeAttribute(float a, float b) { } }
    [AttributeUsage(AttributeTargets.Class)] public sealed class CreateAssetMenuAttribute : Attribute { public string fileName { get; set; } public string menuName { get; set; } }
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)] public sealed class RequireComponent : Attribute { public RequireComponent(Type t) { } }
    [AttributeUsage(AttributeTargets.Class)] public sealed class DisallowMultipleComponent : Attribute { }
    [AttributeUsage(AttributeTargets.Class)] public sealed class DefaultExecutionOrder : Attribute { public DefaultExecutionOrder(int o) { } }

#if ENABLE_LEGACY_INPUT_MANAGER
    public enum KeyCode { Alpha1 = 49, E = 101, Escape = 27, Tab = 9 }
    public static class Input
    {
        public static float GetAxisRaw(string a) => 0f;
        public static bool GetKeyDown(KeyCode k) => false;
        public static Vector2 mouseScrollDelta => default;
    }
#endif
}

namespace UnityEngine.SceneManagement
{
    public struct Scene { public int buildIndex => 0; }
    public static class SceneManager { public static Scene GetActiveScene() => default; public static void LoadScene(int i) { } }
}

namespace UnityEngine.UI
{
    public class Image : Behaviour { public Sprite sprite { get; set; } }
}

namespace TMPro
{
    public class TMP_Text : UnityEngine.Behaviour { public string text { get; set; } }
}

#if ENABLE_INPUT_SYSTEM
namespace UnityEngine.InputSystem
{
    public enum Key { Digit1, Digit2, Digit3, Digit4, Digit5, Digit6, Digit7, Digit8 }
    public class ButtonControl { public bool isPressed => false; public bool wasPressedThisFrame => false; }
    public sealed class KeyControl : ButtonControl { }
    public sealed class Vector2Control { public Vector2 ReadValue() => default; }
    public sealed class Keyboard
    {
        public static Keyboard current => null;
        public KeyControl this[Key key] => null;
        public KeyControl wKey => null; public KeyControl aKey => null; public KeyControl sKey => null; public KeyControl dKey => null;
        public KeyControl eKey => null; public KeyControl escapeKey => null; public KeyControl tabKey => null;
    }
    public sealed class Mouse { public static Mouse current => null; public Vector2Control delta => null; public Vector2Control scroll => null; }
}
#endif
