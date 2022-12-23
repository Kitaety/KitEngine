using OpenTK.Mathematics;

namespace KitEngine.GameObjects;

public abstract class BaseObject
{
    public string Name { get; set; }
    public Transform Transform { get; set; }

    protected BaseObject(string name, Transform transform)
    {
        Transform = transform;
        Name = name;
    }

    public virtual void Rotate(Vector3 rotation)
    {
        Rotate(Quaternion.FromEulerAngles(rotation));
    }

    public virtual void Rotate(Quaternion rotation)
    {
        Transform.Rotate(rotation);
    }
}