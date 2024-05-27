
public interface IDisJointable
{
    IDisJointable Parent { get; set; }
}

public class DisJointSet
{
    public IDisJointable FindRoot(IDisJointable a)
    {
        return a == a.Parent ? a : FindRoot(a.Parent);
    }
    public void Union(IDisJointable a, IDisJointable b)
    {
        var x = FindRoot(a);
        var y = FindRoot(b);

        if (x != y)
            y.Parent = x;
    }
    public bool IsUnion(IDisJointable a, IDisJointable b)
    { return FindRoot(a) == FindRoot(b); }
}
